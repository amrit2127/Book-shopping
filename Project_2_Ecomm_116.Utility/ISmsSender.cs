using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2_Ecomm_116.Utility
{
    public interface ISmsSender
    {
        Task SendSMSAsync(string phoneNumber, string message);

    }
}
