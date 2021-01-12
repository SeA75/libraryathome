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
using System.IO;
using System.Diagnostics;

namespace LibraryAtHomeUI.ViewModel
{
    public class BookDetailsViewModel : INotifyPropertyChanged
    {

        public RelayCommand OpenFileCommand { get; private set; }

        public BookDetailsViewModel()
        {
            MSG.Messenger.Default.Register<PocoBook>(this, GetBookDetailedInformation);
            OpenFileCommand = new RelayCommand(OpenFile);
        }

        private void OpenFile()
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = _file;
            p.Start();
        }

            private void GetBookDetailedInformation(PocoBook obj)
        {
            Title = obj.Title;
            Description = obj.Description;
            Publisher = obj.Publisher;
            Language = obj.Language;
            PublisherDate = obj.PublishedDate.Year.ToString();
            Isbn = obj.Isbn;
            PageCount = obj.PageCount.ToString();

            Authors = string.Join(", ", obj.Authors);
            Publisher = string.Join(", ", obj.Publisher);
            ImageLink = obj.ImageLink;
            _file = obj.File;
        }

        private String _title;

        private String _file;

        private String _description;

        private String _publisher;

        private string _language;

        private string _publisherdate;

        private string _isbn;

        private string _pageCount;

        private string _subjects;

        private string _authors;

        private string _imagelink;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged();
            }
        }

        public string ImageLink
        {
            get { return _imagelink; }
            set
            {
                _imagelink = value;
                NotifyPropertyChanged();
            }
        }

        public string Subjects
        {
            get { return _subjects; }
            set
            {
                _subjects = value;
                NotifyPropertyChanged();
            }
        }

        public string Authors
        {
            get { return _authors; }
            set
            {
                _authors = value;
                NotifyPropertyChanged();
            }
        }

        public string PageCount
        {
            get { return _pageCount; }
            set
            {
                _pageCount = value;
                NotifyPropertyChanged();
            }
        }

        public string Isbn
        {
            get { return _isbn; }
            set
            {
                _isbn = value;
                NotifyPropertyChanged();
            }
        }

        public string Language
        {
            get { return _language; }
            set
            {
                _language = value;
                NotifyPropertyChanged();
            }
        }


        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                NotifyPropertyChanged();
            }
        }

        public string Publisher
        {
            get { return _publisher; }
            set
            {
                _publisher = value;
                NotifyPropertyChanged();
            }
        }

        public string PublisherDate
        {
            get { return _publisherdate; }
            set
            {
                _publisherdate = value;
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
