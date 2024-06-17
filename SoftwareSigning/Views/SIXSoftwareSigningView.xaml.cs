using SoftwareSigning.ViewModels;
using System.Windows.Controls;
using Unity;

namespace SoftwareSigning
{
    /// <summary>
    /// Interaction logic for SIXSoftwareSigningView.xaml
    /// </summary>

    public partial class SIXSoftwareSigningView : UserControl
    {
        //SIXSoftwareSigningViewModel vm = new SIXSoftwareSigningViewModel();
        public SIXSoftwareSigningView()
        {
            
            InitializeComponent();


        }
        public SIXSoftwareSigningView(IUnityContainer container) : this()
        {
            this.DataContext = container.Resolve< SIXSoftwareSigningViewModel>();
        }

        
    }
}
