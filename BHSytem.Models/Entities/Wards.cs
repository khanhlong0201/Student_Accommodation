using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Models;
namespace BHSytem.Models.Entities
{
    public class Wards : Auditable
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public int Distincts_Id { get; set; }
        public Distincts Distincts { get; set; } // Đây là dùng để tham chiếu đến bảng Distincts.
        public virtual ICollection<Users> Users { get; set; }
        public virtual ICollection<BoardingHouses> BoardingHouses { get; set; }

    }
}
