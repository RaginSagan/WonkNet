using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BillisNet.Models
{
    public class PreppedWebClient : WebClient
    {
        public PreppedWebClient()
        {
            Headers.Add("User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:59.0) Gecko/20100101 Firefox/59.s0");
        }
    }
}
