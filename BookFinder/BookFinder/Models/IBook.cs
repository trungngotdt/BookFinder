using BookFinder.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookFinder.Models
{
    abstract class IBook
    {
        private string author;
        private string title;
        private string image;
        private LibraryName library;

        public string Title { get => title; set => title = value; }
        public string Image { get => image; set => image = value; }
        public string Source { get { return Author+ " | "+Library.ToString(); } }

        public string Author { get => author; set => author = value; }
        public LibraryName Library { get => library; set => library = value; }
    }
}
