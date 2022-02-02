﻿using Saas.Admin.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saas.Admin.Web.Services
{
    public interface ITenantService 
    {
        Task<IEnumerable<Tenant>> GetItemsAsync();
        Task<Tenant> GetItemAsync(Guid id);
        Task AddItemAsync(Tenant item);
        Task UpdateItemAsync(Tenant item);
        Task DeleteItemAsync(Guid id);
    }
}
