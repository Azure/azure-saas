#nullable disable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Saas.Admin.Service.Data;
using Saas.Admin.Service.Exceptions;
using Saas.Admin.Service.Services;

namespace Saas.Admin.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IPermissionService _permissionService;
        private readonly ILogger _logger;
        

        public SubscriptionsController(ISubscriptionService subscriptionService, IPermissionService permissionService, ILogger<SubscriptionsController> logger)
        {
            _logger = logger;
            _subscriptionService = subscriptionService;
            _permissionService = permissionService;
        }

        // GET: api/Subscriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubscriptionDTO>>> GetAllSubscriptions()
        {
            IEnumerable<Subscription> allSubscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
            return allSubscriptions.Select(s => new SubscriptionDTO(s)).ToList();
        }

        // GET: api/Subscriptions/5
        [HttpGet("{subscriptionId}")]
        public async Task<ActionResult<SubscriptionDTO>> GetSubscription(Guid subscriptionId)
        {
            try
            {
                var subscription = await _subscriptionService.GetSubscriptionAsync(subscriptionId);
                return new SubscriptionDTO(subscription);
            }
            catch (ItemNotFoundExcepton)
            {
                return NotFound();
            }
        }


        // POST: api/Subscriptions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SubscriptionDTO>> PostSubscription(SubscriptionDTO subscriptionDTO)
        {

            Subscription subscription = subscriptionDTO.ToSubscription();
            subscription= await _subscriptionService.AddSubscriptionAsync(subscription);

            return CreatedAtAction("GetSubscription", new { id = subscription.Id }, subscription);
        }



        // PUT: api/Subscriptions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{subscriptionId}")]
        public async Task<IActionResult> PutSubscription(Guid subscriptionId, SubscriptionDTO subscriptionDTO)
        {
            if (subscriptionId != subscriptionDTO.Id)
            {
                return BadRequest();
            }

            try
            {
                await _subscriptionService.UpdateSubscriptionAsync(subscriptionDTO.ToSubscription());
            }
            catch(ItemNotFoundExcepton)
            {
                return NotFound();
            }

            return NoContent();
        }


        // DELETE: api/Subscriptions/5
        [HttpDelete("{subscriptionId}")]
        public async Task<IActionResult> DeleteSubscription(Guid subscriptionId)
        {

            try
            {
                await _subscriptionService.DeleteSubscriptionAsync(subscriptionId);
            }
            catch (ItemNotFoundExcepton)
            {
                return NotFound();
            }

            return NoContent();
        }


        [HttpGet("{subscriptionId}/users")]
        public async Task<ActionResult<IEnumerable<string>>> GetSubscriptionUsers(Guid subscriptionId)
        {
            IEnumerable<string> users = await _permissionService.GetSubsriptionUsersAsync(subscriptionId);
            return users.ToList();
        }

        [HttpGet("{subscriptionId}/Users/{userId}/permissions")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserPermissions(Guid subscriptionId, string userId)
        {
            var permissions = await _permissionService.GetUserPermissionsForSubscriptionAsync(subscriptionId, userId);
            return permissions.ToList();
        }

        [HttpPost("{subscriptionId}/Users/{userId}/permissions")]
        public async Task<IActionResult> PostUserPermissions(Guid subscriptionId, string userId, [FromBody] string[] permissions)
        {
            await _permissionService.AddUserPermissionsToSubscriptionAsyc(subscriptionId, userId, permissions);
            return NoContent();
        }

        [HttpDelete("{subscriptionId}/Users/{userId}/permissions")]
        public async Task<IActionResult> DeleteUserPermissions(Guid subscriptionId, string userId, [FromBody] string[] permissions)
        {
            await _permissionService.RemoveUserPermissionsFromSubscriptionAsync(subscriptionId, userId, permissions);
            return NoContent();
        }

        [HttpGet("user/{userId}/subscriptions")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        //sysadmin or current user
        public async Task<ActionResult<IEnumerable<Guid>>> UserSubscriptions(string userId, string filter = null)
        {
            this._logger.LogDebug("Geting all subscriptions for user {userId}", userId);

            IEnumerable<Guid> subscriptions = await _permissionService.GetSubscriptionsForUserAsync(userId, filter);
            return subscriptions.ToList();
        }
    }
}
