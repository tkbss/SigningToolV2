using SigningKeyManagment.ViewModels;
using System.Windows.Controls;

namespace SigningKeyManagment.Views
{
    /// <summary>
    /// Interaction logic for HSMStatusView.xaml
    /// </summary>
    public partial class HSMStatusView : UserControl
    {
        public HSMStatusView()
        {
            InitializeComponent();
        }
        public HSMStatusView(HSMStatusViewModel viewModel) : this()
        {
            this.DataContext = viewModel;
        }
    }
}
