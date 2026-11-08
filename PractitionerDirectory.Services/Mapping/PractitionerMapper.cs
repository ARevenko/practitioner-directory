using PractitionerDirectory.Repositories.Models;
using PractitionerDirectory.Services.Models;

namespace PractitionerDirectory.Services.Mappers
{
    public class PractitionerMapper : IPractitionerMapper
    {
        public Practitioner PractitionerDTOToEntity(PractitionerDTO model, Practitioner? entity)
        {
            if (entity == null)
            {
                entity = new Practitioner();
            }

            if (entity.Id == Guid.Empty)
            {
                entity.Id = model.Id == null ? Guid.Empty : model.Id.Value;
            }

            entity.FirstName = model.FirstName;
            entity.LastName = model.LastName;
            entity.Phone = model.Phone;
            entity.Speciality = model.Speciality;
            entity.PlaceOfWork = model.PlaceOfWork;
            entity.Email = model.Email;
            entity.IsActive = model.IsActive;

            return entity;
        }

        public PractitionerDTO PractitionerToDTO(Practitioner entity, PractitionerDTO? model)
        {
            if (model == null)
            {
                model = new PractitionerDTO();
            }

            model.Id = entity.Id;
            model.FirstName = entity.FirstName;
            model.LastName = entity.LastName;
            model.Phone = entity.Phone;
            model.Speciality = entity.Speciality;
            model.PlaceOfWork = entity.PlaceOfWork;
            model.Email = entity.Email;
            model.IsActive = entity.IsActive;
            model.CreatedOn = entity.CreatedOn;
            model.UpdatedOn = entity.UpdatedOn;

            return model;
        }
    }
}
