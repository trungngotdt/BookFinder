using System;
using System.Collections.Generic;
using System.Text;

namespace BookFinder.Models
{
    public class Book
    {
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
    }
}
