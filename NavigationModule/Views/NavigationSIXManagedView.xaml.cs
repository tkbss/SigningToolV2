using NavigationModule.ViewModels;
using System.Windows.Controls;

namespace NavigationModule.Views
{
    /// <summary>
    /// Interaction logic for NavigationSIXManaged.xaml
    /// </summary>
    public partial class NavigationSIXManagedView : UserControl
    {
        public NavigationSIXManagedView()
        {
            InitializeComponent();
        }
        public NavigationSIXManagedView(NavigationViewModel viewModel) : this()
        {
            this.DataContext = viewModel;


        }
        object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                // Add any additional logic here
                
            }
        }
    }
}
