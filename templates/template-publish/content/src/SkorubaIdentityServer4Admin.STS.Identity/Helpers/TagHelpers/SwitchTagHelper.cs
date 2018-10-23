using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SkorubaIdentityServer4Admin.STS.Identity.Helpers.TagHelpers
{
    [HtmlTargetElement("toggle-button")]
    public class SwitchTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = await output.GetChildContentAsync();

            var divSlider = new TagBuilder("div");
            divSlider.AddCssClass("slider round");

            output.TagName = "label";
            output.Attributes.Add("class", "switch");
            output.Content.AppendHtml(childContent);
            output.Content.AppendHtml(divSlider);
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}
