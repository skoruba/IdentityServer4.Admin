namespace Skoruba.IdentityServer4.Admin.Common.Settings
{
    /// Serilog logging settings
    public interface ILoggingSettings
    {
        /// File sink
        string FileLogPath { get; }
        int FileMinimumLogLevel { get; }

        /// Database sink
        bool DBAutoCreation { get; }
        string DbConnectionString { get; }
        string DbTableName { get; }
        int DbMinimumLogLevel { get; }


        /// Azure AppInsights sink
        string AppInsightsInstrumentationKey { get; }
        int AppInsightsMinimumLogLevel { get; }

        /// Splunk sink
        string SplunkEventCollector { get; }
        string SplunkIndex { get; }
        string Splunk_Source { get; }
        bool SplunkLogPostData { get; }
        int SplunkMinimumLogLevel { get; }
    }
}