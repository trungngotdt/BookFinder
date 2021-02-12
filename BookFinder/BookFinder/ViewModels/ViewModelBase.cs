using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookFinder.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
    {
        protected IPageDialogService DialogService { get; private set; }
        protected INavigationService NavigationService { get; private set; }
        private bool isWaiting;

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public bool IsWaiting { get => isWaiting; set { isWaiting = value; RaisePropertyChanged("IsWaiting"); } }
        public ViewModelBase(INavigationService navigationService, IPageDialogService _dialogService)
        {
            DialogService = _dialogService;
            NavigationService = navigationService;
        }

        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }
    }
}
