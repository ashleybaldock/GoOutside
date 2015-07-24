using Autofac;
using GoOutside.Events;
using GoOutside.Scheduling;

namespace GoOutside
{
    public class ViewModelLocator
    {
        private readonly IContainer _Container;

        public ViewModelLocator()
        {
            var containerBuilder = new ContainerBuilder();

//            var period = new Period(Settings.Default.PeriodBetweenBreaks.TotalMilliseconds);
            var period = new Period(2000);
            containerBuilder.RegisterInstance(period).As<IPeriod>();

            containerBuilder.RegisterType<SystemEventsWrapper>().As<ISystemEvents>();
            containerBuilder.RegisterType<SessionTimer>().As<ISessionTimer>().SingleInstance();

            containerBuilder.RegisterType<NotifyIconViewModel>();
            containerBuilder.RegisterType<PopUpViewModel>();

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