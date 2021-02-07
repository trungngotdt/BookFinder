using BookFinder.Models;
using BookFinder.Modules;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BookFinder.ViewModels
{
    public class BookPageViewModel : ViewModelBase
    {
        private ILibrary genesisService;
        private ILibrary gutenbergService;
        private Book book;
        public BookPageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public Book Book { get => book; set => book = value; }
        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            gutenbergService = LibraryRepository.Instance.GetLibrary(LibraryName.Gutenberg);
            genesisService = LibraryRepository.Instance.GetLibrary(LibraryName.Genesis);

            MessagingCenter.Subscribe<object, Dictionary<string, string>>(this, "GetDownloadLink", (obj, result) =>
             {

             });
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters["book"] != null)
            {
                Book = parameters["book"] as Book;
                Task.Factory.StartNew(async () =>
                {
                    Dictionary<string, string> keyValues = new Dictionary<string, string>();
                    switch (book.Library)
                    {
                        case LibraryName.Genesis:
                            break;
                        case LibraryName.ZLibrary:
                            break;
                        case LibraryName.Gutenberg:
                            keyValues= await gutenbergService.GetDownloadLinkAsync(Book.ID);
                            break;
                        default:
                            break;
                    } 
                    MessagingCenter.Instance.Send<object, Dictionary<string, string>>(this, "GetDownloadLink", keyValues);
                });

            }
        }
    }
}
