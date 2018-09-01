using System;
using System.Runtime.InteropServices;
using UrbanSoft.Extensions;

namespace UrbanSoft.Data.SQLite
{
    /// <summary>
    /// Represents a transaction to be performed at a sqlite data source.
    /// </summary>
    public class SQLiteTransaction : IDisposable
    {
        private const string BEGIN_TRANSACTION = "BEGIN {0} TRANSACTION";
        private const string COMMIT            = "END TRANSACTION";
        private const string ROLLBACK          = "ROLLBACK";

        private SQLite3                   connection;
        private SQLiteTransactionBehavior behavior;
        private bool                      hasTransactionEnded;
        /// <summary>
        /// Creates a transaction to be performed at the sqlite db. The
        /// transaction object cannot be reused after if has ended with a 
        /// commit or rollback.
        /// </summary>
        /// <param name="connection">The sqlite3 connection which has to be
        /// open.</param>
        public SQLiteTransaction(SQLite3 connection) 
            : this (connection, SQLiteTransactionBehavior.Default){}
        /// <summary>
        /// Represents a transaction to be performed at the sqlite db.
        /// </summary>
        /// <param name="connection">The sqlite3 connection which has to be
        /// open.</param>
        /// <param name="behavior">Defines the behavior of the Transaction.
        /// </param>
        public SQLiteTransaction(SQLite3 connection,
            SQLiteTransactionBehavior behavior)
        {
            this.connection          = connection;
            this.behavior            = behavior;
            this.hasTransactionEnded = false;

            connection.execute(String.Format(
                BEGIN_TRANSACTION, behavior.ToEnumMemberAttrValue()));
        }
        /// <summary>
        /// Gets the Connection object which is associated with the transaction.
        /// </summary>
        public SQLite3 Connection
        {
            get
            {
                return this.connection;
            }
        }
        /// <summary>
        /// Gets the defined behavior of the transaction.
        /// </summary>
        public SQLiteTransactionBehavior Behavior
        {
            get
            {
                return this.behavior;
            }
        }
        /// <summary>
        /// Commits the database transaction. Explicit usage is not recommended.
        /// Use the SQLiteTransaction object within a using block instead.
        /// </summary>
        public void commit()
        {
            if (!this.hasTransactionEnded)
            {
                connection.execute(COMMIT);
                this.hasTransactionEnded = true;
            }
        }
        /// <summary>
        /// Rolls back a transaction from a pending state. Explicit usage is
        /// not recommended. Use the SQLiteTransaction object within a using
        /// block instead.
        /// </summary>
        public void rollback()
        {
            if (!this.hasTransactionEnded)
            {
                this.connection.execute(ROLLBACK);
                this.hasTransactionEnded = true;
            }
        }
        /// <summary>
        /// Releases all resoucres used by the transaction and commits or rolls
        /// back the transaction depending on whether or not an exception
        /// occured.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Releases all resoucres used by the transaction and commits or rolls
        /// back the transaction depending on whether or not an exception
        /// occured.
        /// </summary>
        /// <param name="disposing">\c true to release both managed and
        /// unmanaged resources; \c false to release only managed resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) // commit or rollback
            {
                // detect if disposing is within an exception
                if (Marshal.GetExceptionPointers() != IntPtr.Zero ||
                    Marshal.GetExceptionCode() != 0) // is in exception
                {
                    rollback();
                }
                else
                {
                    commit();
                }
            }
        }

    }
}