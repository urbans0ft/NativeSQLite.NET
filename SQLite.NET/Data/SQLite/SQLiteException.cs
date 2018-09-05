using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanSoft.Data.SQLite
{
    public class SQLiteException : Exception
    {
        public SQLiteException()
        {
        }

        public SQLiteException(string message) : base(message)
        {
        }

        public SQLiteException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
