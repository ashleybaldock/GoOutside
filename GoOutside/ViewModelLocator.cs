using Autofac;
using GoOutside.ViewModels;

namespace GoOutside
{
    public class ViewModelLocator
    {
        private readonly IContainer _Container;

        public ViewModelLocator()
        {
            _Container = ContainerFactory.BuildContainer();
        }

        public NotifyIconViewModel NotifyIconViewModel
        {
            get { return _Container.Resolve<NotifyIconViewModel>(); }
        }

        public PopUpViewModel PopUpViewModel
        {
            get { return _Container.Resolve<PopUpViewModel>(); }
        }

        public PomoViewModel PomoViewModel
        {
            get { return _Container.Resolve<PomoViewModel>(); }
        }
    }
}