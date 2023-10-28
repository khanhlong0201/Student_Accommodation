using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Models;
namespace BHSytem.Models.Entities
{
    public class RoleMenus : Auditable
    {
        [Key]
        public int Role_Id { get; set; }

        [Key]
        public string Menu_Id { get; set; }
        public Roles Roles { get; set; }
        public Menus Menus { get; set; } 

    }
}
