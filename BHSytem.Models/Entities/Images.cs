using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Models;
namespace BHSytem.Models.Entities
{
    public class Images : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự tăng
        public int Id { get; set; }

        //[Required]
        [MaxLength(255)]
        public string Type { get; set; }
        public virtual ICollection<ImagesDetails> ImagesDetails { get; set; }
        public virtual ICollection<BoardingHouses> BoardingHouses { get; set; }
        public virtual ICollection<Rooms> Rooms { get; set; }
        public virtual ICollection<Comments> Comments { get; set; }

    }
}
