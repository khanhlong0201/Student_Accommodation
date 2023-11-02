using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Entities;
namespace BHSytem.Models.Models
{
    public class BoardingHouseModel : BoardingHouses
    {

        public int Distinct_Id { get; set; }
        public int City_Id { get; set; }
        public string Ward_Name { get; set; }
        public string Distinct_Name { get; set; }
        public string City_Name { get; set; }
        public string Image_Name { get; set; }
    }
}
