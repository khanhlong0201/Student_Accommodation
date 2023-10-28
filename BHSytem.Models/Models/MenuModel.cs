using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Entities;
namespace BHSytem.Models.Models
{
    public class MenuModel : Menus
    {
        public bool ShowDetail { get; set; } = false;
        public string BreadCrumb { get; set; } = "";
        public string AuthorizationDetail { get; set; } = "";
    }
}
