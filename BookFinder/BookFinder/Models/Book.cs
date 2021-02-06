using BookFinder.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookFinder.Models
{
    public class Book
    {
        public string Image { get; set; }
        public LibraryName Library { get; set; }
        public string ID { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public Book()
        {

        }
        public Book(string id,string title,string link)
        {
            this.ID = id;
            this.Title = title;
            this.Link = link;
        }

        public Book(string id, string title, string link,string image,LibraryName library)
        {
            this.ID = id;
            this.Title = title;
            this.Link = link;
            this.Image = image;
            this.Library = library;
        }

        public string GetLink()
        {
            switch (Library)
            {
                case LibraryName.Genesis:
                    return EndPoint.urlGenesis + "/" + Link;
                case LibraryName.ZLibrary:
                    return EndPoint.urlGutenberg + "/ebooks/" + ID;
                    
                default:
                    throw new NotImplementedException();
                    
            }
            
        }
    }
}
