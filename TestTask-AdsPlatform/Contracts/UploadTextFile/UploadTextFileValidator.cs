using FluentValidation;
using System.Text.RegularExpressions;

namespace TestTask_AdsPlatform.Contracts.UploadTextFile
{
    public class UploadTextFileValidator : AbstractValidator<UploadTextFileRequest>
    {
        public UploadTextFileValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage("Файл не загружен. Загрузите файл формата .txt")
                .Must(f => f.Length > 0).WithMessage("Файл пустой")
                .Must(f => Path.GetExtension(f.FileName).ToLowerInvariant() == ".txt")
                    .WithMessage("Поддерживается загрузка только .txt файлов")
                .Must(f => f.Length <= 10 * 1024 * 1024)
                    .WithMessage("Максимальный размер файла - 10 МБ")
                .Must(ValidateTextContentFromFile)
                    .WithMessage("Контент внутри файла невалидный. Критерии для успешной проверки файла:" +
                        " 1) Название компании отделяется от локаций символом [:], без пробелов" +
                        " 2) Локации начинаются с символа [/] и перечисляются через запятую" +
                        " 3) Название новой компании с локациями начинается с новой строки");
        }

        private static readonly Regex LinePattern = new Regex(
            @"^[^:]+:(?:\/[^,\r\n]+)(?:,\/[^,\r\n]+)*$",
            RegexOptions.Compiled
        );
        private bool ValidateTextContentFromFile(IFormFile file)
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var line = string.Empty;

            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (!LinePattern.IsMatch(line))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
