using System;
using System.Windows;
using System.Windows.Threading;

namespace GoOutside.ViewModels
{
    public class Dispatcher : IDispatcher
    {
        public void BeginInvoke(DispatcherPriority priority, Delegate action)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, action);
        }
    }
}