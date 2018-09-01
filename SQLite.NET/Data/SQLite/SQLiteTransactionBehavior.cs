using System.Runtime.Serialization;
namespace UrbanSoft.Data.SQLite
{
    /// <summary>
    /// Defines the behavior of a Transaction
    /// </summary>
    public enum SQLiteTransactionBehavior
    {
        /// <summary>
        /// Same as SQLiteTransactionBehavior.Deffered.
        /// </summary>
        [EnumMember(Value = "DEFERRED")]
        Default,
        /// <summary>Default transaction behavior. no locks are acquired on the
        /// database until the database is first accessed. Thus with a deferred 
        /// transaction, the BEGIN statement itself does nothing to
        /// the filesystem. Locks are not acquired until the first read or
        /// write operation. The first read operation against a database
        /// creates a SHARED lock and the first write operation creates a
        /// RESERVED lock. Because the acquisition of locks is deferred until
        /// they are needed, it is possible that another thread or process
        /// could create a separate transaction and write to the database after
        /// the BEGIN on the current thread has executed.
        /// <see href="https://www.sqlite.org/lang_transaction.html">Source</see></summary>
        [EnumMember(Value = "DEFERRED")]
        Deferred,
        /// <summary>
        /// RESERVED locks are acquired on all databases as soon as the BEGIN
        /// command is executed, without waiting for the database to be used.
        /// After a BEGIN IMMEDIATE, no other database connection will be able
        /// to write to the database or do a BEGIN IMMEDIATE or BEGIN EXCLUSIVE.
        /// Other processes can continue to read from the database, however.
        /// <see href="https://www.sqlite.org/lang_transaction.html">Source</see>
        /// </summary>
        [EnumMember(Value = "IMMEDIATE")]
        Immediate,
        /// <summary>
        /// An exclusive transaction causes EXCLUSIVE locks to be acquired on all
        /// databases. After a BEGIN EXCLUSIVE, no other database connection
        /// except for read_uncommitted connections will be able to read the
        /// database and no other connection without exception will be able to
        /// write the database until the transaction is complete. 
        /// <see href="https://www.sqlite.org/lang_transaction.html">Source</see>
        /// </summary>
        [EnumMember(Value = "EXCLUSIVE")]
        Exclusive,
    }
}