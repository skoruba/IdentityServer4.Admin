using Serilog;
using Serilog.Core;
using Serilog.Sinks.MSSqlServer;
using Skoruba.IdentityServer4.Audit.EntityFramework.Constants;
using Skoruba.IdentityServer4.Audit.Sink;
using Skoruba.IdentityServer4.Audit.Sink.Recorders;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Audit.EntityFramework.Recorders
{
    public class SerilogRecorder : IRecordAuditActions
    {
        private readonly Logger _logger;

        private ColumnOptions ColumnOptions()
        {
            var options = new ColumnOptions
            {
                AdditionalColumns = new Collection<SqlColumn>
                {
                    new SqlColumn("TimeStamp", System.Data.SqlDbType.DateTime, false),
                    new SqlColumn("Category", System.Data.SqlDbType.NVarChar, true),
                    new SqlColumn("Action", System.Data.SqlDbType.NVarChar, true),
                    new SqlColumn("EventType", System.Data.SqlDbType.NVarChar, true),
                    new SqlColumn("SourceType", System.Data.SqlDbType.NVarChar, true, 100),
                    new SqlColumn("SourceId", System.Data.SqlDbType.NVarChar, true, 100),
                    new SqlColumn("Source", System.Data.SqlDbType.NVarChar, true, 100),
                    new SqlColumn("ActorType", System.Data.SqlDbType.NVarChar, true, 100),
                    new SqlColumn("ActorId", System.Data.SqlDbType.NVarChar, true, 100),
                    new SqlColumn("Actor", System.Data.SqlDbType.NVarChar, true, 100),
                    new SqlColumn("SubjectType", System.Data.SqlDbType.NVarChar, true, 100),
                    new SqlColumn("SubjectId", System.Data.SqlDbType.NVarChar, true, 100),
                    new SqlColumn("Subject", System.Data.SqlDbType.NVarChar, true, 100),
                    new SqlColumn("RemoteIpAddress", System.Data.SqlDbType.NVarChar, true, 100),
                    new SqlColumn("LocalIpAddress", System.Data.SqlDbType.NVarChar, true, 100),
                    new SqlColumn("Changes", System.Data.SqlDbType.NVarChar, true)
                },
            };
            options.Store.Remove(StandardColumn.TimeStamp);
            options.Store.Remove(StandardColumn.Exception);
            options.Store.Remove(StandardColumn.Properties);
            options.Store.Add(StandardColumn.LogEvent);
            options.Id.DataType = System.Data.SqlDbType.BigInt;

            return options;
        }

        public SerilogRecorder(string connectionString)
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .AuditTo.MSSqlServer(
                    autoCreateSqlTable: true,
                    connectionString: connectionString,
                    tableName: ConfigurationConsts.AuditLogTableName,
                    columnOptions: ColumnOptions()
                    )
                .CreateLogger();
        }

        public Task RecordFailure(IAuditArgs auditEventArguments)
        {
            return Write(auditEventArguments, false);
        }

        public Task RecordSuccess(IAuditArgs auditEventArguments)
        {
            return Write(auditEventArguments, true);
        }

        private Task Write(IAuditArgs args, bool success)
        {
            string message =
                  "{@timestamp}" +
                  "{@category}" +
                  "{@action}" +
                  "{@eventType}" +
                  "{@sourceType}" +
                  "{@sourceId}" +
                  "{@source}" +
                  "{@actorType}" +
                  "{@actorId}" +
                  "{@actor}" +
                  "{@subjectType}" +
                  "{@subjectId}" +
                  "{@subject}" +
                  "{@remoteIpAddress}" +
                  "{@localIpAddress}" +
                  "{@changes}"
                  ;

            _logger.Information(message,
                args.EventDetail.TimeStamp,
                args.EventDetail.Category,
                args.EventDetail.Name,
                args.EventDetail.EventType,
                args.Source.Type,
                args.Source.Id,
                args.Source.Name,
                args.Actor.Type,
                args.Actor.Id,
                args.Actor.Name,
                args.Subject.Type,
                args.Subject.Id,
                args.Subject.Name,
                args.EventDetail.RemoteIpAddress,
                args.EventDetail.LocalIpAddress,
                args.Changes
                );

            return Task.CompletedTask;
        }
    }
}