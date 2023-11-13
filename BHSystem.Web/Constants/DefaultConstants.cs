namespace BHSystem.Web.Constants
{
    public static class DefaultConstants
    {
        public static readonly DateTime MIN_DATE = DateTime.Now.AddYears(-1);
        public static readonly DateTime MAX_DATE = DateTime.Now.AddYears(1);
        public const string FORMAT_CURRENCY = "#,###.##";
        public const string FORMAT_GRID_CURRENCY = "{0: #,###.##}";
        public const string FORMAT_DATE = "dd/MM/yyyy";
        public const string FORMAT_GRID_DATE = "{0: dd/MM/yyyy}";
        public const string FORMAT_GRID_DATE_TIME = "{0: HH:mm dd/MM/yyyy}";
        public const string FORMAT_DATE_TIME = "HH:mm dd/MM/yyyy";
        public const int PAGE_SIZE = 1000;

        public const string FOLDER_BHOUSE = "ImagesBH";
        public const string FOLDER_ROOM = "ImagesRoom";

        public const string APPROVAL_TYPE_NAME = "Phê duyệt";
        public const string DENY_TYPE_NAME = "Từ chối";
        public const string PENDING_TYPE_NAME = "Chờ xử lý";
        public const string CLOSED_TYPE_NAME = "Đã thuê";
    }
}
