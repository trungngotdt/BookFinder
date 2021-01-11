using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BookFinder.Models;

namespace BookFinder.Modules.Genesis
{
    public class LibraryGenesisService:ILibrary
    {
        public LibraryGenesisService()
        {

        }

        public async Task<Dictionary<string, Book>> SearchBooks(string search)
        {
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            using (var client = new HttpClient(handler))
            {
                try
                {
                    var list = new Dictionary<string, Book>();


                    HttpResponseMessage response = await client.GetAsync(EndPoint.urlGenesisSearch + search);
                    var html = await response.Content.ReadAsStringAsync();
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);
                    var tableBooks = htmlDoc.DocumentNode.Descendants("table").FirstOrDefault(node => node.GetAttributeValue("class", "").Equals("c"));
                    var books = tableBooks.Descendants("tr").Skip(1).ToArray();
                   
                    var length = books.Length;
                    for (int i = 0; i < length; i++)
                    {
                        var node = books[i].Descendants("td").ToArray();
                        var idBook = node[0].InnerText;
                        var nodeBook = htmlDoc.GetElementbyId(idBook);
                        var link = nodeBook.GetAttributeValue("href", "");
                        var title = WebUtility.HtmlDecode(nodeBook.InnerText);
                        list.Add(idBook, new Book(idBook,title, link));
                        
                    }
                    //.ElementAt(0).InnerText;
                    return list;
                }
                catch (Exception ex)
                {

                    throw;
                }

            }
        }
    }
}
