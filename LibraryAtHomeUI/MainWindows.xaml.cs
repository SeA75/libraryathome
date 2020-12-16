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
using FontAwesome5;
using Microsoft.WindowsAPICodePack.Dialogs;
using Color = System.Drawing.Color;

namespace LibraryAtHomeUI
{
    /// <summary>
    /// Interaction logic for MainWindows.xaml
    /// </summary>
    public partial class MainWindows : Window
    {
        public MainWindows()
        {
            InitializeComponent();
        }

      

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
        }

        private bool _handle = true;

        private void cbPublishers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            _handle = !cmb.IsDropDownOpen;
            HandleEvent(cmb);
        }

        private Button CreateRemovableButton(string buttonContent, Brush clr)
        {
            Button newBtn = new Button
            {
                Name = "btn" + buttonContent,
                Height = 20,
                Width = 150,
                VerticalAlignment = VerticalAlignment.Top,
                Foreground = clr
            };

            DockPanel dp = new DockPanel();
            dp.Children.Add(new ImageAwesome
                {Icon = EFontAwesomeIcon.Solid_Trash, Height = 15, Width = 15, HorizontalAlignment = HorizontalAlignment.Left});
            dp.Children.Add(new TextBlock() {Text = buttonContent, HorizontalAlignment = HorizontalAlignment.Right});
            newBtn.Content = dp;
            newBtn.Click += new RoutedEventHandler(this.buttonRemove_Click);
            return newBtn;
        }


        protected void buttonRemove_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if(sbFilterInsedeGroupFilter.UiElementExistsByName(button?.Name))
                sbFilterInsedeGroupFilter.Children.Remove(sbFilterInsedeGroupFilter.GetUiElement(button?.Name));
        }

       
        private void CbPublishers_OnDropDownClosed(object sender, EventArgs e)
        {
            if (_handle) HandleEvent(sender as ComboBox);
            _handle = true;
        }


        private void HandleEvent(ComboBox comboControl)
        {
            if ((ComboBoxItem)comboControl.SelectedItem == null)
                return;
            
            string buttonContent = ((ComboBoxItem)(comboControl.SelectedItem)).Content.ToString();

            if (sbFilterInsedeGroupFilter.UiElementExistsByName("btn" + buttonContent))
                return;

            var newBtn = CreateRemovableButton(buttonContent, comboControl.Foreground);
            sbFilterInsedeGroupFilter.Children.Add(newBtn);
        }

        private void CbAuthors_OnDropDownClosed(object sender, EventArgs e)
        {
            if (_handle)
                HandleEvent(sender as ComboBox);

            _handle = true;
        }

        private void CbAuthors_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            _handle = !cmb.IsDropDownOpen;
            HandleEvent(cmb);
        }

        private void CbLanguages_OnDropDownClosed(object sender, EventArgs e)
        {
            if (_handle) HandleEvent(sender as ComboBox);
            _handle = true;
        }

        private void CbLanguages_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            _handle = !cmb.IsDropDownOpen;
            HandleEvent(cmb);
        }

        private void CbPublicationDate_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            _handle = !cmb.IsDropDownOpen;
            HandleEvent(cmb);
        }

        private void CbPublicationDate_OnDropDownClosed(object sender, EventArgs e)
        {
            if (_handle) HandleEvent(sender as ComboBox);
            _handle = true;
        }

        //private Action<string, object, object> EventCaller = (methodName, sender, callerevent) => typeof(MainWindows).GetMethod(methodName).Invoke(null, new[] { sender, callerevent });

        //private void FacadeHandler(object sender, EventArgs e)
        //{
            
        //}
    }
}
