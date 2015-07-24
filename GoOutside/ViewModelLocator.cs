using Autofac;
using GoOutside.Events;
using GoOutside.Scheduling;
using GoOutside.ViewModels;

namespace GoOutside
{
    public class ViewModelLocator
    {
        private readonly IContainer _Container;

        public ViewModelLocator()
        {
            var containerBuilder = new ContainerBuilder();

#if DEBUG
            var period = new Period(2000);
#else
            var period = new Period(Settings.Default.PeriodBetweenBreaks.TotalMilliseconds);
#endif
            containerBuilder.RegisterInstance(period).As<IPeriod>();

            containerBuilder.RegisterType<SystemEventsWrapper>().As<ISystemEvents>();
            containerBuilder.RegisterType<SessionTimer>().As<ISessionTimer>().SingleInstance();

            containerBuilder.RegisterType<NotifyIconViewModel>();
            containerBuilder.RegisterType<PopUpViewModel>().SingleInstance();

            _Container = containerBuilder.Build();
        }

        public NotifyIconViewModel NotifyIconViewModel
        {
            get { return _Container.Resolve<NotifyIconViewModel>(); }
        }

        public PopUpViewModel PopUpViewModel
        {
            get { return _Container.Resolve<PopUpViewModel>(); }
        }
    }
}