using System.ComponentModel;

namespace Frends.Community.SQL.QueryToFile
{
    public class SaveQueryToCSVParameters
    {
        /// <summary>
        /// Query to execute
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Query parameters
        /// </summary>
        public SQLParameter[] QueryParameters { get; set; }

        /// <summary>
        /// Database connection string
        /// </summary>
        [DefaultValue("\"Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;\"")]
        [PasswordPropertyText]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Operation timeout (seconds)
        /// </summary>
        [DefaultValue(30)]
        public int TimeoutSeconds { get; set; }

        /// <summary>
        /// Output file path
        /// </summary>
        [DefaultValue("")]
        public string OutputFilePath { get; set; }
    }
}
