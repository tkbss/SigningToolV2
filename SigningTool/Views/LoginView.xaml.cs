using Unity;
using SigningTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SigningTool.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    
    public partial class LoginView : UserControl
    {
        LoginViewModel viewM;
        public void SetPwd(string pwd)
        {
            PwdBox.Password = pwd;
        }
        public LoginView()
        {
            InitializeComponent();
        }
        public LoginView(LoginViewModel viewModel, IUnityContainer container) : this()
        {
            this.DataContext = viewModel;
            viewM = viewModel;
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            viewM.StorePassword = viewM.Password;//PwdBox.Password;
        }

        
    }
}
