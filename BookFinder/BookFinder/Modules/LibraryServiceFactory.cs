using BookFinder.Modules.Genesis;
using BookFinder.Modules.Gutenberg;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookFinder.Modules
{
    public class LibraryServiceFactory
    {
        private static LibraryServiceFactory instance = null;
        private static readonly object padlock = new object();
        private readonly LibraryGenesisService genesisService=new LibraryGenesisService();
        private readonly LibraryGutenbergService gutenbergService = new LibraryGutenbergService();
        private LibraryServiceFactory()
        {

        }

        public static LibraryServiceFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new LibraryServiceFactory();
                        }
                    }
                }
                return instance;
            }
        }

        public ILibraryService GetLibrary(LibraryName library)
        {
            switch (library)
            {
                case LibraryName.Genesis:
                    return genesisService;
                case LibraryName.ZLibrary:
                    return null;
                case LibraryName.Gutenberg:
                    return gutenbergService;
                default:
                    throw new NotImplementedException();
            }
        }

    }

    public enum LibraryName
    {
       // All,
        Genesis,
        ZLibrary,
        Gutenberg,
        //https://www.allabout-engineering.com/
        //https://www.epub.vn/
    }
}
