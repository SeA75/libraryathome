using System;
using System.IO;
using LibraryAtHomeRepositoryDriver;


namespace LibraryAtHomeTracerFileMetadataExtractor
{
    public class RtfFileInfoExtractor : FileInfoExtractor
    {
        public override BookAtHome GetPocoBook(string filepath)
        {
            return base.GetPocoBook(filepath);
        }
    }
}
