using BookFinder.Models;
using BookFinder.Modules;
using BookFinder.Modules.Genesis;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Xaml;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BookFinder.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly DelegateCommand<Book> commandItemTapped;
        private IPageDialogService dialogService;
        private readonly DelegateCommand<string> commandSearchTapped;
        private readonly DelegateCommand commandOpenMultiPicker;
        private string nameOfLibrary;
        private ILibrary genesisService;
        private Dictionary<string, Book> books;
        private bool isWaiting;
        public MainPageViewModel(INavigationService navigationService, IPageDialogService _dialogService)
            : base(navigationService)
        {

            Title = "Main Page";
            this.dialogService = _dialogService;
            OnLoad();
        }
        public DelegateCommand<string> CommandSearchTapped { get => commandSearchTapped ?? new DelegateCommand<string>(async (item) => { await SearchLibrary(item); }); }
        public Dictionary<string, Book> Books { get => books; set { books = value; RaisePropertyChanged("Books"); } }

        public DelegateCommand<Book> CommandItemTapped { get => commandItemTapped ?? new DelegateCommand<Book>(async (item) => { await ItemTapped(item); }); }
        public bool IsWaiting { get => isWaiting; set { isWaiting = value; RaisePropertyChanged("IsWaiting"); } }

        public DelegateCommand CommandOpenMultiPicker { get => commandOpenMultiPicker ?? new DelegateCommand(async () => { await OpenPopupMultiPicker(); }); }

        public string NameOfLibrary { get => nameOfLibrary; set { nameOfLibrary = value;RaisePropertyChanged("NameOfLibrary"); } }

        private async Task OpenPopupMultiPicker()
        {
            var param = new Prism.Navigation.NavigationParameters
            {
                { "selectedSource", NameOfLibrary.Split(',') }
            };
            await NavigationService.NavigateAsync("PopupMultiPicker",param);
            //await Navigation.PushPopupAsync(new MultiPicker());

        }

        private async Task ItemTapped(Book book)
        {
            try
            {
                await Browser.OpenAsync(EndPoint.urlGenesis +"/"+ book.Link, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                await dialogService.DisplayAlertAsync("Alert", ex.Message, "OK");

            }

        }
        private async Task SearchLibrary(string search)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(search))
                {
                    throw new Exception("Empty search string");
                }
                IsWaiting = true;
                if (String.IsNullOrWhiteSpace(NameOfLibrary))
                {
                    throw new Exception("Empty search source");
                }
                var listDic = new List<Dictionary<string, Book>>();
                var s = listDic.Capacity;/*
                var sources = NameOfLibrary.Split(',').Select(x=>x.Trim()).ToArray();
                Parallel.ForEach(sources,async( x) => 
                {
                    if (Enum.IsDefined(typeof(LibraryName), x))
                    {
                        LibraryName source =(LibraryName)Enum.Parse(typeof(LibraryName), x, true);
                        var index= Array.IndexOf(sources, x);
                        var service = LibraryRepository.Instance.GetLibrary(source);
                        listDic[index] = await service.SearchBooks(search);
                    }
                });*/
                var service = new LibraryGenesisService();
                Books =await service.SearchBooks(search);

            }
            catch (Exception ex)
            {
                IsWaiting = false;
                await dialogService.DisplayAlertAsync("Alert", ex.Message, "OK");
            }
            finally
            {
                IsWaiting = false;
            }
        }
        public void OnLoad()
        {
            books = new Dictionary<string, Book>();
            NameOfLibrary = "";
            genesisService = LibraryRepository.Instance.GetLibrary(LibraryName.Genesis);
        }
        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            MessagingCenter.Subscribe<object, string[]>(this, "MultiPicker", (obj, result) =>
            {
                NameOfLibrary = String.Join(", ", result);
            });
        }
        public override void Destroy()
        {
            MessagingCenter.Unsubscribe<object, string>(this, "MultiPicker");
            base.Destroy();
        }
    }
}
