namespace BHSystem.Web.Module
{
    public static class Variable
    {
        public const string URL_LOGIN = "DMS/loginweb";
        public const string URL_LOGOUT = "DMS/logoutweb";
        public const string URL_CHANGEPASS = "DMS/ChangePassword";

        public const string URL_GETDOC = "DMS/GetDoc";
        public const string URL_GETDATACOMBO = "DMS/GetDataCombo";
        public const string URL_GETDATA = "DMS/GetData";
        public const string URL_GETDATAREPORT = "DMS/GetDataReport";

        public const string URL_GETLISTDOC = "DMS/GetListDoc";

        public const string URL_GETLISTDOC_APPROVAL = "DMS/GetListDoc";

        public const string URL_GETINFODOC = "DMS/GetInfo";
        public const string URL_SAVEDATA = "DMS/SaveData";
        public const string URL_SYNCDATA = "DMS/SyncData";
        public const string URL_PRINT= "DMS/ExportPDF";


        /// <summary>
        /// 1: DUYỆT, 2: TỪ CHỐI
        /// </summary>
        public const string URL_APPROVAL = "DMS/ApprovalByS1";
        public const string URL_APPROVAL_S1DMS = "DMS/ApprovalS1DMS";

        public const string URL_ADDDOC = "DMS/AddDoc";
        public const string URL_UPDATE_S1DMS = "DMS/UpdateS1DMS";
        public const string URL_UPLOAD_FILE = "DMS/UploadMultiFile";
        public const string URL_CLOSED = "DMS/CloseByS1";

        public static string FormatNumberS1 = "#,##0.####";
        public static string FormatNumberQuantities = "#,##0.##";//kiểu số lượng
        public static string FormatNumber = "#,##0.#";//
        public static string FormatNumberTotal = "#,##0";//
        public static string FormatNumberTotalGrid = "Tổng: {0:#,##0}";
        public static string FormatNumberNumRowGrid = "Số dòng: {0}";
        public static string FormatNumberTotalGridDecimal = "Tổng: {0:#,##0.##}";

        public static string MaskNumberDecimal = "###,###,###,##0.##";//kiểu số co 1e
        public static string MaskNumber = "###,###,###,##0";//k có lẻ

        public static string FormatDate = "dd/MM/yyyy";//kiểu date
        public static string FormatDateTime = "dd/MM/yyyy HH:mm";//kiểu date

        public static string FormatNumberCurrencyVND = "#,###.## VND";//dùng cho tạo chứng từ
        public static string FormatNumberCurrencyUSD = "$ #,###.##";//dùng cho tạo chứng từ

        /// <summary>
        /// danh sách page size
        /// </summary>
        public static int[] PageSizeList = new int[] { 10, 50, 100, 300, 500, 1000 };
        public static int PageSize = 50;

        /// <summary>
        /// page size api trả về
        /// </summary>
        public static int PageSizeAPI = 4000;
        public static string ProjectWeb = "DMS";
    }

    /// <summary>
    /// Danh sách ICON BUTTON
    /// </summary>
    public static class ButtonIconType
    {
        public const string BTN_SAVE = "fa fa-save";
        public const string BTN_EXCEL = "fas fa-file-excel";
        public const string BTN_PRINT = "fa fa-print";
        public const string BTN_RELOAD = "fas fa-sync-alt";
        public const string BTN_COPY = "far fa-copy";
        public const string BTN_STOP_PROCCESS = "far fa-times-circle";
        public const string BTN_COMFIRM = "fas fa-check"; //longtran 20230510 xác nhận
        public const string BTN_REFUSE = "fa fa-user-times"; //longtran 20230510 từ chối hoặc xóa dấu X
        public const string BTN_ADD_NEW = "fa fa-plus-circle"; // icon thêm mới
        public const string BTN_NARROW = "fa fa-angle-left"; //longtran 20230822 thu hẹp 
        public const string BTN_EXTEND = "fa fa-arrows-alt"; //longtran 20230822 mở rộng
    }

    public static class TitleTabContanst
    {
        public const string TAB_ORDER = "Đơn hàng";
        public const string TAB_RELATED_INFO = "Thông tin liên quan";
        public const string TAB_ITEMS = "Mặt hàng";
        public const string TAB_FILE = "Đính kèm";
        public const string TAB_HISTORY_FILE = "Lịch sử chứng từ";
        public const string TAB_WAITING_PROGRESS = "Chờ xử lý";
        public const string TAB_DENY_OR_STOP_PROGRESS = "Từ chối/Dừng xử lý";
        public const string TAB_ALL_PROGRESS = "Tất cả";
        public const string TAB_AUTH_VTYPE = "Chứng từ";
        public const string TAB_AUTH_MENU = "Chức năng";
        public const string TAB_AUTH_STATUS = "Tình trạng";
        public const string TAB_AUTH_GROUP = "Nhóm hàng";
        public const string TAB_AUTH_PARTNER = "Nhà phân phối";
        public const string TAB_MESS_NEW = "Thông báo gần đây";
        public const string TAB_AUTH_MESS = "Thông báo"; //longtran 20230623
        public const string TAB_PURCHASE_DETAILS = "Chi tiết mua hàng"; //longtran 20230704
        public const string TAB_PURCHASE_FORM = "Hình thức mua hàng";  //longtran 20230704
        public const string TAB_CONSUMPTION_AREA = "Vùng tiêu thụ";  //longtran 20230704

    }

    public static class DocConstant
    {
        public static readonly DateTime FROM_DATE_DEFAULT = new DateTime(2020, 01, 01);
        public static readonly DateTime TO_DATE_DEFAULT = new DateTime(2100, 01, 01);
        public const string VTYPE_DEFAULT = "1010";
    }


}