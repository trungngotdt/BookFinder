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
using System.Collections;
using System.Collections.ObjectModel;
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
        private readonly DelegateCommand commandSearchTapped;
        private readonly DelegateCommand commandOpenMultiPicker;
        private string nameOfLibrary;
        private ILibrary genesisService;
        private ILibrary gutenbergService;
        private ObservableCollection<Book> books;
        private bool isWaiting;
        private int lengthBooks;
        private string searchStr;
        private readonly DelegateCommand<object> commandInfiniteLoad;
        private int currentPage;
        public MainPageViewModel(INavigationService navigationService, IPageDialogService _dialogService)
            : base(navigationService)
        {

            Title = "Main Page";
            this.dialogService = _dialogService;
            OnLoad();
        }
        public DelegateCommand CommandSearchTapped { get => commandSearchTapped ?? new DelegateCommand(async () => { await SearchLibrary(); }); }
        public ObservableCollection<Book> Books { get => books; set { books = value; RaisePropertyChanged("Books"); } }

        public DelegateCommand<Book> CommandItemTapped { get => commandItemTapped ?? new DelegateCommand<Book>(async (item) => { await ItemTapped(item); }); }
        public bool IsWaiting { get => isWaiting; set { isWaiting = value; RaisePropertyChanged("IsWaiting"); } }

        public DelegateCommand CommandOpenMultiPicker { get => commandOpenMultiPicker ?? new DelegateCommand(async () => { await OpenPopupMultiPicker(); }); }

        public string NameOfLibrary { get => nameOfLibrary; set { nameOfLibrary = value; RaisePropertyChanged("NameOfLibrary"); } }

        public DelegateCommand<object> CommandInfiniteLoad
        {
            get => commandInfiniteLoad ?? new DelegateCommand<object>(async (obj) =>
            {
                if ((int)obj == lengthBooks - 1)
                {
                    currentPage++;
                    IsWaiting = true;
                    var data = await CombineBookDataSync();
                    lengthBooks = data.Count();
                    IsWaiting = false;
                }
            });
        }
        public string SearchStr { get => searchStr; set => searchStr = value; }

        private async Task OpenPopupMultiPicker()
        {
            var param = new Prism.Navigation.NavigationParameters
            {
                { "selectedSource", NameOfLibrary.Split(',') }
            };
            await NavigationService.NavigateAsync("PopupMultiPicker", param);
            //await Navigation.PushPopupAsync(new MultiPicker());

        }

        private async Task ItemTapped(Book book)
        {
            try
            {
                await Browser.OpenAsync(book.GetLink(), BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                await dialogService.DisplayAlertAsync("Alert", ex.Message, "OK");

            }

        }
        private async Task<List<Book>[]> LoadBookDataSync()
        {
            var listTask = new List<Task<List<Book>>>();
            var sources = NameOfLibrary.Split(',').Select(x => x.Trim()).ToArray();
            foreach (var x in sources)
            {
                if (Enum.IsDefined(typeof(LibraryName), x))
                {
                    LibraryName source = (LibraryName)Enum.Parse(typeof(LibraryName), x, true);
                    var index = Array.IndexOf(sources, x);
                    var service = LibraryRepository.Instance.GetLibrary(source);
                    listTask.Add(service.SearchBooks(SearchStr, currentPage));
                }
            }
            List<Book>[] rawData = await Task.WhenAll(listTask);
            return rawData;
        }

        private async Task<IEnumerable<Book>> CombineBookDataSync()
        {
            List<Book> booksRawData = new List<Book>();
            var rawData = await LoadBookDataSync();
            foreach (var data in rawData)
            {
                booksRawData.AddRange(data);
            }
            booksRawData.Sort();
            var lengthRawData = booksRawData.Count;
            for (int i = 0; i < lengthRawData; i++)
            {
                var item = booksRawData[i];
                if (!(Books.Any(x => x.ID.Equals(item.ID))))
                {
                    Books.Add(item);
                }
            }
            return Books;
        }
        private async Task SearchLibrary()
        {
            try
            {
                if (String.IsNullOrWhiteSpace(SearchStr))
                {
                    throw new Exception("Empty search string");
                }
                IsWaiting = true;
                if (String.IsNullOrWhiteSpace(NameOfLibrary))
                {
                    throw new Exception("Empty search source");
                }
                currentPage = 1;
                var data = await CombineBookDataSync();
                lengthBooks = data.Count();
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
            currentPage = 1;
            books = new ObservableCollection<Book>();
            NameOfLibrary = "";
            SearchStr = "";
            gutenbergService = LibraryRepository.Instance.GetLibrary(LibraryName.Gutenberg);
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
