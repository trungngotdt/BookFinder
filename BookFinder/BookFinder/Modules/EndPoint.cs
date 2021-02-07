using System;
using System.Collections.Generic;
using System.Text;

namespace BookFinder.Modules
{
    public class EndPoint
    {
        public const string urlGenesis= "http://libgen.rs";
        public const string urlGenesisSearch = urlGenesis + "/search.php?req=$search&view=detailed&sortmode=ASC&page=$page";

        public const string urlGutenberg = "http://www.gutenberg.org";
        public const string urlGutenbergDex = "https://gutendex.com";
        public const string urlGutenbergSearch = urlGutenberg + "/ebooks/search/?query=$search&submit_search=Go%21&sort_order=title&start_index=$page";
    }
}
