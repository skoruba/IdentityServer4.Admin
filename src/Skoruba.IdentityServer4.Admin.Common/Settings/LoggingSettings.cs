using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.Common.Settings
{
    public class LoggingSettings : ILoggingSettings
    {
        public string FileLogPath { get; set; }
        public int FileMinimumLogLevel { get; set; }


        public string DbConnectionString { get; set; }
        public string DbTableName { get; set; }
        public bool DBAutoCreation { get; set; }
        public int DbMinimumLogLevel { get; }


        public string SplunkEventCollector { get; set; }
        public string SplunkIndex { get; set; }
        public string Splunk_Source { get; set; }
        public bool SplunkLogPostData { get; set; }
        public int SplunkMinimumLogLevel { get; set; }


        public string AppInsightsInstrumentationKey { get; set; }
        public int AppInsightsMinimumLogLevel { get; set; }

    }
}
