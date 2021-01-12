using DrWPF.Windows.Data;
using GalaSoft.MvvmLight.Command;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using LibraryAtHomeRepositoryDriver;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace LibraryAtHomeUI
{
    public class LibraryConfigurationViewModel : INotifyPropertyChanged
    {
        private LibraryConfigurationData _configurationData;

        public RelayCommand<LibraryConfigurationWindow> ChooseFolderStringCommand { get; set; }

        public RelayCommand DropClosedCommand { get; private set; }

        public RelayCommand SelectionChangedCommand { get; private set; }
       

        public RelayCommand<object> ManageEbookFormatsCommand { get; private set; }

        public RelayCommand<LibraryConfigurationWindow> ConfigurationDoneCommand { get; private set; }

        public LibraryConfigurationViewModel()
        {
            ChooseFolderStringCommand = new RelayCommand<LibraryConfigurationWindow>( o => ChooseFolder(o));
            ManageEbookFormatsCommand = new RelayCommand<object>((s) => ManageEbookFormats(s));
            ConfigurationDoneCommand = new RelayCommand<LibraryConfigurationWindow>(o => ConfigurationDone(o));
            _ebookFormats = new ObservableDictionary<string, bool>();
            _configurationData = new LibraryConfigurationData();

            _libraries = new List<string>();

            SetDefaultEbookFormat();



            string server = string.IsNullOrEmpty(TxtDatabaseServer) ? "localhost" : TxtDatabaseServer;

            _mongoServerManager = new MongodbServerManager(server);
        }

        private void SetDefaultEbookFormat()
        {
            EBookFormats["pdf"] = true;
            EBookFormats["txt"] = true;
            EBookFormats["rtf"] = true;
            EBookFormats["doc"] = true;
            EBookFormats["epub"] = true;

        }


        private IMongodbServerManager _mongoServerManager;
       

        private List<string> _libraries;

        private string _selectedLibrary;

        public string SelectedLibrary
        {
            get { return _selectedLibrary; }
            set
            {
                _selectedLibrary = value;
                NotifyPropertyChanged();
            }
        }

        public string NewLibrary
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _libraries.Add(value);
                }
            }
        }

        public List<string> Libraries
        {
            get
            {
                return GetLibraryAtHomeDatabases();
            }

            set { _libraries = value; }
        }

        private List<string> GetLibraryAtHomeDatabases()
        {
            List<string> retval = new List<string>();
            foreach (var databaseName in _mongoServerManager.ListDatabases())
            {
                IMongodbDatabaseManager manager = new MongodbDatabaseManager(TxtDatabaseServer, databaseName);
                if (manager.CollectionExists("books"))
                    retval.Add(databaseName);
            }

            return retval;
        }


        private void ChooseFolder(LibraryConfigurationWindow win)
        {
            using (var dialog = new CommonOpenFileDialog {IsFolderPicker = true})
            {
                
                CommonFileDialogResult result = dialog.ShowDialog();

                if (result == CommonFileDialogResult.Ok)
                    TxtEbookFolder = dialog.FileName;
            }


            win?.Activate();
        }

        private void ConfigurationDone(LibraryConfigurationWindow confWin)
        {
            if (confWin.tbNewLibrary.IsEnabled)
            {
                _configurationData.DatabaseName = confWin.tbNewLibrary.Text;
                _configurationData.LibraryExits = false;
            }
            else
            {
                _configurationData.DatabaseName = confWin.cbLibraryName.SelectedItem.ToString();
                _configurationData.LibraryExits = true;
            }

            foreach (KeyValuePair<string, bool> entry in EBookFormats)
            {
                _configurationData.BookFormatsCheckStatus.Add(entry.Key, entry.Value);
            }

            MSG.Messenger.Default.Send(_configurationData);

            confWin?.Close();
        }


        private void ManageEbookFormats(object state)
        {
            var result = (object[])state;
            string ebookformat = (string) (result[0]);
            bool isEbookFormatSelected = (bool) (result[1]);

            if (!EBookFormats.TryAdd(ebookformat, isEbookFormatSelected))
            {
                EBookFormats[ebookformat] = isEbookFormatSelected;
            }

            if (!_configurationData.BookFormatsCheckStatus.TryAdd(ebookformat, isEbookFormatSelected))
            {
                _configurationData.BookFormatsCheckStatus[ebookformat] = isEbookFormatSelected;
            }
        }

        private ObservableDictionary<string, bool> _ebookFormats { get; set; }


        public ObservableDictionary<string, bool> EBookFormats
        {
            get { return _ebookFormats; }
            set
            {
                _ebookFormats = value;
                NotifyPropertyChanged();
            }
        }

        public string TxtEbookFolder
        {
            get { return _configurationData.EbookFolder; }
            set
            {
                _configurationData.EbookFolder = value;
                NotifyPropertyChanged();
            }
        }

        public string TxtLibraryName
        {
            get { return _configurationData.DatabaseName; }
            set
            {
                _configurationData.DatabaseName = value;
                NotifyPropertyChanged();
            }
        }

        public string TxtDatabaseServer
        {
            get { return _configurationData.RepositoryHost; }
            set
            {
                _configurationData.RepositoryHost = value;
                _configurationData.ConnectionString = $"mongodb://{value}:27017/"; // TODO database specific
                NotifyPropertyChanged();
            }
        }

        public string TxtPlugInFolder
        {
            get { return _configurationData.PlugInFolder; }
            set
            {
                _configurationData.PlugInFolder = value;
                NotifyPropertyChanged();
            }
        }

        public string TxtPluginAssemblyName
        {
            get { return _configurationData.PluginAssemblyName; }
            set
            {
                _configurationData.PluginAssemblyName = value;
                NotifyPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
