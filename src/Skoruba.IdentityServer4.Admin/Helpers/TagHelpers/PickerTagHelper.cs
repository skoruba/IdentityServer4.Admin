using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;

namespace Skoruba.IdentityServer4.Admin.Helpers.TagHelpers
{
    [HtmlTargetElement("picker")]
    public class PickerTagHelper : TagHelper
    {
        [HtmlAttributeName("url")]
        public string Url { get; set; }

        [Required]
        [HtmlAttributeName("id")]
        public string Id { get; set; }

        [HtmlAttributeName("selected-items-title")]
        public string SelectedItemsTitle { get; set; }

        [HtmlAttributeName("search-input-placeholder")]
        public string SearchInputPlaceholder { get; set; }

        [HtmlAttributeName("selected-items")]
        public List<string> SelectedItems { get; set; }

        [HtmlAttributeName("selected-item")]
        public string SelectedItem { get; set; }

        [HtmlAttributeName("min-search-text")]
        public int MinSearchText { get; set; }

        [HtmlAttributeName("multiple-select")]
        public bool MultipleSelect { get; set; }

        [HtmlAttributeName("required")]
        public bool Required { get; set; }

        [HtmlAttributeName("required-message")]
        public string RequiredMessage { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            AddWrapper(output);
            AddComponent(output);
            AddHiddenField(output);
        }

        private void AddWrapper(TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";
            output.Attributes.Add("class", "hidden picker");
            output.Attributes.Add(new TagHelperAttribute("data-bind", new HtmlString("css: { hidden: false }")));
        }

        private void AddComponent(TagHelperOutput output)
        {
            var selectedItems = GetSelectedItemsToJsonString();

            var rawPickerHtml = new HtmlString(
                "<div data-bind='component: { name: \"picker\", params: { search: \"\", hiddenId: \"" + Id + "\", url: \"" +
                Url + "\", selectedItemsTitle: \"" + SelectedItemsTitle + "\", searchInputPlaceholder: \"" +
                SearchInputPlaceholder + "\", selectedItems: " + selectedItems + ", minSearchText: " + MinSearchText +
                ", multipleSelect: " + MultipleSelect.ToString().ToLower() + " }}'></div>");

            output.Content.AppendHtml(rawPickerHtml);
        }

        /// <summary>
        /// Get Selected Items to JSON string - with removed quotes
        /// </summary>
        /// <returns></returns>
        private string GetSelectedItemsToJsonString()
        {
            for (var i = 0; i < SelectedItems?.Count; i++)
            {
                SelectedItems[i] = SelectedItems[i].Replace("'", "").Replace("\"", "");
            }

            var selectedItems = MultipleSelect ? JsonConvert.SerializeObject(SelectedItems ?? new List<string>()) : JsonConvert.SerializeObject(SelectedItem);

            return selectedItems;
        }

        private void AddHiddenField(TagHelperOutput output)
        {
            var hiddenField = new TagBuilder("input");
            hiddenField.Attributes.Add("type", "hidden");
            hiddenField.Attributes.Add("id", Id);
            hiddenField.Attributes.Add("name", Id);
            hiddenField.Attributes.Add("value", string.Empty);

            if (Required)
            {
                hiddenField.Attributes.Add("required", string.Empty);
                hiddenField.Attributes.Add("data-val", "true");
                hiddenField.Attributes.Add("data-val-required", RequiredMessage ?? $"The {Id} field is required.");
                hiddenField.Attributes.Add("aria-required", "true");
            }

            output.Content.AppendHtml(hiddenField);
        }
    }
}