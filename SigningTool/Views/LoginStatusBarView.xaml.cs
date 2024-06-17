using SigningTool.ViewModels;
using System.Windows.Controls;

namespace SigningTool.Views
{
    /// <summary>
    /// Interaction logic for LoginStatusBarView.xaml
    /// </summary>
    public partial class LoginStatusBarView : UserControl
    {
        public LoginStatusBarView()
        {
            InitializeComponent();
        }
        public LoginStatusBarView(LoginStatusBarViewModel viewModel) : this()
        {
            this.DataContext = viewModel;
        }
    }
}
