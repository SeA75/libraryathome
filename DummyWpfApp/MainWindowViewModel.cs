using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using DummyWpfApp.Annotations;
using GalaSoft.MvvmLight.Command;

namespace DummyWpfApp
{

    public class Item
    {
        public Item(string name, string matches, Collection<string> auth)
        {
            Name = name;
            Matches = matches;
            Authors = auth;
        }

        public string Name { get; set; }
        public string Matches { get; set; }

        public Collection<string> Authors { get; set; }

        public override string ToString()
        {
            return Name + "  " + Matches;
        }


        public override bool Equals(object obj)
        {
            return this.Equals(obj as Item);
        }

        public bool Equals(Item p)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(p, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, p))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != p.GetType())
            {
                return false;
            }

            return (this.ToString() == p.ToString());
        }



        //public override int GetHashCode()
        //{
        //    Regex rs = new Regex("", RegexOptions.IgnoreCase);
        //    return Convert.ToInt32(Name));
        //}


        public static bool IsNullOrEmpty(Item instance)
        {
            if (instance == null)
                return true;
            return (instance == new Item(instance.Name, instance.Matches, instance.Authors));
        }

        public static bool operator ==(Item lhs, Item rhs)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Item lhs, Item rhs)
        {
            return !(lhs == rhs);
        }


    }

    public class ItemHandler
    {
        public ItemHandler()
        {
            Items = new ObservableCollection<Item>();
        }

        public ObservableCollection<Item> Items { get; set; }

        public void Add(Item item)
        {
            Items.Add(item);
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ItemHandler _itemHandler;

        public RelayCommand StartLoadCommand { get; set; }

        public MainWindowViewModel()
        {
            StartLoadCommand = new RelayCommand(LoadData);
            _itemHandler = new ItemHandler();
        }

        public ObservableCollection<Item> Items
        {
            get { return _itemHandler.Items; }
            //set
            //{
            //    _itemHandler.Items = value;
            //    OnPropertyChanged();
            //}
        }

        

        public void LoadData()
        {
            for (int i = 0; i < 100; i++)
            {
                Items.Add(new Item(Guid.NewGuid().ToString(), i.ToString(), new Collection<string>() { "Pino", "Andrea" }));

            }

        }

        //public string Author
        //{
        //    get { return _author; }

        //    set
        //    {
        //        _author = value;
        //        OnPropertyChanged("Author");
        //    }
        //}

        //public string Title
        //{
        //    get { return _title; }

        //    set
        //    {
        //        _title = value;
        //        OnPropertyChanged("Title");
        //    }
        //}



        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
