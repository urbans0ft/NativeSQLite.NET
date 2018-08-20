# NativeSQLite.NET
A SQLite .NET Library using P/Invoke


# Under development

**Do not use.**


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