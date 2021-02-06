using BookFinder.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookFinder.Modules.Gutenberg
{
    public class LibraryGutenbergService : ILibrary
    {
        public bool CanNext(HtmlDocument htmlDoc, int CurrentPage)
        {
            var isNext = htmlDoc.DocumentNode.Descendants("ul").Where(x=>x.GetAttributeValue("class","").Equals("booklink")).Count() > 0;
            return isNext;
        }

        public async Task<List<Book>> SearchBooks(string search, int page)
        {
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            using (var client = new HttpClient(handler))
            {
                try
                {
                    var list = new List<Book>();

                    var url = EndPoint.urlGutenbergSearch.Replace("$search", search).Replace("$page", page.ToString());
                    HttpResponseMessage response = await client.GetAsync(url);
                    var html = await response.Content.ReadAsStringAsync();
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);
                    if (CanNext(htmlDoc, page))
                    {

                        var books = htmlDoc.DocumentNode.Descendants("ul").Where(x => x.GetAttributeValue("class", "").Equals("booklink")).ToArray();
                        var length = books.Length;
                        for (int i = 0; i < length; i++)
                        {
                            var node = books[i];
                            var image = WebUtility.HtmlDecode(node.Descendants("img").FirstOrDefault(x=>x.GetAttributeValue("class","").Equals("cover-thumb")).GetAttributeValue("src", ""));
                            var link = WebUtility.HtmlDecode(node.Descendants("a").FirstOrDefault(x => x.GetAttributeValue("class", "").Equals("link")).GetAttributeValue("href", ""));
                            var idBook = link.Replace("/ebooks/", "");
                            var title = WebUtility.HtmlDecode(node.Descendants("span").FirstOrDefault(x => x.GetAttributeValue("class", "").Equals("title")).InnerText);
                            list.Add(new Book(idBook, title, link, image, LibraryName.Gutenberg));
                        }
                        return list;
                    }
                    return new List<Book>();

                }
                catch (Exception ex)
                {

                    throw;
                }

            }
        }
    }
}
