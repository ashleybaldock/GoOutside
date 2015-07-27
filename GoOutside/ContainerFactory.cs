using Autofac;
using GoOutside.Events;
using GoOutside.Properties;
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
            var periodFactory = new PeriodFactory(2000, 5000);
            var period = new Period(2000);
#else
            var periodFactory = new PeriodFactory(Settings.Default.PeriodBetweenBreaks.TotalMilliseconds,
                                                  Settings.Default.PeriodAfterBreakDelay.TotalMilliseconds);
            var period = new Period(Settings.Default.PeriodBetweenBreaks.TotalMilliseconds);
#endif
            containerBuilder.RegisterInstance(periodFactory).As<IPeriodFactory>();

            containerBuilder.RegisterType<SystemEventsWrapper>().As<ISystemEvents>();
            containerBuilder.RegisterType<Dispatcher>().As<IDispatcher>();
            containerBuilder.RegisterType<SessionTimer>().As<ISessionTimer>().SingleInstance();

            containerBuilder.RegisterType<NotifyIconViewModel>();
            containerBuilder.RegisterType<PopUpViewModel>().SingleInstance();
            containerBuilder.RegisterType<PomoViewModel>().SingleInstance();

            return containerBuilder.Build();
        }
    }
}