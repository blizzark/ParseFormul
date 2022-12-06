using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ChartDirector;


namespace ParseFormuls
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Method method;
        public static ObservableCollection<NameTable> NameTableList { get; set; }
        WinChartViewer chartViewer;
        public MainWindow()
        {
            InitializeComponent();
            DataVariable.Columns[0].IsReadOnly = true;
            NameTableList = new ObservableCollection<NameTable>();
        }

        #region Обработка события нажатия на ЧекБокс в ДатаГрид переменных
        void OnChecked(object sender, RoutedEventArgs e)
        {
            //TODO надо бы сделать так, чтобы при выборе 2-х переменных, остальные нельзя было активировать

            NameTable cust = (NameTable)DataVariable.SelectedItem;
            if ((string)LabelX1.Content != cust.name)
            {
                if ((string)LabelX1.Content == "")
                {
                    LabelX1.Content = cust.name;
                }
                else if ((string)LabelX2.Content == "")
                {
                    LabelX2.Content = cust.name;
                }
            }
          
        }

        void UnChecked(object sender, RoutedEventArgs e)
        {
            NameTable cust = (NameTable)DataVariable.SelectedItem;
            if ((string)LabelX1.Content == cust.name)
            {
                LabelX1.Content = "";
            }
            else if ((string)LabelX2.Content == cust.name)
            {
                LabelX2.Content = "";
            }
        }
        #endregion

        private void ButtFind_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TableXYZ.ItemsSource = null;
                TableXYZ.Items.Refresh();

                string text = TextBoxFormuls.Text;
                string SecondKindConstraint = TextBoxSecondKindConstraint.Text;

                string nameX1 = (string)LabelX1.Content;
                string nameX2 = (string)LabelX2.Content;
                int minX1 = Convert.ToInt32(TextBoxMinX1.Text);
                int minX2 = Convert.ToInt32(TextBoxMinX2.Text);
                int maxX1 = Convert.ToInt32(TextBoxMaxX1.Text);
                int maxX2 = Convert.ToInt32(TextBoxMaxX2.Text);
                int X1X2 = Convert.ToInt32(TextBoxX1X2.Text);
                double accuracy = double.Parse(AccuracyBox.Text);

                if(MinMaxBox.SelectedIndex == 0)
                {
                    text = "-" + text;
                }
                CallCalculator.text = text;
                CallCalculator.SecondKindConstraint = SecondKindConstraint;
                CallCalculator.nameX1 = nameX1;
                CallCalculator.nameX2 = nameX2;

                

                if (ComboBoxMethodBox.IsSelected)
                {
                    method = new BoxMethod(minX1, minX2, maxX1, maxX2, X1X2, accuracy, SymbolBox.SelectedIndex);
                }
                else if (ComboBoxMethodGen.IsSelected)
                {
                    text = "-" + text;
                    CallCalculator.text = text;
                    method = new GeneticMethod(minX1, minX2, maxX1, maxX2, X1X2, accuracy, SymbolBox.SelectedIndex);

                }
                else if(ComboBoxMethodFull.IsSelected)
                {
                    method = new BruteForceMethod(minX1, minX2, maxX1, maxX2, X1X2, accuracy, SymbolBox.SelectedIndex);   
                }
                else if (ComboBoxFull.IsSelected)
                {
                    string enter = "";
                    method = new BoxMethod(minX1, minX2, maxX1, maxX2, X1X2, accuracy, SymbolBox.SelectedIndex);
                    enter += "Метод Бокса:\n" + nameX1 + " = " + Math.Round(method.answerX1, 3) + " " + nameX2 + " = " + Math.Round(method.answerX2, 3) + " F = " + (-method.answer);
                    text = "-" + text;
                    CallCalculator.text = text;
                    method = new GeneticMethod(minX1, minX2, maxX1, maxX2, X1X2, accuracy, SymbolBox.SelectedIndex);
                    text = "-" + text;
                    CallCalculator.text = text;
                    enter += "\nГенетический метод:\n" + nameX1 + " = " + Math.Round(method.answerX1, 3) + " " + nameX2 + " = " + Math.Round(method.answerX2, 3) + " F = " + (-method.answer);
                    method = new BruteForceMethod(minX1, minX2, maxX1, maxX2, X1X2, 0.1, SymbolBox.SelectedIndex);
                    enter += "\nМетод полного перебора (пониженная точность):\n" + nameX1 + " = " + Math.Round(method.answerX1, 3) + " " + nameX2 + " = " + Math.Round(method.answerX2, 3) + " F = " + (-method.answer);
                    System.Windows.MessageBox.Show(enter, "Результат вычислений всеми методами", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                AnswerLabelX1.Content = nameX1 + " = " + Math.Round(method.answerX1, 3);
                AnswerLabelX2.Content = nameX2 + " = " + Math.Round(method.answerX2, 3);
                TableXYZ.ItemsSource = method.InitialDataList;
                SpinOX.IsEnabled = true;
                SpinOY.IsEnabled = true;
                SpinOX.Value = 20;
                SpinOY.Value = 30;
              
                if (MinMaxBox.SelectedIndex == 0)
                {
                    double aRes = Convert.ToDouble(method.answer);
                    AnswerLabelF.Content = "F = " + (-aRes).ToString();

                    for (int i = 0; i < method.InitialDataList.Count; i++)
                    {
                        double tmp = Convert.ToDouble(method.InitialDataList[i].result);
                        method.InitialDataList[i].result = (-tmp).ToString();
                    }
                }
                else
                {
                    AnswerLabelF.Content = "F = " + method.answer;
                }
                CreateChart(method.InitialDataList, chartViewer, (int)SpinOX.Value, (int)SpinOY.Value);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка!");
            }


        }
        private void Grid1_Loaded(object sender, RoutedEventArgs e)
        {
            // Create the interop host control.
            System.Windows.Forms.Integration.WindowsFormsHost host =
                new System.Windows.Forms.Integration.WindowsFormsHost();

            // Create the MaskedTextBox control.
            chartViewer = new WinChartViewer();
            chartViewer.ChartSizeMode = WinChartSizeMode.StretchImage;
            this.windowsFormsHost.Child = chartViewer;

            // Add the interop host control to the Grid
            // control's collection of child controls.
            this.grid1.Children.Add(host);
        }

        private void TextBoxFormuls_TextChanged(object sender, TextChangedEventArgs e)
        {
           // VariantsComboBox.Text = null; //TODO при обновлении текста он уничтоает уже имеющийся верный выбор
            LabelX1.Content = "";
            LabelX2.Content = "";
            NameTableList.Clear();
            DataVariable.ItemsSource = null;
            DataVariable.Items.Refresh();
            string text = TextBoxFormuls.Text;
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsLetter(text[i]))
                {
                    string name = "";
                    while (Char.IsNumber(text[i]) || Char.IsLetter(text[i]))
                    {
                        if(text[i] == ' ')
                        {
                            break;
                        }
                        name += text[i];
                        i++;
                        if (i >= text.Length)
                        {
                            break;
                        }
                    }

                    if (name == "cos" || name == "sqrt" || name == "exp" || name == "sin")
                        continue;

                    bool flag = true;
                    for (int j = 0; j < NameTableList.Count; j++)
                    {
                        if(name == NameTableList[j].name) {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        NameTableList.Add(new NameTable(name));
                    }
                }
            }
            DataVariable.ItemsSource = NameTableList;
            
        }

        #region Очистка ТектБоксов по нажатию

        private void TextBoxMinX1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBoxMinX1.Text = "";
        }

        private void TextBoxMaxX1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBoxMaxX1.Text = "";
        }

        private void TextBoxMinX2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBoxMinX2.Text = "";
        }

        private void TextBoxMaxX2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBoxMaxX2.Text = "";
        }

        private void TextBoxX1X2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBoxX1X2.Text = "";
        }

        #endregion

        private void VariantsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(VariantsComboBox.SelectedIndex == 0)
            {
                
                TextBoxFormuls.Text = "(α*(A1^2 +β*A2 – µ*V1)^N + α1*(β1*A1 + A2^2 – µ1*V2)^N)*t*P";
                NameTableList[0].value = 1;
                NameTableList[1].value = 1;
                NameTableList[2].value = 1;
                NameTableList[3].value = 1;
                NameTableList[4].value = 1;
                NameTableList[5].value = 11;
                NameTableList[6].value = 2;
                NameTableList[7].value = 1;
                NameTableList[8].value = 1;
                NameTableList[9].value = 1;
                NameTableList[10].value = 7;
                NameTableList[11].value = 8;
                NameTableList[12].value = 100;

                DataVariable.SelectedIndex = 1;
                NameTableList[1].variable = true;
                DataVariable.SelectedIndex = 3;
                NameTableList[3].variable = true;
                LabelX1.Content = "A1";
                LabelX2.Content = "A2";

                TextBoxMinX1.Text = "1";
                TextBoxMinX2.Text = "1";
                TextBoxMaxX1.Text = "10";
                TextBoxMaxX2.Text = "10";
                TextBoxX1X2.Text = "20";
                MinMaxBox.SelectedIndex = 0;
                TextBoxSecondKindConstraint.Text = "4*A1+5*A2";
                MinMaxBox.SelectedIndex = 0;
                SymbolBox.SelectedIndex = 0;

            }
            else if (VariantsComboBox.SelectedIndex == 1)
            {
                TextBoxFormuls.Text = "(α * (L - S)^2 + β * 1 / H * (S+ L -  γ *N)^2) * P";
                NameTableList[0].value = 1;
                NameTableList[1].value = 1;
                NameTableList[2].value = 1;
                NameTableList[3].value = 1;
                NameTableList[4].value = 9;
                NameTableList[5].value = 1;
                NameTableList[6].value = 10;
                NameTableList[7].value = 100;

                DataVariable.SelectedIndex = 1;
                NameTableList[1].variable = true;
                DataVariable.SelectedIndex = 2;
                NameTableList[2].variable = true;
                LabelX1.Content = "L";
                LabelX2.Content = "S";

                TextBoxMinX1.Text = "1";
                TextBoxMinX2.Text = "1";
                TextBoxMaxX1.Text = "15";
                TextBoxMaxX2.Text = "12";
                TextBoxX1X2.Text = "12";

                TextBoxSecondKindConstraint.Text = "L+S";
                MinMaxBox.SelectedIndex = 0;
                SymbolBox.SelectedIndex = 1;

            }
            else if (VariantsComboBox.SelectedIndex == 2)
            {
                TextBoxFormuls.Text = "α*(G*µ*((T2-T1)^N + (β *A-T1)^N)) * P";
                NameTableList[0].value = 1;
                NameTableList[1].value = 2;
                NameTableList[2].value = 1;
                NameTableList[3].value = -2;
                NameTableList[4].value = -3;
                NameTableList[5].value = 2;
                NameTableList[6].value = 1;
                NameTableList[7].value = 1;
                NameTableList[8].value = 100;

                DataVariable.SelectedIndex = 3;
                NameTableList[3].variable = true;
                DataVariable.SelectedIndex = 4;
                NameTableList[4].variable = true;
                LabelX1.Content = "T1";
                LabelX2.Content = "T2";

                TextBoxMinX1.Text = "-3";
                TextBoxMinX2.Text = "-2";
                TextBoxMaxX1.Text = "3";
                TextBoxMaxX2.Text = "6";
                TextBoxX1X2.Text = "1";

                MinMaxBox.SelectedIndex = 0;

                TextBoxSecondKindConstraint.Text = "T1+0,5*T2";

                SymbolBox.SelectedIndex = 0;

            }
        }

        private void FormCalc_Click(object sender, RoutedEventArgs e)
        {
            FormForCalculations win = new FormForCalculations();
            win.ShowDialog();

        }

        #region Отрисовка графика и Спинеров

        void CreateChart(List<InitialData> InitialDataList, WinChartViewer viewer, int xAngle, int yAngle)
        {
            
           
            List<double> tmpX = new List<double>();
            List<double> tmpY = new List<double>();
            List<double> tmpZ = new List<double>();
            for (int i = 0; i < InitialDataList.Count; i++)
            {
                tmpX.Add(Convert.ToDouble(InitialDataList[i].X1));
                tmpY.Add(Convert.ToDouble(InitialDataList[i].X2));
                tmpZ.Add(Convert.ToDouble(InitialDataList[i].result));
            }
            double[] dataX = tmpX.ToArray();
            double[] dataY = tmpY.ToArray();
            double[] dataZ = tmpZ.ToArray();

            //Создаем график
            SurfaceChart chart = new SurfaceChart(900, 740);

            //Задаем вид графика
            chart.setPlotRegion(440, 360, 500, 500, 350);
            chart.setViewAngle(xAngle, yAngle);
            chart.setData(dataX, dataY, dataZ);
            chart.setInterpolation(80, 80);
            chart.setSurfaceAxisGrid(unchecked((int)0xdd000000));
            chart.setContourColor(unchecked((int)0x80ffffff));
            chart.setColorAxis(825, 350, ChartDirector.Chart.Left, 200, ChartDirector.Chart.Right).setColorGradient();

            //Подписываем оси
            chart.xAxis().setTitle((string)LabelX1.Content, "Arial Bold", 15);
            chart.yAxis().setTitle((string)LabelX2.Content, "Arial Bold", 15);

            //Передаем график в вьювер
            viewer.Chart = chart;

            //Подсказка
            viewer.ImageMap = chart.getHTMLImageMap("clickable", "",
                "title='(x={x|p}, y={y|p}, z={z|p}'");
        }

      

        private void SpinOX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CreateChart(method.InitialDataList, chartViewer, (int)SpinOX.Value, (int)SpinOY.Value);
        }

        private void SpinOY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CreateChart(method.InitialDataList, chartViewer, (int)SpinOX.Value, (int)SpinOY.Value);
        }


        #endregion

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DescVar6_Click(object sender, RoutedEventArgs e)
        {
            Description win = new Description(6);
            win.ShowDialog();
        }

        private void DescVar3_Click(object sender, RoutedEventArgs e)
        {
            Description win = new Description(3);
            win.ShowDialog();
        }

        private void DescVar5_Click(object sender, RoutedEventArgs e)
        {
            Description win = new Description(5);
            win.ShowDialog();
        }
    }
} 
