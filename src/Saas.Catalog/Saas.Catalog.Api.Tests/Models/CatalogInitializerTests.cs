namespace Saas.Catalog.Api.Tests.Models
{
    using Saas.Catalog.Api.Models;
    using System;
    using Xunit;
    using NSubstitute;
    using Microsoft.Extensions.Logging;
    using TestUtilities;

    public static class CatalogInitializerTests
    {
        [Theory, AutoDataNSubstitute]
        public static void CannotCallInitializeWithNullContext(ILogger logger)
        {
            Assert.Throws<ArgumentNullException>(() => default(CatalogDbContext).Initialize(logger));
        }

        [Theory, AutoDataNSubstitute]
        public static void LoggerIsNotRequriedToCallInitialize(CatalogDbContext context)
        {
            context.Initialize(null);
        }
    }
}