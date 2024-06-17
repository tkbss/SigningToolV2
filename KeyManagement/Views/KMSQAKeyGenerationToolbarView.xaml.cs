using SigningKeyManagment.ViewModels;
using System.Windows.Controls;

namespace SigningKeyManagment.Views
{
    /// <summary>
    /// Interaction logic for KMSQAKeyGenerationToolbarView.xaml
    /// </summary>
    public partial class KMSQAKeyGenerationToolbarView : UserControl
    {
        public KMSQAKeyGenerationToolbarView()
        {
            InitializeComponent();
        }
        public KMSQAKeyGenerationToolbarView(KMSKeyGenerationToolbarViewModel viewmodel) : this()
        {
            this.DataContext = viewmodel;
        }
    }
}
