﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace BooksParser
{

    [BsonIgnoreExtraElements]
    public class PocoBook : BookatHome
    {
        public PocoBook()
        {

        }


        public PocoBook(string file)
        {
            BookReliability = Reliability.Discarded;
            File = file;
        }



        public PocoBook(string file, string title, string[] authors, string isbn, string searchTitle)
        {
            if (searchTitle != null)
            {
                SearchTitle = searchTitle.Trim().Trim();
            }
            File = file;
            Title = title;
            Authors = authors;
            Isbnsearch = isbn;
            Format = Path.GetExtension(file).Replace(".", string.Empty);
            BookReliability = Reliability.High;
        }

        public string Title { get; set; }

        internal string SearchTitle { get; set; }

        public string[] Authors { get; set; }

        public string Publisher { get; set; }


        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]

        public DateTime PublishedDate { get; set; }

        public string Description { get; set; }

        internal string Isbnsearch { get; set; }

        public string Isbn { get; set; }


        public int PageCount { get; set; }

        public string[] Categories { get; set; }

        public string Language { get; set; }

        public string Format { get; set; }

        public override string ToString()
        {
            string authorstr = string.Empty;
            string catstr = string.Empty;
            if (Authors != null)
            {
                authorstr = string.Join(", ", Authors);
            }
            if (Categories != null)
            {
                catstr = string.Join(", ", Categories);
            }


            return "Title: " + Title + " Authors: " + authorstr + " Description: " + Description + " Publisher: " + Publisher + " Publisher Data: " + PublishedDate + " Isbn: " + Isbn +
                 " Language: " + Language + " Categories: " + catstr + "Format: " + Format + "Page Count: " + PageCount + "Reliability: " + BookReliability;
        }


        [BsonRepresentation(BsonType.String)]
        public Reliability BookReliability { get; set; }


        public enum Reliability
        {
            Empty,
            Discarded,
            Low,
            Medium,
            High
        }


        public override bool Equals(object obj)
        {
            return this.Equals(obj as PocoBook);
        }

        public bool Equals(PocoBook p)
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



        public override int GetHashCode()
        {
            Regex rs = new Regex("", RegexOptions.IgnoreCase);
            return Convert.ToInt32(rs.Replace(Isbn, "").Trim(new[] { '0' }));
        }


        public static bool IsNullOrEmpty(PocoBook instance)
        {
            if (instance == null)
                return true;
            return (instance == new PocoBook());
        }

        public static bool operator ==(PocoBook lhs, PocoBook rhs)
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

        public static bool operator !=(PocoBook lhs, PocoBook rhs)
        {
            return !(lhs == rhs);
        }
    }


}
