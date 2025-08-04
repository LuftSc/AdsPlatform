using Microsoft.AspNetCore.Mvc;

namespace TestTask_AdsPlatform.Result
{
    public class Result<TValue> : Result, IResult<TValue>
    {
        public TValue Value { get; }
        private Result(TValue value) => Value = value;
        private Result(ErrorResponse error) => Error = error;
        public static Result<TValue> Success(TValue value) => new(value);
        public static Result<TValue> Failure(ErrorResponse error) => new(error);

        public static implicit operator ActionResult<TValue>(Result<TValue> result)
        {
            if (!result.IsSuccessfull)
                return new BadRequestObjectResult(result.Error);

            return new OkObjectResult(result.Value);
        }
    }
}
