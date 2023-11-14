using BHSytem.Models.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BHSytem.Models.Entities
{
    public class Messages : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50)]
        public string? Type { get; set; } // loại thông báo là cái gì -> (Booking/Create Roome/ Approval)

        [StringLength(2500)]
        public string? Message { get; set; } // thông báo về cái gì
        public string? JText { get; set; } // json lưu thông tin cần parse
    }
}
