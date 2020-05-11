using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Community.SQL.QueryToFile
{
    public enum FileEncoding { UTF8, ANSI, ASCII, Unicode, Other }

    public class SaveQueryToCSVOptions
    {
        

        /// <summary>
        /// Columns to include in the CSV output. Leave empty to include all columns in output.
        /// </summary>
        public string[] ColumnsToInclude { get; set; }

        /// <summary>
        /// What to use as field separators
        /// </summary>
        [DefaultValue(CsvFieldDelimiter.Semicolon)]
        public CsvFieldDelimiter FieldDelimiter { get; set; } = CsvFieldDelimiter.Semicolon;

        /// <summary>
        /// What to use as line breaks
        /// </summary>
        [DefaultValue(CsvLineBreak.CRLF)]
        public CsvLineBreak LineBreak { get; set; } = CsvLineBreak.CRLF;

        /// <summary>
        /// Output file encoding
        /// </summary>
        [DefaultValue(FileEncoding.UTF8)]
        public FileEncoding FileEncoding { get; set; }

        [UIHint(nameof(FileEncoding), "", FileEncoding.UTF8)]
        public bool EnableBom { get; set; }

        /// <summary>
        /// File encoding to be used. A partial list of possible encodings: https://en.wikipedia.org/wiki/Windows_code_page#List
        /// </summary>
        [UIHint(nameof(FileEncoding), "", FileEncoding.Other)]
        public string EncodingInString { get; set; }

        /// <summary>
        /// Whether to include headers in output
        /// </summary>
        [DefaultValue(true)]
        public bool IncludeHeadersInOutput { get; set; } = true;

        /// <summary>
        /// Whether to sanitize headers in output:
        /// - Strip any chars that are not 0-9, a-z or _
        /// - Make sure that column does not start with a number or underscore
        /// - Force lower case
        /// </summary>
        [DefaultValue(true)]
        public bool SanitizeColumnHeaders { get; set; } = true;

        /// <summary>
        /// Whether to add quotes around DATE and DATETIME fields
        /// </summary>
        [DefaultValue(true)]
        public bool AddQuotesToDates { get; set; } = true;

        /// <summary>
        /// Date format to use for formatting DATE columns, use .NET formatting tokens.
        /// Note that formatting is done using invariant culture.
        /// </summary>
        [DefaultValue("\"yyyy-MM-dd\"")]
        public string DateFormat { get; set; } = "yyyy-MM-dd";

        /// <summary>
        /// Date format to use for formatting DATETIME columns, use .NET formatting tokens.
        /// Note that formatting is done using invariant culture.
        /// </summary>
        [DefaultValue("\"yyyy-MM-dd HH:mm:ss\"")]
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

        public string GetFieldDelimeterAsString()
        {
            switch(this.FieldDelimiter)
            {
                case CsvFieldDelimiter.Comma:
                    return ",";
                case CsvFieldDelimiter.Pipe:
                    return "|";
                case CsvFieldDelimiter.Semicolon:
                    return ";";
                default:
                    throw new Exception($"Unknown field delimeter: {this.FieldDelimiter}");
            }
        }
        public string GetLineBreakAsString()
        {
            switch (this.LineBreak)
            {
                case CsvLineBreak.CRLF:
                    return "\r\n";
                case CsvLineBreak.CR:
                    return "\r";
                case CsvLineBreak.LF:
                    return "\n";
                default:
                    throw new Exception($"Unknown field delimeter: {this.FieldDelimiter}");
            }
        }
    }
}
