using System;
using System.Windows.Threading;

namespace GoOutside.ViewModels
{
    public interface IDispatcher
    {
        void BeginInvoke(DispatcherPriority priority, Delegate action);
    }
}