using HookMaker.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HookMaker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Notification notification)
        {
            await _mediator.Publish(notification);
            return Accepted();
        }
    }
}
