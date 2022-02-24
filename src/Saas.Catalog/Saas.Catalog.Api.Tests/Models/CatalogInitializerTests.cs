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
        public static void LoggerIsRequriedToCallInitialize(CatalogDbContext context)
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => context.Initialize(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }
    }
}