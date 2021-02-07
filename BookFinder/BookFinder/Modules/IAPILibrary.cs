using Newtonsoft.Json.Linq;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookFinder.Modules
{
    public interface IAPILibrary
    {
        [Get("/books/?ids={id}")]
        Task<JObject> GetGuntenbergBook([AliasAs("id")] string id);


    }
}
