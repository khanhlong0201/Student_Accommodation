using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Models;
namespace BHSytem.Models.Entities
{
    public class Comments : Auditable
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public string Describe { get; set; }
        public int Image_Id { get; set; }
        public int BoardingHouse_Id { get; set; }
        public int UserId { get; set; }
        public Users Users { get; set; }
        public Images Image { get; set; }
        public BoardingHouses BoardingHouses { get; set; }

    }
}
