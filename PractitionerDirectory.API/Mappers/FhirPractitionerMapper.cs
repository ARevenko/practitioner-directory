using Hl7.Fhir.Model;
using PractitionerDirectory.Services.Models;
using FhirPractitioner = Hl7.Fhir.Model.Practitioner;

namespace PractitionerDirectory.API.Mappers
{
    public class FhirPractitionerMapper : IFhirPractitionerMapper
    {
        public FhirPractitioner PractitionerDTOToFhir(PractitionerDTO modelDTO)
        {
            var model = new FhirPractitioner
            {
                Id = modelDTO.Id.ToString(),
                Active = modelDTO.IsActive,
                Name = new List<HumanName>
                {
                    new HumanName
                    {
                        Family = modelDTO.LastName,
                        Given = new[] { modelDTO.FirstName }
                    }
                },
                Telecom = new List<ContactPoint>()
            };

            if (!string.IsNullOrWhiteSpace(modelDTO.Phone))
            {
                model.Telecom.Add(new ContactPoint
                {
                    System = ContactPoint.ContactPointSystem.Phone,
                    Value = modelDTO.Phone
                });
            }

            model.Telecom.Add(new ContactPoint
            {
                System = ContactPoint.ContactPointSystem.Email,
                Value = modelDTO.Email
            });

            model.Qualification = new List<FhirPractitioner.QualificationComponent>
            {
                new FhirPractitioner.QualificationComponent
                {
                    Code = new CodeableConcept{ Text = modelDTO.Speciality }
                }
            };

            model.Meta = new Meta
            {
                VersionId = modelDTO.UpdatedOn.Ticks.ToString(),
                LastUpdated = modelDTO.UpdatedOn
            };

            return model;
        }
    }
}
