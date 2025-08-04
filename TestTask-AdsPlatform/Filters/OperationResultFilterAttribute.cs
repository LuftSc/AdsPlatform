using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TestTask_AdsPlatform.Filters
{
    public class OperationResultFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync
            (ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var executedContext = await next();

            if (executedContext.Result is ObjectResult objectResult)
            {
                if (objectResult.Value is Result.IResult operationResult)
                {
                    // Результат с ошибками
                    if (!operationResult.IsSuccessfull)
                    {
                        executedContext.Result = new BadRequestObjectResult
                            (operationResult.Error);

                        return;
                    }
                    // Успех - надо вернуть какое-то значение
                    if (operationResult.GetType().IsGenericType)
                    {
                        var valueType = operationResult
                            .GetType()
                            .GetProperty("Value")?
                            .GetValue(operationResult);

                        executedContext.Result = new OkObjectResult
                            (valueType);
                    }
                    // Просто 200 OK без значения
                    else
                    {
                        executedContext.Result = new OkResult();
                    }
                }
            }
        }
    }
}
