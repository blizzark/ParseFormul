using System;

namespace ParseFormuls
{
 

    public class Chromosome : IComparable<Chromosome>
    {
        public double _geneX1;
        public double _geneX2;
        public double _fitness;
        static public double X1X2 = 0;
        static public int SymbolBox = 0;

        private static Random rnd = new Random((int)DateTime.Now.Ticks);

      
	    public Chromosome(double geneX1, double geneX2)
        {
            this._geneX1 = geneX1;
            this._geneX2 = geneX2;
            this._fitness = CalculateFitness(geneX1, geneX2);
        }

        /// <summary>
        /// Вспомогательный метод, который рассчитывает фитнес функцию и проверяет условие 2-го рода
        /// </summary>
        /// <param name="geneX1"></param>
        /// <param name="geneX2"></param>
        /// <returns></returns>
        private static double CalculateFitness(double geneX1, double geneX2)
        {
            double fitness = 0;

            double F = CallCalculator.SecondClassConstraintFunction(geneX1, geneX2);

            if (SymbolBox == 0)
            {
                if (F > X1X2)
                {
                    return 9999999999;
                }
            }
            else
            {
                if (F < X1X2)
                {
                    return 9999999999;
                }
            }

            fitness = CallCalculator.ObjectiveFunction(geneX1, geneX2);

            return fitness;
        }

        public Chromosome Mutate(int minX1, int minX2, int maxX1, int maxX2)
        {
            do
            {
                double tmp = rnd.Next(maxX1);
                _geneX1 = (_geneX1 + tmp) / 2;
                _geneX2 = (_geneX1 + tmp) / 2;
            } while (!(minX1 < _geneX1) && !(_geneX1 > maxX1) && !(minX2 < _geneX2) && !(_geneX2 > maxX2));

            return new Chromosome(_geneX1, _geneX2);
        }

        /// <summary>
        /// Метод скрещивания 2-х хромосом. В каждой хромосоме 2 гена, поэтому мы (с верноятностью 50%) меняем гены отца и матери местами
        /// </summary>
        /// <param name="сrossover"></param>
        /// <returns></returns>
        public Chromosome[] Сrossover(Chromosome сrossover)
        {
            int position = rnd.Next(100);
            double fX1 = _geneX1; // Ген х1 папы
            double fX2 = _geneX2; // Ген х2 папы

            double mX1 = сrossover._geneX1; // Ген х1 мамы
            double mX2 = сrossover._geneX2; // Ген х2 мамы
            double c1X1 = 0;
            double c1X2 = 0;

            double c2X1 = 0;
            double c2X2 = 0;

            c1X1 = fX1;
            c1X2 = mX2;

            c2X1 = mX1;
            c2X2 = fX2;


            return new Chromosome[] {
                new Chromosome(c1X1, c1X2),
                new Chromosome(c2X1, c2X2)
            };
        }

       /// <summary>
       /// Генерация новых генов в хромосоме. Генерация сразуже проводится в пределах ограничений 1-го рода
       /// </summary>
       /// <param name="minX1"></param>
       /// <param name="minX2"></param>
       /// <param name="maxX1"></param>
       /// <param name="maxX2"></param>
       /// <returns></returns>
	    public static Chromosome GenerateRandom(int minX1, int minX2, int maxX1, int maxX2)
        {
            double X1 = 0;
            double X2 = 0;

            X1 = rnd.Next(minX1 * 1000, maxX1 * 1000) / 1000.0;
            X2 = rnd.Next(minX2 * 1000, maxX2 * 1000) / 1000.0;

            return new Chromosome(X1, X2);
        }

        /// <summary>
        /// Метод, позволяющий сравнивать объекты хромосом друг с другом на основе соответствия.   
        /// </summary>
        /// <param name="c">Хромосома</param>
        /// <returns> Целочисленное значение, указывающее относительный порядок сравниваемых объектов.
        /// Меньше нуля: этот объект меньше другого параметра. Ноль: этот объект равен другому.
        /// Больше нуля: этот объект больше другого.</returns>
        public int CompareTo(Chromosome c)
        {
            if (_fitness < c._fitness)
            {
                return -1;
            }
            else if (_fitness > c._fitness)
            {
                return 1;
            }

            return 0;
        }


    }
}
