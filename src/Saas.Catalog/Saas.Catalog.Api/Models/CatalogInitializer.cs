using System;
using Dawn;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Saas.Catalog.Api.Models
{
    /// <summary>
    /// Utilities to initialize and seed the Catalog database
    /// </summary>
    public class CatalogInitializer
    {
        private ILogger _logger;
        private CatalogDbContext _context;

        /// <summary>
        /// Create an instance of catalog initilaizer
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public CatalogInitializer(CatalogDbContext context, ILogger logger)
        {
            this._logger = logger;
            this._context = context;
        }

        /// <summary>
        /// Makes sure that the database is created, or throw an exception
        /// </summary>
        public void CreateDatabase()
        {
            try
            {
                this._context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(ex, "Unable to create the databaase");
                throw;
            }
        }

        /// <summary>
        /// Add the code to seed the database here.
        /// Make sure database is already created or call CreateDatabase
        /// </summary>
        public void SeedDatabase()
        {
            try
            {
                //Add any code required to seed the database here
            }
            catch(Exception ex)
            {
                this._logger.LogCritical(ex, "Error while seeding the database");
                throw;
            }
        }
    }
}