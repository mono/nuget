using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;

namespace NuGet
{
    public class PhysicalPackageFile : IPackageFile
    {
        private readonly Func<Stream> _streamFactory;
        private string _targetPath;
        private FrameworkName _targetFramework;

        public PhysicalPackageFile()
        {
        }

        public PhysicalPackageFile(PhysicalPackageFile file)
        {
            SourcePath = file.SourcePath;
            TargetPath = file.TargetPath;
        }

        internal PhysicalPackageFile(Func<Stream> streamFactory)
        {
            _streamFactory = streamFactory;
        }

        /// <summary>
        /// Path on disk
        /// </summary>
        public string SourcePath
        {
            get;
            set;
        }

        /// <summary>
        /// Path in package
        /// </summary>
        public string TargetPath
        {
            get
            {
                return _targetPath;
            }
            set
            {
                if (String.Compare(_targetPath, value, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    _targetPath = value;
                    string effectivePath;
                    _targetFramework = VersionUtility.ParseFrameworkNameFromFilePath(_targetPath, out effectivePath);
                    EffectivePath = effectivePath;
                }
            }
        }

        public string Path
        {
            get
            {
                return TargetPath;
            }
        }

        public string EffectivePath
        {
            get;
            private set;
        }

        public FrameworkName TargetFramework
        {
            get { return _targetFramework; }
        }

        public IEnumerable<FrameworkName> SupportedFrameworks
        {
            get
            {
                if (TargetFramework != null)
                {
                    yield return TargetFramework;
                }
                yield break;
            }
        }

        public Stream GetStream()
        {
            try
            {
                return _streamFactory != null ? _streamFactory() : File.OpenRead(SourcePath);
            }
            catch (PathTooLongException)
            {
                return Win32File.OpenRead(SourcePath);
            }
        }

        /// <summary>
        /// From: http://www.codeproject.com/Articles/22013/NET-Workaround-for-PathTooLongException
        /// </summary>
        class Win32File
        {
            // Error
            const int ERROR_ALREADY_EXISTS = 183;
            // seek location
            const uint FILE_END = 0x2;

            // access
            const uint GENERIC_READ = 0x80000000;
            const uint GENERIC_WRITE = 0x40000000;

            const uint FILE_APPEND_DATA = 0x00000004;

            // attribute
            const uint FILE_ATTRIBUTE_NORMAL = 0x80;

            // share
            const uint FILE_SHARE_DELETE = 0x00000004;
            const uint FILE_SHARE_READ = 0x00000001;
            const uint FILE_SHARE_WRITE = 0x00000002;

            //mode
            const uint CREATE_NEW = 1;
            const uint CREATE_ALWAYS = 2;
            const uint OPEN_EXISTING = 3;
            const uint OPEN_ALWAYS = 4;
            const uint TRUNCATE_EXISTING = 5;


            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            static extern SafeFileHandle CreateFileW(string lpFileName, uint dwDesiredAccess,
                                                  uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
                                                  uint dwFlagsAndAttributes, IntPtr hTemplateFile);

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            static extern uint SetFilePointer(SafeFileHandle hFile, long lDistanceToMove, IntPtr lpDistanceToMoveHigh, uint dwMoveMethod);

            // uint GetMode( FileMode mode )
            // Converts the filemode constant to win32 constant
            private static uint GetMode(FileMode mode)
            {
                uint umode = 0;
                switch (mode)
                {
                    case FileMode.CreateNew:
                        umode = CREATE_NEW;
                        break;
                    case FileMode.Create:
                        umode = CREATE_ALWAYS;
                        break;
                    case FileMode.Append:
                        umode = OPEN_ALWAYS;
                        break;
                    case FileMode.Open:
                        umode = OPEN_EXISTING;
                        break;
                    case FileMode.OpenOrCreate:
                        umode = OPEN_ALWAYS;
                        break;
                    case FileMode.Truncate:
                        umode = TRUNCATE_EXISTING;
                        break;
                }
                return umode;
            }

            // uint GetAccess(FileAccess access)
            // Converts the FileAccess constant to win32 constant
            private static uint GetAccess(FileAccess access)
            {
                uint uaccess = 0;
                switch (access)
                {
                    case FileAccess.Read:
                        uaccess = GENERIC_READ;
                        break;
                    case FileAccess.ReadWrite:
                        uaccess = GENERIC_READ | GENERIC_WRITE;
                        break;
                    case FileAccess.Write:
                        uaccess = GENERIC_WRITE;
                        break;
                }
                return uaccess;
            }

            // uint GetShare(FileShare share)
            // Converts the FileShare constant to win32 constant
            private static uint GetShare(FileShare share)
            {
                uint ushare = 0;
                switch (share)
                {
                    case FileShare.Read:
                        ushare = FILE_SHARE_READ;
                        break;
                    case FileShare.ReadWrite:
                        ushare = FILE_SHARE_READ | FILE_SHARE_WRITE;
                        break;
                    case FileShare.Write:
                        ushare = FILE_SHARE_WRITE;
                        break;
                    case FileShare.Delete:
                        ushare = FILE_SHARE_DELETE;
                        break;
                    case FileShare.None:
                        ushare = 0;
                        break;

                }
                return ushare;
            }

            public static FileStream Open(string filepath, FileMode mode, FileAccess access, FileShare share)
            {
                //opened in the specified mode , access and  share
                FileStream fs = null;
                uint umode = GetMode(mode);
                uint uaccess = GetAccess(access);
                uint ushare = GetShare(share);
                if (mode == FileMode.Append)
                    uaccess = FILE_APPEND_DATA;
                // If file path is disk file path then prepend it with \\?\
                // if file path is UNC prepend it with \\?\UNC\ and remove \\ prefix in unc path.
                if (filepath.StartsWith(@"\\"))
                {
                    filepath = @"\\?\UNC\" + filepath.Substring(2, filepath.Length - 2);
                }
                else
                    filepath = @"\\?\" + filepath;
                SafeFileHandle sh = CreateFileW(filepath, uaccess, ushare, IntPtr.Zero, umode, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
                int iError = Marshal.GetLastWin32Error();
                if ((iError > 0 && !(mode == FileMode.Append && iError != ERROR_ALREADY_EXISTS)) || sh.IsInvalid)
                    throw new Exception("Error opening file Win32 Error:" + iError);

                fs = new FileStream(sh, access);
                
                // if opened in append mode
                if (mode == FileMode.Append && !sh.IsInvalid)
                    SetFilePointer(sh, 0, IntPtr.Zero, FILE_END);

                return fs;
            }

            public static FileStream OpenRead(string filepath)
            {
                // Open readonly file mode open(String, FileMode.Open, FileAccess.Read, FileShare.Read)
                return Open(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
        }


        public override string ToString()
        {
            return TargetPath;
        }

        public override bool Equals(object obj)
        {
            var file = obj as PhysicalPackageFile;

            return file != null && String.Equals(SourcePath, file.SourcePath, StringComparison.OrdinalIgnoreCase) &&
                                   String.Equals(TargetPath, file.TargetPath, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (SourcePath != null)
            {
                hash = SourcePath.GetHashCode();
            }

            if (TargetPath != null)
            {
                hash = hash * 4567 + TargetPath.GetHashCode();
            }

            return hash;
        }
    }
}
