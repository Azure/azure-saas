using Saas.Billing.Service.Controllers.DTOs;
using Saas.Billing.Service.Interfaces;

namespace Saas.Billing.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly ILogger<SubscriptionController> _logger;
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ISubscriptionService subscriptionService, ILogger<SubscriptionController> logger)
    {
        _logger = logger;
        _subscriptionService = subscriptionService;
    }

    [HttpPost()]
    [ProducesResponseType(typeof(SubscriptionDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public SubscriptionDTO PostSubscription(NewSubscriptionRequest request)
    {
        return _subscriptionService.AddSubscriptionAsync(request);
    }
}