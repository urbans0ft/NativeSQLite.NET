using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

namespace UrbanSoft.Data.SQLite
{
    /// <summary>
    /// Wrapper which encapsulates the calls to the unmanaged SQLite DLL libraries.
    /// This wrapper is designed to work under x64 and x86 as well. All methods from the original
    /// SQLite library need to be added in here. The wrapper itself is internal thus it is only
    /// accessible within the SQLite.NET Library (see SQLite3).
    /// </summary>
    internal static class SQLitePInvoke
    {
        /// <summary>
        /// The success return code of the sqlite3_execute method.
        /// </summary>
        public const int    SQLITE_OK = 0;
        /// <summary>
        /// The encoding used to store strings within the database.
        /// </summary>
        public const string SQLITE_ENCODING = "UTF-8";

        /// <summary>The x86 DLL name of the SQLite library.</summary>
        private const string SQLITE_DLLX86 = "sqlite3_x86.dll";
        /// <summary>The x64 DLL name of the SQLite library.</summary>
        private const string SQLITE_DLLX64 = "sqlite3_x64.dll";
        /// <summary>Namespace of the embedded SQLite resources.</summary>
        private const string SQLITE_NS_RES = "UrbanSoft.Data.SQLite";
        /// <summary>The x86 embedded resource name of the SQLite library.</summary>
        private const string SQLITE_RESX86 = SQLITE_NS_RES + "." + SQLITE_DLLX86;
        /// <summary>The x64 embedded resource name of the SQLite library.</summary>
        private const string SQLITE_RESX64 = SQLITE_NS_RES + "." + SQLITE_DLLX64;

        /*
         * int sqlite3_open(
         *   const char *filename,   // Database filename (UTF-8)
         *   sqlite3 **ppDb          // OUT: SQLite db handle
         * );
         */
        #region sqlite3_open
        [DllImport(SQLITE_DLLX86, EntryPoint = "sqlite3_open", ThrowOnUnmappableChar = true, CharSet = CharSet.Unicode)]
        private static extern int sqlite3_openx86(
            [MarshalAs(UnmanagedType.LPStr)]string filename, ref IntPtr db);

        [DllImport(SQLITE_DLLX64, EntryPoint = "sqlite3_open", ThrowOnUnmappableChar = true, CharSet = CharSet.Unicode)]
        private static extern int sqlite3_openx64(
            [MarshalAs(UnmanagedType.LPStr)]string filename, ref IntPtr db);

        /// <summary>
        /// Call to the unmanaged sqlite3_open SQLite library function.
        /// </summary>
        /// <param name="filename">Database filename (UTF-8)</param>
        /// <param name="db">OUT: SQLite db handle</param>
        /// <returns>Returns 0 on success.</returns>
        public static int sqlite3_open(string filename, ref IntPtr db)
        {
            return IntPtr.Size == 8 /* 64bit */ ? sqlite3_openx64(filename, ref db) : sqlite3_openx86(filename, ref db);
        }
        #endregion

        /*
         * int sqlite3_close(sqlite3 *db);
         */
        #region sqlite3_close
        [DllImport(SQLITE_DLLX86, EntryPoint = "sqlite3_close_v2")]
        private static extern int sqlite3_closex86(IntPtr db);

        [DllImport(SQLITE_DLLX64, EntryPoint = "sqlite3_close_v2")]
        private static extern int sqlite3_closex64(IntPtr db);

        /// <summary>
        /// Call to the unmanaged sqlite3_close SQLite library function.
        /// </summary>
        /// <param name="db">The handle to the DB object to close.</param>
        /// <returns>Unknown</returns>
        public static int sqlite3_close(IntPtr db)
        {
            return IntPtr.Size == 8 /* 64bit */ ? sqlite3_closex64(db) : sqlite3_closex86(db);
        }
        #endregion

