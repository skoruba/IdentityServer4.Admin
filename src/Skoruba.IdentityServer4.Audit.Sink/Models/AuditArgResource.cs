namespace Skoruba.IdentityServer4.Audit.Sink
{
    public class AuditArgResource
    {
        public const string UserType = "User";
        public const string AppType = "App";

        public AuditArgResource(string id, string name, string type)
        {
            Id = id;
            Name = name;
            Type = type;
        }

        public string Id { get; }
        public string Name { get; }
        public string Type { get; }
    }
}