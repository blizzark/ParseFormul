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


            // Размер моделируемой популяции
            const int populationSize = 512;

            // Максимальное количество поколений для моделирования.
            const int maxGenerations = 50;

            // Вероятность кроссовера для любого члена популяции,
            // где 0.0 <= crossoverRatio <= 1.0
            const double crossoverRatio = 0.8d;

            // Часть населения, которая останется без изменений
            // между эволюциями, где 0.0 <= elitismRatio < 1.0
            const double elitismRatio = 0.1d;

            // Вероятность мутации для любого члена популяции,
            // где 0.0 <=mutationRatio <= 1.0
            const double mutationRatio = 0.20d;

            Chromosome.text = text;
            Chromosome.SecondKindConstraint = SecondKindConstraint;
            Chromosome.nameX1 = nameX1;
            Chromosome.nameX2 = nameX2;
            // Создаём начальную популяцию
            Population population = new Population(populationSize, crossoverRatio, elitismRatio, mutationRatio, minX1, minX2, maxX1, maxX2, X1X2, SymbolBox);

            // Начинаем развивать популяцию, останавливаясь, когда максимальное количество
            // поколение достигнуто, или когда мы найдем решение.
            int i = 0;
            Chromosome best = population.GetPopulation()[0];

            while ((i++ <= maxGenerations))
            {
                population.Evolve();
                best = population.GetPopulation()[0];
            }
            answerX1 = best._geneX1;
            answerX2 = best._geneX2;
            answer = best._fitness;
        }
    }
}
