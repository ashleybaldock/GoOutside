namespace GoOutside.Scheduling
{
    public interface IPeriodFactory
    {
        IPeriod PeriodBetweenBreaks();
        IPeriod PostponeBreakPeriod();
    }
}