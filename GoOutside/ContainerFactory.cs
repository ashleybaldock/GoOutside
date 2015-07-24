using Autofac;
using GoOutside.Events;
using GoOutside.Scheduling;
using GoOutside.Timers;
using GoOutside.ViewModels;

namespace GoOutside
{
    public static class ContainerFactory
    {
        public static IContainer BuildContainer()
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

            return containerBuilder.Build();
        }
    }
}