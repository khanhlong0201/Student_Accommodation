using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHSytem.Models.Entities;

namespace BHSytem.Models.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Vui lòng điền Tên người dùng")]
        public string? FullName { get; set; }
        public string? Address { get; set; }
        [Required(ErrorMessage = "Vui lòng điền Số điện thoại")]
        public string? Phone { get; set; }
        public string? Email { get; set; }
        [Required(ErrorMessage = "Vui lòng điền Tên tài khoản")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Vui lòng điền Mật khẩu")]
        public string? Password { get; set; }
        public string? PasswordReset { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? Date_Create { get; set; }
        public int? User_Create { get; set; }
        public DateTime? Date_Update { get; set; }
        public int? User_Update { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn Thành phố")]
        public int City_Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn Huyện quận")]
        public int Distinct_Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn Xã phường")]
        public int Ward_Id { get; set; }
        public string? Ward_Name { get; set; }
        public string? Distinct_Name { get; set; }
        public string? City_Name { get; set; }
        public bool IsAdmin { get; set; }
    }
}
