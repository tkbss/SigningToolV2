using NavigationModule.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace NavigationModule.Views
{
    /// <summary>
    /// Interaction logic for NavigationHSMTreeView.xaml
    /// </summary>
    public partial class NavigationHSMTreeView : UserControl
    {
        public NavigationHSMTreeView()
        {
            InitializeComponent();
        }
        public NavigationHSMTreeView(NavigationViewModel viewModel) : this()
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

        private void Rectangle_CertReqDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var docPath = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (File.Exists(docPath[0]))
                {
                    NavigationViewModel vm = (NavigationViewModel)DataContext;
                    vm.KeyGenerationViewModel.ManuCertRequest = docPath[0];
                    vm.KeyGenerationViewModel.CertExportDir = System.IO.Path.GetDirectoryName(docPath[0]);
                }
            }
        }
        private void Rectangle_CertExportDir(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var docPath = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (Directory.Exists(docPath[0]))
                {
                    NavigationViewModel vm = (NavigationViewModel)DataContext;
                    vm.KeyGenerationViewModel.CertExportDir = docPath[0];
                }
            }
        }
        private void Rectangle_CertManuDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var docPath = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (File.Exists(docPath[0]))
                {
                    NavigationViewModel vm = (NavigationViewModel)DataContext;
                    vm.KeyGenerationViewModel.CertifiedManuDrop(docPath[0]);
                }
            }
        }
    }
}
