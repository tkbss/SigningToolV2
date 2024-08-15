using NavigationModule.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace NavigationModule.Views
{
    /// <summary>
    /// Interaction logic for NavigationATMView.xaml
    /// </summary>
    public partial class NavigationATMView : UserControl
    {
        public NavigationATMView()
        {
            InitializeComponent();
        }
        public NavigationATMView(NavigationViewModel viewModel) : this()
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

        private void Rectangle_DropTest(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var docPath = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (Directory.Exists(docPath[0]))
                {
                    NavigationViewModel vm = (NavigationViewModel)DataContext;
                    vm.PackageDrop.ExportTest = docPath[0];
                    vm.PackageDrop.ExportFileName();
                }
            }
        }
        private void Rectangle_DropProd(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var docPath = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (Directory.Exists(docPath[0]))
                {
                    NavigationViewModel vm = (NavigationViewModel)DataContext;
                    vm.PackageDrop.ExportProd = docPath[0];
                    vm.PackageDrop.ExportFileName();
                }
            }
        }
    }
}
