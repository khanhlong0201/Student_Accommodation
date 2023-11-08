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
    }

    public class BHouseModel : Auditable
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng điền Tên phòng trọ")]
        public string? Name { get; set; }
        public int User_Id { get; set; }
        public string? UserName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn Thành phố")]
        public int City_Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn Huyện quận")]
        public int Distinct_Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn Xã phường")]
        public int Ward_Id { get; set; }
        public string? WardName { get; set; }

        [Required(ErrorMessage = "Vui lòng điền Địa chỉ")]
        public string? Adddress { get; set; }
        public int Qty { get; set; }
        public int Image_Id { get; set; }

        public List<ImagesDetailModel>? ListFile { get; set; }
    }
}
