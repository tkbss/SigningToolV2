using SigningKeyManagment.ViewModels;
using System.Windows.Controls;

namespace SigningKeyManagment.Views
{
    /// <summary>
    /// Interaction logic for MANUKeyGenerationToolbarView.xaml
    /// </summary>
    public partial class MANUKeyGenerationToolbarView : UserControl
    {
        public MANUKeyGenerationToolbarView()
        {
            InitializeComponent();
        }
        public MANUKeyGenerationToolbarView(KeyGenerationToolbarViewModel viewmodel) : this()
        {
            this.DataContext = viewmodel;
        }
    }
}
