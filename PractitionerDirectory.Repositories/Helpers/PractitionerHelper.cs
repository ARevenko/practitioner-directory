using PractitionerDirectory.Repositories.Models;

namespace PractitionerDirectory.Repositories.Helpers
{
    public static class PractitionerHelper
    {
        public static void Normalize(Practitioner entity)
        {
            entity.FirstName = entity.FirstName.Trim();
            entity.LastName = entity.LastName.Trim();
            entity.Email = entity.Email.Trim().ToLowerInvariant();
            entity.Phone = entity.Phone?.Trim();
            entity.PlaceOfWork = entity.PlaceOfWork?.Trim();
        }
    }
}
