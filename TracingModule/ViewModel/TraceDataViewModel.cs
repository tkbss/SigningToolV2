using Prism.Commands;
using Prism.Mvvm;
using Unity;
using Prism.Regions;
using System.Data;
using System.Windows.Input;
using System;
using TracingModule.View;

namespace TracingModule.ViewModel
{
    public class TraceDataViewModel : BindableBase, INavigationAware
    {
        DataTable trace_data;
        
        IUnityContainer _container;
        DataRowView selected_item;
        int trace_data_index;
        public ICommand SelectedItemChangedCommand { get; private set; }
        public DataRowView Item
        {
            get
            {
                return selected_item;
            }
            set
            {
                SetProperty(ref selected_item, value);
            }
        }
        SignerDataModel signer_data;
        public SignerDataModel SignerData
        {
            get
            {
                return signer_data;
            }
            set
            {
                SetProperty(ref signer_data, value);
            }
        }
        PackageDataModel package_data;
        public PackageDataModel PackageData
        {
            get
            {
                return package_data;
            }
            set
            {
                SetProperty(ref package_data, value);
            }
        }
        DataTable trace_data_view;
        public DataTable TraceDataView
        {
            get
            {
                return trace_data_view;
            }
            set
            {
                //trace_data_view = value;
                SetProperty(ref trace_data_view, value);
            }
        }
        public TraceDataViewModel(IUnityContainer container)
        {
            _container = container;
            //trace_data = TraceDataTable.GetTable(TraceDataTable.DT_TRACE);
            //trace_data_index = TraceDataTable.GetIndex(trace_data);
            //TraceDataView = trace_data;//trace_data.DefaultView;
            SelectedItemChangedCommand = new DelegateCommand(this.OnSelectedItemChanged);
            PackageData = new PackageDataModel();
            SignerData = new SignerDataModel();
        }
        public void OnSelectedItemChanged()
        {
            //string s=(string)Item.Row[1];
            //if (Item == null)
            //    return;
            //SignerData.SetData(Item);
            //string em = string.Empty;
            //if (DBNull.Value.Equals(Item.Row["ErrorMsg"]) == false)
            //    em = (string)Item.Row["ErrorMsg"];
            //if (DBNull.Value.Equals(Item.Row["Data_Ref"]) == false)
            //{
            //    //not null
            //    int PackageIndex = (int)Item.Row["Data_Ref"];
            //    TracePackageData pd = TraceDataTable.ReadPackage(PackageIndex);
            //    PackageData.SetData(pd,em);
            //}
            //else
            //    PackageData.ClearData();

            
        }
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
        private static readonly DataTable _dt = new DataTable();
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Item = null;
            //trace_data = TraceDataTable.GetTable(TraceDataTable.DT_TRACE);            
            //TraceDataView = _dt;
            //TraceDataView = trace_data;

        }
    }
}
