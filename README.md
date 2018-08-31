# NativeSQLite.NET
A SQLite .NET Library using P/Invoke


# Under development

**Beta Release**


# Usage

```csharp
// HelloSQLite.cs
// compile: csc /reference:SQLite.NET.dll HelloSQLite.cs
using System;
using UrbanSoft.Data.SQLite;

namespace SQLiteTest
{
  public class SQLiteProgram
  {
    public static void Main()
    {
      using (var sqlite = new SQLite3("sqlite.db"))
      {
        sqlite.execute(@"
          CREATE TABLE IF NOT EXISTS hello_world (
          id  INTEGER PRIMARY KEY,
          msg TEXT)
        ");

     // using (var transaction = new SQLiteTransaction(sqlite))
        using (var transaction = sqlite.beginTransaction())
        {
          sqlite.execute(@"
            INSERT INTO hello_world(msg)
            VALUES ('Hello World!');
          ");
          sqlite.execute(@"
            INSERT INTO hello_world(msg)
            VALUES ('Hello SQLite!');
          ");
          // throw new Exception("ROLLBACK");
        } // implicit commit or rollback

        sqlite.query("SELECT * FROM hello_world");
        foreach(var row in sqlite.LastQuery)
        {
          foreach(var kv in row)
          {
            Console.WriteLine("{0}: {1}", kv.Key, kv.Value);
          }
          Console.WriteLine("--");
        }
      }
    }
  }
}
```


# SQLite

* Current version: 3.24.0
* Source code: [SQLite Amalgamation](https://www.sqlite.org/download.html) 
* `#define SQLITE_API __declspec(dllexport)`
* `dumpbin /exports sqlite3.dll`


# Namespaces / Classes / Files

* UrbanSoft
    * Data
        * SQLite
            * **SQLite3**
            * **SQLiteException**
            * **SQLitePInvoke**
	* Windows
		* Native
			* **Win32**


# Naming Conventions

In general _all_ members and namespaces are written using [**camelCase**](https://en.wikipedia.org/wiki/Camel_case).  
(Other rules might overrule this one.)

## Namespaces

* **All** namespaces start with an upper case regardless of any proper names.
* A part from that follow the [Microsoft guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/names-of-namespaces).
* `<Company>.(<Product>|<Technology>)[.<Feature>][.<Subnamespace>]`


## Classes and its Members

### Methods

* **Non** `static` methods start with a lower case.
    * E.g.: `private void start()`
* `static` methods start with an upper case.
    * E.g.: `public static void Start()`
* Properties start with an upper case.
    * E.g.: `protected string Start`
* Constants only consist of upper case and optional underscores (_) to separate
  words and increase the readability.
    * E.g.: `internal const string START_NOW = "constant"`