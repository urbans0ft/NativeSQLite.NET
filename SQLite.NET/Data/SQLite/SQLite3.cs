using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UrbanSoft.Data.SQLite
{
    public class SQLite3 : IDisposable
    {
        /// <summary>The hande to the DB file.</summary>
        private IntPtr db;
        /// <summary>The filename of the DB.</summary>
        private string filename;
        /// <summary>Tells if the DB connection thus the file is open.</summary>
        private bool isOpen;
        private List<Dictionary<string, string>> lastQuery;
        /// <summary>
        /// Initialize a new SQLite connection. Before one can use this connection a call to
        /// open(string filename) is necessary.
        /// </summary>
        public SQLite3()
        {
            this.init(null);
        }
        /// <summary>
        /// Initialize a new SQLite connection with a given filename. Before one can use the
        /// connection a call to the open() method is mandatory.
        /// </summary>
        /// <param name="filename">The files name where to store the DB.</param>
        public SQLite3(string filename)
        {
            this.init(filename);
        }
        /// <summary>
        /// Initializes all private attributes.
        /// </summary>
        /// <param name="filename">The file name where to store the DB.</param>
        private void init(string filename)
        {
            this.db = IntPtr.Zero;
            this.filename = filename;
            this.isOpen = false;
        }

        /// <summary>
        /// Opens up a new connection. A valid filename must have been set if not use
        /// open(string filename) or set the filename first.
        /// </summary>
        public void open()
        {
            this.open(this.filename);
        }
        /// <summary>
        /// Opens up a new connection with a given file.
        /// At the file's location the connector also places the necessary library files.
        /// </summary>
        /// <param name="filename">The file to use for storing the DB.</param>
        /// <exception cref="Exception">Thrown if the connection is already open or an internal
        /// library error occured while executing sqlite3_open().</exception>
        /// <exception cref="ArgumentNullException">Thrown if the provided file name is
        /// null or empty.</exception>
        public void open(string filename)
        {
            if (this.isOpen)
                throw new Exception("DB connection is already open.");
            this.filename = filename;
            if (String.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename", "The parameter is null or empty.");
            }
            try
            {
                SQLitePInvoke.installUnmanagedLib(Path.GetDirectoryName(filename));
                if (SQLitePInvoke.sqlite3_open(this.filename, ref db) != 0)
                {
                    throw new Exception("Error executing sqlite3_open()!");
                }
                this.isOpen = true;
            }
            catch (Exception e)
            {
                this.db = IntPtr.Zero;
                throw new Exception("Could not connect to DB.", e);
            }
        }
        /// <summary>
        /// Executes an SQL statemant on an opened connection. Before using
        /// execute ensure that you have opened up the connection using the
        /// open method. This method shell be used for data manipulation
        /// statemants. If you'd like to query data use query().
        /// </summary>
        /// <param name="sql">The SQL statemant to execute.</param>
        public void execute(string sql)
        {
            try
            {
                IntPtr errMsgPtr = IntPtr.Zero;
                if (SQLitePInvoke.sqlite3_exec(db, sql, ref errMsgPtr) != SQLitePInvoke.SQLITE_OK)
                {
                    var errMsg = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(errMsgPtr);
                    SQLitePInvoke.sqlite3_free(errMsgPtr);
                    throw new Exception(
                        String.Format("Error executing sqlite3_exec() - {0}!", errMsg));
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not execute SQL statement.", e);
            }
        }
        /// <summary>
        /// Execute a data query statemant (\c SELECT). This method differs from
        /// execute() only in that it fills the LastQuery property. You may use
        /// this method for data manipulation statemants as well but this would
        /// clear the LastQuery property.
        /// </summary>
        /// <param name="sql">The sql statemant to query the database.</param>
        public void query(string sql)
        {
            lastQuery = new List<Dictionary<string, string>>();
            try
            {
                IntPtr errMsgPtr = IntPtr.Zero;
                if (SQLitePInvoke.sqlite3_exec(db, sql, ref errMsgPtr, query)
                    != SQLitePInvoke.SQLITE_OK)
                {
                    var errMsg = Marshal.PtrToStringAnsi(errMsgPtr);
                    SQLitePInvoke.sqlite3_free(errMsgPtr);
                    throw new Exception(
                        String.Format("Error executing sqlite3_exec() - {0}!", errMsg));
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not execute SQL statement.", e);
            }
        }
        /// <summary>
        /// This method represents the callback needed for sqlite3_exec.
        /// Its c definition is: int (*callback)(void*,int,char**,char**)
        /// This method is implictly called for every row which is part of the
        /// result set.
        /// </summary>
        /// <param name="NotUsed">Not used</param>
        /// <param name="argc">The number of columns.</param>
        /// <param name="argv">The columns values (char**).</param>
        /// <param name="azColName">The columns names (char**).</param>
        /// <returns>Should always return 0.</returns>
        protected int query(IntPtr NotUsed, int argc, IntPtr argv, IntPtr azColName)
        {
            var enc = Encoding.GetEncoding(SQLitePInvoke.SQLITE_ENCODING);
            var row = new Dictionary<string, string>(argc);
            for (int i = 0; i < argc; i++)
            {
                IntPtr pValue = Marshal.ReadIntPtr(argv, IntPtr.Size * i);
                IntPtr pKey   = Marshal.ReadIntPtr(azColName, IntPtr.Size * i);
                string value  = pValue.ToString(enc);
                string key    = pKey.ToString(enc);
                row.Add(key, value);
            }
            lastQuery.Add(row);
            return 0;
        }

        /// <summary>
        /// Closes the handle to the SQLite DB.
        /// Consider not using close(). Instead initialize the object within a
        /// using block.
        /// </summary>
        /// <example><code>
        /// using (var con = new SQLiteConnection("database.db"))
        /// {
        ///   con.open();
        /// } // closed implicitly => disposed
        /// </code></example>
        public void close()
        {
            Dispose();
        }
        /// <summary>
        /// Releases all resoucres used by the connection. 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Releases the unmanaged resources use by the SQLiteConnection and
        /// optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">\c true to release both managed and
        /// unmanaged resources; \c false to release only managed resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) // release (un)managed resources
            {
                if (this.db != IntPtr.Zero)
                {
                    SQLitePInvoke.sqlite3_close(this.db);
                    this.db = IntPtr.Zero;
                    this.isOpen = false;
                }
            }
        }

        /// <summary>
        /// The current file to use to store the DB. A setting will fail in case the DB connection
        /// is already open (see SQLiteConnection::IsOpen property).
        /// </summary>
        /// <exception cref="AccessViolationException">thrown if trying to set the filename
        /// on an opened connection.</exception>
        public string Filename
        {
            get { return this.filename; }
            set
            {
                if (this.isOpen)
                {
                    throw new AccessViolationException(
                        "Can't change the file name while the connection is open.");
                }
                this.filename = value;
            }
        }

        /// <summary>
        /// Tells if the current DB connection is already open (see open() method).
        /// </summary>
        public bool IsOpen
        {
            get { return this.isOpen; }
        }
        /// <summary>
        /// Gets the result of the last call to query().
        /// </summary>
        public List<Dictionary<string, string>> LastQuery
        {
            get { return this.lastQuery; }
        }
    }
}
