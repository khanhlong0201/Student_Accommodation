using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Entities;
namespace BHSytem.Models.Models
{
    public class RoleModel : Auditable
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng điền Tên quyền")]
        public string? Name { get; set; }
    }
}
