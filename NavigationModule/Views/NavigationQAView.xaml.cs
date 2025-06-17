using NavigationModule.ViewModels;
using SoftwareSigning.Model;
using SoftwareSigning.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace NavigationModule.Views
{
    /// <summary>
    /// Interaction logic for NavigationQAView.xaml
    /// </summary>
    public partial class NavigationQAView : UserControl
    {
        public NavigationQAView()
        {
            InitializeComponent();
        }
        public NavigationQAView(NavigationViewModel viewModel) : this()
        {
            this.DataContext = viewModel;


        }
        private void Rectangle_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private async void Rectangle_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var docPath = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (File.Exists(docPath[0]))
                {
                    NavigationViewModel vm = (NavigationViewModel)DataContext;
                    await vm.PackageDrop.PackageDrop(docPath[0]);                    
                }
            }
        }
    }
    
}
