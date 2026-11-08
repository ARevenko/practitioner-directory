using PractitionerDirectory.Repositories.Models;
using PractitionerDirectory.Services.Models;

namespace PractitionerDirectory.Services.Mappers
{
    public interface IPractitionerMapper
    {
        PractitionerDTO PractitionerToDTO(Practitioner entity, PractitionerDTO? model);

        Practitioner PractitionerDTOToEntity(PractitionerDTO model, Practitioner? entity);
    }
}
