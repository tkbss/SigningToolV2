using SigningTool.ViewModels;
using System.Windows.Controls;

namespace SigningTool.Views
{
    /// <summary>
    /// Interaction logic for LoginToolbarView.xaml
    /// </summary>
    public partial class LoginToolbarView : UserControl
    {
        public LoginToolbarView()
        {
            InitializeComponent();
        }
        public LoginToolbarView(LoginViewModel viewModel) : this()
        {
            this.DataContext = viewModel;

        }
    }
}
