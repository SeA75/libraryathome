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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LibraryAtHomeUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCollect_Click(object sender, RoutedEventArgs e)
        {
            bool test = true;
            if (true)
            {
                var newForm = new BookDetails(); //create your new form.
                newForm.Show(); //show the new form.
               // this.Close(); //only if you want to close the current form.
            }
        }
    }
}
