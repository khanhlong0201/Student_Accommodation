using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Models;
namespace BHSytem.Models.Entities
{
    public class ImagesDetails : Auditable
    {
        [Key]
        public int Id { get; set; }

        [Key]
        public int Image_Id { get; set; }

        [MaxLength(500)]
        public string File_Path { get; set; }
        public Images Image { get; set; } 

    }
}
