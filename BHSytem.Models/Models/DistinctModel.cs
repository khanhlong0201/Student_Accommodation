using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Entities;
namespace BHSytem.Models.Models
{
    public class DistinctModel : Distincts
    {
        public string City_Name { get; set; }
    }
}
