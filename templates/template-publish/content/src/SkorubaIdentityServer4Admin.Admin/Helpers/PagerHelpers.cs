using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace SkorubaIdentityServer4Admin.Admin.Helpers
{
    public static class PagerHelpers
    {
        public static int GetTotalPages(int pageSize, int totalCount)
        {
            return (int) Math.Ceiling((double) totalCount / pageSize);
        }

        public static bool IsActivePage(int currentPage, int currentIteration)
        {
            return currentPage == currentIteration;
        }

        public static bool ShowRightPagerButton(int maxPages, int totalPages, int currentPage)
        {
            var maxPageToRender = GetMaxPageToRender(maxPages, totalPages, currentPage);

            return maxPageToRender < totalPages;
        }

        public static bool ShowLeftPagerButton(int maxPages, int totalPages, int currentPage)
        {
            var minPageToRender = GetMinPageToRender(maxPages, totalPages, currentPage);

            return minPageToRender > maxPages;
        }

        public static int GetMinPageToRender(int maxPages, int totalPages, int currentPage)
        {
            const int defaultPageNumber = 1;
            var currentMaxPages = GetMaxPageToRender(maxPages, totalPages, currentPage);

            if (currentMaxPages == defaultPageNumber) return currentMaxPages;

            if (currentMaxPages == totalPages)
            {
                currentMaxPages = GetMaxPage(maxPages, totalPages, currentPage);
            }

            var minPageToRender = currentMaxPages - maxPages + defaultPageNumber;

            return minPageToRender;
        }

        public static int GetMaxPage(int maxPages, int totalPages, int currentPage)
        {
            var result = (int) Math.Ceiling((double) currentPage / maxPages);
            return result * maxPages;
        }

        public static int GetMaxPageToRender(int maxPages, int totalPages, int currentPage)
        {
            var currentMaxPages = GetMaxPage(maxPages, totalPages, currentPage);

            return currentMaxPages > totalPages ? totalPages : currentMaxPages;
        }

        public static int GetCurrentPage(string currentPage)
        {
            const int defaultPageNumber = 1;

            try
            {
                return string.IsNullOrEmpty(currentPage)
                    ? defaultPageNumber
                    : (Convert.ToInt32(currentPage) <= 0 ? defaultPageNumber : Convert.ToInt32(currentPage));
            }
            catch
            {
                return 1;
            }
        }

        public static QueryString GetQueryString(HttpContext context, int page)
        {
            const string pageKey = "page";

            var queryString = context.Request.QueryString.Value;
            var queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(queryString);
            var items = queryDictionary.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
            
            // Remove existing page key
            items.RemoveAll(x => x.Key == pageKey);

            // Setup new page key
            var queryBuilder = new QueryBuilder(items)
            {
                { pageKey, page.ToString() }
            };

            return queryBuilder.ToQueryString();
        }

        public static string GetUrl(string baseUrl, QueryString queryString)
        {
            return $"{baseUrl}{queryString}";
        }
    }
}






