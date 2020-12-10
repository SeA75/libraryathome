using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace LibraryAtHomeControlLibrary
{
    class BookPreviewViewModel : INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title { get; set; }

        public string Description { get; set; }

        public string Authors { get; set; }

        public string Language { get; set; }

        public string Publisher { get; set; }

        public string Subjects { get; set; }

        public string ImageLink { get; set; }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));      
        }
    }

}
