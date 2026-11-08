using PractitionerDirectory.Repositories.Models;

namespace PractitionerDirectory.Repositories.Repositories
{
    public interface IPractitionerRepository
    {
        Task<Practitioner?> GetPractitionerByIdAsync(Guid id);

        Task<Practitioner?> GetPractitionerByEmailAsync(string email);

        Task<Guid> AddPractitionerAsync(Practitioner entity);

        Task UpdatePractitionerAsync(Practitioner entity);

        Task DeletePractitionerAsync(Practitioner entity);

        Task<List<Practitioner>> GetPractitionersAsync(string? firstName, string? lastName, int resultSize);

        Task<List<Practitioner>> GetPractitionersPageAsync(int page, int pageSize);
    }
}
