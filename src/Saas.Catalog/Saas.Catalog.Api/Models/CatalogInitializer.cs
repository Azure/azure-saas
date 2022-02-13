using System;
using Dawn;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Saas.Catalog.Api.Models
{
    /// <summary>
    /// Utilities to initialize and seed the Catalog database
    /// </summary>
    public static class CatalogInitializer
    {
        public static void Initialize(this CatalogDbContext context, ILogger logger)
        {
            //_ = context ?? throw new ArgumentNullException(nameof(context));
            Guard.Argument(context, nameof(context)).NotNull();

            context.Database.EnsureCreated();
        }
    }
}