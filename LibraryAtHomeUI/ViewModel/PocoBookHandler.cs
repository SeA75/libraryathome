using System.Collections.ObjectModel;
using LibraryAtHomeRepositoryDriver;

namespace LibraryAtHomeUI
{
    public class PocoBookHandler
    {
        public PocoBookHandler()
        {
            Items = new ObservableCollection<PocoBook>();
        }

        public ObservableCollection<PocoBook> Items { get; set; }

        public void Add(PocoBook item)
        {
            Items.Add(item);
        }
    }
}