using PractitionerDirectory.Services.Models;
using FhirPractitioner = Hl7.Fhir.Model.Practitioner;

namespace PractitionerDirectory.API.Mappers
{
    public interface IFhirPractitionerMapper
    {
        FhirPractitioner PractitionerDTOToFhir(PractitionerDTO model);
    }
}
