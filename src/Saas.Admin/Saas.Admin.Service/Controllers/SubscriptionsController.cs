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

        /// <summary>
        /// Get all subscriptions in the system
        /// </summary>
        /// <returns>List of all subscriptions</returns>
        /// <remarks>
        /// <para><b>Requires:</b> admin.subscription.read</para>
        /// <para>This call will return all the subscriptions in the system.</para>
        /// </remarks>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<SubscriptionDTO>>> GetAllSubscriptions()
        {
            IEnumerable<Subscription> allSubscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
            return allSubscriptions.Select(s => new SubscriptionDTO(s)).ToList();
        }

        /// <summary>
        /// Get a subscription by subscription ID
        /// </summary>
        /// <param name="subscriptionId">Guid representing the subscription</param>
        /// <returns>Information about the subscription</returns>
        /// <remarks>
        /// <para><b>Requires:</b> admin.subscription.read  or  {subscriptionId}.subscription.read</para>
        /// <para>Will return details of a single subscription, if user has access.</para>
        /// </remarks>
        [HttpGet("{subscriptionId}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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


        /// <summary>
        /// Add a new subscription
        /// </summary>
        /// <param name="subscriptionDTO"></param>
        /// <returns></returns>
        /// <remarks>
        /// <para><b>Requires:</b> Authenticated user</para>
        /// <para>This call needs a user to make admin of this subscription.  TBD explicitly pass in the user ID or 
        /// make the current user the admin (would prevent a third party creating subscriptions on behalf of user)</para>
        /// </remarks>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<SubscriptionDTO>> PostSubscription(SubscriptionDTO subscriptionDTO)
        {

            Subscription subscription = subscriptionDTO.ToSubscription();
            subscription= await _subscriptionService.AddSubscriptionAsync(subscription);

            return CreatedAtAction("GetSubscription", new { id = subscription.Id }, subscription);
        }



        /// <summary>
        /// Update an existing subscription
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="subscriptionDTO"></param>
        /// <returns></returns>
        /// <remarks>
        /// <para><b>Requires:</b> admin.subscription.write  or  {subscriptionId}.subscription.write</para>
        /// </remarks>
        [HttpPut("{subscriptionId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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


       /// <summary>
       /// Deletes a subscription
       /// </summary>
       /// <param name="subscriptionId"></param>
       /// <returns></returns>
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

        /// <summary>
        /// Get all users associated with a subscription
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// <remarks>
        /// <para>Right now only returns user IDs, should consider returning a user object with 
        /// user info + permissions for the subscription</para>
        /// </remarks>
        [HttpGet("{subscriptionId}/users")]
        public async Task<ActionResult<IEnumerable<string>>> GetSubscriptionUsers(Guid subscriptionId)
        {
            IEnumerable<string> users = await _permissionService.GetSubsriptionUsersAsync(subscriptionId);
            return users.ToList();
        }

        /// <summary>
        /// Get all permissions a user has in a subscription
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <remarks>This might be better combined with GetSubscriptionUsers, all usescases seem like they would need both</remarks>
        [HttpGet("{subscriptionId}/Users/{userId}/permissions")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserPermissions(Guid subscriptionId, string userId)
        {
            var permissions = await _permissionService.GetUserPermissionsForSubscriptionAsync(subscriptionId, userId);
            return permissions.ToList();
        }


        /// <summary>
        /// Add a set of permissions for a user on a subscription
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="userId"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        [HttpPost("{subscriptionId}/Users/{userId}/permissions")]
        public async Task<IActionResult> PostUserPermissions(Guid subscriptionId, string userId, [FromBody] string[] permissions)
        {
            await _permissionService.AddUserPermissionsToSubscriptionAsyc(subscriptionId, userId, permissions);
            return NoContent();
        }


        /// <summary>
        /// Delete a set of permissions for a user on a subscription
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="userId"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        [HttpDelete("{subscriptionId}/Users/{userId}/permissions")]
        public async Task<IActionResult> DeleteUserPermissions(Guid subscriptionId, string userId, [FromBody] string[] permissions)
        {
            await _permissionService.RemoveUserPermissionsFromSubscriptionAsync(subscriptionId, userId, permissions);
            return NoContent();
        }

        /// <summary>
        /// Get all subscription IDs that a user has access to
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="filter">Optionally filter by access type</param>
        /// <returns></returns>
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
