using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseFormuls
{
    public class InitialData
    {
        public string X1 { get; set; }
        public string X2 { get; set; }
        public string result { get; set; }

        public InitialData(string X1, string X2, string result)
        {
            this.X1 = X1;
            this.X2 = X2;
            this.result = result;
        }
    }
}
