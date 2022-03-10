namespace System.Security.Claims
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Extension to ClaimsPrincipal to allow checking for user roles within a context.
        /// </summary>
        /// <param name="cp">ClaimsPrincipal</param>
        /// <param name="context">User context (ie. subscriptionId)</param>
        /// <param name="role">Role</param>
        /// <returns>true if user is in role</returns>
        /// <example>
        /// user.IsInRole(mySubscriptionId, "SubscriptionAdmin");
        /// </example>
        public static bool IsInRole(this ClaimsPrincipal cp, string context, string role)
        {
            string newRole = RoleFormatter(context, role);
            return cp.IsInRole(newRole);
        }

        public static Func<string, string, string> RoleFormatter { get; set; } = (context, role) => string.Format("{0}.{1}", context, role);
    }
}
