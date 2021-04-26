namespace Skoruba.IdentityServer4.Admin.UI.Helpers
{
    public static class UrlHelpers
    {
        public static string QueryStringSafeHash(string hash)
        {
            hash = hash.Replace('+', '-');
            return hash.Replace('/', '_');
        }

        public static string QueryStringUnSafeHash(string hash)
        {
            hash = hash.Replace('-', '+');
            return hash.Replace('_', '/');
        }

        public static bool IsNotPresentedValidNumber(this string id)
        {
            int.TryParse(id, out var parsedId);

            return !string.IsNullOrEmpty(id) && parsedId == default;
        }
    }
}
