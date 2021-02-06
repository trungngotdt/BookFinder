using BookFinder.Modules.Genesis;
using BookFinder.Modules.Gutenberg;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookFinder.Modules
{
    public class LibraryRepository
    {
        private static LibraryRepository instance = null;
        private static readonly object padlock = new object();
        private LibraryGenesisService genesisService=new LibraryGenesisService();
        private LibraryGutenbergService gutenbergService = new LibraryGutenbergService();
        private LibraryRepository()
        {

        }

        public static LibraryRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new LibraryRepository();
                        }
                    }
                }
                return instance;
            }
        }

        public ILibrary GetLibrary(LibraryName library)
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

    }
}
