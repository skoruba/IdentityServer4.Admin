using System;

namespace Skoruba.IdentityServer4.Admin.Helpers
{
    public static class PagerHelpers
    {
        public static int GetTotalPages(int pageSize, int totalCount)
        {
            return (int)Math.Ceiling((double)totalCount / pageSize);
        }

        public static bool ShowRightPagerButton(int maxPages, int totalPages, string currentPage)
        {
            var maxPageToRender = GetMaxPageToRender(maxPages, totalPages, currentPage);

            return maxPageToRender < totalPages;
        }

        public static bool ShowLeftPagerButton(int maxPages, int totalPages, string currentPage)
        {
            var minPageToRender = GetMinPageToRender(maxPages, totalPages, currentPage);

            return minPageToRender > maxPages;
        }

        public static int GetMinPageToRender(int maxPages, int totalPages, string currentPage)
        {
            var currentMaxPages = GetMaxPageToRender(maxPages, totalPages, currentPage);

            if (currentMaxPages < maxPages) return currentMaxPages;
            
            if (currentMaxPages == totalPages)
            {
                currentMaxPages = GetMaxPage(maxPages, totalPages, currentPage);
            }

            var minPageToRender = currentMaxPages - maxPages + 1;

            return minPageToRender;
        }

        public static int GetMaxPage(int maxPages, int totalPages, string currentPage)
        {
            var page = GetCurrentPage(currentPage);
            var result = (int)Math.Ceiling((double)page / maxPages);
            return result * maxPages;
        }

        public static int GetMaxPageToRender(int maxPages, int totalPages, string currentPage)
        {
            var currentMaxPages = GetMaxPage(maxPages, totalPages, currentPage);

            return currentMaxPages > totalPages ? totalPages : currentMaxPages;
        }

        public static int GetCurrentPage(string currentPage)
        {
            try
            {
                return string.IsNullOrEmpty(currentPage) ? 1 : Convert.ToInt32(currentPage);
            }
            catch
            {
                return 1;
            }
        }
    }
}
