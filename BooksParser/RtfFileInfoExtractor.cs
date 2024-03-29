﻿using System;
using System.IO;
using LibraryAtHomeRepositoryDriver;
using Net.Sgoliver.NRtfTree.Core;
using Net.Sgoliver.NRtfTree.Util;


namespace BooksParser
{
    public class RtfFileInfoExtractor : FileInfoExtractor
    {
        public override BookatHome GetPocoBook(string filepath)
        {
            //RtfTree tree = new RtfTree();
            //tree.LoadRtfFile(filepath);

            //InfoGroup info = tree.GetInfoGroup();
            // if(!string.IsNullOrEmpty(info.Title) || !string.IsNullOrEmpty(info.Author) || info != null)
            //     return new PocoBook(filepath, info.Title, new []{info.Author}, string.Empty, string.Empty);
            return base.GetPocoBook(filepath);
        }
    }
}
