using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Models;
namespace BHSytem.Models.Entities
{
    public class Menus : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MenuId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Icon { get; set; }

        [MaxLength(255)]
        public string Link { get; set; }

        public int Level { get; set; }

        [MaxLength(255)]
        public string Parent { get; set; }

        [NotMapped] // không muốn lưu thuộc tính này dưới db
        public bool ShowDetail { get; set; } = false;

        [NotMapped]
        public string BreadCrumb { get; set; } = "";

        [NotMapped]
        public string AuthorizationDetail { get; set; } = "";
    }
}
