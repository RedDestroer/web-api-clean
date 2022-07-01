using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClean.Controllers.ActionResults
{
    public class RawJsonResult : IActionResult
    {
        public RawJsonResult(string content)
        {
            Content = content;
        }

        public string Content { get; }

        public Task ExecuteResultAsync(ActionContext context)
        {
            var result = new ContentResult
            {
                StatusCode = StatusCodes.Status200OK,
                ContentType = $"{MediaTypeNames.Application.Json}; charset={Encoding.UTF8.WebName}",
                Content = Content
            };

            return result.ExecuteResultAsync(context);
        }
    }
}
