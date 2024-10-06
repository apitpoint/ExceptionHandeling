using ExceptionHandeling.Models;
using Logger;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace ExceptionHandeling.Extension
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILoggerManager logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        string rootLogFolder = "Logs"; // Change this to your desired root log folder path
                        string monthFolderName = DateTime.Now.ToString("yyyy-MM"); // Get the current year and month as "yyyy-MM" format
                        string logFilePath = Path.Combine(rootLogFolder, monthFolderName, "error.log");

                        // Ensure the directory for the log file exists
                        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

                        using (StreamWriter writer = new StreamWriter(logFilePath, true))
                        {
                            writer.WriteLine(); // 
                            writer.WriteLine($"Date and Time: {DateTime.Now}");
                            writer.WriteLine($"Error: {contextFeature.Error}");
                            writer.WriteLine($"Path: {contextFeature.Path}");
                            writer.WriteLine($"Endpoint: {contextFeature.Endpoint}");
                            writer.WriteLine("******************************************************************"); // 
                        }

                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error",
                        }.ToString());
                    }
                });
            });
        }


        
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            // app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
