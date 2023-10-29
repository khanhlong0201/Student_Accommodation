using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Models;
namespace BHSytem.Models.Entities
{
    public class RoomPrices : Auditable
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public int Room_Id { get; set; }
        public decimal Price { get; set; }
        public Rooms Rooms { get; set; } 

    }
}
