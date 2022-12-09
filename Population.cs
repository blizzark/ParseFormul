using System;

namespace ParseFormuls
{
    class Population
    {

        private static Random rnd = new Random((int)DateTime.Now.Ticks);

        private double _elitism;
        private double _mutation;
        private double _crossover;
        private Chromosome[] _populace;
        private int minX1 = 0;
        private int minX2 = 0;
        private int maxX1 = 0;
        private int maxX2 = 0;
        private int X1X2 = 0;
        /// <summary>
        /// Конструктор для создания популяции
        /// </summary>
        /// <param name="size">Размер популяции, где размер > 0</param>
        /// <param name="crossoverRatio">Коэффициент кроссовера для популяции в процессе эволюции (вероятноесть скрещивания) <= crossoverRatio <= 1.0</param>
        /// <param name="elitismRatio">Коэффициент оставления населения в процессе эволюции (другая часть отбрасывается (т.е. не выжила)) <= elitismRatio <= 1.0</param>
        /// <param name="mutationRatio">Коэффициент мутаций для популяции в процессе эволюции, где 0,0<= mutationRatio <= 1.0</param>
        /// <param name="minX1">Минимальные ограничения Х1</param>
        /// <param name="minX2">Минимальные ограничения Х2</param>
        /// <param name="maxX1">Максимальные ограничения Х1</param>
        /// <param name="maxX2">Максимальные ограничения Х2</param>
        /// <param name="X1X2">Ограничение 2-го рода</param>
        /// <param name="SymbolBox">Показывает в какую сторону направлен знак для ограничений 2-го рода</param>
        public Population(int size, double crossoverRatio, double elitismRatio, double mutationRatio, int minX1, int minX2, int maxX1, int maxX2, int X1X2, int SymbolBox)
        {
            #region Передача в поля класса
            this.minX1 = minX1;
            this.minX2 = minX2;
            this.maxX1 = maxX1;
            this.maxX2 = maxX2;
            this.X1X2 = X1X2;
            #endregion
            this._crossover = crossoverRatio;
            this._elitism = elitismRatio;
            this._mutation = mutationRatio;

           
            this._populace = new Chromosome[size];
            Chromosome.SymbolBox = SymbolBox;
            Chromosome.X1X2 = X1X2;
            // Генерируем начальную популяцию
            for (int i = 0; i < size; i++)
            {
                this._populace[i] = Chromosome.GenerateRandom(minX1, minX2, maxX1, maxX2);
            }

            Array.Sort(this._populace);



        }

        /// <summary>
        /// Эволюция популяции
        /// </summary>
        public void Evolve(int tournamentSize)
        {
            // Создайте буфер для нового поколения
            Chromosome[] buffer = new Chromosome[_populace.Length];

            // Скопируйте часть населения без изменений на основе
            // коэффициент элитарности ()процент, который выживает.
            int idx = (int)Math.Round(_populace.Length * _elitism, 3);
            Array.Copy(_populace, 0, buffer, 0, idx);

            // Перебираем оставшуюся часть популяции и развиваемся по мере
            // соответствтия.
            while (idx < buffer.Length)
            {
                // Проверяем, должны ли мы выполнять кроссовер.
                if (rnd.NextDouble() <= _crossover)
                {
                    // Выбираем родителей и пару, чтобы получить их потомков
                    Chromosome[] parents = SelectParents(tournamentSize);
                    Chromosome[] children = parents[0].Сrossover(parents[1]);

                    // Проверяем, должен ли быть мутирован первый потомок.
                    if (rnd.NextDouble() <= _mutation)
                    {
                        buffer[idx++] = children[0].Mutate(minX1, minX2, maxX1, maxX2);
                    }
                    else
                    {
                        buffer[idx++] = children[0];
                    }

                    // Повторить для второго потомка, если есть место.
                    if (idx < buffer.Length)
                    {
                        if (rnd.NextDouble() <= _mutation)
                        {
                            buffer[idx] = children[1].Mutate(minX1, minX2, maxX1, maxX2);
                        }
                        else
                        {
                            buffer[idx] = children[1];
                        }
                    }
                }
                else
                {
                  // Нет кроссовера, так что скопируйте то, что было.
                  // Определяем, должна ли произойти мутация.
                    if (rnd.NextDouble() <= _mutation)
                    {
                        buffer[idx] = _populace[idx].Mutate(minX1, minX2, maxX1, maxX2);
                    }
                    else
                    {
                        buffer[idx] = _populace[idx];
                    }
                }
                ++idx;
            }

            // Сортируем буфер по пригодности.
            Array.Sort(buffer);

            // Меняем популяцию
            _populace = buffer;
        }

        /// <summary>
        /// Получаем копию текущей популяции
        /// </summary>
        /// <returns>Массив хромосом, представляющий текущую популяцию</returns>
        public Chromosome[] GetPopulation()
        {
            Chromosome[] arr = new Chromosome[_populace.Length];
            Array.Copy(_populace, 0, arr, 0, _populace.Length);

            return arr;
        }

        /// <summary>
        /// Вспомогательный метод, который можно использовать для выбора двух случайных родителей из популяции для использования в кроссовере во время эволюции.
        /// </summary>
        /// <returns>Две случайно выбранные хромосомы для скрещивания.</returns>
	    private Chromosome[] SelectParents(int tournamentSize)
        {
            Chromosome[] parents = new Chromosome[2];
            // Randomly select two parents via tournament selection.
            for (int i = 0; i < 2; i++)
            {
                parents[i] = _populace[rnd.Next(_populace.Length)];

                for (int j = 0; j < tournamentSize; j++) 
                {
                    int idx = rnd.Next(_populace.Length);

                    if (_populace[idx].CompareTo(parents[i]) < 0)
                        parents[i] = _populace[idx];
                }
            }

            return parents;
        }
    }
}
