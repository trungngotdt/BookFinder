using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BookFinder.Views.BookViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GutenbergPage : ContentPage
    {
        public GutenbergPage()
        {
            InitializeComponent();
        }
    }
}