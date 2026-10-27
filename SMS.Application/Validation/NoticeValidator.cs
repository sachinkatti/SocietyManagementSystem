namespace SMS.Application
{
    public static class NoticeValidator
    {
        public static List<string> Validate(NoticeDto notice)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(notice.Title))
            {
                errors.Add("Notice title is required.");
            }
            else if (notice.Title.Length > 300)
            {
                errors.Add("Notice title cannot exceed 300 characters.");
            }

            if (string.IsNullOrWhiteSpace(notice.Message))
            {
                errors.Add("Notice message is required.");
            }
            else if (notice.Message.Length > 2000)
            {
                errors.Add("Notice message cannot exceed 2000 characters.");
            }

            if (string.IsNullOrWhiteSpace(notice.Category))
            {
                errors.Add("Notice category is required.");
            }
            else if (notice.Category.Length > 60)
            {
                errors.Add("Notice category cannot exceed 60 characters.");
            }

            if (notice.TenantId <= 0)
            {
                errors.Add("Please select a valid tenant.");
            }

            if (notice.PublishedByAdminId <= 0)
            {
                errors.Add("Please select a valid publishing admin.");
            }

            return errors;
        }
    }
}