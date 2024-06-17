using SigningKeyManagment.ViewModels;
using System.Windows.Controls;

namespace SigningKeyManagment.Views
{
    /// <summary>
    /// Interaction logic for KeyGenerationView.xaml
    /// </summary>
    public partial class KeyGenerationView : UserControl
    {
        public KeyGenerationView()
        {
            InitializeComponent();
        }
        public KeyGenerationView(KeyGenerationViewModel viewModel) : this()
        {
            this.DataContext = viewModel;
        }

        
        
    }
}
