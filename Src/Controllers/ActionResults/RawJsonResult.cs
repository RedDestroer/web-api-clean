using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var result = new ContentResult
            {
                StatusCode = StatusCodes.Status200OK,
                ContentType = "application/json; charset=utf-8",
                Content = Content
            };

            await result.ExecuteResultAsync(context);
        }
    }
}
