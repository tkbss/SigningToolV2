using SoftwareSigning.ViewModels;
using System.Windows.Controls;

namespace SoftwareSigning.Views
{
    /// <summary>
    /// Interaction logic for ATMDeviceSoftwareSigningTollbarView.xaml
    /// </summary>
    public partial class ATMDeviceSoftwareSigningToolbarView : UserControl
    {
        public ATMDeviceSoftwareSigningToolbarView()
        {
            InitializeComponent();
        }
        public ATMDeviceSoftwareSigningToolbarView(SoftwareSigningToolbarViewModel view) : this()
        {
            this.DataContext = view;
        }
    }
}
