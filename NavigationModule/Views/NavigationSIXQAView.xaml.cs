using NavigationModule.ViewModels;
using System.Windows.Controls;

namespace NavigationModule.Views
{
    /// <summary>
    /// Interaction logic for NavigationSIXQAView.xaml
    /// </summary>
    public partial class NavigationSIXQAView : UserControl
    {
       
        public NavigationSIXQAView()
        {
            InitializeComponent();
        }
        public NavigationSIXQAView(NavigationViewModel viewModel) : this()
        {
            this.DataContext = viewModel;
           

        }

       
    }
}
