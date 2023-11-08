using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Entities;
namespace BHSytem.Models.Models
{
    public class RoomPriceModel : RoomPrices
    {
        public string Room_Name { get; set; }
        public int BoardingHouse_Id { get; set; }
        public string BoardingHouse_Name { get; set; }
    }
}
