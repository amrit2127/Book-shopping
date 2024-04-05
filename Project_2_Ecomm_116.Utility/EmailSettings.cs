using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2_Ecomm_116.Utility
{
    public class EmailSettings
    {
        public string PrimaryDomain { get; set; }
        public int PrimaryPost { get; set; }
        public string SecondaryDomain { get; set; }
        public int SecondaryPost { get; set; }
        public string UsernameEmail { get; set; }
        public string UsernamePassword { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string CcEmail { get; set; }

    }
}
