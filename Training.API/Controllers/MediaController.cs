using MediatR;
using Microsoft.AspNetCore.Mvc;
using Training.Application.Features.Media.Queries.GetMediaFile;
using Training.Application.Interfaces;
using Training.Domain;

namespace Training.API.Controllers
{
    [Route("api/v1/media")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMediaFileStore _files;

        public MediaController(IMediator mediator, IMediaFileStore files)
        {
            _mediator = mediator;
            _files = files;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var includeUnpublished = User.IsInRole(TrainingRoles.Manager) || User.IsInRole(TrainingRoles.Admin);
            var media = await _mediator.Send(new GetMediaFileQuery(id, includeUnpublished), cancellationToken);
            if (media is null)
            {
                return NotFound();
            }

            var stream = _files.OpenRead(media.StoredFileName);
            return stream is null ? NotFound() : File(stream, media.ContentType);
        }
    }
}
