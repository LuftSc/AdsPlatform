
using FluentValidation;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TestTask_AdsPlatform.Abstractions;
using TestTask_AdsPlatform.Contracts.UploadTextFile;
using TestTask_AdsPlatform.Filters;
using TestTask_AdsPlatform.Services;
using TestTask_AdsPlatform.Swagger;

namespace TestTask_AdsPlatform
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidationAndExceptionFilter>();
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
            });

            builder.Services.AddValidatorsFromAssemblyContaining<UploadTextFileValidator>();

            builder.Services.AddSingleton<IAdsPlatformService, AdsPlatformService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }

}
