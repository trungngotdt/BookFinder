﻿using HtmlAgilityPack;
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
        public bool CanNext(HtmlDocument htmlDoc, int CurrentPage)
        {
            var pageInfo = htmlDoc.DocumentNode.Descendants("table").ElementAt(1).Descendants("font").ElementAt(0);
            var datas = pageInfo.InnerText.Split(' ');
            var totalItem = int.Parse(datas[0]);

            if (totalItem - CurrentPage * 25 > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<Dictionary<string, string>> GetDownloadLinkAsync(string id)
        {
            throw new NotImplementedException();
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

                    var url = EndPoint.urlGenesisSearch.Replace("$search", search).Replace("$page", page.ToString());
                    HttpResponseMessage response = await client.GetAsync(url);
                    var html = await response.Content.ReadAsStringAsync();
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);
                    if (CanNext(htmlDoc, page))
                    {
                        var books = htmlDoc.DocumentNode.Descendants("table").Where(node => node.GetAttributeValue("rules", "").Equals("cols")).ToArray();
                        var length = books.Length;
                        for (int i = 0; i < length; i += 2)
                        {
                            string image = "";
                            string link = "";
                            string idBook = "";
                            string title = "";
                            string author = "";
                            var node = books[i];
                            var titleNode = node.Descendants("td").FirstOrDefault(x => x.GetAttributeValue("colspan", "").Equals("2"));
                            if (!(titleNode is null))
                            {
                                var archor = titleNode.Descendants("a").ElementAt(0);
                                title = WebUtility.HtmlDecode(archor.InnerText);
                                link = WebUtility.HtmlDecode(archor.GetAttributeValue("href", "").Remove(0, 2));
                                
                            }
                            var idBookNode = node.Descendants("tr").ElementAt(7);
                            if(!(idBookNode is null))
                            {
                                idBook =WebUtility.HtmlDecode( idBookNode.Descendants("td").ElementAt(3).InnerText);
                            }
                            var imageNode = node.Descendants("img").ElementAt(0);
                            if (!(imageNode is null))
                            {
                                image = WebUtility.HtmlDecode(imageNode.GetAttributeValue("src", ""));
                            }
                            var authorNode = node.Descendants("td").FirstOrDefault(x => x.GetAttributeValue("colspan", "").Equals("3"));
                            if (!(authorNode is null))
                            {
                                author = WebUtility.HtmlDecode(authorNode.Descendants("a").ElementAt(0).InnerText);
                            }
                            list.Add(new Book(idBook, title, link, image, LibraryName.Genesis, author));
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
