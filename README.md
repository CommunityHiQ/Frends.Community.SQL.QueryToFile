# Frends.Community.SQL

FRENDS Community Task for SQL query results to CSV-file

[![Actions Status](https://github.com/CommunityHiQ/Frends.Community.SQL/workflows/PackAndPushAfterMerge/badge.svg)](https://github.com/CommunityHiQ/Frends.Community.SQL/actions) ![MyGet](https://img.shields.io/myget/frends-community/v/Frends.Community.SQL) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) 

- [Installing](#installing)
- [Tasks](#tasks)
     - [SaveQueryToCSV](#SaveQueryToCSV)
- [License](#license)
- [Building](#building)
- [Contributing](#contributing)
- [Change Log](#change-log)

# Installing

You can install the task via FRENDS UI Task View or you can find the NuGet package from the following NuGet feed
https://www.myget.org/F/frends-community/api/v3/index.json and in Gallery view in MyGet https://www.myget.org/feed/frends-community/package/nuget/Frends.Community.SQL

# Tasks

## SaveQueryToCSV

### Parameters

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| Query | string | SQL query to execute | `SELECT id FROM foo` |
| ConnectionString | string | Database connection string | `Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword` |
| TimeoutSeconds | int | Timeout in seconds | 30 |
| OutputFilePath | string | CSV file path for output | `C:\output\path.csv` |
| QueryParameters | SQLParameter[] | Query parameters | `[ { "@Param1", "Value1" }, { "@Param2", "Value2" }]`

### Options

Settings for included attachments

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| ColumnsToInclude | string[] | Columns to include in the output CSV. If no columns defined here then all columns will be written to output CSV. | `[id, name, value]` |
| FieldDelimiter | enum { Comma, Semicolon, Pipe } | Field delimeter to use in output CSV | Comma |
| LineBreak | enum { CR, LF, CRLF } | Line break style to use in output CSV. | CRLF |
| FileEncoding | enum | Encoding for the read content. By selecting 'Other' you can use any encoding. |
| EncodingInString | string | The name of encoding to use. Required if the FileEncoding choice is 'Other'. A partial list of supported encoding names: https://msdn.microsoft.com/en-us/library/system.text.encoding.getencodings(v=vs.110).aspx | `iso-8859-1` |
| IncludeHeadersInOutput | bool | Wherther to include headers in output CSV. | `true` |
| SanitizeColumnHeaders | bool | Whether to sanitize headers in output: (1) Strip any chars that are not 0-9, a-z or _ (2) Make sure that column does not start with a number or underscore (3) Force lower case | `true` |
| AddQuotesToDates | bool | Whether to add quotes around DATE and DATETIME fields | `true` |
| DateFormat | string | Date format to use for formatting DATE columns, use .NET formatting tokens. Note that formatting is done using invariant culture. | `yyyy-MM-dd` |
| DateTimeFormat | string | Date format to use for formatting DATETIME columns, use .NET formatting tokens. Note that formatting is done using invariant culture. | `yyyy-MM-dd HH:mm:ss` |

### Notes
Newlines in text fields are replaced with spaces.

### Returns

Result contains the amount of lines written to output CSV.

# License

This project is licensed under the MIT License - see the LICENSE file for details

# Building

Clone a copy of the repo

`git clone https://github.com/CommunityHiQ/Frends.Community.SQL.git`

Restore dependencies

`nuget restore`

Rebuild the project

Run Tests with MSTest. Tests can be found under

`Frends.Community.SQL.Tests\bin\Release\Frends.Community.SQL.dll`

Create a nuget package

`nuget pack Frends.Community.SQL.nuspec`

# Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

# Change Log

| Version             | Changes                 |
| ---------------------| ---------------------|
| 1.0.0 | Initial version of SaveQueryToCSV task |
| 1.1.0 | Added options for destination file encoding. |
| 1.1.1 | Converted to support .Net Framework 4.7.1 and .Net Standard 2.0. Renamed task. |