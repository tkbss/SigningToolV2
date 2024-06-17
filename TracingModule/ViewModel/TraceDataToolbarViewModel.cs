using Prism.Commands;
using Prism.Mvvm;
using Unity;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using System;

namespace TracingModule.ViewModel
{
    public class TraceDataToolbarViewModel : BindableBase, INavigationAware
    {
        public ICommand SelectCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand SelectedDateChangedCommand { get; private set; }
        ObservableCollection<string> names;
        private bool StartDateSet=false;
        public ObservableCollection<string> Names
        {
            get { return names; }
            set
            {
                SetProperty(ref names, value);
            }

        }
        ObservableCollection<string> operations;
        public ObservableCollection<string> Operations
        {
            get { return operations; }
            set
            {
                SetProperty(ref operations, value);
            }

        }
        DateTime start_date;
        public DateTime StartDate
        {
            get { return start_date; }
            set
            {
                SetProperty(ref start_date, value);
            }

        }
        DateTime end_date;
        public DateTime EndDate
        {
            get { return end_date; }
            set
            {
                SetProperty(ref end_date, value);
            }

        }
        
        string selected_tr;
        public string SelectedTimeRange
        {
            get { return selected_tr; }
            set
            {
                SetProperty(ref selected_tr, value);
            }

        }
        string session_id;
        public string SessionId
        {
            get { return session_id; }
            set
            {
                SetProperty(ref session_id, value);
            }

        }
        string selected_name;
        public string SelectedName
        {
            get { return selected_name; }
            set
            {
                SetProperty(ref selected_name, value);
            }

        }
        string selected_operation;
        public string SelectedOperation
        {
            get { return selected_operation; }
            set
            {
                SetProperty(ref selected_operation, value);
            }

        }
        IUnityContainer _container;
        public TraceDataToolbarViewModel(IUnityContainer container)
        {
            _container = container;
            SelectCommand = new DelegateCommand(this.OnSelect);
            ClearCommand = new DelegateCommand(this.OnClear);
            SelectedDateChangedCommand = new DelegateCommand(this.OnSelectedDateChanged);

        }
        private void OnSelectedDateChanged()
        {
            string format = "MMM d yy";
            StartDateSet = true;
            string s=start_date.ToString(format);
            string e = end_date.ToString(format);
            SelectedTimeRange = s + "-" + e;
        }
        private void OnSelect()
        {
            //int res = EndDate.CompareTo(StartDate);
            //if (EndDate.CompareTo(StartDate)<0)
            //    EndDate = StartDate+new TimeSpan(1,0,0,0);
            //if (EndDate.CompareTo(StartDate)==0)
            //    EndDate = StartDate + new TimeSpan(1, 0, 0, 0);
            //TraceDataViewModel view= _container.Resolve<TraceDataViewModel>();
            //string[] fln;
            //string sn = SelectedName;
            //if(string.IsNullOrEmpty(sn)==false)
            //    fln=sn.Split(new char[] { ' ' });
            //else
            //{
            //    fln = new string[2];
            //    fln[0] = string.Empty;
            //    fln[1] = string.Empty;
            //}
            //DataTable dt=TraceDataTable.SelectData(fln[0], fln[1], SelectedOperation, SessionId, StartDateSet,StartDate,EndDate);            
            //DataTable _dt = new DataTable();
            //view.Item = null;
            //view.TraceDataView = _dt;
            //view.TraceDataView = dt;
        }
        private void OnClear()
        {
            TraceDataViewModel view = _container.Resolve<TraceDataViewModel>();
            OnNavigatedTo(null);
            view.OnNavigatedTo(null);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
           
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //SessionId = string.Empty;
            //SelectedTimeRange = string.Empty;
            //StartDate = DateTime.Now;
            //EndDate = DateTime.Now;
            //StartDateSet = false;          
            //DataTable dt_names= TraceDataTable.GetNames();
            //Names = new ObservableCollection<string>();
            //Names.Add(string.Empty);
            //foreach (DataRow r in dt_names.Rows)
            //{
            //    string pn=(string)r["Prename"];
            //    string n= (string)r["Name"];
            //    string fn = pn + " " + n;
            //    Names.Add(fn);
            //}
            //DataTable dt_op = TraceDataTable.GetOperations();
            //Operations = new ObservableCollection<string>();
            //Operations.Add(string.Empty);
            //foreach (DataRow r in dt_op.Rows)
            //{
            //    string op = (string)r["Operation"];                
            //    Operations.Add(op);
            //}

        }
    }
}
