using BooksParser;
using GalaSoft.MvvmLight.Command;
using LibraryAtHomeRepositoryDriver;
using LibraryAtHomeUI.Annotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace LibraryAtHomeUI
{
    public class MainWindowsViewModel : INotifyPropertyChanged
    {
        public MainWindowsViewModel()
        {
            StartCollectCommand = new RelayCommand(ExecuteStartCollect);
            MSG.Messenger.Default.Register<LibraryConfigurationData>(this, UpdateLibraryConfigurationData);
            DeleteLibraryCommand = new RelayCommand(DeleteLibrary);
            _pocoBookHandler = new PocoBookHandler();
        }

        private void DeleteLibrary()
        {
            var res = MessageBox.Show($"Do you want to delete database {ConfData.DatabaseName} ?", "Libraryathome", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
            {
                _cataloger?.DropDatabase();
                MessageBox.Show($"Database {ConfData.DatabaseName} deleted!", "Libraryathome", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string _libraryName;

        public string LibraryName
        {
            get { return _libraryName; }
            set
            {
                _libraryName = value;
                OnPropertyChanged();
            }
        }

        private string _bookFolder;

        public string BookFolder
        {
            get { return _bookFolder; }
            set
            {
                _bookFolder = value;
                OnPropertyChanged();
            }
        }

        private PocoBookHandler _pocoBookHandler;

        private LibraryCataloguer _cataloger;

        public RelayCommand StartCollectCommand { get; set; }

        public RelayCommand DeleteLibraryCommand { get; set; }

        private LibraryConfigurationData _confData;

        private int _booksCollectionProgress;

        private int _totalBookCount;

        private int _totalBookCatalogedCount;

        private int _totalBookDiscarterCount;

        public LibraryConfigurationData ConfData
        {
            get
            {
                return _confData;
            }
            set
            {
                _confData = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PocoBook> Books
        {
            get { return _pocoBookHandler.Items; }
        }

      


        public async void ExecuteStartCollect()
        {
            if (!CanExecuteStartCollection())
            {
                MessageBox.Show("Library not properly Configured", "Libraryathome", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var configuration = new BookParserConfig
            {
                ebookdirectory = _confData.EbookFolder,
                libraryContext = new LibraryContextConfig
                {
                    hostname = _confData.RepositoryHost, databasename = _confData.DatabaseName
                }
            };

            configuration.ebookformat = _confData.BookFormatsCheckStatus.Keys.ToList();
            configuration.providerPlugin.pluginfolder = AppDomain.CurrentDomain.BaseDirectory;
            configuration.providerPlugin.pluginassemblyname = "LibraryAtHomeProvider";
           
          
            var exceptions = new ConcurrentQueue<Exception>();

            Action<int> updateProg = i => UpdateprogressBar(i);

            _cataloger = new LibraryCataloguer(configuration, exceptions, null, updateProg);

            TotalBookCount = _cataloger.FileCount;
            

            try
            {
                await Task.Run(async () =>
                {
                    await _cataloger.CatalogBooksAsync().ConfigureAwait(false);
                });
                

                List<PocoBook> bookCollected = _cataloger.BooksInLibrary.Read();
                List<BookToBeReviewed> booktoreview = _cataloger.BookToReview.Read();

                TotalBookCataloged = bookCollected.Count;
                TotalBookDiscarted = booktoreview.Count;

                foreach (var book in bookCollected)
                {
                    if (!Books.Contains(book))
                        Books.Add(book);
                }
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.Flatten().InnerExceptions)
                {
                    if (ex is ArgumentException)
                        Console.WriteLine(ex.Message);
                }
            }
        }


        public void UpdateLibraryConfigurationData(LibraryConfigurationData message)
        {
            Books.Clear();

            ConfData = message;
            LibraryName = message.DatabaseName;
            BookFolder = message.EbookFolder;

            if (message.LibraryExits)
            {
                BooksCollectedDataMapper mapper = new BooksCollectedDataMapper(message.RepositoryHost, message.DatabaseName) ;
                LibraryStatisticsDataMapper stat = new LibraryStatisticsDataMapper(message.RepositoryHost, message.DatabaseName);

                BookFolder = stat.Read().FirstOrDefault().LibraryDirectory;


                foreach (var book in mapper.Read())
                {
                    if (!Books.Contains(book))
                        Books.Add(book);
                }
            }

            LibraryAtHomeMain.Default.EbookFolder = message.EbookFolder;
            LibraryAtHomeMain.Default.LastLibraryOpened = message.DatabaseName;
            LibraryAtHomeMain.Default.LibraryExists = true;
            LibraryAtHomeMain.Default.RepositoryHost = message.RepositoryHost;
            LibraryAtHomeMain.Default.Save();

        }

        public void UpdateprogressBar(int value)
        {
            this.BookCollectionProgress = value;
        }


        public bool CanExecuteStartCollection()
        {
            if (ConfData == null)
                return false;
            if (string.IsNullOrEmpty(_confData.ConnectionString))
                return false;
            if (string.IsNullOrEmpty(_confData.DatabaseName))
                return false;
            if (string.IsNullOrEmpty(_confData.EbookFolder))
                return false;
            return true;
        }


        public int TotalBookCount
        {
            get { return _totalBookCount; }
            private set
            {
                if (this._totalBookCount != value)
                {
                    _totalBookCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TotalBookCataloged
        {
            get { return _totalBookCatalogedCount; }
            private set
            {
                if (this._totalBookCatalogedCount != value)
                {
                    _totalBookCatalogedCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TotalBookDiscarted
        {
            get { return _totalBookDiscarterCount; }
            private set
            {
                if (this._totalBookDiscarterCount != value)
                {
                    _totalBookDiscarterCount = value;
                    OnPropertyChanged();
                }
            }
        }


        public int BookCollectionProgress
        {
            get { return _booksCollectionProgress; }
            private set
            {
                if (this._booksCollectionProgress != value)
                {
                    _booksCollectionProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
