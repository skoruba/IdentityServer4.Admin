using System;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts
{
    public class ClientConfigAudit : AuditBase
    {

    }

    public class AuditBase
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public DateTime DateTime { get; set; }
        public string KeyValues { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
    }
}