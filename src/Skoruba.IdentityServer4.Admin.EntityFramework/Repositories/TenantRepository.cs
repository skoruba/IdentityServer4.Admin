﻿using Microsoft.EntityFrameworkCore;
using Skoruba.AuditLogging.EntityFramework.Helpers;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Repositories
{
    public class TenantRepository<TDbContext> : ITenantRepository
        where TDbContext : DbContext, IMultiTenantDbContext
    {
        protected readonly TDbContext DbContext;
        
        public TenantRepository(TDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task DeleteAsync(Tenant entity)
        {
            DbContext.Set<Tenant>().Remove(entity);
            await DbContext.SaveChangesAsync();
        }

        public async  Task<Tenant> FindByIdAsync(Guid id) =>
            await DbContext.Tenants
            .Include(t => t.ConnectionStrings)
            .Include(t => t.Edition)
            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<Tenant> FindByNameAsync(string name) =>
            await DbContext.Tenants
            .Include(t => t.ConnectionStrings)
            .Include(t => t.Edition)
            .FirstOrDefaultAsync(x => x.Name == name);

        public async Task<int> GetCountAsync() =>
            await DbContext.Tenants.CountAsync();

        public async Task<List<Tenant>> GetListAsync() =>
            await DbContext.Tenants
            .Include(t => t.ConnectionStrings)
            .Include(t => t.Edition)
            .ToListAsync();

        public async Task<PagedList<Tenant>> GetListAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<Tenant>();

            Expression<Func<Tenant, bool>> searchCondition = x => x.Name.Contains(search);

            var tenants = await DbContext.Tenants
                .Include(t => t.Edition)
                .WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.Name, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(tenants);
            pagedList.TotalCount = await DbContext.Tenants.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public async Task<Tenant> AddAsync(Tenant entity)
        {
            var savedEntity = await DbContext.Tenants.AddAsync(entity);
            await DbContext.SaveChangesAsync();
            return savedEntity.Entity;
        }

        public async Task<Tenant> UpdateAsync(Tenant entity)
        {
            var updatedEntity = DbContext.Tenants.Update(entity);
            await DbContext.SaveChangesAsync();
            return updatedEntity.Entity;
        }

        public async Task DeleteEditionAsync(Edition entity)
        {
            DbContext.Set<Edition>().Remove(entity);
            await DbContext.SaveChangesAsync();
        }

        public async Task<Edition> FindEditionByIdAsync(Guid id) =>
            await DbContext.Editions.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<Edition> FindEditionByNameAsync(string name) =>
            await DbContext.Editions.FirstOrDefaultAsync(x => x.Name == name);

        public async Task<Edition> AddEditionAsync(Edition entity)
        {
            var savedEntity = await DbContext.Editions.AddAsync(entity);
            await DbContext.SaveChangesAsync();
            return savedEntity.Entity;
        }

        public async Task<Edition> UpdateEditionAsync(Edition entity)
        {
            var updatedEntity = DbContext.Editions.Update(entity);
            await DbContext.SaveChangesAsync();
            return updatedEntity.Entity;
        }

        public async Task<PagedList<Edition>> GetEditionListAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<Edition>();

            Expression<Func<Edition, bool>> searchCondition = x => x.Name.Contains(search);

            var editions = await DbContext.Editions.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.Name, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(editions);
            pagedList.TotalCount = await DbContext.Editions.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public async Task<List<Edition>> GetEditionListAsync() =>
            await DbContext.Editions.ToListAsync();
    }
}