using Microsoft.AspNetCore.Mvc;

namespace TestTask_AdsPlatform.Result
{
    public class Result : IResult
    {
        public bool IsSuccessfull => Error == null;
        public ErrorResponse? Error { get; private protected set; } = null;

        private static readonly Result _success = new();
        public static Result Success() => _success;
        public static Result Failure(ErrorResponse error) => new Result() { Error = error };

        public static implicit operator ActionResult(Result result)
        {
            if (!result.IsSuccessfull)
                return new BadRequestObjectResult(result.Error);

            return new OkResult();
        }
    }
}
