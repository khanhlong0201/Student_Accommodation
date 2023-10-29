using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Models;
namespace BHSytem.Models.Entities
{
    public class UserRoles : Auditable
    {
        [Key]
        public int Role_Id { get; set; }

        [Key]
        public int UserId { get; set; }
        public Roles Roles { get; set; }
        public Users Users { get; set; }

    }
}
