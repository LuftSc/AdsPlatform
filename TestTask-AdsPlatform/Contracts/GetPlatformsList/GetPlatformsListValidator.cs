using FluentValidation;

namespace TestTask_AdsPlatform.Contracts.GetPlatformsList
{
    public class GetPlatformsListValidator : AbstractValidator<GetPlatformsListRequest>
    {
        public GetPlatformsListValidator()
        {
            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Локация для поиска должна быть указана")
                .Must(IsValidLocationPath).WithMessage("Некорректный формат локации. Примеры допустимых значений:" +
                    "1) /ru 2) /ru/svrd 3) /ru/svrd/pervik");
        }
        private bool IsValidLocationPath(string location)
        {
            return !string.IsNullOrWhiteSpace(location) && location.StartsWith('/');
        }
    }
}
