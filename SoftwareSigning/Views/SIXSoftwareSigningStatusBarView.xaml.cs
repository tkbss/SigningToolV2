using SoftwareSigning.ViewModels;
using System.Windows.Controls;

namespace SoftwareSigning.Views
{
    /// <summary>
    /// Interaction logic for SIXSoftwareSigningStatusBarView.xaml
    /// </summary>
    public partial class SIXSoftwareSigningStatusBarView : UserControl
    {
        public SIXSoftwareSigningStatusBarView()
        {
            InitializeComponent();
            
        }
        public SIXSoftwareSigningStatusBarView(SIXSoftwareSigningStatusBarViewModel viewModel) : this()
        {
            this.DataContext = viewModel;
        }
    }
}
