using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    internal class Solver
    {
        internal const sbyte LOWER_BOUND = 0;
        internal const sbyte UPPER_BOUND = 1;
        internal const sbyte FREE = 2;
        internal static readonly double INF = Double.PositiveInfinity;
        internal double Cn;
        internal double Cp;
        internal double[] G; // gradient of objective function
        internal double[] G_bar; // gradient, if we treat free variables as 0
        internal Kernel Q;
        internal int[] active_set;
        internal int active_size;
        internal double[] alpha;
        internal sbyte[] alpha_status; // LOWER_BOUND, UPPER_BOUND, FREE
        internal double[] b;
        internal double eps;
        internal int l;
        internal bool unshrinked; // XXX
        internal sbyte[] y;

        //UPGRADE_NOTE: Final was removed from the declaration of 'INF '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003_3"'

        internal virtual double get_C(int i)
        {
            return (y[i] > 0) ? Cp : Cn;
        }

        internal virtual void update_alpha_status(int i)
        {
            if (alpha[i] >= get_C(i))
                alpha_status[i] = UPPER_BOUND;
            else if (alpha[i] <= 0)
                alpha_status[i] = LOWER_BOUND;
            else
                alpha_status[i] = FREE;
        }

        internal virtual bool is_upper_bound(int i)
        {
            return alpha_status[i] == UPPER_BOUND;
        }

        internal virtual bool is_lower_bound(int i)
        {
            return alpha_status[i] == LOWER_BOUND;
        }

        internal virtual bool is_free(int i)
        {
            return alpha_status[i] == FREE;
        }

        // java: information about solution except alpha,
        // because we cannot return multiple values otherwise...

        internal virtual void swap_index(int i, int j)
        {
            Q.swap_index(i, j);
            do
            {
                sbyte _ = y[i];
                y[i] = y[j];
                y[j] = _;
            } while (false);
            do
            {
                double _ = G[i];
                G[i] = G[j];
                G[j] = _;
            } while (false);
            do
            {
                sbyte _ = alpha_status[i];
                alpha_status[i] = alpha_status[j];
                alpha_status[j] = _;
            } while (false);
            do
            {
                double _ = alpha[i];
                alpha[i] = alpha[j];
                alpha[j] = _;
            } while (false);
            do
            {
                double _ = b[i];
                b[i] = b[j];
                b[j] = _;
            } while (false);
            do
            {
                int _ = active_set[i];
                active_set[i] = active_set[j];
                active_set[j] = _;
            } while (false);
            do
            {
                double _ = G_bar[i];
                G_bar[i] = G_bar[j];
                G_bar[j] = _;
            } while (false);
        }

        internal virtual void reconstruct_gradient()
        {
            // reconstruct inactive elements of G from G_bar and free variables

            if (active_size == l)
                return;

            int i;
            for (i = active_size; i < l; i++)
                G[i] = G_bar[i] + b[i];

            for (i = 0; i < active_size; i++)
                if (is_free(i))
                {
                    float[] Q_i = Q.get_Q(i, l);
                    double alpha_i = alpha[i];
                    for (int j = active_size; j < l; j++)
                        G[j] += alpha_i * Q_i[j];
                }
        }

        internal virtual void Solve(int l, Kernel Q, double[] b_, sbyte[] y_, double[] alpha_, double Cp, double Cn,
                                    double eps, SolutionInfo si, int shrinking)
        {
            this.l = l;
            this.Q = Q;
            b = new double[b_.Length];
            b_.CopyTo(b, 0);
            y = new sbyte[y_.Length];
            y_.CopyTo(y, 0);
            alpha = new double[alpha_.Length];
            alpha_.CopyTo(alpha, 0);
            this.Cp = Cp;
            this.Cn = Cn;
            this.eps = eps;
            unshrinked = false;

            // initialize alpha_status
            {
                alpha_status = new sbyte[l];
                for (int i = 0; i < l; i++)
                    update_alpha_status(i);
            }

            // initialize active set (for shrinking)
            {
                active_set = new int[l];
                for (int i = 0; i < l; i++)
                    active_set[i] = i;
                active_size = l;
            }

            // initialize gradient
            {
                G = new double[l];
                G_bar = new double[l];
                int i;
                for (i = 0; i < l; i++)
                {
                    G[i] = b[i];
                    G_bar[i] = 0;
                }
                for (i = 0; i < l; i++)
                    if (!is_lower_bound(i))
                    {
                        float[] Q_i = Q.get_Q(i, l);
                        double alpha_i = alpha[i];
                        int j;
                        for (j = 0; j < l; j++)
                            G[j] += alpha_i * Q_i[j];
                        if (is_upper_bound(i))
                            for (j = 0; j < l; j++)
                                G_bar[j] += get_C(i) * Q_i[j];
                    }
            }

            // optimization step

            int iter = 0;
            int counter = Math.Min(l, 1000) + 1;
            var working_set = new int[2];

            while (true)
            {
                // max iterations?
                if (iter > 10000)
                    break;
                // show progress and do shrinking

                if (--counter == 0)
                {
                    counter = Math.Min(l, 1000);
                    if (shrinking != 0)
                        do_shrinking();
                    //Console.Error.Write(".");
                }

                if (select_working_set(working_set) != 0)
                {
                    // reconstruct the whole gradient
                    reconstruct_gradient();
                    // reset active set size and check
                    active_size = l;
                    //Console.Error.Write("*");
                    if (select_working_set(working_set) != 0)
                        break;
                    else
                        counter = 1; // do shrinking next iteration
                }

                int i = working_set[0];
                int j = working_set[1];

                ++iter;

                // update alpha[i] and alpha[j], handle bounds carefully

                float[] Q_i = Q.get_Q(i, active_size);
                float[] Q_j = Q.get_Q(j, active_size);

                double C_i = get_C(i);
                double C_j = get_C(j);

                double old_alpha_i = alpha[i];
                double old_alpha_j = alpha[j];

                if (y[i] != y[j])
                {
                    double delta = (-G[i] - G[j]) / Math.Max(Q_i[i] + Q_j[j] + 2 * Q_i[j], 0);
                    double diff = alpha[i] - alpha[j];
                    alpha[i] += delta;
                    alpha[j] += delta;

                    if (diff > 0)
                    {
                        if (alpha[j] < 0)
                        {
                            alpha[j] = 0;
                            alpha[i] = diff;
                        }
                    }
                    else
                    {
                        if (alpha[i] < 0)
                        {
                            alpha[i] = 0;
                            alpha[j] = -diff;
                        }
                    }
                    if (diff > C_i - C_j)
                    {
                        if (alpha[i] > C_i)
                        {
                            alpha[i] = C_i;
                            alpha[j] = C_i - diff;
                        }
                    }
                    else
                    {
                        if (alpha[j] > C_j)
                        {
                            alpha[j] = C_j;
                            alpha[i] = C_j + diff;
                        }
                    }
                }
                else
                {
                    double delta = (G[i] - G[j]) / Math.Max(Q_i[i] + Q_j[j] - 2 * Q_i[j], 0);
                    double sum = alpha[i] + alpha[j];
                    alpha[i] -= delta;
                    alpha[j] += delta;
                    if (sum > C_i)
                    {
                        if (alpha[i] > C_i)
                        {
                            alpha[i] = C_i;
                            alpha[j] = sum - C_i;
                        }
                    }
                    else
                    {
                        if (alpha[j] < 0)
                        {
                            alpha[j] = 0;
                            alpha[i] = sum;
                        }
                    }
                    if (sum > C_j)
                    {
                        if (alpha[j] > C_j)
                        {
                            alpha[j] = C_j;
                            alpha[i] = sum - C_j;
                        }
                    }
                    else
                    {
                        if (alpha[i] < 0)
                        {
                            alpha[i] = 0;
                            alpha[j] = sum;
                        }
                    }
                }

                // update G

                double delta_alpha_i = alpha[i] - old_alpha_i;
                double delta_alpha_j = alpha[j] - old_alpha_j;

                for (int k = 0; k < active_size; k++)
                {
                    G[k] += Q_i[k] * delta_alpha_i + Q_j[k] * delta_alpha_j;
                }

                // update alpha_status and G_bar

                {
                    bool ui = is_upper_bound(i);
                    bool uj = is_upper_bound(j);
                    update_alpha_status(i);
                    update_alpha_status(j);
                    int k;
                    if (ui != is_upper_bound(i))
                    {
                        Q_i = Q.get_Q(i, l);
                        if (ui)
                            for (k = 0; k < l; k++)
                                G_bar[k] -= C_i * Q_i[k];
                        else
                            for (k = 0; k < l; k++)
                                G_bar[k] += C_i * Q_i[k];
                    }

                    if (uj != is_upper_bound(j))
                    {
                        Q_j = Q.get_Q(j, l);
                        if (uj)
                            for (k = 0; k < l; k++)
                                G_bar[k] -= C_j * Q_j[k];
                        else
                            for (k = 0; k < l; k++)
                                G_bar[k] += C_j * Q_j[k];
                    }
                }
            }

            // calculate rho

            si.rho = calculate_rho();

            // calculate objective value
            {
                double v = 0;
                int i;
                for (i = 0; i < l; i++)
                    v += alpha[i] * (G[i] + b[i]);

                si.obj = v / 2;
            }

            // put back the solution
            {
                for (int i = 0; i < l; i++)
                    alpha_[active_set[i]] = alpha[i];
            }

            si.upper_bound_p = Cp;
            si.upper_bound_n = Cn;

            //Console.Out.Write("\noptimization finished, #iter = " + iter + "\n");
        }

        // return 1 if already optimal, return 0 otherwise
        internal virtual int select_working_set(int[] working_set)
        {
            // return i,j which maximize -grad(f)^T d , under constraint
            // if alpha_i == C, d != +1
            // if alpha_i == 0, d != -1

            double Gmax1 = -INF; // max { -grad(f)_i * d | y_i*d = +1 }
            int Gmax1_idx = -1;

            double Gmax2 = -INF; // max { -grad(f)_i * d | y_i*d = -1 }
            int Gmax2_idx = -1;

            for (int i = 0; i < active_size; i++)
            {
                if (y[i] == +1)
                // y = +1
                {
                    if (!is_upper_bound(i))
                    // d = +1
                    {
                        if (-G[i] > Gmax1)
                        {
                            Gmax1 = -G[i];
                            Gmax1_idx = i;
                        }
                    }
                    if (!is_lower_bound(i))
                    // d = -1
                    {
                        if (G[i] > Gmax2)
                        {
                            Gmax2 = G[i];
                            Gmax2_idx = i;
                        }
                    }
                }
                // y = -1
                else
                {
                    if (!is_upper_bound(i))
                    // d = +1
                    {
                        if (-G[i] > Gmax2)
                        {
                            Gmax2 = -G[i];
                            Gmax2_idx = i;
                        }
                    }
                    if (!is_lower_bound(i))
                    // d = -1
                    {
                        if (G[i] > Gmax1)
                        {
                            Gmax1 = G[i];
                            Gmax1_idx = i;
                        }
                    }
                }
            }

            if (Gmax1 + Gmax2 < eps)
                return 1;

            working_set[0] = Gmax1_idx;
            working_set[1] = Gmax2_idx;
            return 0;
        }

        internal virtual void do_shrinking()
        {
            int i, j, k;
            var working_set = new int[2];
            if (select_working_set(working_set) != 0)
                return;
            i = working_set[0];
            j = working_set[1];
            double Gm1 = (-y[j]) * G[j];
            double Gm2 = y[i] * G[i];

            // shrink

            for (k = 0; k < active_size; k++)
            {
                if (is_lower_bound(k))
                {
                    if (y[k] == +1)
                    {
                        if (-G[k] >= Gm1)
                            continue;
                    }
                    else if (-G[k] >= Gm2)
                        continue;
                }
                else if (is_upper_bound(k))
                {
                    if (y[k] == +1)
                    {
                        if (G[k] >= Gm2)
                            continue;
                    }
                    else if (G[k] >= Gm1)
                        continue;
                }
                else
                    continue;

                --active_size;
                swap_index(k, active_size);
                --k; // look at the newcomer
            }

            // unshrink, check all variables again before final iterations

            if (unshrinked || -(Gm1 + Gm2) > eps * 10)
                return;

            unshrinked = true;
            reconstruct_gradient();

            for (k = l - 1; k >= active_size; k--)
            {
                if (is_lower_bound(k))
                {
                    if (y[k] == +1)
                    {
                        if (-G[k] < Gm1)
                            continue;
                    }
                    else if (-G[k] < Gm2)
                        continue;
                }
                else if (is_upper_bound(k))
                {
                    if (y[k] == +1)
                    {
                        if (G[k] < Gm2)
                            continue;
                    }
                    else if (G[k] < Gm1)
                        continue;
                }
                else
                    continue;

                swap_index(k, active_size);
                active_size++;
                ++k; // look at the newcomer
            }
        }

        internal virtual double calculate_rho()
        {
            double r;
            int nr_free = 0;
            double ub = INF, lb = -INF, sum_free = 0;
            for (int i = 0; i < active_size; i++)
            {
                double yG = y[i] * G[i];

                if (is_lower_bound(i))
                {
                    if (y[i] > 0)
                        ub = Math.Min(ub, yG);
                    else
                        lb = Math.Max(lb, yG);
                }
                else if (is_upper_bound(i))
                {
                    if (y[i] < 0)
                        ub = Math.Min(ub, yG);
                    else
                        lb = Math.Max(lb, yG);
                }
                else
                {
                    ++nr_free;
                    sum_free += yG;
                }
            }

            if (nr_free > 0)
                r = sum_free / nr_free;
            else
                r = (ub + lb) / 2;

            return r;
        }

        #region Nested type: SolutionInfo

        internal class SolutionInfo
        {
            internal double obj;
            internal double r; // for Solver_NU
            internal double rho;
            internal double upper_bound_n;
            internal double upper_bound_p;
        }

        #endregion
    }
}
