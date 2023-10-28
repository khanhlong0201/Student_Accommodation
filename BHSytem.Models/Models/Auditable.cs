using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHSytem.Models.Models
{
    public interface IAuditable
    {
        bool IsDeleted { get; set; }
        DateTime? Date_Create { get; set; }
        int? User_Create { get; set; }
        DateTime? Date_Update { get; set; }
        int? User_Update { get; set; }
    }
    public abstract class Auditable : IAuditable
    {
        public bool IsDeleted { get; set; }
        public DateTime? Date_Create { get; set; }
        public int? User_Create { get; set; }
        public DateTime? Date_Update { get; set; }
        public int? User_Update { get; set; }
    }
}
