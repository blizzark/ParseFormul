using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseFormuls
{

    static class CallCalculator
    {
        static public string text = "";
        static public string SecondKindConstraint = "";
        static public string nameX1 = "";
        static public string nameX2 = "";

        static public double ObjectiveFunction(double X1, double X2)
        {
            double F = 0;
            string tmpText = "";
            tmpText = text.Replace(nameX1, Math.Round(X1, 3).ToString());
            tmpText = tmpText.Replace(nameX2, Math.Round(X2, 3).ToString());
            Parser parser = new Parser(tmpText);
            F = Math.Round(Сalculator.EvaluateExpression(parser), 3);
            return F;
        }

        static public double SecondClassConstraintFunction(double X1, double X2)
        {
            double F = 0;
            string tmpText = "";
            tmpText = SecondKindConstraint.Replace(nameX1, Math.Round(X1, 3).ToString());
            tmpText = tmpText.Replace(nameX2, Math.Round(X2, 3).ToString());
            Parser parser = new Parser(tmpText);
            F = Math.Round(Сalculator.EvaluateExpression(parser), 3);
            return F;
        }
    }
}
