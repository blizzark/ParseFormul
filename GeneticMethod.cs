using System;
using System.Collections.Generic;

namespace ParseFormuls
{ 
    class GeneticMethod : Method
    {

        /// <summary>
        /// Генетический алгоритм
        /// </summary>
        /// <param name="minX1">Минимум для первой переменной (огрничение 1-го рода)</param>
        /// <param name="minX2">Минимум для второй переменной (огрничение 1-го рода)</param>
        /// <param name="maxX1">Максимум для первой переменной (огрничение 1-го рода)</param>
        /// <param name="maxX2">Максимум для второй переменной (огрничение 1-го рода)</param>
        /// <param name="X1X2">Значение ограничения второго рода</param>
        /// <param name="accuracy">Погрешность</param>
        /// <param name="SymbolBox">Символ больше или меньше в ограничении 2-го рода</param>
        /// <param name="populationSize"> Размер моделируемой популяции</param>
        /// <param name="maxGenerations"> Максимальное количество поколений для моделирования. критерий остановки</param>
        /// <param name="crossoverRatio">Вероятность кроссовера для любого члена популяции,  де 0.0 <= crossoverRatio <= 1.0</param>
        /// <param name="elitismRatio"> Часть населения, которая останется без изменений между эволюциями, где 0.0 <= elitismRatio < 1.0</param>
        /// <param name="mutationRatio">Вероятность мутации для любого члена популяции, где 0.0 <=mutationRatio <= 1.0</param>
        /// <param name="tournamentSize">Вероятность мутации для любого члена популяции, где 0.0 <=mutationRatio <= 1.0</param>
        public GeneticMethod(int minX1, int minX2, int maxX1, int maxX2, int X1X2, double accuracy, int SymbolBox, int populationSize, int maxGenerations, double crossoverRatio, double elitismRatio, double mutationRatio, int tournamentSize) 
            : base(minX1, minX2, maxX1, maxX2, X1X2, accuracy, SymbolBox)
        {

            const long maxFitnes = 9999999999;

            // Создаём начальную популяцию
            Population population = new Population(populationSize, crossoverRatio, elitismRatio, mutationRatio, minX1, minX2, maxX1, maxX2, X1X2, SymbolBox);

            // Начинаем развивать популяцию, останавливаясь, когда максимальное количество
            // поколение достигнуто, или когда мы найдем решение.
            int i = 0;
            Chromosome best = population.GetPopulation()[0];
            Chromosome[] answerList = population.GetPopulation();
            while ((i++ <= maxGenerations) && (StopCriteriaCheck(answerList))) //записываем каждый раз лучший ответ, пока не достигнем критерия остановки (не достигнем макс колич поколений)
            {
                population.Evolve(tournamentSize);
                best = population.GetPopulation()[0]; // временно записываемлучшую хромосому
                answerList = population.GetPopulation();
            }
            answerX1 = best._geneX1;
            answerX2 = best._geneX2;
            answer = -best._fitness;
            i -= 2;
            NumberGenerations = i;

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
                for (int k = 0; k < c.Length * elitismRatio; k++)
                {
                    if (c[k]._fitness != maxFitnes)
                    {
                        double deltaX1new = Math.Abs(c[k]._geneX1 - c[k + 1]._geneX1); //ищем разницу между значениями генов
                        double deltaX2new = Math.Abs(c[k]._geneX2 - c[k + 1]._geneX2);//чтоб выяснить, на сколько они изменяются с поколением

                        if (deltaX1new > deltaX1)
                            deltaX1 = deltaX1new;
                        if (deltaX2new > deltaX2)
                            deltaX2 = deltaX2new;
                    }
                }
                if (deltaX1 < accuracy && deltaX2 < accuracy)
                    return false;

                return true;
            }
        }

        
    }
}
