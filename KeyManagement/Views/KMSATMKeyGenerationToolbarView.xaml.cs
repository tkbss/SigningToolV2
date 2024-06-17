using SigningKeyManagment.ViewModels;
using System.Windows.Controls;

namespace SigningKeyManagment.Views
{
    /// <summary>
    /// Interaction logic for KMSATMKeyGenerationToolbarView.xaml
    /// </summary>
    public partial class KMSATMKeyGenerationToolbarView : UserControl
    {
        public KMSATMKeyGenerationToolbarView()
        {
            InitializeComponent();
        }
        public KMSATMKeyGenerationToolbarView(KMSKeyGenerationToolbarViewModel viewmodel) : this()
        {
            this.DataContext = viewmodel;
        }
    }
}
