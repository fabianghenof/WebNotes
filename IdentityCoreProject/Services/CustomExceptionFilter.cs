using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityCoreProject.Services
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly ILogger _logger;

        public CustomExceptionFilterAttribute(
            IHostingEnvironment hostingEnvironment,
            IModelMetadataProvider modelMetadataProvider,
            ILogger<CustomExceptionFilterAttribute> logger)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
        }

        public override void OnException(ExceptionContext context)
        {
            if (_hostingEnvironment.IsDevelopment())
            {
                FileStream fileStream = new FileStream(@"C:\Users\Fabi\Desktop\WebNotes-with-IdentityCore-master\IdentityCoreProject\wwwroot\lib\WebNotesErrors.txt",
                    FileMode.Append);

                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    var dateTime = new DateTime();
                    dateTime = DateTime.Now;
                    writer.WriteLine(@"Date: {0}/{1}/{2} Time: {3}:{4}:{5}",
                        dateTime.Day,
                        dateTime.Month,
                        dateTime.Year,
                        dateTime.Hour,
                        dateTime.Minute,
                        dateTime.Second);
                    writer.WriteLine(context.Exception);
                    writer.WriteLine(/*spatiu intre exceptii*/);
                }
                return;
            }
            //var result = new ViewResult { ViewName = "CustomError" };
            //result.ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState);
            //result.ViewData.Add("Exception", context.Exception);
            //// TODO: Pass additional detailed data via ViewData
            //context.Result = result;
        }
    }
}
