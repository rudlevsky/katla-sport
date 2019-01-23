using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KatlaSport.DataAccess;
using KatlaSport.DataAccess.ProductStoreHive;
using DbHiveSection = KatlaSport.DataAccess.ProductStoreHive.StoreHiveSection;

namespace KatlaSport.Services.HiveManagement
{
    /// <summary>
    /// Represents a hive section service.
    /// </summary>
    public class HiveSectionService : IHiveSectionService
    {
        private readonly IProductStoreHiveContext _context;
        private readonly IUserContext _userContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="HiveSectionService"/> class with specified <see cref="IProductStoreHiveContext"/> and <see cref="IUserContext"/>.
        /// </summary>
        /// <param name="context">A <see cref="IProductStoreHiveContext"/>.</param>
        /// <param name="userContext">A <see cref="IUserContext"/>.</param>
        public HiveSectionService(IProductStoreHiveContext context, IUserContext userContext)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userContext = userContext ?? throw new ArgumentNullException();
        }

        /// <inheritdoc/>
        public async Task<List<HiveSectionListItem>> GetHiveSectionsAsync()
        {
            var dbHiveSections = await _context.Sections.OrderBy(s => s.Id).ToArrayAsync();
            var hiveSections = dbHiveSections.Select(s => Mapper.Map<HiveSectionListItem>(s)).ToList();
            return hiveSections;
        }

        /// <inheritdoc/>
        public async Task<HiveSection> GetHiveSectionAsync(int hiveSectionId)
        {
            var dbHiveSections = await _context.Sections.Where(s => s.Id == hiveSectionId).ToArrayAsync();
            if (dbHiveSections.Length == 0)
            {
                throw new RequestedResourceNotFoundException();
            }

            return Mapper.Map<DbHiveSection, HiveSection>(dbHiveSections[0]);
        }

        /// <inheritdoc/>
        public async Task<List<HiveSectionListItem>> GetHiveSectionsAsync(int hiveId)
        {
            var dbHiveSections = await _context.Sections.Where(s => s.StoreHiveId == hiveId).OrderBy(s => s.Id).ToArrayAsync();
            var hiveSections = dbHiveSections.Select(s => Mapper.Map<HiveSectionListItem>(s)).ToList();
            return hiveSections;
        }

        /// <inheritdoc/>
        public async Task SetStatusAsync(int hiveSectionId, bool deletedStatus)
        {
            var dbSections = await _context.Sections.Where(c => hiveSectionId == c.Id).ToArrayAsync();

            if (dbSections.Length == 0)
            {
                throw new RequestedResourceNotFoundException();
            }

            var dbSection = dbSections[0];
            if (dbSection.IsDeleted != deletedStatus)
            {
                dbSection.IsDeleted = deletedStatus;
                dbSection.LastUpdated = DateTime.UtcNow;
                dbSection.LastUpdatedBy = _userContext.UserId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<HiveSection> CreateHiveSectionAsync(UpdateHiveSectionRequest create)
        {
            var sections = await _context.Sections.Where(h => h.Code == create.Code).ToArrayAsync();
            if (sections.Length > 0)
            {
                throw new RequestedResourceHasConflictException("code");
            }

            var dbHiveSections = Mapper.Map<UpdateHiveSectionRequest, DbHiveSection>(create);
            dbHiveSections.Created = DateTime.UtcNow;
            dbHiveSections.LastUpdated = DateTime.UtcNow;
            dbHiveSections.StoreHiveId = create.StoreHiveId;
            _context.Sections.Add(dbHiveSections);

            await _context.SaveChangesAsync();

            return Mapper.Map<HiveSection>(dbHiveSections);
        }

        public async Task<HiveSection> UpdateHiveSectionAsync(int sectionId, UpdateHiveSectionRequest update)
        {
            var sections = await _context.Sections.Where(h => h.Code == update.Code && h.Id != sectionId).ToArrayAsync();
            if (sections.Length > 0)
            {
                throw new RequestedResourceHasConflictException("Multiple sections affected");
            }

            sections = await _context.Sections.Where(h => h.Id == sectionId).ToArrayAsync();
            if (sections.Length == 0)
            {
                throw new RequestedResourceNotFoundException();
            }

            var dbHiveSection = sections[0];
            Mapper.Map(update, dbHiveSection);
            dbHiveSection.LastUpdatedBy = _userContext.UserId;
            dbHiveSection.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Mapper.Map<HiveSection>(dbHiveSection);
        }

        public async Task DeleteHiveSectionAsync(int sectionId)
        {
            var sections = await _context.Sections.Where(h => h.Id == sectionId).ToArrayAsync();
            if (sections.Length == 0)
            {
                throw new RequestedResourceNotFoundException();
            }

            var dbHiveSection = sections[0];
            /*if (dbHiveSection.IsDeleted == false)
            {
                throw new RequestedResourceHasConflictException();
            }*/

            _context.Sections.Remove(dbHiveSection);
            await _context.SaveChangesAsync();
        }
    }
}
