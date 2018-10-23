using System;

namespace SkorubaIdentityServer4Admin.Admin.Helpers
{
    public static class PagerHelpers
    {
        public static int GetTotalPages(int pageSize, int totalCount)
        {
            return (int)Math.Ceiling((double)totalCount / pageSize);
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
            var result = (int)Math.Ceiling((double)currentPage / maxPages);
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
    }
}
