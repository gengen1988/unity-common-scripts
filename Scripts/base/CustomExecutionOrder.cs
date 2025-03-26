public struct CustomExecutionOrder
{
    public const int BeginningOfTheTime = int.MinValue;
    public const int ExtremeEarly = -300;
    public const int TooEarly = -200;
    public const int Early = -100;
    public const int Default = 0;
    public const int Late = 100;
    public const int TooLate = 200;
    public const int ExtremeLate = 300;
    public const int EndingOfTheTime = int.MaxValue;
}