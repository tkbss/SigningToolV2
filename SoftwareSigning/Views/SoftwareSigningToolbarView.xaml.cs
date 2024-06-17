using SoftwareSigning.ViewModels;
using System.Windows.Controls;

namespace SoftwareSigning
{
    /// <summary>
    /// Interaction logic for SoftwareSigningToolbar.xaml
    /// </summary>
    public partial class SoftwareSigningToolbarView : UserControl
    {
        public SoftwareSigningToolbarView()
        {
            InitializeComponent();
        }
        public SoftwareSigningToolbarView(SoftwareSigningToolbarViewModel view) : this()
        {
            this.DataContext = view;
        }
    }
}
