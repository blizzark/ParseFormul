using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseFormuls
{ 
    class GeneticMethod : Method
    {
        private int minX1 = 0;
        private int minX2 = 0;
        private int maxX1 = 0;
        private int maxX2 = 0;
        private int X1X2 = 0;
        private string text = "";

        public GeneticMethod(int minX1, int minX2, int maxX1, int maxX2, int X1X2, string text, string nameX1, string nameX2, string SecondKindConstraint, double accuracy, int SymbolBox) 
            : base(nameX1, nameX2)
        {
            this.minX1 = minX1;
            this.minX2 = minX2;
            this.maxX1 = maxX1;
            this.maxX2 = maxX2;
            this.X1X2 = X1X2;
            this.text = text;
        }
    }
}
