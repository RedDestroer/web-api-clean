using AutoMapper;
using WebApiClean.Application.Models.Sample;
using WebApiClean.Application.Samples.Queries.Sample;
using WebApiClean.Controllers.SwaggerExamples;
using WebApiClean.Controllers.SwaggerExamples.Sample;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using WebApiClean.Domain.ServiceResult;

namespace WebApiClean.Controllers
{
    [ApiController]
    [Route("api")]
    public class SampleController : BaseController
    {
        private readonly IMapper _mapper;

        public SampleController(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Sample request.
        /// </summary>
        [HttpPost("sample/getall")]
        [AllowAnonymous]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json, Type = typeof(GetAllSampleResponse))]
        [SwaggerOperation(
            Summary = "Sample request",
            Description = "Sample request description",
            OperationId = OperationIds.Sample.Get)
        ]
        [SwaggerRequestExample(typeof(GetAllSampleRequest), typeof(GetAllSampleRequestExample))]
        [SwaggerResponse(StatusCodes.Status200OK, "When all is OK.", typeof(GetAllSampleResponse))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetAllOkExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "When request is invalid.\r\n\r\nWhen operation is cancelled.", typeof(FailureResponse))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(GetAllBadRequestExample))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "In case of internal exception.", typeof(FailureResponse))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorExample))]
        public async Task<IActionResult> GetAll([FromBody] GetAllSampleRequest request)
        {
            var query = _mapper.Map<SampleQuery>(request);

            var response = await Mediator.Send(query, CancellationToken.None);

            return OkResult(new GetAllSampleResponse
            {
                ResponseData = response
            });
        }
    }
}
