using BookFinder.Models;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookFinder.Modules.Gutenberg
{
    public class LibraryGutenbergService : ILibraryService
    {
        public bool CanNext(HtmlDocument htmlDoc, int CurrentPage)
        {
            var isNext = htmlDoc.DocumentNode.Descendants("li").Where(x => x.GetAttributeValue("class", "").Equals("booklink")).Count() > 0;
            return isNext;
        }

        public async Task<Dictionary<string, string>> GetDownloadLinkAsync(string id)
        {
            var dic = new Dictionary<string, string>();
            var api= RestService.For<IAPILibrary>(EndPoint.urlGutenbergDex);
            var obj =await api.GetGuntenbergBook(id);
            var results = (obj["results"] as JArray);
            if(results.Count > 0)
            {
                var book = results[0];
                var formats = book["formats"] as JObject;
                foreach (KeyValuePair<string, JToken> item in formats)
                {
                    dic.Add(item.Key, item.Value.ToString());
                }
            }
            return dic;
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
                    page = (page - 1) * 25;
                    var url = EndPoint.urlGutenbergSearch.Replace("$search", search).Replace("$page", page.ToString());
                    HttpResponseMessage response = await client.GetAsync(url);
                    var html = await response.Content.ReadAsStringAsync();
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);
                    if (CanNext(htmlDoc, page))
                    {

                        var books = htmlDoc.DocumentNode.Descendants("li").Where(x => x.GetAttributeValue("class", "").Equals("booklink")).ToArray();
                        var length = books.Length;
                        for (int i = 0; i < length; i++)
                        {
                            var node = books[i];
                            string image = "";
                            string link = "";
                            string idBook = "";
                            string title = "";
                            string author = "";
                            var img = node.Descendants("img").FirstOrDefault(x => x.GetAttributeValue("class", "").Equals("cover-thumb"));
                            if (!(img is null))
                            {
                                image = WebUtility.HtmlDecode(img.GetAttributeValue("src", ""));
                            }
                            var linkNode = node.Descendants("a").FirstOrDefault(x => x.GetAttributeValue("class", "").Equals("link"));
                            if (!(linkNode is null))
                            {
                                link = WebUtility.HtmlDecode(linkNode.GetAttributeValue("href", ""));
                            }
                            var titleNode = node.Descendants("span").FirstOrDefault(x => x.GetAttributeValue("class", "").Equals("title"));
                            if (!(titleNode is null))
                            {
                                title = WebUtility.HtmlDecode(titleNode.InnerText);
                            }
                            var authorNode = node.Descendants("span").FirstOrDefault(x => x.GetAttributeValue("class", "").Equals("subtitle"));
                            if(!(authorNode is null))
                            {
                                author = WebUtility.HtmlDecode(authorNode.InnerText);
                            }
                            idBook = link.Replace("/ebooks/", "");
                            list.Add(new Book(idBook, title, link, image, LibraryName.Gutenberg,author));
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