        /*
         * int sqlite3_exec(
         *   sqlite3*,                                  // An open database
         *   const char *sql,                           // SQL to be evaluated
         *   int (*callback)(void*,int,char**,char**),  // Callback function (may be null)
         *   void *,                                    // 1st argument to callback (may be null)
         *   char **errmsg                              // Error msg written here (may be null)
         * );
         */
        #region sqlite3_exec

        /// <summary>
        /// Delegate used by the sqlite3_exec() function.
        /// Its c definition is:
        /// \code{.c}int (*callback)(void*,int,char**,char**)\endcode
        /// </summary>
        /// <param name="NotUsed">Not used</param>
        /// <param name="argc">The number of columns.</param>
        /// <param name="argv">The columns values (char**).</param>
        /// <param name="azColName">The columns names (char**).</param>
        /// <returns>Should always return 0.</returns>
        internal delegate int SQLiteCallback(IntPtr NotUsed, int argc, IntPtr argv, IntPtr azColName);

        [DllImport(SQLITE_DLLX86, EntryPoint = "sqlite3_exec")]
        private static extern int sqlite3_execx86(
            IntPtr db, byte[] sql, IntPtr callback, IntPtr notUsed, ref IntPtr errmsg);

        [DllImport(SQLITE_DLLX64, EntryPoint = "sqlite3_exec")]
        private static extern int sqlite3_execx64(
            IntPtr db, byte[] sql, IntPtr callback, IntPtr notUsed, ref IntPtr errmsg);

        /// <summary>
        /// Call to the unmanaged sqlite3_exec SQLite library function.
        /// </summary>
        /// <param name="db">An open database</param>
        /// <param name="sql">SQL to be evaluated</param>
        /// <param name="errmsg">Error msg written here (may be null)</param>
        /// <returns>SQLITE_OK on sucess.</returns>
        public static int sqlite3_exec(IntPtr db, string sql, ref IntPtr errmsg)
        {
            byte[] utf8SQL = System.Text.Encoding.UTF8.GetBytes(sql + "\0");
            return IntPtr.Size == 8 /* 64bit */ ? sqlite3_execx64(db, utf8SQL, IntPtr.Zero, IntPtr.Zero, ref errmsg) : sqlite3_execx64(db, utf8SQL, IntPtr.Zero, IntPtr.Zero, ref errmsg);
        }
        /// <summary>
        /// Call to the unmanaged sqlite3_exec SQLite library function.
        /// </summary>
        /// <param name="db">An open database</param>
        /// <param name="sql">SQL to be evaluated</param>
        /// <param name="errmsg">Error msg written here (may be null)</param>
        /// <param name="callback">The method which gets called with the result
        /// set.</param>
        /// <returns>SQLITE_OK on sucess.</returns>
        public static int sqlite3_exec(IntPtr db, string sql, ref IntPtr errmsg, SQLiteCallback callback)
        {
            byte[] utf8SQL = System.Text.Encoding.UTF8.GetBytes(sql + "\0");
            IntPtr funcPtr = Marshal.GetFunctionPointerForDelegate(callback);
            return IntPtr.Size == 8 /* 64bit */ ? sqlite3_execx64(db, utf8SQL, funcPtr, IntPtr.Zero, ref errmsg) : sqlite3_execx64(db, utf8SQL, funcPtr, IntPtr.Zero, ref errmsg);
        }

        #endregion

        /*
         * void sqlite3_free(void*);
         */
        #region sqlite3_free
        [DllImport(SQLITE_DLLX86, EntryPoint = "sqlite3_close")]
        private static extern void sqlite3_freex86(IntPtr ptr);

        [DllImport(SQLITE_DLLX64, EntryPoint = "sqlite3_close")]
        private static extern void sqlite3_freex64(IntPtr ptr);

        /// <summary>
        /// Frees unmanaged allocated memory.
        /// </summary>
        /// <param name="ptr">The pointer to the unmanaged allocated memory.</param>
        public static void sqlite3_free(IntPtr ptr)
        {
            if (IntPtr.Size == 8) /* 64bit */
                sqlite3_freex64(ptr);
            else
                sqlite3_freex86(ptr);
        }
        #endregion

