using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Entities;
namespace BHSytem.Models.Models
{
    public class BoardingHouseModel : BoardingHouses
    {

        public int Distinct_Id { get; set; }
        public int City_Id { get; set; }
        public string Ward_Name { get; set; }
        public string Distinct_Name { get; set; }
        public string City_Name { get; set; }
        public string Image_Name { get; set; }

        public List<ImagesDetailModel> ImageDetail = new List<ImagesDetailModel>();
        public string File_Path { get; set; }
        public string? UserName { get; set; }
        public string? Phone { get; set; }
    }

    /// <summary>
    /// model để call trên web page admin
    /// </summary>
    public class BHouseModel : Auditable
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng điền Tên phòng trọ")]
        public string? Name { get; set; }
        public int User_Id { get; set; }
        public string? UserName { get; set; }
        public string? Phone { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn Thành phố")]
        public int City_Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn Huyện quận")]
        public int Distinct_Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn Xã phường")]
        public int Ward_Id { get; set; }
        public string? Ward_Name { get; set; }
        public string? Distinct_Name { get; set; }
        public string? City_Name { get; set; }

        [Required(ErrorMessage = "Vui lòng điền Địa chỉ")]
        public string? Adddress { get; set; }
        public int Qty { get; set; }
        public int Image_Id { get; set; }

        public List<ImagesDetailModel>? ListFile { get; set; }
    }

    public class BHouseSearchModel
    {
        public int CityId { get; set; }
        public int DistinctId { get; set;}
        public int WardId { get;set; }
        public string? KeyPrice { get; set; }
        public string? KeyAcreage { get; set; }

        public int Page { get; set; }
        public int Limit { get; set; }
    }

    public class CliBoardingHouseModel
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string? BHouseName { get; set; }
        public string? RoomName { get; set; }
        public string? Addresss { get; set; }
        public string? RoomAddresss { get; set; }
        public string? DistinctName { get; set; }
        public string? CityName { get; set; }
        public string? WardName { get; set; }
        public string? Phone { get; set; }
        public string? FullName { get; set; }
        public DateTime? DateCreate { get; set; }
        public decimal Acreage { get; set; } // diện tích
        public decimal Price { get; set; }
        public string? Desciption { get; set; }
        public string? ImageUrlBHouse { get; set; } // ảnh đại diện cho phòng
        public List<string>? ListImages { get; set; }
        public bool IsHorizontal { get; set; }
    }
}
