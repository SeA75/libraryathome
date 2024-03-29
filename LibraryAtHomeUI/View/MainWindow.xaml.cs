﻿using FontAwesome5;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LibraryAtHomeRepositoryDriver;

using MSG = GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;

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

            if (LibraryAtHomeMain.Default.LibraryExists)
            {
                LibraryConfigurationData confData = new LibraryConfigurationData();
                confData.DatabaseName = LibraryAtHomeMain.Default.LastLibraryOpened;
                confData.EbookFolder = LibraryAtHomeMain.Default.EbookFolder;
                confData.RepositoryHost = LibraryAtHomeMain.Default.RepositoryHost;
                confData.LibraryExits = LibraryAtHomeMain.Default.LibraryExists;
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(confData);
            }
        }

        private LibraryConfigurationWindow _configWindow;
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {

            _configWindow = new LibraryConfigurationWindow();
            _configWindow.ShowDialog();

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
            { Icon = EFontAwesomeIcon.Solid_Trash, Height = 15, Width = 15, HorizontalAlignment = HorizontalAlignment.Left });
            dp.Children.Add(new TextBlock() { Text = buttonContent, HorizontalAlignment = HorizontalAlignment.Right });
            newBtn.Content = dp;
            newBtn.Click += new RoutedEventHandler(this.buttonRemove_Click);
            return newBtn;
        }


        protected void buttonRemove_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (sbFilterInsedeGroupFilter.UiElementExistsByName(button?.Name))
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

        private void ListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BookDetailsWindow details = new BookDetailsWindow();
            details.Show();

            PocoBook selectedBook = (sender as ListView).SelectedItem as PocoBook;

            MSG.Messenger.Default.Send(selectedBook);

        }

        private void tbFilters_LostFocus(object sender, RoutedEventArgs e)
        {
            string filter = (sender as TextBox).Text;
            MSG.Messenger.Default.Send(filter);
        }

        private void tbFilters_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string filter = tbFilters.Text;
                MSG.Messenger.Default.Send(filter);
            }
        }
    }
}
