﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SyntDirectoryPersistence
    {
        /// <summary>
        /// The directory that holds the EG files.
        /// </summary>
        ///
        private readonly FileInfo _parent;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="parent">The directory to use.</param>
        public SyntDirectoryPersistence(FileInfo parent)
        {
            _parent = parent;
        }

        /// <value>The directory.</value>
        public FileInfo Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Load the specified object.
        /// </summary>
        ///
        /// <param name="file">The file to load.</param>
        /// <returns>The loaded object.</returns>
        public static Object LoadObject(FileInfo file)
        {
            FileStream fis = null;

            try
            {
                fis = file.OpenRead();
                Object result = LoadObject(fis);

                return result;
            }
            catch (IOException ex)
            {
                throw new PersistError(ex);
            }
            finally
            {
                if (fis != null)
                {
                    try
                    {
                        fis.Close();
                    }
                    catch (IOException e)
                    {
                        SyntLogging.Log(e);
                    }
                }
            }
        }

        /// <summary>
        /// Load an EG object as a reousrce.
        /// </summary>
        /// <param name="res">The resource to load.</param>
        /// <returns>The loaded object.</returns>
        public static Object LoadResourceObject(String res)
        {
            using (Stream s = ResourceLoader.CreateStream(res))
            {
                return LoadObject(s);
            }
        }

        /// <summary>
        /// Load an object from an input stream.
        /// </summary>
        ///
        /// <param name="mask0">The input stream to read from.</param>
        /// <returns>The loaded object.</returns>
        public static Object LoadObject(Stream mask0)
        {
            String header = ReadLine(mask0);
            String[] paras = header.Split(',');

            if (!"Synt".Equals(paras[0]))
            {
                throw new PersistError("Not a valid EG file.");
            }

            String name = paras[1];

            ISyntPersistor p = PersistorRegistry.Instance.GetPersistor(
                name);

            if (p == null)
            {

            }

            if (p.FileVersion < Int32.Parse(paras[4]))
            {

            }

            return p.Read(mask0);
        }

        /// <summary>
        /// Read a line from the input stream.
        /// </summary>
        ///
        /// <param name="mask0">The input stream.</param>
        /// <returns>The line read.</returns>
        private static String ReadLine(Stream mask0)
        {
            try
            {
                var result = new StringBuilder();

                char ch;

                do
                {
                    int b = mask0.ReadByte();
                    if (b == -1)
                    {
                        return result.ToString();
                    }

                    ch = (char)b;

                    if ((ch != 13) && (ch != 10))
                    {
                        result.Append(ch);
                    }
                } while (ch != 10);

                return result.ToString();
            }
            catch (IOException ex)
            {
                throw new PersistError(ex);
            }
        }

        /// <summary>
        /// Save the specified object.
        /// </summary>
        ///
        /// <param name="filename">The filename to save to.</param>
        /// <param name="obj">The Object to save.</param>
        public static void SaveObject(FileInfo filename, Object obj)
        {
            FileStream fos = null;

            try
            {
                filename.Delete();
                fos = filename.OpenWrite();
                SaveObject(fos, obj);
            }
            catch (IOException ex)
            {
                throw new PersistError(ex);
            }
            finally
            {
                try
                {
                    if (fos != null)
                    {
                        fos.Close();
                    }
                }
                catch (IOException e)
                {
                    SyntLogging.Log(e);
                }
            }
        }

        /// <summary>
        /// Save the specified object.
        /// </summary>
        ///
        /// <param name="os">The output stream to write to.</param>
        /// <param name="obj">The object to save.</param>
        public static void SaveObject(Stream os, Object obj)
        {
            try
            {
                ISyntPersistor p = PersistorRegistry.Instance
                    .GetPersistor(obj.GetType());

                if (p == null)
                {
                    throw new PersistError("Do not know how to persist object: "
                                           + obj.GetType().Name);
                }

                os.Flush();
                var pw = new StreamWriter(os);
                DateTime now = DateTime.Now;
                pw.WriteLine("Synt," + p.PersistClassString + ",java,"
                             + SyntFramework.Version + "," + p.FileVersion + ","
                             + (now.Ticks / 10000));
                pw.Flush();
                p.Save(os, obj);
            }
            catch (IOException ex)
            {
                throw new PersistError(ex);
            }
        }

        /// <summary>
        /// Get the type of an Synt object in an EG file, without the 
        /// need to read the entire file.
        /// </summary>
        ///
        /// <param name="name">The filename to read.</param>
        /// <returns>The type.</returns>
        public String GetSyntType(String name)
        {
            try
            {
                var path = new FileInfo(Path.Combine(_parent.FullName, name));
                TextReader br = new StreamReader(path.OpenRead());
                String header = br.ReadLine();
                String[] paras = header.Split(',');
                br.Close();
                return paras[1];
            }
            catch (IOException ex)
            {
                throw new PersistError(ex);
            }
        }


        /// <summary>
        /// Load a file from the directory that this object refers to.
        /// </summary>
        ///
        /// <param name="name">The name to load.</param>
        /// <returns>The object.</returns>
        public Object LoadFromDirectory(String name)
        {
            var path = new FileInfo(Path.Combine(_parent.FullName, name));
            return LoadObject(path);
        }

        /// <summary>
        /// Save a file to the directory that this object refers to.
        /// </summary>
        ///
        /// <param name="name">The name to load.</param>
        /// <param name="obj">The object.</param>
        public void SaveToDirectory(String name, Object obj)
        {
            var path = new FileInfo(Path.Combine(_parent.FullName, name));
            SaveObject(path, obj);
        }
    }
}