        #region installUnmanagedLib
        [DllImport("kernel32.dll",
            EntryPoint = "LoadLibraryA", CharSet = CharSet.Ansi, ThrowOnUnmappableChar = true,
            SetLastError = true)]
        private static extern IntPtr LoadLibrary(
            [MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        /// <summary>
        /// Extract the x86 and x64 SQLite embedded resource as a library DLL file.
        /// </summary>
        /// <param name="path">The path where to install the DLL files to.</param>
        /// <exception cref="System.DllNotFoundException">Thrown if LoadLibrary failed after
        /// trying to installl the managed library.</exception>
        /// \warning If the path is not fully qualified it gets combined with the execution
        /// context's current directory. The process must have write permission within this
        /// directory.
        internal static void installUnmanagedLib(string path)
        {
            String dllPath = Path.IsPathRooted(path)
                ? path
                : Path.Combine(Directory.GetCurrentDirectory(), path);

            String pathX64 = Path.Combine(dllPath, SQLITE_DLLX64);
            String pathX86 = Path.Combine(dllPath, SQLITE_DLLX86);

            // install x86 or x64 libs if not present
            //! \todo Check if the embedded DLLs are newer than the 'installed' ones and if so
            //! replace them / it.
            String dll = null;
            if (IntPtr.Size == 8)
            { /* 64bit */
                if (!File.Exists(pathX64))
                    installUnmanagedLib(dllPath, SQLITE_DLLX64, SQLITE_RESX64);
                dll = pathX64;
            }
            else
            {
                if (!File.Exists(pathX86))
                    installUnmanagedLib(dllPath, SQLITE_DLLX86, SQLITE_RESX86);
                dll = pathX86;
            }
            // the files are present thus we can load the libraries.
            if (LoadLibrary(dll) == IntPtr.Zero)
            {
                throw new DllNotFoundException(
                    String.Format("LoadLibrary failed with last error code {0}",
                        Marshal.GetLastWin32Error()));
            }
        }
        /// <summary>
        /// Extract the specified resource as library DLL file.
        /// </summary>
        /// <param name="path">The path where to install the DLL files to.</param>
        /// <param name="lib">The library name (e.g. SQLite_x86.dll).</param>
        /// <param name="resource">The fully qualified resource name (see: SQLITE_RESX86).</param>
        /// \todo Update existing DLL file if the one in the resource is an updated version.
        private static void installUnmanagedLib(string path, string lib, string resource)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string dllPath = Path.Combine(path, lib);

            // Get the embedded resource stream that holds the Internal DLL in this assembly.
            using (Stream stm = Assembly.GetExecutingAssembly().GetManifestResourceStream(
              resource))
            {
                // Copy the assembly to the temporary file
                try
                {
                    using (Stream outFile = File.Create(dllPath))
                    {
                        const int sz = 4096;
                        byte[] buf = new byte[sz];
                        while (true)
                        {
                            int nRead = stm.Read(buf, 0, sz);
                            if (nRead < 1)
                                break;
                            outFile.Write(buf, 0, nRead);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new IOException(
                        String.Format("Could not write {0} from resource {1}", lib, resource), e);
                }
            }
            // LoadLibrary(dllPath); // done by the caller
        }
        #endregion installUnmanagedLib

        #region Extensions
        /// <summary>
        /// Converts a zero-terminated string which is referenced by this
        /// pointer to a System.String object.
        /// </summary>
        /// <param name="ptr">The pointer referencing a string.</param>
        /// <param name="enc">The encoding of the string referenced by this
        /// pointer.</param>
        /// <returns></returns>
        internal static string ToString(this IntPtr ptr, Encoding enc)
        {
            byte       read;
            int        offset  = 0;
            List<byte> azValue = new List<byte>();
            while ((read = Marshal.ReadByte(ptr, offset++)) != 0x00)
            {
                azValue.Add(read);
            }
            return enc.GetString(azValue.ToArray());
        }
        #endregion

    }
}
