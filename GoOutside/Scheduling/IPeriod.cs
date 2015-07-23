namespace GoOutside.Scheduling
{
    public interface IPeriod
    {
        void Start();
        void Stop();
        double Interval { get; set; }
        event PeriodElapsedEventHandler Elapsed;
    }
}