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
        public string? Phone { get; set; }
        public string? Email { get; set; }
        [Required(ErrorMessage = "Vui lòng điền Tên tài khoản")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Vui lòng điền Mật khẩu")]
        public string? Password { get; set; }
        public string? PasswordReset { get; set; }
    }
}
