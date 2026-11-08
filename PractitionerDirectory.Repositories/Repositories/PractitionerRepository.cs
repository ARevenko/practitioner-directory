using Microsoft.EntityFrameworkCore;
using PractitionerDirectory.Repositories.Helpers;
using PractitionerDirectory.Repositories.Models;

namespace PractitionerDirectory.Repositories.Repositories
{
    public class PractitionerRepository : IPractitionerRepository
    {
        private readonly PractitionerDirectoryDbContext _dbContext;

        public PractitionerRepository(PractitionerDirectoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Practitioner?> GetPractitionerByIdAsync(Guid id)
        {
            var entity = await _dbContext.Practitioners.
                               FirstOrDefaultAsync(p => p.Id == id &&
                                                        p.DeletedOn == null);

            return entity;
        }

        public async Task<Practitioner?> GetPractitionerByEmailAsync(string email)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            var entity = await _dbContext.Practitioners.
                               FirstOrDefaultAsync(p => p.Email == normalizedEmail &&
                                                        p.DeletedOn == null);

            return entity;
        }

        public async Task<Guid> AddPractitionerAsync(Practitioner entity)
        {
            var date = DateTime.UtcNow;
            entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
            entity.CreatedOn = date;
            entity.UpdatedOn = date;
            PractitionerHelper.Normalize(entity);

            _dbContext.Practitioners.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task UpdatePractitionerAsync(Practitioner entity)
        {
            entity.UpdatedOn = DateTime.UtcNow;
            PractitionerHelper.Normalize(entity);

            _dbContext.Practitioners.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePractitionerAsync(Practitioner entity)
        {
            var date = DateTime.UtcNow;
            entity.UpdatedOn = date;
            entity.DeletedOn = date;
            entity.IsActive = false;

            _dbContext.Practitioners.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Practitioner>> GetPractitionersAsync(string? firstName, string? lastName, int resultSize)
        {
            var query = _dbContext.Practitioners.Where(p => p.DeletedOn == null);

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                query = query.Where(p => p.FirstName.Contains(firstName.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                query = query.Where(p => p.LastName.Contains(lastName.Trim()));
            }

            var result = await query.OrderBy(p => p.LastName)
                                    .ThenBy(p => p.FirstName)
                                    .Take(resultSize)
                                    .ToListAsync();
            return result;
        }

        public async Task<List<Practitioner>> GetPractitionersPageAsync(int page, int pageSize)
        {
            var result = await _dbContext.Practitioners
                                         .Where(p => p.DeletedOn == null)
                                         .OrderBy(p => p.LastName)
                                         .Skip((page - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToListAsync();
            return result;
        }
    }
}
