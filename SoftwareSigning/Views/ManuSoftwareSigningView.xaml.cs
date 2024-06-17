using SoftwareSigning.ViewModels;
using System.ComponentModel;
using System.Windows.Controls;
using Unity;

namespace SoftwareSigning
{
    /// <summary>
    /// Interaction logic for ManuSoftwareSigning.xaml
    /// </summary>
    public partial class ManuSoftwareSigningView : UserControl
    {
        public ManuSoftwareSigningView()
        {
            InitializeComponent();
        }
        public ManuSoftwareSigningView(IUnityContainer container) : this()
        {
            this.DataContext = container.Resolve<SIXSoftwareSigningViewModel>();
        }
    }
}
