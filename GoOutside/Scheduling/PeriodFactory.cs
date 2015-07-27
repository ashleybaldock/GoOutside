namespace GoOutside.Scheduling
{
    class PeriodFactory : IPeriodFactory
    {
        private readonly double _PeriodBetweenBreaks;
        private readonly double _PeriodAfterBreakDelay;

        public PeriodFactory(double periodBetweenBreaks, double periodAfterBreakDelay)
        {
            _PeriodBetweenBreaks = periodBetweenBreaks;
            _PeriodAfterBreakDelay = periodAfterBreakDelay;
        }

        public IPeriod PeriodBetweenBreaks()
        {
            return new Period(_PeriodBetweenBreaks);
        }

        public IPeriod PostponeBreakPeriod()
        {
            return new Period(_PeriodAfterBreakDelay);
        }
    }
}