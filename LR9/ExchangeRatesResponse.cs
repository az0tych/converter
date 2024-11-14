using System;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR9
{
    public class ExchangeRatesResponse
    {
        public Dictionary<string, double> Rates { get; set; }
    }
}
