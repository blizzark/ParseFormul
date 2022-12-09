using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseFormuls
{
    class Method
    {
        public int minX1 = 0;
        public int minX2 = 0;
        public int maxX1 = 0;
        public int maxX2 = 0;
        public int X1X2 = 0;
        public double accuracy = 0;
        public int SymbolBox = 0;

        public double answerX1 = 0;
        public double answerX2 = 0;
        public double answer = double.MinValue;
        public List<InitialData> InitialDataList { get; set; }
        public int NumberGenerations = 0;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="minX1">Минимум для первой переменной (огрничение 1-го рода)</param>
        /// <param name="minX2">Минимум для второй переменной (огрничение 1-го рода)</param>
        /// <param name="maxX1">Максимум для первой переменной (огрничение 1-го рода)</param>
        /// <param name="maxX2">Максимум для второй переменной (огрничение 1-го рода)</param>
        /// <param name="X1X2">Значение ограничения второго рода</param>
        public Method(int minX1, int minX2, int maxX1, int maxX2, int X1X2, double accuracy, int SymbolBox)
        {
            this.minX1 = minX1;
            this.minX2 = minX2;
            this.maxX1 = maxX1;
            this.maxX2 = maxX2;
            this.X1X2 = X1X2;
            this.accuracy = accuracy;
            this.SymbolBox = SymbolBox;
        }
    }
}
