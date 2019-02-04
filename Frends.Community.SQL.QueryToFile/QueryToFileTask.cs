﻿using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.Community.SQL.QueryToFile
{
    public class QueryToFileTask
    {
        /// <summary>
        /// Saves SQL query results to CSV file.
        /// </summary>
        /// <param name="parameters">Parameters of task</param>
        /// <param name="options">Additional options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Returns amount of entries written</returns>
        public async static Task<int> SaveQueryToCSV([PropertyTab] SaveQueryToCSVParameters parameters, [PropertyTab] SaveQueryToCSVOptions options, CancellationToken cancellationToken)
        {
            int output = 0;
            using (var writer = new StreamWriter(parameters.OutputFilePath))
            using (var csvFile = CreateCsvWriter(options.GetFieldDelimeterAsString(), writer))
            using (var sqlConnection = new SqlConnection(parameters.ConnectionString))
            {
                writer.NewLine = options.GetLineBreakAsString();

                await sqlConnection.OpenAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                using (var command = BuildSQLCommand(parameters.Query, parameters.QueryParameters))
                {
                    command.CommandTimeout = parameters.TimeoutSeconds;
                    command.Connection = sqlConnection;

                    var reader = await command.ExecuteReaderAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    output = DataReaderToCsv(reader, csvFile, options, cancellationToken);
                }

                csvFile.Flush();
            }

            return output;
        }

        internal static CsvWriter CreateCsvWriter(string delimeter, TextWriter writer)
        {
            var csvOptions = new Configuration
            {
                Delimiter = delimeter,
            };
            return new CsvWriter(writer, csvOptions);
        }
        internal static string FormatDbHeader(string header, bool forceSpecialFormatting)
        {
            if (!forceSpecialFormatting) return header;

            // First part of regex removes all non-alphanumeric ('_' also allowed) chars from the whole string
            // Second part removed any leading numbers or underscoress
            Regex rgx = new Regex("[^a-zA-Z0-9_-]|^[0-9_]+");
            header = rgx.Replace(header, "");
            return header.ToLower();
        }

        /// <summary>
        /// Formats a value according to options
        /// </summary>
        /// <param name="value">Value from the database</param>
        /// <param name="dbTypeName">Type of database column. E.g. for differentiating between DATE and DATETIME types</param>
        /// <param name="options">Formatting options</param>
        /// <returns></returns>
        internal static string FormatDbValue(object value, string dbTypeName, Type dotnetType, SaveQueryToCSVOptions options)
        {
            if (value == null || value == DBNull.Value)
            {
                if (dotnetType == typeof(string)) return "\"\"";
                if (dotnetType == typeof(DateTime) && options.AddQuotesToDates) return "\"\"";
                return "";
            }

            if (dotnetType == typeof(string))
            {
                var str = (string)value;
                var delimiter = options.GetFieldDelimeterAsString();
                str = str.Replace("\"", "\\\"");
                str = str.Replace("\r\n", " ");
                str = str.Replace("\r", " ");
                str = str.Replace("\n", " ");
                return $"\"{str}\"";
            }

            if (dotnetType == typeof(DateTime))
            {
                var dateTime = (DateTime)value;
                var dbType = dbTypeName?.ToLower();
                string output = null;
                switch(dbType)
                {
                    case "date":
                        output = dateTime.ToString(options.DateFormat, CultureInfo.InvariantCulture);
                        break;
                    case "datetime":
                    default:
                        output = dateTime.ToString(options.DateTimeFormat, CultureInfo.InvariantCulture);
                        break;
                }

                if (options.AddQuotesToDates) return $"\"{output}\"";
                return output;
            }
            
            if (dotnetType == typeof(float))
            {
                var floatValue = (float)value;
                return floatValue.ToString("0.###########", CultureInfo.InvariantCulture);
            }

            if (dotnetType == typeof(double))
            {
                var doubleValue = (double)value;
                return doubleValue.ToString("0.###########", CultureInfo.InvariantCulture);
            }

            if (dotnetType == typeof(decimal))
            {
                var decimalValue = (decimal)value;
                return decimalValue.ToString("0.###########", CultureInfo.InvariantCulture);
            }

            return value.ToString();
        }

        internal static int DataReaderToCsv(
            DbDataReader reader,
            CsvWriter csvWriter,
            SaveQueryToCSVOptions options,
            CancellationToken cancellationToken)
        {
            // Write header and remember column indexes to include
            var columnIndexesToInclude = new List<int>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var includeColumn = 
                    options.ColumnsToInclude == null ||
                    options.ColumnsToInclude.Length == 0 ||
                    options.ColumnsToInclude.Contains(columnName);

                if (includeColumn)
                {
                    if (options.IncludeHeadersInOutput)
                    {
                        var formattedHeader = FormatDbHeader(columnName, options.SanitizeColumnHeaders);
                        csvWriter.WriteField(formattedHeader);
                    }
                    columnIndexesToInclude.Add(i);
                }
            }

            if (options.IncludeHeadersInOutput) csvWriter.NextRecord();

            int count = 0;
            while (reader.Read())
            {
                foreach (var columnIndex in columnIndexesToInclude)
                {
                    var value = reader.GetValue(columnIndex);
                    var dbTypeName = reader.GetDataTypeName(columnIndex);
                    var dotnetType = reader.GetFieldType(columnIndex);
                    var formattedValue = FormatDbValue(value, dbTypeName, dotnetType, options);
                    csvWriter.WriteField(formattedValue, false);
                }
                csvWriter.NextRecord();
                count++;
            }

            return count;
        }

        private static SqlCommand BuildSQLCommand(string query, SQLParameter[] parmeters)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                foreach (SQLParameter parameter in parmeters)
                {
                    command.Parameters.AddWithValue(parameter.Name, parameter.Value);
                }

                return command;
            }
        }
    }
}
