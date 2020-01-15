namespace Skoruba.MultiTenant.Finbuckle.Configuration
{
    public class RequestParam
    {
        public string Controller { get; set; }
        public string Action { get; set; }

        /// <summary>
        /// The name of the variable
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 1 = Code, 2 = Identifier, 3 = Id
        /// </summary>
        public int Type { get; set; } = 1;
    }
}