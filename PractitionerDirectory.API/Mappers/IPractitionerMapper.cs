using PractitionerDirectory.API.Models;
using PractitionerDirectory.Services.Models;

namespace PractitionerDirectory.API.Mappers
{
    public interface IPractitionerMapper
    {
        PractitionerDTO CreatePractitionerRequestToPractitionerDTO(CreatePractitionerRequest requestModel);
    }
}
