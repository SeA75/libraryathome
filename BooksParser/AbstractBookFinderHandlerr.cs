using System.Collections.Generic;
using LibraryAtHomeRepositoryDriver;

namespace BooksParser
{
    public abstract class AbstractBookFinderHandlerr : IBookFinderHandler
    {
        private IBookFinderHandler _nextHandler;

        public IBookFinderHandler SetNext(IBookFinderHandler handler)
        {
            this._nextHandler = handler;

            return handler;
        }

        public virtual BookatHome HandleTheBook(string fileunderanalysis, List<PocoBook> booklist, string maybetitle, IBookParserTrace trace)
        {
            if (this._nextHandler != null)
            {
                return this._nextHandler.HandleTheBook(fileunderanalysis, booklist, maybetitle, trace);
            }
            else
            {
                return new BookToBeReviewed(fileunderanalysis, "book not handled");
            }
        }

    }
}
