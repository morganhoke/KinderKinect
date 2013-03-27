using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KinderKinect.Utils
{
    /// <summary>
    /// Outputs a log for debugging purposes
    /// </summary>
    class Logger
    {
        /// <summary>
        /// The stream out
        /// </summary>
        StreamWriter outputStream;

        /// <summary>
        /// The patht to the file we want to write to
        /// </summary>
        Uri outFilePath;


        public Logger(Uri path)
        {
            outFilePath = path;
        }

        /// <summary>
        /// Writes a line to our file
        /// </summary>
        /// <param name="line">The line to write</param>
        public void WriteLine(string line)
        {
            using (outputStream = File.CreateText(outFilePath.OriginalString))
            {
                outputStream.WriteLine(line);
            }
        }
    }
}
