using BookFinder.Models;
using BookFinder.Modules;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BookFinder.ViewModels
{
    public class BookPageViewModel : ViewModelBase
    {
        private ILibrary genesisService;
        private ILibrary gutenbergService;
        private Book book;
        private Dictionary<string, string> sourceLink;
        private readonly DelegateCommand<object> commandOpenItem;
        private readonly DelegateCommand commandOpenInBrowser;

        public BookPageViewModel(INavigationService navigationService, IPageDialogService _dialogService) : base(navigationService, _dialogService)
        {
            SourceLink = new Dictionary<string, string>();
        }

        public Book Book { get => book; set { book = value; RaisePropertyChanged("Book"); } }

        public DelegateCommand<object> CommandOpenItem =>
            commandOpenItem ?? new DelegateCommand<object>(async (item) => { await OpenItemAsync(item); });

        public Dictionary<string, string> SourceLink { get => sourceLink; set { sourceLink = value; RaisePropertyChanged("SourceLinkList"); } }
        public List<KeyValuePair<string, string>> SourceLinkList => SourceLink.ToList();

        public DelegateCommand CommandOpenInBrowser => commandOpenInBrowser ?? new DelegateCommand(async () => { await OpenInBrowserAsync(Book.GetLink()); });
        private async Task OpenInBrowserAsync(string link)
        {
            try
            {
                await Browser.OpenAsync(link, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                await DialogService.DisplayAlertAsync("Alert", ex.Message, "OK");

            }
        }
        private async Task OpenItemAsync(object item)
        {
            var link = item.ToString();
            if (link.StartsWith("http"))
            {
                await OpenInBrowserAsync(link);
            }
            else
            {
                await Clipboard.SetTextAsync(link);
                await DialogService.DisplayActionSheetAsync("Information", "Your torrent link is copied to clipboard", "OK");
            }

        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            gutenbergService = LibraryRepository.Instance.GetLibrary(LibraryName.Gutenberg);
            genesisService = LibraryRepository.Instance.GetLibrary(LibraryName.Genesis);

            MessagingCenter.Subscribe<object, Dictionary<string, string>>(this, "GetDownloadLink", (obj, result) =>
             {
                 IsWaiting = false;
                 SourceLink = result;
             });
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters["book"] != null)
            {
                IsWaiting = true;
                Book = parameters["book"] as Book;
                Title = book.Title;
                Task.Factory.StartNew(async () =>
                {
                    Dictionary<string, string> keyValues = new Dictionary<string, string>();
                    switch (book.Library)
                    {
                        case LibraryName.Genesis:
                            keyValues = await genesisService.GetDownloadLinkAsync(Book.MD5);
                            break;
                        case LibraryName.ZLibrary:
                            break;
                        case LibraryName.Gutenberg:
                            keyValues = await gutenbergService.GetDownloadLinkAsync(Book.ID);
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
