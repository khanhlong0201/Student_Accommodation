namespace BHSystem.Web.Constants
{
    public class EndpointConstants
    {
        public const string URL_USER_LOGIN = "Users/Login";
        public const string URL_USER_UPDATE = "Users/Update";
        public const string URL_USER_GETALL = "Users/GetAll";
        public const string URL_USER_GET_USER_ROLE = "Users/GetUserByRole";
        public const string URL_USER_DELETE = "Users/Delete";
        public const string URL_USER_REGISTER = "Users/Register";
        public const string URL_USER_BY_ID = "Users/GetUserById";


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
        public const string URL_BOOKING_GET_BY_PHONE = "Bookings/GetAllByPhone";

        public const string URL_IMAGE_DETAIL_GET_BY_IMAGE_ID = "ImagesDetails/GetImageDetailByImageIdAsync";
        public const string URL_IMAGE_DETAIL_DELTETE = "ImagesDetails/Delete";

        public const string URL_ROOMPRICE_CREATE = "RoomPrices/Create";
        public const string URL_ROOMPRICE_GETALL = "RoomPrices/GetAll";
        public const string URL_ROOMPRICE_UPDATE = "RoomPrices/Update";
        public const string URL_ROOMPRICE_DELETE = "RoomPrices/Delete";

        public const string URL_ROOM_CREATE = "Rooms/Create";
        public const string URL_ROOM_GETALL = "Rooms/GetAll";
        public const string URL_ROOM_UPDATE = "Rooms/Update";
        public const string URL_ROOM_DELETE = "Rooms/Delete";
        public const string URL_ROOM_GET_BY_BHOUSE = "Rooms/GetAllByBHouse";
        public const string URL_ROOM_GET_BY_STATUS = "Rooms/GetAllByStatus";
        public const string URL_ROOM_UPDATE_STATUS = "Rooms/UpdateStatus";


        public const string URL_BOOKING_GET_BY_STATUS = "Bookings/GetAll";
        public const string URL_BOOKING_UPDATE_STATUS = "Bookings/UpdateStatus";

        public const string URL_MESSAGE_BY_USER = "Messages/GetUnReadMessageByUser";
        public const string URL_MESSAGE_UPDATE_ISREAD = "Messages/UpdateReadMessage";

        #region Client URL
        public const string URL_CLI_BHOUSE_GETDATA = "BHouses/CliGetDataBHouse";
        public const string URL_CLI_BHOUSE_GETDATA_DETAIL = "BHouses/CliGetDataDetails";
        #endregion
    }
}
