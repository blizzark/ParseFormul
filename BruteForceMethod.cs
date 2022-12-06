using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.ObjectModel;

namespace ParseFormuls
{
    class BruteForceMethod : Method
    {
        public BruteForceMethod(int minX1, int minX2, int maxX1, int maxX2, int X1X2, double accuracy, int SymbolBox) 
        {
            const int less = 0;
            const int more = 1;
            double result = 0;
            InitialDataList = new List<InitialData>();

            for (double i = minX1; i <= maxX1; i = i + accuracy)
            {
                for (double j = minX2; j <= maxX2; j = j + accuracy)
                {
                  
                    double tmp = CallCalculator.SecondClassConstraintFunction(i, j);

                    if (SymbolBox == less)
                    {
                        if (tmp <= X1X2)
                        {

                            result = CallCalculator.ObjectiveFunction(i, j);

                            if (answer < result)
                            {
                                answer = result;
                                answerX1 = Math.Round(i, 3);
                                answerX2 = Math.Round(j, 3);
                            }

                            InitialDataList.Add(new InitialData(Math.Round(i, 3).ToString(), Math.Round(j, 3).ToString(), Math.Round(result, 3).ToString()));
                        }
                    }
                    else if (SymbolBox == more)
                    {
                        if (tmp >= X1X2)
                        {
                            result = CallCalculator.ObjectiveFunction(i, j);

                            if (answer < result)
                            {
                                answer = result;
                                answerX1 = Math.Round(i, 3);
                                answerX2 = Math.Round(j, 3);
                            }

                            InitialDataList.Add(new InitialData(Math.Round(i, 3).ToString(), Math.Round(j, 3).ToString(), Math.Round(result, 3).ToString()));
                        }
                    }
                }
            }

        }
    }
}
