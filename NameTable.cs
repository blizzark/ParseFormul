using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseFormuls
{
    public class NameTable
    {
        public NameTable(string name, double value = 0, bool variable = false)
        {
            this.name = name;
        }
        public string name { get; set; }
        public double value { get; set; }
        public bool variable { get; set; }
    }

}
