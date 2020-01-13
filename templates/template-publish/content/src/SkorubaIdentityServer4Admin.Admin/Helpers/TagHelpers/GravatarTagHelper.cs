using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SkorubaIdentityServer4Admin.Admin.Helpers.TagHelpers
{
    [HtmlTargetElement("img-gravatar")]
    public class GravatarTagHelper : TagHelper
    {
        [HtmlAttributeName("email")]
        public string Email { get; set; }

        [HtmlAttributeName("alt")]
        public string Alt { get; set; }

        [HtmlAttributeName("class")]
        public string Class { get; set; }

        [HtmlAttributeName("size")]
        public int Size { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrWhiteSpace(Email))
            {
                var hash = Md5HashHelper.GetHash(Email);

                output.TagName = "img";
                if (!string.IsNullOrWhiteSpace(Class))
                {
                    output.Attributes.Add("class", Class); 
                }

                if (!string.IsNullOrWhiteSpace(Alt))
                {
                    output.Attributes.Add("alt", Alt);
                }
                
                output.Attributes.Add("src", GetAvatarUrl(hash, Size));
                output.TagMode = TagMode.SelfClosing;
            } 
        }

        private static string GetAvatarUrl(string hash, int size)
        {
            var sizeArg = size > 0 ? $"?s={size}" : "";

            return $"https://www.gravatar.com/avatar/{hash}{sizeArg}";
        }
    }
}






