using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Models;
namespace BHSytem.Models.Entities
{
    public class Distincts : Auditable
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public int City_Id { get; set; }
        public Citys Citys { get; set; } // Đây là dùng để tham chiếu đến bảng Citys.
        public virtual ICollection<Wards> Wards { get; set; }

    }
}
