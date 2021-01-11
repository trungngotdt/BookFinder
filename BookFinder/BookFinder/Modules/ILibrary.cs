using BookFinder.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookFinder.Modules
{
    public interface ILibrary
    {
        Task<Dictionary<string, Book>> SearchBooks(string search);
    }
}
