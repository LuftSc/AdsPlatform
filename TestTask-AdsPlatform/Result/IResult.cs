using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestTask_AdsPlatform.Result
{
    public interface IResult
    {
        bool IsSuccessfull { get; }
        ErrorResponse? Error { get; }
    }
}
