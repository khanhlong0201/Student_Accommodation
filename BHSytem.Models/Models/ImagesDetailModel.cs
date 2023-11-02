using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Entities;

namespace BHSytem.Models.Models
{
    public class ImagesDetailModel : ImagesDetails
    {
        public string ImageUrl { get; set; }
        public string File_Name { get; set; }
    }
}
