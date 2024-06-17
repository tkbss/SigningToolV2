using NavigationModule.ViewModels;
using System.Windows.Controls;

namespace NavigationModule.Views
{
    /// <summary>
    /// Interaction logic for NavigationSIXATMView.xaml
    /// </summary>
    public partial class NavigationMANUView : UserControl
    {
        public NavigationMANUView()
        {
            InitializeComponent();
        }
        public NavigationMANUView(NavigationViewModel viewModel) : this()
        {
            this.DataContext = viewModel;          

        }
    }
}
