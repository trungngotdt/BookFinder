using BookFinder.ViewModels;
using BookFinder.Views;
using BookFinder.Views.BookViews;
using Prism;
using Prism.Ioc;
using Prism.Plugin.Popups;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace BookFinder
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            //containerRegistry.RegisterPopupNavigationService();
          

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<MultiPickerPage, MultiPickerViewModel>("PopupMultiPicker");
            containerRegistry.RegisterForNavigation<GutenbergPage, BookPageViewModel>();
            containerRegistry.RegisterForNavigation<GenesisPage, BookPageViewModel>();
        }
    }
}
