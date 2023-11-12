using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Entities;
namespace BHSytem.Models.Models
{
    public class BookingModel
    {
        [Required(ErrorMessage = "Vui lòng điền Họ & tên")]
        public string? FullName { get; set; }
        [Required(ErrorMessage = "Vui lòng điền Số điện thoại")]
        public string? Phone { get; set; }
        public int Room_Id { get; set; }
        public string? Room_Name { get; set; }
        public int UserId { get; set; }
        public int Id { get; set; }
        public string? Status { get; set; }
        public DateTime? Date_Create { get; set; }
        public DateTime? Date_Update { get; set; }
        public string? BHouse_Name { get; set; }

    }
}
