using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ParseFormuls
{
    /// <summary>
    /// Логика взаимодействия для FormForCalculations.xaml
    /// </summary>
    public partial class FormForCalculations : Window
    {
        public FormForCalculations()
        {
            InitializeComponent();
        }

        private void Begin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = Formul.Text;
                Parser parser = new Parser(text);
                double answer = Сalculator.EvaluateExpression(parser);
                Answer.Content = answer.ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!");
            }
        }
    }
}
