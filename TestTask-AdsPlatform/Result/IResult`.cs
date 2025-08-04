namespace TestTask_AdsPlatform.Result
{
    public interface IResult<TValue> : IResult
    {
        TValue Value { get; }
    }
}
