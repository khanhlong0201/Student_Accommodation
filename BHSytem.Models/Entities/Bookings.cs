using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Models;
namespace BHSytem.Models.Entities
{
    public class Bookings : Auditable
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(255)]
        public string FullName { get; set; }
        [MaxLength(12)]
        public string Phone { get; set; }
        public int Room_Id { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public Users Users { get; set; }
        public Rooms Rooms { get; set; }

    }
}

