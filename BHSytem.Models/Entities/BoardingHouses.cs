using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Models;
namespace BHSytem.Models.Entities
{
    public class BoardingHouses : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự tăng
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public int User_Id { get; set; }
        public int Ward_Id { get; set; }
        [MaxLength(255)]
        public string Adddress { get; set; }
        public int Qty { get; set; }
        public int Image_Id { get; set; }
        public Users Users { get; set; }
        public Wards Wards { get; set; }
        public Images Images { get; set; }
        public virtual ICollection<Rooms> Rooms { get; set; }
        public virtual ICollection<Comments> Comments { get; set; }

    }
}
