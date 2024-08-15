using NavigationModule.ViewModels;
using System.Windows.Controls;

namespace NavigationModule.Views
{
    /// <summary>
    /// Interaction logic for NavigationSIXQAView.xaml
    /// </summary>
    public partial class PackageTreeView : UserControl
    {
       
        public PackageTreeView()
        {
            InitializeComponent();
        }
        public PackageTreeView(NavigationViewModel viewModel) : this()
        {
            this.DataContext = viewModel;
           

        }

       
    }
}
