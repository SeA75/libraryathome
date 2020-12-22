using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using LibraryAtHomeUI.Annotations;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BooksParser;
using iText.Layout.Element;
using LibraryAtHomeRepositoryDriver;
using LibraryAtHomeTracer;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace LibraryAtHomeUI
{
    public class MainWindowsViewModel : INotifyPropertyChanged
    {
        public MainWindowsViewModel()
        {
            StartCollectCommand = new RelayCommand(ExecuteStartCollect);
            MSG.Messenger.Default.Register<LibraryConfigurationData>(this, UpdateLibraryConfigurationData);
           
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<PocoBook> _MyBooks;
       

        public RelayCommand StartCollectCommand { get; set; }

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

        public ObservableCollection<PocoBook> MyBooks {
            get
            {
                return _MyBooks;
            }
            set
            {
                _MyBooks = value;
                OnPropertyChanged();
            }
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
                    connectionstring = _confData.ConnectionString, databasename = _confData.DatabaseName
                }
            };

            configuration.ebookformat = _confData.BookFormatsCheckStatus.Keys.ToList();
            configuration.providerPlugin.pluginfolder = AppDomain.CurrentDomain.BaseDirectory;
            configuration.providerPlugin.pluginassemblyname = "LibraryAtHomeProvider";
           
          
            var exceptions = new ConcurrentQueue<Exception>();

            Action<int> updateProg = i => UpdateprogressBar(i);

            LibraryCataloguer cataloger = new LibraryCataloguer(configuration, exceptions, null, updateProg);

            TotalBookCount = cataloger.FileCount;


            try
            {

                await Task.Run(async () =>
                {
                    await cataloger.CatalogBooksAsync().ConfigureAwait(false);
                });
                

                List<PocoBook> bookCollected = cataloger.BooksInLibrary.Read();
                List<BookToBeReviewed> booktoreview = cataloger.BookToReview.Read();

                TotalBookCataloged = bookCollected.Count;
                TotalBookDiscarted = booktoreview.Count;

                _MyBooks = new ObservableCollection<PocoBook>();

          
                MyBooks = new ObservableCollection<PocoBook>(bookCollected);
                //foreach (var book in )
                //{
                //    MyBooks.AddAdd(book);
                //}
            

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
            ConfData = message;
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
