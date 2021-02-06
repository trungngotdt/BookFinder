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
    public class LibraryGenesisService : ILibrary
    {
        public LibraryGenesisService()
        {

        }
        public  bool CanNext(HtmlDocument htmlDoc, int CurrentPage)
        {
            var startPage = CurrentPage - 3 > 1 ? CurrentPage - 3 : 1;
            var pageInfo = htmlDoc.DocumentNode.Descendants("table").ElementAt(1).Descendants("font").ElementAt(0);
            var datas = pageInfo.InnerText.Split(' ');
            var length = datas.Length;
            var totalItem = int.Parse(datas[0]);

            if (totalItem - CurrentPage * 25 > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<List<Book>> SearchBooks(string search,int page)
        {
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            using (var client = new HttpClient(handler))
            {
                try
                {
                    var list = new List< Book>();

                    var url = EndPoint.urlGenesisSearch.Replace("$search", search).Replace("$page", page.ToString());
                    HttpResponseMessage response = await client.GetAsync(url);
                    var html = await response.Content.ReadAsStringAsync();
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);
                    if (CanNext(htmlDoc,page))
                    {
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
                            list.Add(new Book(idBook, title, link));
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
