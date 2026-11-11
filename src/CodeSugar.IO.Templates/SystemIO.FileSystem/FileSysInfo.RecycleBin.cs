using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

#nullable disable

using __IOPATH = System.IO.Path;
using __FINFO = System.IO.FileInfo;
using __DINFO = System.IO.DirectoryInfo;
using __SINFO = System.IO.FileSystemInfo;
using __MATCHCASING = System.IO.MatchCasing;


namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        /// <summary>
        /// Tries to delete a file or directory by sending it to the Recycle Bin, so it can be reclaimed back by the user.
        /// </summary>
        /// <remarks>
        /// On Windows, it requires referencing "Microsoft.VisualBasic" nuget package.
        /// </remarks>
        /// <returns>
        /// true on success or no action required, false if unable to comply.
        /// </returns>
        public static async Task<bool> TrySendToRecycleBinAsync(this __SINFO sinfo)
        {
            if (sinfo == null) return true;

            sinfo.Refresh();
            if (!sinfo.Exists) return true;

            // https://stackoverflow.com/questions/3282418/send-a-file-to-the-recycle-bin

            #if NET && !ANDROID            

            if (OperatingSystem.IsWindows())
            {
                return await Task.Run( ()=> _TrySendToRecycleBin_Windows(sinfo));
            }

            if (OperatingSystem.IsLinux())
            {
                // syntax:
                // gio trash "path/to/file"
                // gio trash "path/to/directory"

                return await _TrySendToRecycleBin_Process("gio", $"trash \"{sinfo.FullName}\"").ConfigureAwait(false);
            }

            if (OperatingSystem.IsMacOS())
            {
                return await _TrySendToRecycleBin_Mac(sinfo.FullName).ConfigureAwait(false);
            }

            #endif

            return false;
        }


        #region implementations

        #if !ANDROID

        #if NET

        private static bool _TrySendToRecycleBin_Windows(this __SINFO sinfo)
        {
            if (sinfo == null) return false;

            // https://stackoverflow.com/questions/3282418/send-a-file-to-the-recycle-bin            

            #if __REFERENCES_MICROSOFTVISUALBASIC

            if (sinfo is __FINFO finfo)
            {
                if (!finfo.PhysicallyExists()) return false;

                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(
                        finfo.FullName,
                        Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                        Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin
                    );

                finfo.Refresh();
                return true;
            }
            else if (sinfo is __DINFO dinfo)
            {
                if (!dinfo.PhysicallyExists()) return false;

                Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(
                        dinfo.FullName,
                        Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                        Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin
                    );

                dinfo.Refresh();
                return true;
            }

            #else
            throw new InvalidOperationException("Reference to "Microsoft.VisualBasic" nuget package required");
            #endif            

            return false;
        }

        #endif        

        private static async Task<bool> _TrySendToRecycleBin_Mac(string path)
        {
            // Escaping double quotes inside the path just in case
            string escapedPath = path.Replace("\"", "\\\"");

            // This specific AppleScript syntax handles both files and directories perfectly
            string appleScript = $"-e \"tell application \\\"Finder\\\" to delete (POSIX file \\\"{escapedPath}\\\" as alias)\"";

            return await _TrySendToRecycleBin_Process("osascript", appleScript).ConfigureAwait(false);
        }

        private static async Task<bool> _TrySendToRecycleBin_Process(string fileName, string arguments)
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (var process = System.Diagnostics.Process.Start(psi))
                {
                    if (process == null) return false;
                    
                    await process.WaitForExitAsync().ConfigureAwait(false);                    

                    return process?.ExitCode == 0;
                }
            }
            catch { return false; }
        }


        // this method is here for reference in case it's needed in the future
        private static bool _TrySendToRecycleBin_Linux_Manual(__FINFO finfo)
        {
            try
            {                
                if (!finfo.Exists) return false;

                // 1. get current user trash bin folder
                string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string trashFilesDir = __IOPATH.Combine(homeDir, ".local/share/Trash/files");
                string trashInfoDir = __IOPATH.Combine(homeDir, ".local/share/Trash/info");

                // ensure the folders exist
                Directory.CreateDirectory(trashFilesDir);
                Directory.CreateDirectory(trashInfoDir);

                // 2. generate unique name to avoid collisions                
                string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
                string trashFileName = $"{__IOPATH.GetFileNameWithoutExtension(finfo.Name)}_{uniqueId}{finfo.Extension}";

                string destinationPath = __IOPATH.Combine(trashFilesDir, trashFileName);
                string infoFilePath = __IOPATH.Combine(trashInfoDir, trashFileName + ".trashinfo");

                // 3. Create metadata file (.trashinfo) required by Linux
                string deletionDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                string trashInfoContent = $"[Trash Info]\nPath={finfo.FullName}\nDeletionDate={deletionDate}\n";

                File.WriteAllText(infoFilePath, trashInfoContent);

                // 4. Actually move the file to the recycle bin
                finfo.MoveTo(destinationPath);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al mover a la papelera manualmente: {ex.Message}");
                return false;
            }
        }

        #endif

        #endregion
    }
}
