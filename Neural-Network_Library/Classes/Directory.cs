﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public static class Directory
    {
        /// <summary>
        /// Default buffer size for read/write operations.
        /// </summary>
        ///
        public const int BufferSize = 1024;

        /// <summary>
        /// Copy the specified file.
        /// </summary>
        ///
        /// <param name="source">The file to copy.</param>
        /// <param name="target">The target of the copy.</param>
        public static void CopyFile(FileInfo source, FileInfo target)
        {
            try
            {
                var buffer = new byte[BufferSize];

                // open the files before the copy
                FileStream
                    ins0 = source.OpenRead();
                target.Delete();
                FileStream xout = target.OpenWrite();

                // perform the copy
                int packetSize = 0;

                while (packetSize != -1)
                {
                    packetSize = ins0.Read(buffer, 0, buffer.Length);
                    if (packetSize != -1)
                    {
                        xout.Write(buffer, 0, packetSize);
                    }
                }

                // close the files after the copy
                ins0.Close();
                xout.Close();
            }
            catch (IOException e)
            {
                throw new SyntError(e);
            }
        }

        /// <summary>
        /// Delete a directory and all children.
        /// </summary>
        ///
        /// <param name="path">The path to delete.</param>
        /// <returns>True if successful.</returns>
        public static bool DeleteDirectory(FileInfo path)
        {
            return DeleteDirectory(path);
        }

        /// <summary>
        /// Read the entire contents of a stream into a string.
        /// </summary>
        ///
        /// <param name="mask0">The input stream to read from.</param>
        /// <returns>The string that was read in.</returns>
        public static String ReadStream(Stream mask0)
        {
            try
            {
                var sb = new StringBuilder(1024);
                TextReader reader = new StreamReader(mask0);

                var chars = new char[BufferSize];
                int numRead;
                while ((numRead = reader.Read(chars, 0, chars.Length)) > -1)
                {
                    sb.Append(new String(chars, 0, numRead));
                }
                reader.Close();

                return sb.ToString();
            }
            catch (IOException e)
            {
                throw new SyntError(e);
            }
        }

        /// <summary>
        /// Read the entire contents of a stream into a string.
        /// </summary>
        ///
        /// <param name="filename">The input stream to read from.</param>
        /// <returns>The string that was read in.</returns>
        public static String ReadTextFile(String filename)
        {
            try
            {
                var result = new StringBuilder();
                using (var sr = new StreamReader(filename))
                {
                    String line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        result.Append(line);
                        result.Append("\r\n");
                    }
                }
                return result.ToString();
            }
            catch (IOException e)
            {
                throw new SyntError(e);
            }
        }
    }
}
