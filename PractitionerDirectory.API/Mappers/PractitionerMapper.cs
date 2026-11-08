using PractitionerDirectory.API.Models;
using PractitionerDirectory.Services.Models;

namespace PractitionerDirectory.API.Mappers
{
    public class PractitionerMapper : IPractitionerMapper
    {
        public PractitionerDTO CreatePractitionerRequestToPractitionerDTO(CreatePractitionerRequest requestModel)
        {

            var model = new PractitionerDTO();

            model.FirstName = requestModel.FirstName;
            model.LastName = requestModel.LastName;
            model.Phone = requestModel.Phone;
            model.Speciality = requestModel.Speciality;
            model.PlaceOfWork = requestModel.PlaceOfWork;
            model.Email = requestModel.Email;
            model.IsActive = requestModel.IsActive;
            model.CreatedOn = requestModel.CreatedOn;
            model.UpdatedOn = requestModel.UpdatedOn;

            return model;
        }
    }
}
