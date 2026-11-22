using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Training.API.Auth;
using Training.Application.Features.Media;
using Training.Application.Features.Media.Commands.InsertLessonMedia;
using Training.Application.Features.Media.Commands.UploadLessonMedia;

namespace Training.API.Controllers
{
    [Authorize(Policy = TrainingPolicies.EditContent)]
    [Route("api/v1/admin/lessons/{lessonId:int}/media")]
    [ApiController]
    public class AdminMediaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAntiforgery _antiforgery;

        public AdminMediaController(IMediator mediator, IAntiforgery antiforgery)
        {
            _mediator = mediator;
            _antiforgery = antiforgery;
        }

        [HttpPost]
        [RequestSizeLimit(MediaLimits.MaxRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MediaLimits.MaxRequestBytes)]
        public async Task<IActionResult> Upload(int lessonId, IFormFile? file, CancellationToken cancellationToken)
        {
            if (!await IsRequestValidAsync() || file is null)
            {
                return BadRequest();
            }

            await using var content = file.OpenReadStream();
            var result = await _mediator.Send(
                new UploadLessonMediaCommand(lessonId, file.FileName, content),
                cancellationToken);
            return result.Status switch
            {
                UploadLessonMediaStatus.Uploaded => Ok(new
                {
                    id = result.Id,
                    url = result.Url,
                    contentType = result.ContentType,
                    kind = result.Kind.ToString()
                }),
                UploadLessonMediaStatus.LessonNotFound => NotFound(),
                UploadLessonMediaStatus.LimitReached => Conflict(new { code = "limit" }),
                UploadLessonMediaStatus.TooLarge => StatusCode(StatusCodes.Status413PayloadTooLarge),
                _ => BadRequest()
            };
        }

        [HttpPost("{mediaId:int}/insert")]
        public async Task<IActionResult> Insert(int lessonId, int mediaId, [FromBody] InsertMediaRequest? request, CancellationToken cancellationToken)
        {
            if (!await IsRequestValidAsync())
            {
                return BadRequest();
            }

            var status = await _mediator.Send(
                new InsertLessonMediaCommand(lessonId, mediaId, request?.Alt),
                cancellationToken);
            return status switch
            {
                InsertLessonMediaStatus.Inserted => NoContent(),
                InsertLessonMediaStatus.LessonNotFound => NotFound(),
                _ => NotFound()
            };
        }

        private async Task<bool> IsRequestValidAsync()
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(HttpContext);
                return true;
            }
            catch (AntiforgeryValidationException)
            {
                return false;
            }
        }
    }

    public record InsertMediaRequest(string? Alt);
}
