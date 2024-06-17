using SigningKeyManagment.ViewModels;
using System.Windows.Controls;

namespace SigningKeyManagment.Views
{
    /// <summary>
    /// Interaction logic for QAKeyGenerationToolbarView.xaml
    /// </summary>
    public partial class QAKeyGenerationToolbarView : UserControl
    {
        public QAKeyGenerationToolbarView()
        {
            InitializeComponent();
        }
        public QAKeyGenerationToolbarView(KeyGenerationToolbarViewModel viewmodel) : this()
        {
            this.DataContext = viewmodel;
        }
    }
}
