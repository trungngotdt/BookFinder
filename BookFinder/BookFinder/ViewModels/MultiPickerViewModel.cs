using BookFinder.Models;
using BookFinder.Modules;
using BookFinder.ViewModels;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BookFinder.ViewModels
{
    public class MultiPickerViewModel: ViewModelBase
    {
        private List<Model> listModel;
        private readonly DelegateCommand commandClosePopup;
        private readonly DelegateCommand commandCancelPopup;
        public MultiPickerViewModel(INavigationService navigationService) : base(navigationService)
        {
            listModel = new List<Model>();
            
        }
        private void OnLoad()
        {
            var libraryNames = typeof(LibraryName).GetEnumValues();
            foreach (var item in libraryNames)
            {

                ListModel.Add(new Model() { Text = Enum.GetName(typeof(LibraryName), item) });
            }
        }
        public List<Model> ListModel { get => listModel; set { listModel = value;RaisePropertyChanged("ListModel"); } }
        public DelegateCommand CommandClosePopup { get => commandClosePopup??new DelegateCommand(async()=> {await ClosePopup(); });  }
        public DelegateCommand CommandCancelPopup { get => commandCancelPopup ?? new DelegateCommand(async () => { await CancelPopup(); }); }

        public async Task CancelPopup()
        {
            await NavigationService.GoBackAsync();
        }
        public async Task ClosePopup()
        {
            var result = ListModel.Where(w => w.IsChecked == true).Select(x => x.Text).ToArray();
            await NavigationService.GoBackAsync();
            MessagingCenter.Send<object, string[]>(this, "MultiPicker", result);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters["selectedSource"] != null)
            {
                var listStr = parameters["selectedSource"] as string[];
                var length = listStr.Length;
                for (int i = 0; i < length; i++)
                {
                    var model= ListModel.FirstOrDefault(x => x.Text.Equals(listStr[i].Trim()));
                    if (model != null)
                    {
                        model.IsChecked = true;
                    }
                }
            }
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            OnLoad();
        }
    }
}
