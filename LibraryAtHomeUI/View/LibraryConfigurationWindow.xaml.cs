using System.Windows;
using System.Windows.Controls;

namespace LibraryAtHomeUI
{
    /// <summary>
    /// Interaction logic for LibraryConfigurationWindow.xaml
    /// </summary>
    public partial class LibraryConfigurationWindow : Window
    {
        
        public LibraryConfigurationWindow()
        {
            InitializeComponent();
        }
     
        private void CbNewLib_OnChecked(object sender, RoutedEventArgs e)
        {
            NewLibraryOrExistingLibrary(sender, e);
        }

        private void CbNewLib_OnUnchecked(object sender, RoutedEventArgs e)
        {
            NewLibraryOrExistingLibrary(sender, e);
        }

        private void NewLibraryOrExistingLibrary(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            tbNewLibrary.IsEnabled = (bool)cb.IsChecked;
            tbEbookFolder.IsEnabled = tbNewLibrary.IsEnabled;
            btnAddLibrary.IsEnabled = tbNewLibrary.IsEnabled;
            cbLibraryName.IsEnabled = !tbNewLibrary.IsEnabled;
        }

        private void BtnConfCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
