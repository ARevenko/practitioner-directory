using PractitionerDirectory.Services.Models;

namespace PractitionerDirectory.Services
{
    public interface IPractitionerService
    {
        Task<PractitionerDTO> UpsertAsync(PractitionerDTO model);

        Task<bool> DeleteAsync(Guid id);

        Task<PractitionerDTO?> GetPractitionerByIdAsync(Guid id);

        Task<List<PractitionerDTO>> GetPractitionersAsync(string? firstName, string? lastName, int resultSize);

        Task<List<PractitionerDTO>> GetPractitionersPageAsync(int page, int pageSize);
    }
}
