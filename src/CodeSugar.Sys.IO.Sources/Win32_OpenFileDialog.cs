using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace CodeSugar
{
    /// <summary>
    /// OpenFileDialog with no dependencies
    /// </summary>
    /// <remarks>
    /// Credits: <see href="https://gist.github.com/gotmachine/4ffaf7837f9fbb0ab4a648979ee40609"/>
    /// </remarks>
    internal class Win32_OpenFileDialog
    {
        #region native

        [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

        #endregion

        #region nested types

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct OpenFileName
        {
            public int structSize;
            public IntPtr dlgOwner;
            public IntPtr instance;
            public string filter;
            public string customFilter;
            public int maxCustFilter;
            public int filterIndex;
            public IntPtr file;
            public int maxFile;
            public string fileTitle;
            public int maxFileTitle;
            public string initialDir;
            public string title;
            public int flags;
            public short fileOffset;
            public short fileExtension;
            public string defExt;
            public IntPtr custData;
            public IntPtr hook;
            public string templateName;
            public IntPtr reservedPtr;
            public int reservedInt;
            public int flagsEx;
        }

        private enum OpenFileNameFlags
        {
            OFN_HIDEREADONLY = 0x4,
            OFN_FORCESHOWHIDDEN = 0x10000000,
            OFN_ALLOWMULTISELECT = 0x200,
            OFN_EXPLORER = 0x80000,
            OFN_FILEMUSTEXIST = 0x1000,
            OFN_PATHMUSTEXIST = 0x800
        }

        #endregion

        #region data

        public string Title { get; set; } = "Open a file...";
        public bool Multiselect { get; set; } = false;
        public string InitialDirectory { get; set; } = null;
        public string Filter { get; set; } = "All files(*.*)\0\0";
        public bool ShowHidden { get; set; } = false;
        public bool Success { get; private set; }
        public string[] Files { get; private set; }

        #endregion

        #region API

        /// <summary>
        /// Open a single file
        /// </summary>
        /// <param name="file">Path to the selected file, or null if the return value is false</param>
        /// <param name="title">Title of the dialog</param>
        /// <param name="filter">File name filter. Example : "txt files (*.txt)|*.txt|All files (*.*)|*.*"</param>
        /// <param name="initialDirectory">Example : "c:\\"</param>
        /// <param name="showHidden">Forces the showing of system and hidden files</param>
        /// <returns>True of a file was selected, false if the dialog was cancelled or closed</returns>
        public static bool TryOpenFile(out string file, string title = null, string filter = null, string initialDirectory = null, bool showHidden = false)
        {
            _CheckPlatform();

            var dialog = new Win32_OpenFileDialog();
            dialog.Title = title;
            dialog.InitialDirectory = initialDirectory;
            dialog.Filter = filter;
            dialog.ShowHidden = showHidden;

            dialog._OpenDialog();
            if (dialog.Success)
            {
                file = dialog.Files[0];
                return true;
            }

            file = null;
            return false;
        }

        /// <summary>
        /// Open multiple files
        /// </summary>
        /// <param name="files">Paths to the selected files, or null if the return value is false</param>
        /// <param name="title">Title of the dialog</param>
        /// <param name="filter">File name filter. Example : "txt files (*.txt)|*.txt|All files (*.*)|*.*"</param>
        /// <param name="initialDirectory">Example : "c:\\"</param>
        /// <param name="showHidden">Forces the showing of system and hidden files</param>
        /// <returns>True of one or more files were selected, false if the dialog was cancelled or closed</returns>
        public static bool TryOpenFiles(out string[] files, string title = null, string filter = null, string initialDirectory = null, bool showHidden = false)
        {
            _CheckPlatform();

            var dialog = new Win32_OpenFileDialog();
            dialog.Title = title;
            dialog.InitialDirectory = initialDirectory;
            dialog.Filter = filter;
            dialog.ShowHidden = showHidden;
            dialog.Multiselect = true;

            dialog._OpenDialog();
            if (dialog.Success)
            {
                files = dialog.Files;
                return true;
            }

            files = null;
            return false;
        }

        private void _OpenDialog()
        {
            _CheckPlatform();

            var thread = new Thread(() => _ShowOpenFileDialog());
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        private static void _CheckPlatform()
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT) throw new PlatformNotSupportedException("Windows only");
        }

        private void _ShowOpenFileDialog()
        {
            const int MAX_FILE_LENGTH = 2048;

            var flags = OpenFileNameFlags.OFN_HIDEREADONLY | OpenFileNameFlags.OFN_EXPLORER | OpenFileNameFlags.OFN_FILEMUSTEXIST | OpenFileNameFlags.OFN_PATHMUSTEXIST;
            if (ShowHidden) flags |= OpenFileNameFlags.OFN_FORCESHOWHIDDEN;
            if (Multiselect) flags |= OpenFileNameFlags.OFN_ALLOWMULTISELECT;

            Success = false;
            Files = null;

            var ofn = new OpenFileName();
            ofn.structSize = Marshal.SizeOf(ofn);
            ofn.filter = Filter?.Replace("|", "\0") + "\0";
            ofn.fileTitle = new string(new char[MAX_FILE_LENGTH]);
            ofn.maxFileTitle = ofn.fileTitle.Length;
            ofn.initialDir = InitialDirectory;
            ofn.flags = (int)(flags);
            ofn.title = Title;
            ofn.dlgOwner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

            // Create buffer for file names
            ofn.maxFile = MAX_FILE_LENGTH;
            var allocLen = MAX_FILE_LENGTH * Marshal.SystemDefaultCharSize;
            ofn.file = Marshal.AllocHGlobal(allocLen);           

            // Initialize buffer with NULL bytes
            for (int i = 0; i < allocLen; i++)
            {
                Marshal.WriteByte(ofn.file, i, 0);
            }            

            Success = GetOpenFileName(ofn);

            if (Success)
            {
                IntPtr filePointer = ofn.file;
                long pointer = (long)filePointer;
                string file = Marshal.PtrToStringAuto(filePointer);
                List<string> strList = new List<string>();

                // Retrieve file names
                while (file.Length > 0)
                {
                    strList.Add(file);

                    pointer += file.Length * Marshal.SystemDefaultCharSize + Marshal.SystemDefaultCharSize;
                    filePointer = (IntPtr)pointer;
                    file = Marshal.PtrToStringAuto(filePointer);
                }

                if (strList.Count > 1)
                {
                    Files = new string[strList.Count - 1];
                    for (int i = 1; i < strList.Count; i++)
                    {
                        Files[i - 1] = Path.Combine(strList[0], strList[i]);
                    }
                }
                else
                {
                    Files = strList.ToArray();
                }
            }

            Marshal.FreeHGlobal(ofn.file);
        }

        #endregion
    }
}
