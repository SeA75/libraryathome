using System.Collections.Generic;
using LibraryAtHomeRepositoryDriver;

namespace LibraryAtHomeTracerFileMetadataExtractor
{
    public abstract class AbstractBookFinderHandlerr : IBookFinderHandler
    {
        private IBookFinderHandler _nextHandler;

        protected string _fileUnderAnalysis;


        public AbstractBookFinderHandlerr(string fileunderanalysis)
        {
            _fileUnderAnalysis = fileunderanalysis;
        }

        public IBookFinderHandler SetNext(IBookFinderHandler handler)
        {
            this._nextHandler = handler;

            return handler;
        }

        public virtual BookAtHome HandleTheBookFromList(string title, List<PocoBook> booklist)
        {
            if (this._nextHandler != null)
            {
                return this._nextHandler.HandleTheBookFromList(title, booklist);
            }
            else
            {
                return new BookToBeReviewed(_fileUnderAnalysis, "book not handled.");
            }
        }

       
    }
}
