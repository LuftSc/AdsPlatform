using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TestTask_AdsPlatform.Filters
{
    public class ValidationAndExceptionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
                var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var validationResult = await validator.ValidateAsync(
                        new ValidationContext<object>(argument),
                        context.HttpContext.RequestAborted
                    );

                    if (!validationResult.IsValid)
                    {
                        context.Result = new BadRequestObjectResult(
                            validationResult.Errors.Select(e => e.ErrorMessage)
                        );
                        return;
                    }
                }
            }

            var executedContext = await next();

            if (executedContext.Exception != null)
            {
                executedContext.Result = new BadRequestObjectResult(executedContext.Exception.Message);
                executedContext.ExceptionHandled = true;
            }
        }
    }
}
