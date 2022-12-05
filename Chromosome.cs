using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseFormuls
{
 

    public class Chromosome : IComparable<Chromosome>
    {
        public double _geneX1;
        public double _geneX2;
        public double _fitness;
        static public string text = "";
        static public string SecondKindConstraint = "";
        static public string nameX1 = "";
        static public string nameX2 = "";

        // The target gene, converted to an array for convenience
        private static char[] TARGET_GENE = "Hello, world!".ToCharArray();

        // Convenience randomizer
        private static Random rnd = new Random((int)DateTime.Now.Ticks);

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="gene">The gene representing this Chromosome</param>
	    public Chromosome(double geneX1, double geneX2)
        {
            this._geneX1 = geneX1;
            this._geneX2 = geneX2;
            this._fitness = CalculateFitness(geneX1, geneX2);
        }

        /// <summary>
        /// Method to retrieve the fitness of this Chromosome.  
        /// Note that a lower fitness indicates a better Chromosome for the solution.
        /// </summary>
        /// <returns>The fitness of this Chromosome</returns>
        public double GetFitness()
        {
            return _fitness;
        }

        /// <summary>
        /// Helper method used to calculate the fitness for a given gene. 
        /// The fitness is defined as being the sum of the absolute value of the difference 
        /// between the current gene and the target gene. 
        /// </summary>
        /// <param name="gene">The gene to calculate the fitness for</param>
        /// <returns>The calculated fitness of the given gene</returns>
        private static double CalculateFitness(double geneX1, double geneX2)
        {
            double fitness = 0;

            string tmpText = "";
            tmpText = text.Replace(nameX1, Math.Round(geneX1, 3).ToString());
            tmpText = tmpText.Replace(nameX2, Math.Round(geneX2, 3).ToString());
            Parser parser = new Parser(tmpText);
            fitness = Math.Round(Сalculator.EvaluateExpression(parser), 3);

            return fitness;
        }

        /// <summary>
        /// Method to generate a new Chromosome that is a random mutation of this Chromosome.  
        /// This method randomly selects one character in the Chromosomes gene, then replaces 
        /// it with another random (but valid) character. Note that this method returns a new Chromosome, 
        /// it does not modify the existing Chromosome.
        /// </summary>
        /// <returns>A mutation of this Chromosome</returns>
        public Chromosome Mutate(int minX1, int minX2, int maxX1, int maxX2)
        {
            double X1 = rnd.Next(minX1, maxX1);
            double X2 = rnd.Next(minX2, maxX2);

            return new Chromosome(X1, X2);
        }

        // get a double number (Example: -12.345) and give Mantissa part of number (Example: 345)
        private double breakMantissa(double no)
        {
            char[] NO = no.ToString().ToCharArray();
            // Example: -12.345
            //             ^
            //           (Dot)
            bool find_Dot = false;
            string strMantissa = "";
            for (int i = 0; i < NO.Length; i++)
                if (NO[i] != '.' && find_Dot)
                    strMantissa += NO[i].ToString();
                else if (NO[i] == '.')
                    find_Dot = true;
                else continue;
            return Convert.ToDouble("0,"+strMantissa); // in Example: return 345           
        }

        /// <summary>
        /// Method used to mate this Chromosome with another. The resulting child Chromosomes are returned.
        /// </summary>
        /// <param name="mate">The Chromosome to mate with</param>
        /// <returns>The resulting Chromosome children</returns>
        public Chromosome[] Mate(Chromosome mate)
        {
            // Convert the genes to arrays to make thing easier.
            double fX1 = _geneX1; // Ген х1 папы
            double mX1 = mate._geneX1; // Ген х1 мамы

            double fX2 = _geneX2; // Ген х2 папы
            double mX2 = mate._geneX2; // Ген х2 мамы

            // Provide a container for the child gene data

            double c1X1 = Math.Round(fX1,0) + breakMantissa(mX1);
            double c1X2 = Math.Round(mX1,0) + breakMantissa(fX1);

            double c2X1 = Math.Round(fX2,0) + breakMantissa(mX2);
            double c2X2 = Math.Round(mX2,0) + breakMantissa(fX2); 

            /////////////////////////////////////////////////////////////////// делает целое всё...


            return new Chromosome[] {
                new Chromosome(c1X1, c1X2),
                new Chromosome(c2X1, c2X2)
            };
        }

        /// <summary>
        /// A convenience method to generate a random Chromosome.
        /// </summary>
        /// <returns>A randomly generated Chromosome</returns>
	    public static Chromosome GenerateRandom(int minX1, int minX2, int maxX1, int maxX2, int X1X2, int SymbolBox)
        {
            double X1 = 0;
            double X2 = 0;
            double F = 0;
            if (SymbolBox == 0)
            {
                do
                {
                    X1 = rnd.Next(minX1 * 1000, maxX1 * 1000) / 1000.0;
                    X2 = rnd.Next(minX2 * 1000, maxX2 * 1000) / 1000.0;

                    string tmpText = "";
                    tmpText = SecondKindConstraint.Replace(nameX1, Math.Round(X1, 3).ToString());
                    tmpText = tmpText.Replace(nameX2, Math.Round(X2, 3).ToString());
                    Parser parser = new Parser(tmpText);
                    F = Math.Round(Сalculator.EvaluateExpression(parser), 3);
                } while (F >= X1X2);
            }
            else
            {
                do
                {
                    X1 = rnd.Next(minX1 * 1000, maxX1 * 1000) / 1000.0;
                    X2 = rnd.Next(minX2 * 1000, maxX2 * 1000) / 1000.0;

                    string tmpText = "";
                    tmpText = SecondKindConstraint.Replace(nameX1, Math.Round(X1, 3).ToString());
                    tmpText = tmpText.Replace(nameX2, Math.Round(X2, 3).ToString());
                    Parser parser = new Parser(tmpText);
                    F = Math.Round(Сalculator.EvaluateExpression(parser), 3);
                } while (F <= X1X2);
            }

            return new Chromosome(X1, X2);
        }

        /// <summary>
        /// Method to allow for comparing Chromosome objects with one another based on fitness. 
        /// Chromosome ordering is based on the natural ordering of the fitnesses of the Chromosomes.  
        /// </summary>
        /// <param name="c">Chromosome to compare against</param>
        /// <returns> An integer value that indicates the relative order of the objects being compared. 
        /// Less than zero: This object is less than the other parameter. Zero: This object is equal to other. 
        /// Greater than zero: This object is greater than other.</returns>
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
