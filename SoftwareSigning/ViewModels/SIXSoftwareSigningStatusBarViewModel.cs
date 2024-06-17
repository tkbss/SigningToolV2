using Prism.Mvvm;
using Prism.Regions;
using System.Windows;
using System.Windows.Media;

namespace SoftwareSigning.ViewModels
{
    public class SIXSoftwareSigningStatusBarViewModel :BindableBase,INavigationAware
    {
        ResourceDictionary _resourceDictionary;
        LinearGradientBrush stb_bckg;
        public LinearGradientBrush StatusBarBackground
        {
            get { return stb_bckg; }
            set
            {
                SetProperty(ref stb_bckg, value);
            }
        }
        string _operation;
        public string Operation
        {
            get { return _operation; }
            set
            {
                SetProperty(ref _operation, value);
            }
        }
        string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                SetProperty(ref _message, value);
            }
        }
        public SIXSoftwareSigningStatusBarViewModel()
        {
            _resourceDictionary = new ResourceDictionary()
            {
                Source = new Uri(@"/Dictionary.xaml", UriKind.Relative)
            };
        }
        public void Error(string operation, string message)
        {
            
            StatusBarBackground = _resourceDictionary["ErrorBrush"] as LinearGradientBrush;
            Operation = operation;
            Message = message;
        }
        public void Success(string operation,string message)
        {
            StatusBarBackground = _resourceDictionary["SuccessBrush"] as LinearGradientBrush;
            Operation = operation;
            Message = message;
        }
        public void Initialze()
        {
            StatusBarBackground = _resourceDictionary["InitBrush"] as LinearGradientBrush;
            Operation = string.Empty;
            Message = string.Empty;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Initialze();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
