using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;

namespace SkorubaIdentityServer4Admin.Admin.Helpers.TagHelpers
{
    [HtmlTargetElement("picker")]
    public class PickerTagHelper : TagHelper
    {
        public string Url { get; set; }

        [Required]
        public string Id { get; set; }

        public string SelectedItemsTitle { get; set; }

        public string SearchInputPlaceholder { get; set; }

        public string SearchResultTitle { get; set; }

        public string SuggestedItemsTitle { get; set; }

        public string NoItemSelectedTitle { get; set; }

        public List<string> SelectedItems { get; set; }

        public string SelectedItem { get; set; }

        public string ShowAllItemsTitle { get; set; }

        public int MinSearchText { get; set; }

        public bool MultipleSelect { get; set; }

        public bool AllowItemAlreadySelectedNotification { get; set; } = true;

        public string ItemAlreadySelectedTitle { get; set; }
        
        public bool AllowSuggestedItems { get; set; } = true;

        public int TopSuggestedItems { get; set; } = 5;

        public bool Required { get; set; }

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
            var selectedItems = GetSelectedItems();

            var component = new
            {
                name = "picker",
                @params = new
                {
                    search = "",
                    hiddenId = Id,
                    url = Url,
                    selectedItemsTitle = SelectedItemsTitle,
                    allowSuggestedItems = AllowSuggestedItems,
                    searchResultTitle = SearchResultTitle,
                    suggestedItemsTitle = SuggestedItemsTitle,
                    noItemSelectedTitle = NoItemSelectedTitle,
                    searchInputPlaceholder = SearchInputPlaceholder,
                    showAllItemsTitle = ShowAllItemsTitle,
                    selectedItems,
                    minSearchText = MinSearchText,
                    topSuggestedItems = TopSuggestedItems,
                    multipleSelect = MultipleSelect,
                    allowItemAlreadySelectedNotification = AllowItemAlreadySelectedNotification,
                    itemAlreadySelectedTitle = ItemAlreadySelectedTitle
                }
            };

            var rawPickerHtml = new HtmlString($"<div data-bind='component: {JsonConvert.SerializeObject(component, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml })}'></div>");

            output.Content.AppendHtml(rawPickerHtml);
        }

        /// <summary>
        /// Get Selected Items
        /// </summary>
        /// <returns></returns>
        private List<string> GetSelectedItems()
        {
            return MultipleSelect ? GetSelectedItemsWithRemovedQuotes() : GetSelectedItemWithRemovedQuotes();
        }

        /// <summary>
        /// Get Selected Items - with removed quotes
        /// </summary>
        /// <returns></returns>
        private List<string> GetSelectedItemsWithRemovedQuotes()
        {
            for (var i = 0; i < SelectedItems?.Count; i++)
            {
                SelectedItems[i] = SelectedItems[i].Replace("'", "").Replace("\"", "");
            }

            return SelectedItems;
        }

        /// <summary>
        /// Get Selected Item - with removed quotes, the picker component expect the collection of items, therefore it is used the list for single value as well
        /// </summary>
        /// <returns></returns>
        private List<string> GetSelectedItemWithRemovedQuotes()
        {
            SelectedItem = SelectedItem.Replace("'", "").Replace("\"", "");

            return string.IsNullOrWhiteSpace(SelectedItem) ? new List<string>() : new List<string> { SelectedItem };
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





