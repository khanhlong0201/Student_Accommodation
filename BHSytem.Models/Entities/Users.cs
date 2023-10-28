using BHSytem.Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHSytem.Models.Entities
{
    public class Users : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [StringLength(250)]
        public string? FullName { get; set; }
        [StringLength(250)]
        public string? Address { get; set; }
        [StringLength(50)]
        public string? Phone { get; set; }
        [StringLength(250)]
        public string? Email { get; set; }
        [StringLength(50)]
        public string? UserName { get; set; }
        [StringLength(100)]
        public string? Password { get; set; }
        public string? PasswordReset { get; set; }
    }
}
