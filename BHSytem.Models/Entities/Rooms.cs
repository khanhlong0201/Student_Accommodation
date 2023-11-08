using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Models;
namespace BHSytem.Models.Entities
{
    public class Rooms : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự tăng
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public int Boarding_House_Id { get; set; }
        [MaxLength(50)]
        public string Status { get; set; }
        [MaxLength(250)]
        public string Address { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Price { get; set; }
        public int Image_Id { get; set; }
        [MaxLength(int.MaxValue)]
        public string? Description { get; set; }
        public Images Images { get; set; }
        public BoardingHouses BoardingHouses { get; set; }
        public virtual ICollection<RoomPrices> RoomPrices { get; set; }
        public virtual ICollection<Bookings> Bookings { get; set; }
    }
}
