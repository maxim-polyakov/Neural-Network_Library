using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ReflectionUtil
    {
        /// <summary>
        /// Path to the activation functions.
        /// </summary>
        public const String AfPath = "Synt.Engine.Network.Activation.";

        /// <summary>
        /// Path to RBF's.
        /// </summary>
        public const String RBFPath = "Synt.MathUtil.RBF.";

        /// <summary>
        /// A map between short class names and the full path names.
        /// </summary>
        private static readonly IDictionary<String, String> ClassMap = new Dictionary<String, String>();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private ReflectionUtil()
        {
        }


        /// <summary>
        /// Find the specified field, look also in superclasses.
        /// </summary>
        /// <param name="c">The class to search.</param>
        /// <param name="name">The name of the field we are looking for.</param>
        /// <returns>The field.</returns>
        public static FieldInfo FindField(Type c, String name)
        {
            ICollection<FieldInfo> list = GetAllFields(c);
            return list.FirstOrDefault(field => field.Name.Equals(name));
        }

        /// <summary>
        /// Get all of the fields from the specified class as a collection.
        /// </summary>
        /// <param name="c">The class to access.</param>
        /// <returns>All of the fields from this class and subclasses.</returns>
        public static IList<FieldInfo> GetAllFields(Type c)
        {
            IList<FieldInfo> result = new List<FieldInfo>();
            GetAllFields(c, result);
            return result;
        }

        /// <summary>
        /// Get all of the fields for the specified class and recurse to check the base class.
        /// </summary>
        /// <param name="c">The class to scan.</param>
        /// <param name="result">A list of fields.</param>
        public static void GetAllFields(Type c, IList<FieldInfo> result)
        {
            foreach (
                FieldInfo field in c.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                result.Add(field);
            }

            if (c.BaseType != null)
                GetAllFields(c.BaseType, result);
        }

        /// <summary>
        /// Load the classmap file. This allows classes to be resolved using just the
        /// simple name.
        /// </summary>
        public static void LoadClassmap()
        {
            {
                Stream istream = ResourceLoader.CreateStream("Synt.Resources.classes.txt");
                var sr = new StreamReader(istream);

                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    int idx = line.LastIndexOf('.');
                    if (idx != -1)
                    {
                        String simpleName = line.Substring(idx + 1);
                        ClassMap[simpleName] = line;
                    }
                }
                sr.Close();
                istream.Close();
            }
        }

        /// <summary>
        /// Resolve an Synt class using its simple name.
        /// </summary>
        /// <param name="name">The simple name of the class.</param>
        /// <returns>The class requested.</returns>
        public static String ResolveSyntClass(String name)
        {
            if (ClassMap.Count == 0)
            {
                LoadClassmap();
            }

            return !ClassMap.ContainsKey(name) ? null : ClassMap[name];
        }


        /// <summary>
        /// Determine if the specified field has the specified attribute.
        /// </summary>
        /// <param name="field">The field to check.</param>
        /// <param name="t">See if the field has this attribute.</param>
        /// <returns>True if the field has the specified attribute.</returns>
        public static bool HasAttribute(FieldInfo field, Type t)
        {
            return field.GetCustomAttributes(true).Any(obj => obj.GetType() == t);
        }


        /// <summary>
        /// Determine if the specified type contains the specified attribute.
        /// </summary>
        /// <param name="t">The type.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns>True if the type contains the attribute.</returns>
        public static bool HasAttribute(Type t, Type attribute)
        {
            return t.GetCustomAttributes(true).Any(obj => obj.GetType() == attribute);
        }

        /// <summary>
        /// Resolve an enumeration.
        /// </summary>
        /// <param name="field">The field to resolve.</param>
        /// <param name="v">The value to get the enum for.</param>
        /// <returns>The enum that was resolved.</returns>
        public static Object ResolveEnum(FieldInfo field, FieldInfo v)
        {
            Type type = field.GetType();
            Object[] objs = type.GetMembers(BindingFlags.Public | BindingFlags.Static);
            return objs.Cast<MemberInfo>().FirstOrDefault(obj => obj.Name.Equals(v));
        }

        /// <summary>
        /// Loop over all loaded assembles and try to create the class.
        /// </summary>
        /// <param name="name">The class to create.</param>
        /// <returns>The created class.</returns>
        public static Object LoadObject(String name)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Object result = null;

            foreach (Assembly a in assemblies)
            {
                result = a.CreateInstance(name);
                if (result != null)
                    break;
            }

            return result;
        }
    }
}
