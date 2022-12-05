using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseFormuls
{
    class BoxMethod : Method
    {
        private int minX1 = 0;
        private int minX2 = 0;
        private int maxX1 = 0;
        private int maxX2 = 0;
        private int X1X2 = 0;
        private string text = "";
        private string nameX1 = "";
        private string nameX2 = "";
        private string SecondKindConstraint = "";
        private double accuracy = 0;
        private int SymbolBox = 0;

        private struct VX
        {
            public VX(List<double> X1, List<double> X2, List<double> F, List<bool> isFixedVx)
            {
                this.X1 = X1;
                this.X2 = X2;
                this.F = F;
                this.isFixedVx = isFixedVx;
            }

            public List<double> X1 { get; set; }
            public List<double> X2 { get; set; }
            public List<double> F { get; set; }
            public List<bool> isFixedVx { get; set; }
        }

        public BoxMethod(int minX1, int minX2, int maxX1, int maxX2, int X1X2, string text, string nameX1, string nameX2, string SecondKindConstraint, double accuracy, int SymbolBox) 
            : base(nameX1, nameX2)
        {
            #region Передача в поля класса
            this.minX1 = minX1;
            this.minX2 = minX2;
            this.maxX1 = maxX1;
            this.maxX2 = maxX2;
            this.X1X2 = X1X2;
            this.text = text;
            this.nameX1 = nameX1;
            this.nameX2 = nameX2;
            this.SecondKindConstraint = SecondKindConstraint;
            this.accuracy = accuracy;
            this.SymbolBox = SymbolBox;
            #endregion

            VX vx;
            try
            {
                vx = CreateInitialComplex();
            }
            catch (StackOverflowException)
            {
                throw new Exception("Не удалось найти удовлетворяющий условиям исходный комплекс за поставленное число итераций! Запустите рассчет еще раз.");
            }
            catch (Exception)
            {
                throw;
            }
            try
            {
                vx = MoveOutVxsToCenter(vx);
            }
            catch (StackOverflowException)
            {
                throw new Exception("Не удалось сдвинуть незафиксированные вершины за поставленное число итераций! Запустите рассчет еще раз.");
            }
            catch (Exception)
            {
                throw;
            }

            int countToBreak = 0;
            vx = FindCinVxs(vx);
            while (true)
            {
                int worstIndex = FindWorstCVxIndex(vx);
                int bestIndex = FindBestCVxIndex(vx);
                VX complexCenter = FindComplexCenterCoords(vx, worstIndex);
                if (CheckIfSearchCompleted(complexCenter, vx, worstIndex, bestIndex, accuracy))
                {
                    InitialDataList = new List<InitialData>();
                    for (int i = 0; i < vx.F.Count; i++)
                    {
                        InitialDataList.Add(new InitialData(Math.Round(vx.X1[i],3).ToString(), Math.Round(vx.X2[i],3).ToString(), vx.F[i].ToString()));
                    }

                    complexCenter.F[0] = CalculateF(text, complexCenter.X1[0], complexCenter.X2[0]);
                    InitialDataList.Add(new InitialData(Math.Round(complexCenter.X1[0],3).ToString(), Math.Round(complexCenter.X2[0],3).ToString(), complexCenter.F[0].ToString()));

                    answerX1 = Math.Round(complexCenter.X1[0], 3);
                    answerX2 = Math.Round(complexCenter.X2[0], 3);
                    answer = Math.Round(complexCenter.F[0], 3);


                    vx.X1.Clear();
                    vx.X2.Clear();
                    vx.F.Clear();
                    vx.isFixedVx.Clear();
                    complexCenter.X1.Clear();
                    complexCenter.X2.Clear();
                    complexCenter.F.Clear();
                    complexCenter.isFixedVx.Clear();
                    break;
                }
                else
                {
                    try
                    {
                        VX newWorst = FindNewComplexCoordsInsteadWorst(vx, complexCenter, worstIndex, accuracy);
                        newWorst = CheckSecondRestrictions(newWorst, complexCenter,worstIndex, accuracy);
                        newWorst.F[0] = CalculateF(text, newWorst.X1[0], newWorst.X2[0]);
                        vx = FixVertexAndSwapFs(vx, newWorst, worstIndex, bestIndex);

                        newWorst.X1.Clear();
                        newWorst.X2.Clear();
                        newWorst.F.Clear();
                        newWorst.isFixedVx.Clear();

                        if (countToBreak > 100)
                        {
                            throw new Exception("Превышено чилсо попыток замены наихудшей вершины!\nПопробуйте задать меньшую точность.");
                        }

                        countToBreak++;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

        }

        /// <summary>
        /// Формирование исходного Комплекса
        /// </summary>
        /// <returns></returns>
        VX CreateInitialComplex()
        {

            int n = 2;
            int N = 2 * n; //Число независимых переменных * 2

            int[] lowRestricts = new int[2];
            lowRestricts[0] = minX1; lowRestricts[1] = minX2;

            int[] topRestricts = new int[2];
            topRestricts[0] = maxX1; topRestricts[1] = maxX2;

            VX vx = new VX(new List<double>(), new List<double>(), new List<double>(), new List<bool>());
            vx.isFixedVx.Add(false);
            double esp = 0.01;
            while (FindP(vx) == 0)
            {
                vx.X1.Clear();
                vx.X2.Clear();
                vx.F.Clear();
                vx.isFixedVx.Clear();
                //Находим начальные вершины
                Random rand = new Random();
                for (int j = 0; j < N; j++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        double rCoef = rand.Next(1, 100) * esp;
                        double x = lowRestricts[i] + rCoef * (topRestricts[i] - lowRestricts[i]);
                        if (i == 0)
                            vx.X1.Add(x);
                        else if (i == 1)
                            vx.X2.Add(x);
                    }
                    
                    double tmp = CalculateF(SecondKindConstraint, vx.X1[j], vx.X2[j]);
                    if (SymbolBox == 0)
                    {
                        if (tmp <= X1X2) // ограничение второго рода
                            vx.isFixedVx.Add(true);
                        else
                            vx.isFixedVx.Add(false);
                    }
                    else
                    {
                        if (tmp >= X1X2) // ограничение второго рода
                            vx.isFixedVx.Add(true);
                        else
                            vx.isFixedVx.Add(false);
                    }

                }
            }

            return vx;
        }

        /// <summary>
        /// Проверка на ограничение 
        /// </summary>
        /// <param name="vx"></param>
        /// <returns></returns>
        int FindP(VX vx)
        {
            int P = 0;
            for (int i = 0; i < vx.isFixedVx.Count; i++)
                if (vx.isFixedVx[i])
                    P++;
            return P;
        }

        /// <summary>
        /// Сумма всех фиксированных вершин
        /// </summary>
        /// <param name="vx"></param>
        /// <param name="indx">Выбор переменной целевой функции X1 или X2</param>
        /// <returns></returns>
        double SumAllFixedVxs(VX vx, int indx)
        {
            double sum = 0;
            for (int i = 0; i < vx.isFixedVx.Count; i++)
            {
                if (vx.isFixedVx[i])
                {
                    if (indx == 0)
                        sum += vx.X1[i];
                    else if (indx == 1)
                        sum += vx.X2[i];
                }
            }
            return sum;
        }
        /// <summary>
        /// Операция по её смещению к центру Р вершин Комплекса
        /// </summary>
        /// <param name="vx"></param>
        /// <returns></returns>
        VX MoveOutVxsToCenter(VX vx)
        {
            int P = FindP(vx), n = 2;
            for (int i = 0; i < vx.isFixedVx.Count; i++)
            {
                int check = 0;
                while (!vx.isFixedVx[i])
                {
                    if(check > 300)
                    {
                        break;
                    }
                    for (int j = 0; j < n; j++)
                    {
                        if (!vx.isFixedVx[i])
                        {
                            if (j == 0)
                                vx.X1[i] = 0.5 * (vx.X1[i] + ((1 / P) * SumAllFixedVxs(vx, j)));
                            else if (j == 1)
                            {
                                vx.X2[i] = 0.5 * (vx.X2[i] + ((1 / P) * SumAllFixedVxs(vx, j)));
                            }
                            double tmp = CalculateF(SecondKindConstraint, vx.X1[j], vx.X2[j]); 
                            if (SymbolBox == 0)
                            {
                                if (tmp <= X1X2) // ограничение второго рода
                                {
                                    vx.isFixedVx[i] = true;
                                    P = FindP(vx);
                                }
                            }
                            else
                            {
                                if (tmp >= X1X2) // ограничение второго рода
                                {
                                    vx.isFixedVx[i] = true;
                                    P = FindP(vx);
                                }
                            }
                            check++;
                        }
                    }
                }
            }
            return vx;
        }
        /// <summary>
        /// Вычисление значений целевой функции Fj для всех N вершин Комплекса
        /// </summary>
        /// <param name="vx"></param>
        /// <returns></returns>
        VX FindCinVxs(VX vx)
        {
            for (int i = 0; i < vx.isFixedVx.Count; i++)
            {
                vx.F.Add(CalculateF(text, vx.X1[i], vx.X2[i]));
            }
            return vx;
        }

        /// <summary>
        /// Выбор наилучшего решения (экстремум) (максимизация)
        /// </summary>
        /// <param name="vx"></param>
        /// <returns></returns>
        int FindBestCVxIndex(VX vx)
        {
            double maxC = double.MinValue;
            int bestIndex = -1;
            for (int i = 0; i < vx.F.Count; i++)
            {
                if (maxC < vx.F[i])
                {
                    maxC = vx.F[i];
                    bestIndex = i;
                }
            }
            return bestIndex;
        }

        /// <summary>
        /// Выбор наихудшего решения (экстремум) (минимизация)
        /// </summary>
        /// <param name="vx"></param>
        /// <returns></returns>
        int FindWorstCVxIndex(VX vx)
        {
            double minC = double.MaxValue;
            int worstIndex = -1;
            for (int i = 0; i < vx.F.Count; i++)
            {
                if (minC > vx.F[i])
                {
                    minC = vx.F[i];
                    worstIndex = i;
                }
            }
            return worstIndex;
        }

        /// <summary>
        /// Определение координат Ci центра Комплек
        /// </summary>
        /// <param name="vx"></param>
        /// <param name="worstIndex"></param>
        /// <returns></returns>
        VX FindComplexCenterCoords(VX vx, int worstIndex)
        {
            int freeVarCount = 2;
            int N = 2 * freeVarCount;
            VX complexCenter = new VX(new List<double>(), new List<double>(), new List<double>(), new List<bool>());

            complexCenter.X1.Add((SumAllFixedVxs(vx, 0) - vx.X1[worstIndex]) / (N - 1));
            complexCenter.X2.Add((SumAllFixedVxs(vx, 1) - vx.X2[worstIndex]) / (N - 1));
            complexCenter.F.Add(0);
            complexCenter.isFixedVx.Add(true);

            return complexCenter;
        }

        /// <summary>
        /// Проверка условия окончания поиска
        /// </summary>
        /// <param name="complexCenter"></param>
        /// <param name="vx"></param>
        /// <param name="worstIndex"></param>
        /// <param name="bestIndex"></param>
        /// <param name="eps">Точность</param>
        /// <returns></returns>
        bool CheckIfSearchCompleted(VX complexCenter, VX vx, int worstIndex, int bestIndex, double accuracy)
        {
            bool isExtremumFound = false;
            int n = 2;

            double sum = Math.Abs(complexCenter.X1[0] - vx.X1[worstIndex]) + Math.Abs(complexCenter.X1[0] - vx.X1[bestIndex]);
            sum += Math.Abs(complexCenter.X2[0] - vx.X2[worstIndex]) + Math.Abs(complexCenter.X2[0] - vx.X2[bestIndex]);

            double B = sum / (2 * n);

            if (B < accuracy)
                isExtremumFound = true;
            else
                isExtremumFound = false;

            return isExtremumFound;
        }
        /// <summary>
        /// Вычисление координаты новой точки Комплекса взамен наихудшей
        /// </summary>
        /// <param name="vx"></param>
        /// <param name="complexCenter"></param>
        /// <param name="worstIndex"></param>
        /// <param name="accuracy"></param>
        /// <returns></returns>
        VX FindNewComplexCoordsInsteadWorst(VX vx, VX complexCenter, int worstIndex, double accuracy)
        {
            VX newWorst = new VX(new List<double>(), new List<double>(), new List<double>(), new List<bool>());

            newWorst.F.Add(0);
            newWorst.isFixedVx.Add(true);
            newWorst.X1.Add((2.3 * complexCenter.X1[0]) - (1.3 * vx.X1[worstIndex]));
            newWorst.X2.Add((2.3 * complexCenter.X2[0]) - (1.3 * vx.X2[worstIndex]));

            if (newWorst.X1[0] < minX1)
                Math.Round(newWorst.X1[0] = minX1 + accuracy,3);
            else if (newWorst.X1[0] > maxX1)
                Math.Round(newWorst.X1[0] = maxX1 - accuracy,3);

            if (newWorst.X2[0] < minX2)
                Math.Round(newWorst.X2[0] = minX2 + accuracy,3);
            else if (newWorst.X2[0] > maxX2)
                Math.Round(newWorst.X2[0] = maxX2 - accuracy,3);

            return newWorst;
        }
        /// <summary>
        /// Проверка выполнения ограничений 2.го рода для новой точки
        /// </summary>
        /// <param name="newWorst"></param>
        /// <param name="complexCenter"></param>
        /// <returns></returns>
        VX CheckSecondRestrictions(VX newWorst, VX complexCenter, int worstIndex, double accuracy)
        {
            int index = 0;
            if (SymbolBox == 0)
                while (CalculateF(SecondKindConstraint, newWorst.X1[0], newWorst.X2[0]) >= X1X2)
                {
                    newWorst.X1[0] = (newWorst.X1[0] + complexCenter.X1[0]) / 2 ;
                    newWorst.X2[0] = (newWorst.X2[0] + complexCenter.X2[0]) / 2 ;
                    index++;
                    if (index == 100)
                        break;
                }
            else
                while (CalculateF(SecondKindConstraint, newWorst.X1[0], newWorst.X2[0]) <= X1X2)
                {
                    newWorst.X1[0] = (newWorst.X1[0] + complexCenter.X1[0]) / 2;
                    newWorst.X2[0] = (newWorst.X2[0] + complexCenter.X2[0]) / 2;
                    index++;
                    if (index == 100)
                        break;
                }

            return newWorst;
        }
        /// <summary>
        /// Нахождение новой вершины смещением xi0   на половину расстояния к лучшей из вершин комплекса  с номером G
        /// </summary>
        /// <param name="vx"></param>
        /// <param name="newWorst"></param>
        /// <param name="worstIndex"></param>
        /// <param name="bestIndex"></param>
        /// <returns></returns>
        VX FixVertexAndSwapFs(VX vx, VX newWorst, int worstIndex, int bestIndex)
        {
            int index = 0;
            while (CalculateF(text, newWorst.X1[0], newWorst.X2[0]) < CalculateF(text, vx.X1[worstIndex], vx.X2[worstIndex]))
            {
                newWorst.X1[0] = (newWorst.X1[0] + vx.X1[bestIndex]) / 2;
                newWorst.X2[0] = (newWorst.X2[0] + vx.X2[bestIndex]) / 2;
                index++;
                if (index == 100)
                    break;
            }

            vx.X1[worstIndex] = newWorst.X1[0];
            vx.X2[worstIndex] = newWorst.X2[0];
            vx.F[worstIndex] = CalculateF(text, newWorst.X1[0], newWorst.X2[0]);
            return vx;
        }
    }
}
