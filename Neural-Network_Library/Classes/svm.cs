using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class svm
    {
        //
        // construct and solve various formulations
        //
        private static void solve_c_svc(svm_problem prob, svm_parameter param, double[] alpha, Solver.SolutionInfo si,
                                        double Cp, double Cn)
        {
            int l = prob.l;
            var minus_ones = new double[l];
            var y = new sbyte[l];

            int i;

            for (i = 0; i < l; i++)
            {
                alpha[i] = 0;
                minus_ones[i] = -1;
                if (prob.y[i] > 0)
                    y[i] = (+1);
                else
                    y[i] = -1;
            }

            var s = new Solver();
            s.Solve(l, new SVC_Q(prob, param, y), minus_ones, y, alpha, Cp, Cn, param.eps, si, param.shrinking);

            double sum_alpha = 0;
            for (i = 0; i < l; i++)
                sum_alpha += alpha[i];

            /*if (Cp == Cn)
                Console.Out.Write("nu = " + sum_alpha/(Cp*prob.l) + "\n");*/

            for (i = 0; i < l; i++)
                alpha[i] *= y[i];
        }

        private static void solve_nu_svc(svm_problem prob, svm_parameter param, double[] alpha, Solver.SolutionInfo si)
        {
            int i;
            int l = prob.l;
            double nu = param.nu;

            var y = new sbyte[l];

            for (i = 0; i < l; i++)
                if (prob.y[i] > 0)
                    y[i] = (+1);
                else
                    y[i] = -1;

            double sum_pos = nu * l / 2;
            double sum_neg = nu * l / 2;

            for (i = 0; i < l; i++)
                if (y[i] == +1)
                {
                    alpha[i] = Math.Min(1.0, sum_pos);
                    sum_pos -= alpha[i];
                }
                else
                {
                    alpha[i] = Math.Min(1.0, sum_neg);
                    sum_neg -= alpha[i];
                }

            var zeros = new double[l];

            for (i = 0; i < l; i++)
                zeros[i] = 0;

            var s = new Solver_NU();
            s.Solve(l, new SVC_Q(prob, param, y), zeros, y, alpha, 1.0, 1.0, param.eps, si, param.shrinking);
            double r = si.r;

            //Console.Out.Write("C = " + 1/r + "\n");

            for (i = 0; i < l; i++)
                alpha[i] *= y[i] / r;

            si.rho /= r;
            si.obj /= (r * r);
            si.upper_bound_p = 1 / r;
            si.upper_bound_n = 1 / r;
        }

        private static void solve_one_class(svm_problem prob, svm_parameter param, double[] alpha,
                                            Solver.SolutionInfo si)
        {
            int l = prob.l;
            var zeros = new double[l];
            var ones = new sbyte[l];
            int i;

            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1042_3"'
            var n = (int)(param.nu * prob.l); // # of alpha's at upper bound

            for (i = 0; i < n; i++)
                alpha[i] = 1;
            alpha[n] = param.nu * prob.l - n;
            for (i = n + 1; i < l; i++)
                alpha[i] = 0;

            for (i = 0; i < l; i++)
            {
                zeros[i] = 0;
                ones[i] = 1;
            }

            var s = new Solver();
            s.Solve(l, new ONE_CLASS_Q(prob, param), zeros, ones, alpha, 1.0, 1.0, param.eps, si, param.shrinking);
        }

        private static void solve_epsilon_svr(svm_problem prob, svm_parameter param, double[] alpha,
                                              Solver.SolutionInfo si)
        {
            int l = prob.l;
            var alpha2 = new double[2 * l];
            var linear_term = new double[2 * l];
            var y = new sbyte[2 * l];
            int i;

            for (i = 0; i < l; i++)
            {
                alpha2[i] = 0;
                linear_term[i] = param.p - prob.y[i];
                y[i] = 1;

                alpha2[i + l] = 0;
                linear_term[i + l] = param.p + prob.y[i];
                y[i + l] = -1;
            }

            var s = new Solver();
            s.Solve(2 * l, new SVR_Q(prob, param), linear_term, y, alpha2, param.C, param.C, param.eps, si,
                    param.shrinking);

            double sum_alpha = 0;
            for (i = 0; i < l; i++)
            {
                alpha[i] = alpha2[i] - alpha2[i + l];
                sum_alpha += Math.Abs(alpha[i]);
            }
            //Console.Out.Write("nu = " + sum_alpha/(param.C*l) + "\n");
        }

        private static void solve_nu_svr(svm_problem prob, svm_parameter param, double[] alpha, Solver.SolutionInfo si)
        {
            int l = prob.l;
            double C = param.C;
            var alpha2 = new double[2 * l];
            var linear_term = new double[2 * l];
            var y = new sbyte[2 * l];
            int i;

            double sum = C * param.nu * l / 2;
            for (i = 0; i < l; i++)
            {
                alpha2[i] = alpha2[i + l] = Math.Min(sum, C);
                sum -= alpha2[i];

                linear_term[i] = -prob.y[i];
                y[i] = 1;

                linear_term[i + l] = prob.y[i];
                y[i + l] = -1;
            }

            var s = new Solver_NU();
            s.Solve(2 * l, new SVR_Q(prob, param), linear_term, y, alpha2, C, C, param.eps, si, param.shrinking);

            //Console.Out.Write("epsilon = " + (- si.r) + "\n");

            for (i = 0; i < l; i++)
                alpha[i] = alpha2[i] - alpha2[i + l];
        }

        //
        // decision_function
        //
        internal class decision_function
        {
            internal double[] alpha;
            internal double rho;
        }
        public class SupportClass
        {
            /// <summary>
            /// Provides access to a static System.Random class instance
            /// </summary>
            public static Random Random = new Random();

            /*******************************/

            #region Nested type: Tokenizer

            /// <summary>
            /// The class performs token processing in strings
            /// </summary>
            public class Tokenizer : IEnumerator
            {
                /// Char representation of the String to tokenize.
                private readonly char[] chars;

                /// Include demiliters in the results.
                private readonly bool includeDelims;

                /// Position over the string
                private long currentPos;

                //The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character and the form-feed character
                private string delimiters = " \t\n\r\f";

                /// <summary>
                /// Initializes a new class instance with a specified string to process
                /// </summary>
                /// <param name="source">String to tokenize</param>
                public Tokenizer(String source)
                {
                    chars = source.ToCharArray();
                }

                /// <summary>
                /// Initializes a new class instance with a specified string to process
                /// and the specified token delimiters to use
                /// </summary>
                /// <param name="source">String to tokenize</param>
                /// <param name="delimiters">String containing the delimiters</param>
                public Tokenizer(String source, String delimiters)
                    : this(source)
                {
                    this.delimiters = delimiters;
                }


                /// <summary>
                /// Initializes a new class instance with a specified string to process, the specified token 
                /// delimiters to use, and whether the delimiters must be included in the results.
                /// </summary>
                /// <param name="source">String to tokenize</param>
                /// <param name="delimiters">String containing the delimiters</param>
                /// <param name="includeDelims">Determines if delimiters are included in the results.</param>
                public Tokenizer(String source, String delimiters, bool includeDelims)
                    : this(source, delimiters)
                {
                    this.includeDelims = includeDelims;
                }


                /// <summary>
                /// Remaining tokens count
                /// </summary>
                public int Count
                {
                    get
                    {
                        //keeping the current pos
                        long pos = currentPos;
                        int i = 0;

                        try
                        {
                            while (true)
                            {
                                NextToken();
                                i++;
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            currentPos = pos;
                            return i;
                        }
                    }
                }

                #region IEnumerator Members

                /// <summary>
                ///  Performs the same action as NextToken.
                /// </summary>
                public Object Current
                {
                    get { return NextToken(); }
                }

                /// <summary>
                ///  Performs the same action as HasMoreTokens.
                /// </summary>
                /// <returns>True or false, depending if there are more tokens</returns>
                public bool MoveNext()
                {
                    return HasMoreTokens();
                }

                /// <summary>
                /// Does nothing.
                /// </summary>
                public void Reset()
                {
                    ;
                }

                #endregion

                /// <summary>
                /// Returns the next token from the token list
                /// </summary>
                /// <returns>The string value of the token</returns>
                public String NextToken()
                {
                    return NextToken(delimiters);
                }

                /// <summary>
                /// Returns the next token from the source string, using the provided
                /// token delimiters
                /// </summary>
                /// <param name="delimiters">String containing the delimiters to use</param>
                /// <returns>The string value of the token</returns>
                public String NextToken(String delimiters)
                {
                    //According to documentation, the usage of the received delimiters should be temporary (only for this call).
                    //However, it seems it is not true, so the following line is necessary.
                    this.delimiters = delimiters;

                    //at the end 
                    if (currentPos == chars.Length)
                        throw new ArgumentOutOfRangeException();
                    //if over a delimiter and delimiters must be returned
                    else if ((Array.IndexOf(delimiters.ToCharArray(), chars[currentPos]) != -1)
                             && includeDelims)
                        return "" + chars[currentPos++];
                    //need to get the token wo delimiters.
                    else
                        return nextToken(delimiters.ToCharArray());
                }

                //Returns the nextToken wo delimiters
                private String nextToken(char[] delimiters)
                {
                    string token = "";
                    long pos = currentPos;

                    //skip possible delimiters
                    while (Array.IndexOf(delimiters, chars[currentPos]) != -1)
                        //The last one is a delimiter (i.e there is no more tokens)
                        if (++currentPos == chars.Length)
                        {
                            currentPos = pos;
                            throw new ArgumentOutOfRangeException();
                        }

                    //getting the token
                    while (Array.IndexOf(delimiters, chars[currentPos]) == -1)
                    {
                        token += chars[currentPos];
                        //the last one is not a delimiter
                        if (++currentPos == chars.Length)
                            break;
                    }
                    return token;
                }


                /// <summary>
                /// Determines if there are more tokens to return from the source string
                /// </summary>
                /// <returns>True or false, depending if there are more tokens</returns>
                public bool HasMoreTokens()
                {
                    //keeping the current pos
                    long pos = currentPos;

                    try
                    {
                        NextToken();
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        return false;
                    }
                    finally
                    {
                        currentPos = pos;
                    }
                    return true;
                }
            }

            #endregion
        }

        internal static decision_function svm_train_one(svm_problem prob, svm_parameter param, double Cp, double Cn)
        {
            var alpha = new double[prob.l];
            var si = new Solver.SolutionInfo();
            switch (param.svm_type)
            {
                case svm_parameter.C_SVC:
                    solve_c_svc(prob, param, alpha, si, Cp, Cn);
                    break;

                case svm_parameter.NU_SVC:
                    solve_nu_svc(prob, param, alpha, si);
                    break;

                case svm_parameter.ONE_CLASS:
                    solve_one_class(prob, param, alpha, si);
                    break;

                case svm_parameter.EPSILON_SVR:
                    solve_epsilon_svr(prob, param, alpha, si);
                    break;

                case svm_parameter.NU_SVR:
                    solve_nu_svr(prob, param, alpha, si);
                    break;
            }

            //Console.Out.Write("obj = " + si.obj + ", rho = " + si.rho + "\n");

            // output SVs

            int nSV = 0;
            int nBSV = 0;
            for (int i = 0; i < prob.l; i++)
            {
                if (Math.Abs(alpha[i]) > 0)
                {
                    ++nSV;
                    if (prob.y[i] > 0)
                    {
                        if (Math.Abs(alpha[i]) >= si.upper_bound_p)
                            ++nBSV;
                    }
                    else
                    {
                        if (Math.Abs(alpha[i]) >= si.upper_bound_n)
                            ++nBSV;
                    }
                }
            }

            //Console.Out.Write("nSV = " + nSV + ", nBSV = " + nBSV + "\n");

            var f = new decision_function();
            f.alpha = alpha;
            f.rho = si.rho;
            return f;
        }

        // Platt's binary SVM Probablistic Output: an improvement from Lin et al.
        private static void sigmoid_train(int l, double[] dec_values, double[] labels, double[] probAB)
        {
            double A, B;
            double prior1 = 0, prior0 = 0;
            int i;

            for (i = 0; i < l; i++)
                if (labels[i] > 0)
                    prior1 += 1;
                else
                    prior0 += 1;

            int max_iter = 100; // Maximal number of iterations
            double min_step = 1e-10; // Minimal step taken in line search
            double sigma = 1e-3; // For numerically strict PD of Hessian
            double eps = 1e-5;
            double hiTarget = (prior1 + 1.0) / (prior1 + 2.0);
            double loTarget = 1 / (prior0 + 2.0);
            var t = new double[l];
            double fApB, p, q, h11, h22, h21, g1, g2, det, dA, dB, gd, stepsize;
            double newA, newB, newf, d1, d2;
            int iter;

            // Initial Point and Initial Fun Value
            A = 0.0;
            B = Math.Log((prior0 + 1.0) / (prior1 + 1.0));
            double fval = 0.0;

            for (i = 0; i < l; i++)
            {
                if (labels[i] > 0)
                    t[i] = hiTarget;
                else
                    t[i] = loTarget;
                fApB = dec_values[i] * A + B;
                if (fApB >= 0)
                    fval += t[i] * fApB + Math.Log(1 + Math.Exp(-fApB));
                else
                    fval += (t[i] - 1) * fApB + Math.Log(1 + Math.Exp(fApB));
            }
            for (iter = 0; iter < max_iter; iter++)
            {
                // Update Gradient and Hessian (use H' = H + sigma I)
                h11 = sigma; // numerically ensures strict PD
                h22 = sigma;
                h21 = 0.0;
                g1 = 0.0;
                g2 = 0.0;
                for (i = 0; i < l; i++)
                {
                    fApB = dec_values[i] * A + B;
                    if (fApB >= 0)
                    {
                        p = Math.Exp(-fApB) / (1.0 + Math.Exp(-fApB));
                        q = 1.0 / (1.0 + Math.Exp(-fApB));
                    }
                    else
                    {
                        p = 1.0 / (1.0 + Math.Exp(fApB));
                        q = Math.Exp(fApB) / (1.0 + Math.Exp(fApB));
                    }
                    d2 = p * q;
                    h11 += dec_values[i] * dec_values[i] * d2;
                    h22 += d2;
                    h21 += dec_values[i] * d2;
                    d1 = t[i] - p;
                    g1 += dec_values[i] * d1;
                    g2 += d1;
                }

                // Stopping Criteria
                if (Math.Abs(g1) < eps && Math.Abs(g2) < eps)
                    break;

                // Finding Newton direction: -inv(H') * g
                det = h11 * h22 - h21 * h21;
                dA = (-(h22 * g1 - h21 * g2)) / det;
                dB = (-((-h21) * g1 + h11 * g2)) / det;
                gd = g1 * dA + g2 * dB;


                stepsize = 1; // Line Search
                while (stepsize >= min_step)
                {
                    newA = A + stepsize * dA;
                    newB = B + stepsize * dB;

                    // New function value
                    newf = 0.0;
                    for (i = 0; i < l; i++)
                    {
                        fApB = dec_values[i] * newA + newB;
                        if (fApB >= 0)
                            newf += t[i] * fApB + Math.Log(1 + Math.Exp(-fApB));
                        else
                            newf += (t[i] - 1) * fApB + Math.Log(1 + Math.Exp(fApB));
                    }
                    // Check sufficient decrease
                    if (newf < fval + 0.0001 * stepsize * gd)
                    {
                        A = newA;
                        B = newB;
                        fval = newf;
                        break;
                    }
                    else
                        stepsize = stepsize / 2.0;
                }

                if (stepsize < min_step)
                {
                    //Console.Error.Write("Line search fails in two-class probability estimates\n");
                    break;
                }
            }

            /*if (iter >= max_iter)
                Console.Error.Write("Reaching maximal iterations in two-class probability estimates\n");*/
            probAB[0] = A;
            probAB[1] = B;
        }

        private static double sigmoid_predict(double decision_value, double A, double B)
        {
            double fApB = decision_value * A + B;
            if (fApB >= 0)
                return Math.Exp(-fApB) / (1.0 + Math.Exp(-fApB));
            else
                return 1.0 / (1 + Math.Exp(fApB));
        }

        // Method 2 from the multiclass_prob paper by Wu, Lin, and Weng
        private static void multiclass_probability(int k, double[][] r, double[] p)
        {
            int t;
            int iter = 0, max_iter = 100;
            var Q = new double[k][];
            for (int i = 0; i < k; i++)
            {
                Q[i] = new double[k];
            }
            var Qp = new double[k];
            double pQp, eps = 0.001;

            for (t = 0; t < k; t++)
            {
                p[t] = 1.0 / k; // Valid if k = 1
                Q[t][t] = 0;
                for (int j = 0; j < t; j++)
                {
                    Q[t][t] += r[j][t] * r[j][t];
                    Q[t][j] = Q[j][t];
                }
                for (int j = t + 1; j < k; j++)
                {
                    Q[t][t] += r[j][t] * r[j][t];
                    Q[t][j] = (-r[j][t]) * r[t][j];
                }
            }
            for (iter = 0; iter < max_iter; iter++)
            {
                // stopping condition, recalculate QP,pQP for numerical accuracy
                pQp = 0;
                for (t = 0; t < k; t++)
                {
                    Qp[t] = 0;
                    for (int j = 0; j < k; j++)
                        Qp[t] += Q[t][j] * p[j];
                    pQp += p[t] * Qp[t];
                }
                double max_error = 0;
                for (t = 0; t < k; t++)
                {
                    double error = Math.Abs(Qp[t] - pQp);
                    if (error > max_error)
                        max_error = error;
                }
                if (max_error < eps)
                    break;

                for (t = 0; t < k; t++)
                {
                    double diff = (-Qp[t] + pQp) / Q[t][t];
                    p[t] += diff;
                    pQp = (pQp + diff * (diff * Q[t][t] + 2 * Qp[t])) / (1 + diff) / (1 + diff);
                    for (int j = 0; j < k; j++)
                    {
                        Qp[j] = (Qp[j] + diff * Q[t][j]) / (1 + diff);
                        p[j] /= (1 + diff);
                    }
                }
            }
            /*if (iter >= max_iter)
                Console.Error.Write("Exceeds max_iter in multiclass_prob\n");*/
        }

        // Cross-validation decision values for probability estimates
        private static void svm_binary_svc_probability(svm_problem prob, svm_parameter param, double Cp, double Cn,
                                                       double[] probAB)
        {
            int i;
            int nr_fold = 5;
            var perm = new int[prob.l];
            var dec_values = new double[prob.l];

            // random shuffle
            for (i = 0; i < prob.l; i++)
                perm[i] = i;
            for (i = 0; i < prob.l; i++)
            {
                //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1042_3"'
                int j = i + (int)(SupportClass.Random.NextDouble() * (prob.l - i));
                do
                {
                    int _ = perm[i];
                    perm[i] = perm[j];
                    perm[j] = _;
                } while (false);
            }
            for (i = 0; i < nr_fold; i++)
            {
                int begin = i * prob.l / nr_fold;
                int end = (i + 1) * prob.l / nr_fold;
                int j, k;
                var subprob = new svm_problem();

                subprob.l = prob.l - (end - begin);
                subprob.x = new svm_node[subprob.l][];
                subprob.y = new double[subprob.l];

                k = 0;
                for (j = 0; j < begin; j++)
                {
                    subprob.x[k] = prob.x[perm[j]];
                    subprob.y[k] = prob.y[perm[j]];
                    ++k;
                }
                for (j = end; j < prob.l; j++)
                {
                    subprob.x[k] = prob.x[perm[j]];
                    subprob.y[k] = prob.y[perm[j]];
                    ++k;
                }
                int p_count = 0, n_count = 0;
                for (j = 0; j < k; j++)
                    if (subprob.y[j] > 0)
                        p_count++;
                    else
                        n_count++;

                if (p_count == 0 && n_count == 0)
                    for (j = begin; j < end; j++)
                        dec_values[perm[j]] = 0;
                else if (p_count > 0 && n_count == 0)
                    for (j = begin; j < end; j++)
                        dec_values[perm[j]] = 1;
                else if (p_count == 0 && n_count > 0)
                    for (j = begin; j < end; j++)
                        dec_values[perm[j]] = -1;
                else
                {
                    var subparam = (svm_parameter)param.Clone();
                    subparam.probability = 0;
                    subparam.C = 1.0;
                    subparam.nr_weight = 2;
                    subparam.weight_label = new int[2];
                    subparam.weight = new double[2];
                    subparam.weight_label[0] = +1;
                    subparam.weight_label[1] = -1;
                    subparam.weight[0] = Cp;
                    subparam.weight[1] = Cn;
                    svm_model submodel = svm_train(subprob, subparam);
                    for (j = begin; j < end; j++)
                    {
                        var dec_value = new double[1];
                        svm_predict_values(submodel, prob.x[perm[j]], dec_value);
                        dec_values[perm[j]] = dec_value[0];
                        // ensure +1 -1 order; reason not using CV subroutine
                        dec_values[perm[j]] *= submodel.label[0];
                    }
                }
            }
            sigmoid_train(prob.l, dec_values, prob.y, probAB);
        }

        // Return parameter of a Laplace distribution 
        private static double svm_svr_probability(svm_problem prob, svm_parameter param)
        {
            int i;
            int nr_fold = 5;
            var ymv = new double[prob.l];
            double mae = 0;

            var newparam = (svm_parameter)param.Clone();
            newparam.probability = 0;
            svm_cross_validation(prob, newparam, nr_fold, ymv);
            for (i = 0; i < prob.l; i++)
            {
                ymv[i] = prob.y[i] - ymv[i];
                mae += Math.Abs(ymv[i]);
            }
            mae /= prob.l;
            double std = Math.Sqrt(2 * mae * mae);
            int count = 0;
            mae = 0;
            for (i = 0; i < prob.l; i++)
                if (Math.Abs(ymv[i]) > 5 * std)
                    count = count + 1;
                else
                    mae += Math.Abs(ymv[i]);
            mae /= (prob.l - count);
            /*Console.Error.Write(
                "Prob. model for test data: target value = predicted value + z,\nz: Laplace distribution e^(-|z|/sigma)/(2sigma),sigma=" +
                mae + "\n");*/
            return mae;
        }

        //
        // Interface functions
        //
        public static svm_model svm_train(svm_problem prob, svm_parameter param)
        {
            var model = new svm_model();
            model.param = param;

            if (param.svm_type == svm_parameter.ONE_CLASS || param.svm_type == svm_parameter.EPSILON_SVR ||
                param.svm_type == svm_parameter.NU_SVR)
            {
                // regression or one-class-svm
                model.nr_class = 2;
                model.label = null;
                model.nSV = null;
                model.probA = null;
                model.probB = null;
                model.sv_coef = new double[1][];

                if (param.probability == 1 &&
                    (param.svm_type == svm_parameter.EPSILON_SVR || param.svm_type == svm_parameter.NU_SVR))
                {
                    model.probA = new double[1];
                    model.probA[0] = svm_svr_probability(prob, param);
                }

                decision_function f = svm_train_one(prob, param, 0, 0);
                model.rho = new double[1];
                model.rho[0] = f.rho;

                int nSV = 0;
                int i;
                for (i = 0; i < prob.l; i++)
                    if (Math.Abs(f.alpha[i]) > 0)
                        ++nSV;
                model.l = nSV;
                model.SV = new svm_node[nSV][];
                model.sv_coef[0] = new double[nSV];
                int j = 0;
                for (i = 0; i < prob.l; i++)
                    if (Math.Abs(f.alpha[i]) > 0)
                    {
                        model.SV[j] = prob.x[i];
                        model.sv_coef[0][j] = f.alpha[i];
                        ++j;
                    }
            }
            else
            {
                // classification
                // find out the number of classes
                int l = prob.l;
                int max_nr_class = 16;
                int nr_class = 0;
                var label = new int[max_nr_class];
                var count = new int[max_nr_class];
                var index = new int[l];

                int i;
                for (i = 0; i < l; i++)
                {
                    //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1042_3"'
                    var this_label = (int)prob.y[i];
                    int j;
                    for (j = 0; j < nr_class; j++)
                        if (this_label == label[j])
                        {
                            ++count[j];
                            break;
                        }
                    index[i] = j;
                    if (j == nr_class)
                    {
                        if (nr_class == max_nr_class)
                        {
                            max_nr_class *= 2;
                            var new_data = new int[max_nr_class];
                            Array.Copy(label, 0, new_data, 0, label.Length);
                            label = new_data;

                            new_data = new int[max_nr_class];
                            Array.Copy(count, 0, new_data, 0, count.Length);
                            count = new_data;
                        }
                        label[nr_class] = this_label;
                        count[nr_class] = 1;
                        ++nr_class;
                    }
                }

                // group training data of the same class

                var start = new int[nr_class];
                start[0] = 0;
                for (i = 1; i < nr_class; i++)
                    start[i] = start[i - 1] + count[i - 1];

                var x = new svm_node[l][];

                for (i = 0; i < l; i++)
                {
                    x[start[index[i]]] = prob.x[i];
                    ++start[index[i]];
                }

                start[0] = 0;
                for (i = 1; i < nr_class; i++)
                    start[i] = start[i - 1] + count[i - 1];

                // calculate weighted C

                var weighted_C = new double[nr_class];
                for (i = 0; i < nr_class; i++)
                    weighted_C[i] = param.C;
                for (i = 0; i < param.nr_weight; i++)
                {
                    int j;
                    for (j = 0; j < nr_class; j++)
                        if (param.weight_label[i] == label[j])
                            break;
                    if (j == nr_class) ;
                    /*Console.Error.Write("warning: class label " + param.weight_label[i] +
                                        " specified in weight is not found\n");*/
                    else
                        weighted_C[j] *= param.weight[i];
                }

                // train k*(k-1)/2 models

                var nonzero = new bool[l];
                for (i = 0; i < l; i++)
                    nonzero[i] = false;
                var f = new decision_function[nr_class * (nr_class - 1) / 2];

                double[] probA = null, probB = null;
                if (param.probability == 1)
                {
                    probA = new double[nr_class * (nr_class - 1) / 2];
                    probB = new double[nr_class * (nr_class - 1) / 2];
                }

                int p = 0;
                for (i = 0; i < nr_class; i++)
                    for (int j = i + 1; j < nr_class; j++)
                    {
                        var sub_prob = new svm_problem();
                        int si = start[i], sj = start[j];
                        int ci = count[i], cj = count[j];
                        sub_prob.l = ci + cj;
                        sub_prob.x = new svm_node[sub_prob.l][];
                        sub_prob.y = new double[sub_prob.l];
                        int k;
                        for (k = 0; k < ci; k++)
                        {
                            sub_prob.x[k] = x[si + k];
                            sub_prob.y[k] = +1;
                        }
                        for (k = 0; k < cj; k++)
                        {
                            sub_prob.x[ci + k] = x[sj + k];
                            sub_prob.y[ci + k] = -1;
                        }

                        if (param.probability == 1)
                        {
                            var probAB = new double[2];
                            svm_binary_svc_probability(sub_prob, param, weighted_C[i], weighted_C[j], probAB);
                            probA[p] = probAB[0];
                            probB[p] = probAB[1];
                        }

                        f[p] = svm_train_one(sub_prob, param, weighted_C[i], weighted_C[j]);
                        for (k = 0; k < ci; k++)
                            if (!nonzero[si + k] && Math.Abs(f[p].alpha[k]) > 0)
                                nonzero[si + k] = true;
                        for (k = 0; k < cj; k++)
                            if (!nonzero[sj + k] && Math.Abs(f[p].alpha[ci + k]) > 0)
                                nonzero[sj + k] = true;
                        ++p;
                    }

                // build output

                model.nr_class = nr_class;

                model.label = new int[nr_class];
                for (i = 0; i < nr_class; i++)
                    model.label[i] = label[i];

                model.rho = new double[nr_class * (nr_class - 1) / 2];
                for (i = 0; i < nr_class * (nr_class - 1) / 2; i++)
                    model.rho[i] = f[i].rho;

                if (param.probability == 1)
                {
                    model.probA = new double[nr_class * (nr_class - 1) / 2];
                    model.probB = new double[nr_class * (nr_class - 1) / 2];
                    for (i = 0; i < nr_class * (nr_class - 1) / 2; i++)
                    {
                        model.probA[i] = probA[i];
                        model.probB[i] = probB[i];
                    }
                }
                else
                {
                    model.probA = null;
                    model.probB = null;
                }

                int nnz = 0;
                var nz_count = new int[nr_class];
                model.nSV = new int[nr_class];
                for (i = 0; i < nr_class; i++)
                {
                    int nSV = 0;
                    for (int j = 0; j < count[i]; j++)
                        if (nonzero[start[i] + j])
                        {
                            ++nSV;
                            ++nnz;
                        }
                    model.nSV[i] = nSV;
                    nz_count[i] = nSV;
                }

                //Console.Out.Write("Total nSV = " + nnz + "\n");

                model.l = nnz;
                model.SV = new svm_node[nnz][];
                p = 0;
                for (i = 0; i < l; i++)
                    if (nonzero[i])
                        model.SV[p++] = x[i];

                var nz_start = new int[nr_class];
                nz_start[0] = 0;
                for (i = 1; i < nr_class; i++)
                    nz_start[i] = nz_start[i - 1] + nz_count[i - 1];

                model.sv_coef = new double[nr_class - 1][];
                for (i = 0; i < nr_class - 1; i++)
                    model.sv_coef[i] = new double[nnz];

                p = 0;
                for (i = 0; i < nr_class; i++)
                    for (int j = i + 1; j < nr_class; j++)
                    {
                        // classifier (i,j): coefficients with
                        // i are in sv_coef[j-1][nz_start[i]...],
                        // j are in sv_coef[i][nz_start[j]...]

                        int si = start[i];
                        int sj = start[j];
                        int ci = count[i];
                        int cj = count[j];

                        int q = nz_start[i];
                        int k;
                        for (k = 0; k < ci; k++)
                            if (nonzero[si + k])
                                model.sv_coef[j - 1][q++] = f[p].alpha[k];
                        q = nz_start[j];
                        for (k = 0; k < cj; k++)
                            if (nonzero[sj + k])
                                model.sv_coef[i][q++] = f[p].alpha[ci + k];
                        ++p;
                    }
            }
            return model;
        }

        public static void svm_cross_validation(svm_problem prob, svm_parameter param, int nr_fold, double[] target)
        {
            int i;
            var perm = new int[prob.l];

            // random shuffle
            for (i = 0; i < prob.l; i++)
                perm[i] = i;
            for (i = 0; i < prob.l; i++)
            {
                //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1042_3"'
                int j = i + (int)(SupportClass.Random.NextDouble() * (prob.l - i));
                do
                {
                    int _ = perm[i];
                    perm[i] = perm[j];
                    perm[j] = _;
                } while (false);
            }
            for (i = 0; i < nr_fold; i++)
            {
                int begin = i * prob.l / nr_fold;
                int end = (i + 1) * prob.l / nr_fold;
                int j, k;
                var subprob = new svm_problem();

                subprob.l = prob.l - (end - begin);
                subprob.x = new svm_node[subprob.l][];
                subprob.y = new double[subprob.l];

                k = 0;
                for (j = 0; j < begin; j++)
                {
                    subprob.x[k] = prob.x[perm[j]];
                    subprob.y[k] = prob.y[perm[j]];
                    ++k;
                }
                for (j = end; j < prob.l; j++)
                {
                    subprob.x[k] = prob.x[perm[j]];
                    subprob.y[k] = prob.y[perm[j]];
                    ++k;
                }
                svm_model submodel = svm_train(subprob, param);
                if (param.probability == 1 &&
                    (param.svm_type == svm_parameter.C_SVC || param.svm_type == svm_parameter.NU_SVC))
                {
                    var prob_estimates = new double[svm_get_nr_class(submodel)];
                    for (j = begin; j < end; j++)
                        target[perm[j]] = svm_predict_probability(submodel, prob.x[perm[j]], prob_estimates);
                }
                else
                    for (j = begin; j < end; j++)
                        target[perm[j]] = svm_predict(submodel, prob.x[perm[j]]);
            }
        }

        public static int svm_get_svm_type(svm_model model)
        {
            return model.param.svm_type;
        }

        public static int svm_get_nr_class(svm_model model)
        {
            return model.nr_class;
        }

        public static void svm_get_labels(svm_model model, int[] label)
        {
            if (model.label != null)
                for (int i = 0; i < model.nr_class; i++)
                    label[i] = model.label[i];
        }

        public static double svm_get_svr_probability(svm_model model)
        {
            if ((model.param.svm_type == svm_parameter.EPSILON_SVR || model.param.svm_type == svm_parameter.NU_SVR) &&
                model.probA != null)
                return model.probA[0];
            else
            {
                //Console.Error.Write("Model doesn't contain information for SVR probability inference\n");
                return 0;
            }
        }

        public static void svm_predict_values(svm_model model, svm_node[] x, double[] dec_values)
        {
            if (model.param.svm_type == svm_parameter.ONE_CLASS || model.param.svm_type == svm_parameter.EPSILON_SVR ||
                model.param.svm_type == svm_parameter.NU_SVR)
            {
                double[] sv_coef = model.sv_coef[0];
                double sum = 0;
                for (int i = 0; i < model.l; i++)
                    sum += sv_coef[i] * Kernel.k_function(x, model.SV[i], model.param);
                sum -= model.rho[0];
                dec_values[0] = sum;
            }
            else
            {
                int i;
                int nr_class = model.nr_class;
                int l = model.l;

                var kvalue = new double[l];
                for (i = 0; i < l; i++)
                    kvalue[i] = Kernel.k_function(x, model.SV[i], model.param);

                var start = new int[nr_class];
                start[0] = 0;
                for (i = 1; i < nr_class; i++)
                    start[i] = start[i - 1] + model.nSV[i - 1];

                int p = 0;
                int pos = 0;
                for (i = 0; i < nr_class; i++)
                    for (int j = i + 1; j < nr_class; j++)
                    {
                        double sum = 0;
                        int si = start[i];
                        int sj = start[j];
                        int ci = model.nSV[i];
                        int cj = model.nSV[j];

                        int k;
                        double[] coef1 = model.sv_coef[j - 1];
                        double[] coef2 = model.sv_coef[i];
                        for (k = 0; k < ci; k++)
                            sum += coef1[si + k] * kvalue[si + k];
                        for (k = 0; k < cj; k++)
                            sum += coef2[sj + k] * kvalue[sj + k];
                        sum -= model.rho[p++];
                        dec_values[pos++] = sum;
                    }
            }
        }

        public static double svm_predict(svm_model model, svm_node[] x)
        {
            if (model.param.svm_type == svm_parameter.ONE_CLASS || model.param.svm_type == svm_parameter.EPSILON_SVR ||
                model.param.svm_type == svm_parameter.NU_SVR)
            {
                var res = new double[1];
                svm_predict_values(model, x, res);

                if (model.param.svm_type == svm_parameter.ONE_CLASS)
                    return (res[0] > 0) ? 1 : -1;
                else
                    return res[0];
            }
            else
            {
                int i;
                int nr_class = model.nr_class;
                var dec_values = new double[nr_class * (nr_class - 1) / 2];
                svm_predict_values(model, x, dec_values);

                var vote = new int[nr_class];
                for (i = 0; i < nr_class; i++)
                    vote[i] = 0;
                int pos = 0;
                for (i = 0; i < nr_class; i++)
                    for (int j = i + 1; j < nr_class; j++)
                    {
                        if (dec_values[pos++] > 0)
                            ++vote[i];
                        else
                            ++vote[j];
                    }

                int vote_max_idx = 0;
                for (i = 1; i < nr_class; i++)
                    if (vote[i] > vote[vote_max_idx])
                        vote_max_idx = i;
                return model.label[vote_max_idx];
            }
        }

        public static double svm_predict_probability(svm_model model, svm_node[] x, double[] prob_estimates)
        {
            if ((model.param.svm_type == svm_parameter.C_SVC || model.param.svm_type == svm_parameter.NU_SVC) &&
                model.probA != null && model.probB != null)
            {
                int i;
                int nr_class = model.nr_class;
                var dec_values = new double[nr_class * (nr_class - 1) / 2];
                svm_predict_values(model, x, dec_values);

                double min_prob = 1e-7;
                var tmpArray = new double[nr_class][];
                for (int i2 = 0; i2 < nr_class; i2++)
                {
                    tmpArray[i2] = new double[nr_class];
                }
                double[][] pairwise_prob = tmpArray;

                int k = 0;
                for (i = 0; i < nr_class; i++)
                    for (int j = i + 1; j < nr_class; j++)
                    {
                        pairwise_prob[i][j] =
                            Math.Min(
                                Math.Max(sigmoid_predict(dec_values[k], model.probA[k], model.probB[k]), min_prob),
                                1 - min_prob);
                        pairwise_prob[j][i] = 1 - pairwise_prob[i][j];
                        k++;
                    }
                multiclass_probability(nr_class, pairwise_prob, prob_estimates);

                int prob_max_idx = 0;
                for (i = 1; i < nr_class; i++)
                    if (prob_estimates[i] > prob_estimates[prob_max_idx])
                        prob_max_idx = i;
                return model.label[prob_max_idx];
            }
            else
                return svm_predict(model, x);
        }

        //UPGRADE_NOTE: Final was removed from the declaration of 'svm_type_table'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003_3"'
        internal static readonly String[] svm_type_table = new[]
                                                               {"c_svc", "nu_svc", "one_class", "epsilon_svr", "nu_svr"};

        //UPGRADE_NOTE: Final was removed from the declaration of 'kernel_type_table'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003_3"'
        internal static readonly String[] kernel_type_table = new[] { "linear", "polynomial", "rbf", "sigmoid" };

        public static void svm_save_model(StreamWriter fp, svm_model model)
        {
            svm_parameter param = model.param;

            fp.Write("svm_type " + svm_type_table[param.svm_type] + "\n");
            fp.Write("kernel_type " + kernel_type_table[param.kernel_type] + "\n");

            if (param.kernel_type == svm_parameter.POLY)
                fp.Write("degree " + param.degree + "\n");

            if (param.kernel_type == svm_parameter.POLY || param.kernel_type == svm_parameter.RBF ||
                param.kernel_type == svm_parameter.SIGMOID)
                fp.Write("gamma " + param.gamma + "\n");

            if (param.kernel_type == svm_parameter.POLY || param.kernel_type == svm_parameter.SIGMOID)
                fp.Write("coef0 " + param.coef0 + "\n");

            int nr_class = model.nr_class;
            int l = model.l;
            fp.Write("nr_class " + nr_class + "\n");
            fp.Write("total_sv " + l + "\n");

            {
                fp.Write("rho");
                for (int i = 0; i < nr_class * (nr_class - 1) / 2; i++)
                    fp.Write(" " + model.rho[i]);
                fp.Write("\n");
            }

            if (model.label != null)
            {
                fp.Write("label");
                for (int i = 0; i < nr_class; i++)
                    fp.Write(" " + model.label[i]);
                fp.Write("\n");
            }

            if (model.probA != null)
            // regression has probA only
            {
                fp.Write("probA");
                for (int i = 0; i < nr_class * (nr_class - 1) / 2; i++)
                    fp.Write(" " + model.probA[i]);
                fp.Write("\n");
            }
            if (model.probB != null)
            {
                fp.Write("probB");
                for (int i = 0; i < nr_class * (nr_class - 1) / 2; i++)
                    fp.Write(" " + model.probB[i]);
                fp.Write("\n");
            }

            if (model.nSV != null)
            {
                fp.Write("nr_sv");
                for (int i = 0; i < nr_class; i++)
                    fp.Write(" " + model.nSV[i]);
                fp.Write("\n");
            }

            fp.Write("SV\n");
            double[][] sv_coef = model.sv_coef;
            svm_node[][] SV = model.SV;

            for (int i = 0; i < l; i++)
            {
                for (int j = 0; j < nr_class - 1; j++)
                    fp.Write(sv_coef[j][i] + " ");

                svm_node[] p = SV[i];
                for (int j = 0; j < p.Length; j++)
                    fp.Write(p[j].index + ":" + p[j].value_Renamed + " ");
                fp.Write("\n");
            }

            fp.Close();
        }

        private static double atof(String s)
        {
            return Double.Parse(s);
        }

        private static int atoi(String s)
        {
            return Int32.Parse(s);
        }

        public static svm_model svm_load_model(StringReader fp)
        {
            // read parameters

            var model = new svm_model();
            var param = new svm_parameter();
            model.param = param;
            model.rho = null;
            model.probA = null;
            model.probB = null;
            model.label = null;
            model.nSV = null;

            while (true)
            {
                String cmd = fp.ReadLine();
                String arg = cmd.Substring(cmd.IndexOf(' ') + 1);

                if (cmd.StartsWith("svm_type"))
                {
                    int i;
                    for (i = 0; i < svm_type_table.Length; i++)
                    {
                        if (arg.IndexOf(svm_type_table[i]) != -1)
                        {
                            param.svm_type = i;
                            break;
                        }
                    }
                    if (i == svm_type_table.Length)
                    {
                        //Console.Error.Write("unknown svm type.\n");
                        return null;
                    }
                }
                else if (cmd.StartsWith("kernel_type"))
                {
                    int i;
                    for (i = 0; i < kernel_type_table.Length; i++)
                    {
                        if (arg.IndexOf(kernel_type_table[i]) != -1)
                        {
                            param.kernel_type = i;
                            break;
                        }
                    }
                    if (i == kernel_type_table.Length)
                    {
                        //Console.Error.Write("unknown kernel function.\n");
                        return null;
                    }
                }
                else if (cmd.StartsWith("degree"))
                    param.degree = atof(arg);
                else if (cmd.StartsWith("gamma"))
                    param.gamma = atof(arg);
                else if (cmd.StartsWith("coef0"))
                    param.coef0 = atof(arg);
                else if (cmd.StartsWith("nr_class"))
                    model.nr_class = atoi(arg);
                else if (cmd.StartsWith("total_sv"))
                    model.l = atoi(arg);
                else if (cmd.StartsWith("rho"))
                {
                    int n = model.nr_class * (model.nr_class - 1) / 2;
                    model.rho = new double[n];
                    var st = new SupportClass.Tokenizer(arg);
                    for (int i = 0; i < n; i++)
                        model.rho[i] = atof(st.NextToken());
                }
                else if (cmd.StartsWith("label"))
                {
                    int n = model.nr_class;
                    model.label = new int[n];
                    var st = new SupportClass.Tokenizer(arg);
                    for (int i = 0; i < n; i++)
                        model.label[i] = atoi(st.NextToken());
                }
                else if (cmd.StartsWith("probA"))
                {
                    int n = model.nr_class * (model.nr_class - 1) / 2;
                    model.probA = new double[n];
                    var st = new SupportClass.Tokenizer(arg);
                    for (int i = 0; i < n; i++)
                        model.probA[i] = atof(st.NextToken());
                }
                else if (cmd.StartsWith("probB"))
                {
                    int n = model.nr_class * (model.nr_class - 1) / 2;
                    model.probB = new double[n];
                    var st = new SupportClass.Tokenizer(arg);
                    for (int i = 0; i < n; i++)
                        model.probB[i] = atof(st.NextToken());
                }
                else if (cmd.StartsWith("nr_sv"))
                {
                    int n = model.nr_class;
                    model.nSV = new int[n];
                    var st = new SupportClass.Tokenizer(arg);
                    for (int i = 0; i < n; i++)
                        model.nSV[i] = atoi(st.NextToken());
                }
                else if (cmd.StartsWith("SV"))
                {
                    break;
                }
                else
                {
                    //Console.Error.Write("unknown text in model file\n");
                    return null;
                }
            }

            // read sv_coef and SV

            int m = model.nr_class - 1;
            int l = model.l;
            model.sv_coef = new double[m][];
            for (int i = 0; i < m; i++)
            {
                model.sv_coef[i] = new double[l];
            }
            model.SV = new svm_node[l][];

            for (int i = 0; i < l; i++)
            {
                String line = fp.ReadLine();
                var st = new SupportClass.Tokenizer(line, " \t\n\r\f:");

                for (int k = 0; k < m; k++)
                    model.sv_coef[k][i] = atof(st.NextToken());
                int n = st.Count / 2;
                model.SV[i] = new svm_node[n];
                for (int j = 0; j < n; j++)
                {
                    model.SV[i][j] = new svm_node();
                    model.SV[i][j].index = atoi(st.NextToken());
                    model.SV[i][j].value_Renamed = atof(st.NextToken());
                }
            }

            return model;
        }

        public static String svm_check_parameter(svm_problem prob, svm_parameter param)
        {
            // svm_type

            int svm_type = param.svm_type;
            if (svm_type != svm_parameter.C_SVC && svm_type != svm_parameter.NU_SVC &&
                svm_type != svm_parameter.ONE_CLASS && svm_type != svm_parameter.EPSILON_SVR &&
                svm_type != svm_parameter.NU_SVR)
                return "unknown svm type";

            // kernel_type

            int kernel_type = param.kernel_type;
            if (kernel_type != svm_parameter.LINEAR && kernel_type != svm_parameter.POLY &&
                kernel_type != svm_parameter.RBF && kernel_type != svm_parameter.SIGMOID)
                return "unknown kernel type";

            // cache_size,eps,C,nu,p,shrinking

            if (param.cache_size <= 0)
                return "cache_size <= 0";

            if (param.eps <= 0)
                return "eps <= 0";

            if (svm_type == svm_parameter.C_SVC || svm_type == svm_parameter.EPSILON_SVR ||
                svm_type == svm_parameter.NU_SVR)
                if (param.C <= 0)
                    return "C <= 0";

            if (svm_type == svm_parameter.NU_SVC || svm_type == svm_parameter.ONE_CLASS ||
                svm_type == svm_parameter.NU_SVR)
                if (param.nu < 0 || param.nu > 1)
                    return "nu < 0 or nu > 1";

            if (svm_type == svm_parameter.EPSILON_SVR)
                if (param.p < 0)
                    return "p < 0";

            if (param.shrinking != 0 && param.shrinking != 1)
                return "shrinking != 0 and shrinking != 1";

            if (param.probability != 0 && param.probability != 1)
                return "probability != 0 and probability != 1";

            if (param.probability == 1 && svm_type == svm_parameter.ONE_CLASS)
                return "one-class SVM probability output not supported yet";

            // check whether nu-svc is feasible

            if (svm_type == svm_parameter.NU_SVC)
            {
                int l = prob.l;
                int max_nr_class = 16;
                int nr_class = 0;
                var label = new int[max_nr_class];
                var count = new int[max_nr_class];

                int i;
                for (i = 0; i < l; i++)
                {
                    //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1042_3"'
                    var this_label = (int)prob.y[i];
                    int j;
                    for (j = 0; j < nr_class; j++)
                        if (this_label == label[j])
                        {
                            ++count[j];
                            break;
                        }

                    if (j == nr_class)
                    {
                        if (nr_class == max_nr_class)
                        {
                            max_nr_class *= 2;
                            var new_data = new int[max_nr_class];
                            Array.Copy(label, 0, new_data, 0, label.Length);
                            label = new_data;

                            new_data = new int[max_nr_class];
                            Array.Copy(count, 0, new_data, 0, count.Length);
                            count = new_data;
                        }
                        label[nr_class] = this_label;
                        count[nr_class] = 1;
                        ++nr_class;
                    }
                }

                for (i = 0; i < nr_class; i++)
                {
                    int n1 = count[i];
                    for (int j = i + 1; j < nr_class; j++)
                    {
                        int n2 = count[j];
                        if (param.nu * (n1 + n2) / 2 > Math.Min(n1, n2))
                            return "specified nu is infeasible";
                    }
                }
            }

            return null;
        }

        public static int svm_check_probability_model(svm_model model)
        {
            if (((model.param.svm_type == svm_parameter.C_SVC || model.param.svm_type == svm_parameter.NU_SVC) &&
                 model.probA != null && model.probB != null) ||
                ((model.param.svm_type == svm_parameter.EPSILON_SVR || model.param.svm_type == svm_parameter.NU_SVR) &&
                 model.probA != null))
                return 1;
            else
                return 0;
        }
    }
}
