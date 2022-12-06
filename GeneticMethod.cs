using System;
using System.Collections.Generic;

namespace ParseFormuls
{ 
    class GeneticMethod : Method
    {
        private int minX1 = 0;
        private int minX2 = 0;
        private int maxX1 = 0;
        private int maxX2 = 0;
        private int X1X2 = 0;

        public GeneticMethod(int minX1, int minX2, int maxX1, int maxX2, int X1X2, double accuracy, int SymbolBox)
        {
            this.minX1 = minX1;
            this.minX2 = minX2;
            this.maxX1 = maxX1;
            this.maxX2 = maxX2;
            this.X1X2 = X1X2;

            const long maxFitnes = 9999999999;
            // Размер моделируемой популяции
            const int populationSize = 200;

            // Максимальное количество поколений для моделирования.
            const int maxGenerations = 300;

            // Вероятность кроссовера для любого члена популяции,
            // где 0.0 <= crossoverRatio <= 1.0
            const double crossoverRatio = 5.0d;

            // Часть населения, которая останется без изменений
            // между эволюциями, где 0.0 <= elitismRatio < 1.0
            const double elitismRatio = 0.6d;

            // Вероятность мутации для любого члена популяции,
            // где 0.0 <=mutationRatio <= 1.0
            const double mutationRatio = 0.10d;

            // Создаём начальную популяцию
            Population population = new Population(populationSize, crossoverRatio, elitismRatio, mutationRatio, minX1, minX2, maxX1, maxX2, X1X2, SymbolBox);

            // Начинаем развивать популяцию, останавливаясь, когда максимальное количество
            // поколение достигнуто, или когда мы найдем решение.
            int i = 0;
            Chromosome best = population.GetPopulation()[0];
            Chromosome[] answerList = population.GetPopulation();
            while ((i++ <= maxGenerations) && (StopCriteriaCheck(answerList)))
            {
                population.Evolve();
                best = population.GetPopulation()[0];
                answerList = population.GetPopulation();
            }
            answerX1 = best._geneX1;
            answerX2 = best._geneX2;
            answer = -best._fitness;

            InitialDataList = new List<InitialData>();
            answerList = population.GetPopulation();
            for (int j = 0; j < answerList.Length; j++)
            {
                if (answerList[j]._fitness != maxFitnes)
                    InitialDataList.Add(new InitialData(Math.Round(answerList[j]._geneX1, 5).ToString(), Math.Round(answerList[j]._geneX2, 5).ToString(), (-answerList[j]._fitness).ToString()));
            }


            bool StopCriteriaCheck(Chromosome[] c)
            {
                double deltaX1 = 0;
                double deltaX2 = 0;
                for (int k = 0; k < c.Length - 1; k++)
                {
                    if (c[k]._fitness != maxFitnes)
                    {
                        double deltaX1new = Math.Abs(c[k]._geneX1 - c[k + 1]._geneX1);
                        double deltaX2new = Math.Abs(c[k]._geneX2 - c[k + 1]._geneX2);

                        if (deltaX1new > deltaX1)
                            deltaX1 = deltaX1new;
                        if (deltaX2new > deltaX2)
                            deltaX2 = deltaX2new;
                    }
                }
                if (deltaX1 < 0.01 && deltaX2 < 0.01)
                    return false;

                return true;
            }
        }

        
    }
}
