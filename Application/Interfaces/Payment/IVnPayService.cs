using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Payment
{
    public interface IVnPayService
    {
        public void Init(DateTime createDate, string ipAddress,
            decimal amount, string currCode, string orderType, string orderInfo, string txnRef);
        public string GetLink(string urlHostName);
        public void MakeRequestData();
    }
}
