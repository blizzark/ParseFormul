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
using System.IO;


namespace ParseFormuls
{
    /// <summary>
    /// Логика взаимодействия для Description.xaml
    /// </summary>
    public partial class Description : Window
    {
        public Description(int variant)
        {
            InitializeComponent();
            if(variant == 6)
            {
                using (StreamReader reader = new StreamReader("var6.txt"))
                {
                    DescTextBox.Text = reader.ReadToEnd();
                }
            }
            else if (variant == 3)
            {
                using (StreamReader reader = new StreamReader("var3.txt"))
                {
                    DescTextBox.Text = reader.ReadToEnd();
                }
            }
            else if (variant == 5)
            {
                using (StreamReader reader = new StreamReader("var5.txt"))
                {
                    DescTextBox.Text = reader.ReadToEnd();
                }
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
