using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHSytem.Models
{
    public interface IAuditable
    {
        bool IsDeleted { get; set; }
        DateTime? DateCreate { get; set; }
        int? UserCreate { get; set; }
        DateTime? DateUpdate { get; set; }
        int? UserUpdate { get; set; }
    }
    public abstract class Auditable : IAuditable
    {
        public bool IsDeleted { get; set; }
        public DateTime? DateCreate { get; set; }
        public int? UserCreate { get; set; }
        public DateTime? DateUpdate { get; set; }
        public int? UserUpdate { get; set; }
    }
}
