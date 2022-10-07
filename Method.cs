using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseFormuls
{
    class Method
    {
        public double answerX1 = 0;
        public double answerX2 = 0;
        public double answer = double.MinValue;
        private string nameX1 = "";
        private string nameX2 = "";


        protected Method(string nameX1, string nameX2)
        {
            this.nameX1 = nameX1;
            this.nameX2 = nameX2;
        }

        public List<InitialData> InitialDataList { get; set; }

        protected double CalculateF(string text, double x1, double x2)
        {
            double F = 0;
            string tmpText = "";
            tmpText = text.Replace(nameX1, Math.Round(x1, 3).ToString());
            tmpText = tmpText.Replace(nameX2, Math.Round(x2, 3).ToString());
            Parser parser = new Parser(tmpText);
            F = Math.Round(Сalculator.EvaluateExpression(parser), 3);
            return F;
        }

      
    }
}
