# NativeSQLite.NET
A SQLite .NET Library using P/Invoke

# Under development

# SQLite

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
