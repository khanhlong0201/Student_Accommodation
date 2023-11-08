namespace BHSystem.Web.Constants
{
    public class EndpointConstants
    {
        public const string URL_USER_LOGIN = "Users/Login";
        public const string URL_USER_UPDATE = "Users/Update";
        public const string URL_USER_GETALL = "Users/GetAll";
        public const string URL_USER_GET_USER_ROLE = "Users/GetUserByRole";
        public const string URL_USER_DELETE = "Users/Delete";

        public const string URL_BOARDINGHOUSE_CREATE = "BoardingHouses/Create";
        public const string URL_BOARDINGHOUSE_GETALL = "BoardingHouses/GetAll";
        public const string URL_BOARDINGHOUSE_UPDATE = "BoardingHouses/Update";
        public const string URL_BOARDINGHOUSE_DELETE = "BoardingHouses/Delete";

        public const string URL_ROLE_DELETE = "Roles/Delete";
        public const string URL_ROLE_UPDATE = "Roles/Update";
        public const string URL_ROLE_GETALL = "Roles/GetAll";

        public const string URL_CITY_GETALL = "Citys/GetAll";

        public const string URL_DISTINCT_GETALL = "Distincts/GetAll";
        public const string URL_DISTINCT_GET_BY_CITY = "Distincts/GetAllByCity";

        public const string URL_WARD_GETALL = "Wards/GetAll";
        public const string URL_WARD_GET_BY_DISTINCT = "Wards/GetAllByDistinct";

        public const string URL_USER_ROLE_UPDATE= "UserRoles/AddOrDelete";
        public const string URL_ROLE_MENU_UPDATE= "RoleMenus/AddOrDelete";
        public const string URL_MENU_GET_MENU_ROLE = "Menus/GetMenuByRole";
        public const string URL_MENU_GET_BY_USER = "Menus/GetMenuByUser";

        public const string URL_BOOKING_UPDATE = "Bookings/Update";

        public const string URL_IMAGE_DETAIL_GET_BY_IMAGE_ID = "ImagesDetails/GetImageDetailByImageIdAsync";
        public const string URL_IMAGE_DETAIL_DELTETE = "ImagesDetails/Delete";
    }
}
