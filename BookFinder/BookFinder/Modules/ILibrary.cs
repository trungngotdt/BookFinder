using BookFinder.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookFinder.Modules
{
    public interface ILibrary
    {
        bool CanNext(HtmlDocument htmlDoc, int CurrentPage);
        Task<List<Book>> SearchBooks(string search,int page);
    }
}
