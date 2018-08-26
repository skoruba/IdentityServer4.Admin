using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.Common.Settings
{
    public class LoggingSettings : ILoggingSettings
    {
        public string File_LogPath { get; set; }
        public int File_MinimumLogLevel { get; set; }


        public string Db_ConnectionString { get; set; }
        public string Db_TableName { get; set; }
        public bool DB_AutoCreation { get; set; }
        public int Db_MinimumLogLevel { get; }


        public string Splunk_EventCollector { get; set; }
        public string Splunk_Index { get; set; }
        public string Splunk_Source { get; set; }
        public bool Splunk_LogPostData { get; set; }
        public int Splunk_MinimumLogLevel { get; set; }


        public string AppInsights_InstrumentationKey { get; set; }
        public int AppInsights_MinimumLogLevel { get; set; }

    }
}
