using BookFinder.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookFinder.Models
{
    public class Book
    {
        private string author;
        private string image;
        private LibraryName library;
        private string iD;
        private string title;
        private string link;
        private string md5;

        public string Link { get => link; set => link = value; }
        public string Title { get => title; set => title = value; }
        public string ID { get => iD; set => iD = value; }
        public LibraryName Library { get => library; set => library = value; }
        public string Image { get 
            {

                return GetEndPoint()+ image;
            } 
            set => image = value; }
        public string Author { get => author; set => author = value; }
        public string Source { get { return author+ " | "+Library.ToString(); } }

        public string MD5 { get => md5; set => md5 = value; }

        public Book()
        {

        }
        public Book(string id,string title,string link)
        {
            this.ID = id;
            this.Title = title;
            this.Link = link;
        }

        public Book(string id, string title, string link,string image,LibraryName library,string author)
        {
            this.ID = id;
            this.Title = title;
            this.Link = link;
            this.Image = image;
            this.Library = library;
            this.Author = author;
        }

        public Book(string id, string title, string link, string image, LibraryName library, string author,string md5)
        {
            this.ID = id;
            this.Title = title;
            this.Link = link;
            this.Image = image;
            this.Library = library;
            this.Author = author;
            this.MD5 = md5;
        }

        private string GetEndPoint()
        {
            switch (Library)
            {
                case LibraryName.Genesis:
                    return EndPoint.urlGenesis;
                case LibraryName.Gutenberg:
                    return EndPoint.urlGutenberg;
                default:
                    throw new NotImplementedException();

            }
        }
        public string GetLink()
        {
            switch (Library)
            {
                case LibraryName.Genesis:
                    return EndPoint.urlGenesis + "/" + Link;
                case LibraryName.Gutenberg:
                    return EndPoint.urlGutenberg + "/ebooks/" + ID;
                    
                default:
                    throw new NotImplementedException();
                    
            }
        }
    }
}
