using Unity;
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
using TracingModule.ViewModel;

namespace TracingModule.View
{
    /// <summary>
    /// Interaction logic for TraceDataToolbarView.xaml
    /// </summary>
    public partial class TraceDataToolbarView : UserControl
    {
        public TraceDataToolbarView()
        {
            InitializeComponent();
        }
        public TraceDataToolbarView(TraceDataToolbarViewModel viewModel, IUnityContainer container) : this()
        {
            this.DataContext = viewModel;
        }
    }
}
