using SigningKeyManagment.ViewModels;
using System.Windows.Controls;

namespace SigningKeyManagment.Views
{
    /// <summary>
    /// Interaction logic for KeyGenerationToolbarView.xaml
    /// </summary>
    public partial class ATMKeyGenerationToolbarView : UserControl
    {
        public ATMKeyGenerationToolbarView()
        {
            InitializeComponent();
        }
        public ATMKeyGenerationToolbarView(KeyGenerationToolbarViewModel viewmodel) : this()
        {
            this.DataContext = viewmodel;
        }
    }
}
