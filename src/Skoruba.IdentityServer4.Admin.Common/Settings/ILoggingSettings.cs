namespace Skoruba.IdentityServer4.Admin.Common.Settings
{
    /// Serilog logging settings
    public interface ILoggingSettings
    {
        /// File sink
        string File_LogPath { get; }
        int File_MinimumLogLevel { get; }

        /// Database sink
        bool DB_AutoCreation { get; }
        string Db_ConnectionString { get; }
        string Db_TableName { get; }
        int Db_MinimumLogLevel { get; }


        /// Azure AppInsights sink
        string AppInsights_InstrumentationKey { get; }
        int AppInsights_MinimumLogLevel { get; }

        /// Splunk sink
        string Splunk_EventCollector { get; }
        string Splunk_Index { get; }
        string Splunk_Source { get; }
        bool Splunk_LogPostData { get; }
        int Splunk_MinimumLogLevel { get; }
    }
}