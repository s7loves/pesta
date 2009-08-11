using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CustomBuildTasks
{
    /// <summary>
    /// Yui Compress MSBuild Task
    /// Adapted from http://www.coderjournal.com/2008/05/how-to-create-a-yui-compressor-msbuild-task/
    /// </summary>
    public class YuiCompress : Task
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [Required]
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the files.
        /// </summary>
        /// <value>The files.</value>
        [Required]
        public ITaskItem[] Files
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show warnings].
        /// </summary>
        /// <value><c>true</c> if [show warnings]; otherwise, <c>false</c>.</value>
        public bool ShowWarnings
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the compressed files.
        /// </summary>
        /// <value>The compressed files.</value>
        [Output]
        public ITaskItem[] CompressedFiles
        {
            get;
            set;
        }

        /// <summary>
        /// Formats the warning.
        /// </summary>
        /// <param name="warning">The warning.</param>
        /// <returns></returns>
        private string FormatWarning(string warning)
        {
            return warning
                .Trim()
                .Replace("[WARNING] ", String.Empty);
        }

        /// <summary>
        /// Compresses the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="type"></param>
        private string Compress(ITaskItem file, string type)
        {
            string[] warnings;
            string oldFile = file.ItemSpec;
            string newFile = oldFile.Replace("." + type, ".opt." + type);
            if (File.Exists(newFile) && File.GetLastWriteTimeUtc(newFile) > File.GetLastWriteTimeUtc(oldFile))
            {
                Log.LogMessage(MessageImportance.High, "Skipped file: " + newFile);
                return null;
            }
            Log.LogMessage(MessageImportance.High, "Compressing " + oldFile + " to " + newFile);

            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                                        {
                                            FileName = @"c:\program files\java\jdk1.6.0_12\bin\java.exe",
                                            Arguments = String.Format(
                                                @"-jar ""{0}"" --type {1} --charset utf8 {2} -o ""{3}"" ""{4}""",
                                                Environment.CurrentDirectory + @"\..\CustomBuildTasks\bin\yuicompressor-2.4.2.jar",
                                                type,
                                                ShowWarnings ? "--verbose" : String.Empty,
                                                newFile,
                                                oldFile
                                                ),
                                            UseShellExecute = false,
                                            CreateNoWindow = true,
                                            RedirectStandardOutput = true,
                                            RedirectStandardError = true
                                        };
                process.Start();
                warnings = process.StandardError.ReadToEnd()
                    .Replace("\r", String.Empty)
                    .Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
                process.WaitForExit(5000);
            }
            
            foreach (string warning in warnings)
                Log.LogWarning(null, null, null, oldFile, 1, 1, 1, 1, FormatWarning(warning), null);

            return newFile;
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            List<ITaskItem> compressedFiles = new List<ITaskItem>(Files.Length);
            string type = Type.ToLower();

            foreach (ITaskItem file in Files)
            {
                // make sure the file at least has a value before compressing
                if (file.ItemSpec.Length > 0 && file.ItemSpec.EndsWith(type))
                {
                    try
                    {
                        if (File.Exists(file.ItemSpec))
                        {
                            // delete any old files already compressed
                            if (file.ItemSpec.EndsWith(".opt." + type))
                            {
                                //File.Delete(file.ItemSpec);
                            }
                            else
                            {
                                // compress the file
                                string compressedFile = Compress(file, type);

                                if (!string.IsNullOrEmpty(compressedFile))
                                {
                                    // add the file to the list of successfully compressed files
                                    compressedFiles.Add(new TaskItem(compressedFile));
                                }
                            }
                        }
                        else
                        {
                            Log.LogError("Error in trying to find " + file.ItemSpec + ", it doesn't exist.");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is IOException
                            || ex is UnauthorizedAccessException
                            || ex is PathTooLongException
                            || ex is DirectoryNotFoundException
                            || ex is SecurityException)
                        {
                            Log.LogErrorFromException(ex, false, true, file.ItemSpec);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }

            // return all the new compressed files
            CompressedFiles = compressedFiles.ToArray();

            // return if there were any errors while running this task
            return !Log.HasLoggedErrors;
        }

    }
}