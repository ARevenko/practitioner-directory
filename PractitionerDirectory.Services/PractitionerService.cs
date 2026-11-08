using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PractitionerDirectory.Repositories.Models;
using PractitionerDirectory.Repositories.Repositories;
using PractitionerDirectory.Services.CustomExceptions;
using PractitionerDirectory.Services.Mappers;
using PractitionerDirectory.Services.Models;

namespace PractitionerDirectory.Services
{
    public class PractitionerService : IPractitionerService
    {
        private readonly IPractitionerMapper _mapper;
        private readonly IPractitionerRepository _practitionerRepository;

        public PractitionerService(
            IPractitionerMapper mapper,
            IPractitionerRepository practitionerRepository)
        {
            _mapper = mapper;
            _practitionerRepository = practitionerRepository;
        }


        public async Task<PractitionerDTO> UpsertAsync(PractitionerDTO model)
        {
            var entity = model.Id.HasValue ?
                await _practitionerRepository.GetPractitionerByIdAsync(model.Id.Value) :
                await _practitionerRepository.GetPractitionerByEmailAsync(model.Email);

            var isExist = entity != null;

            entity = _mapper.PractitionerDTOToEntity(model, entity);

            if (isExist)
            {
                await UpdatePractitionerAsync(entity);
            }
            else
            {
                model = await AddPractitionerAsync(model, entity);
            }

            model.CreatedOn = entity.CreatedOn;
            model.UpdatedOn = entity.UpdatedOn;

            return model;
        }

        public async Task<PractitionerDTO> CreateAsync(PractitionerDTO model)
        {
            var entity = await _practitionerRepository.GetPractitionerByEmailAsync(model.Email);

            var isExist = entity != null;

            if (entity != null)
            {
                throw new DuplicatePractitionerException();
            }

            entity = _mapper.PractitionerDTOToEntity(model, null);
            model = await AddPractitionerAsync(model, entity);

            model.CreatedOn = entity.CreatedOn;
            model.UpdatedOn = entity.UpdatedOn;

            return model;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _practitionerRepository.GetPractitionerByIdAsync(id);

            if (entity == null)
            {
                return false;
            }

            await _practitionerRepository.DeletePractitionerAsync(entity);
            return true;
        }

        public async Task<PractitionerDTO?> GetPractitionerByIdAsync(Guid id)
        {
            var entity = await _practitionerRepository.GetPractitionerByIdAsync(id);

            if (entity == null)
            {
                return null;
            }

            var model = _mapper.PractitionerToDTO(entity, null);
            return model;
        }

        public async Task<List<PractitionerDTO>> GetPractitionersAsync(string? firstName, string? lastName, int resultSize)
        {
            var entities = await _practitionerRepository.GetPractitionersAsync(firstName, lastName, resultSize);

            var result = entities.Select(p => _mapper.PractitionerToDTO(p, null)).ToList();

            return result;
        }

        public async Task<List<PractitionerDTO>> GetPractitionersPageAsync(int page, int pageSize)
        {
            var entities = await _practitionerRepository.GetPractitionersPageAsync(page, pageSize);

            var result = entities.Select(p => _mapper.PractitionerToDTO(p, null)).ToList();

            return result;
        }

        private async Task<PractitionerDTO> AddPractitionerAsync(PractitionerDTO model, Practitioner entity)
        {
            try
            {
                var id = await _practitionerRepository.AddPractitionerAsync(entity);
                model.Id = id;

                return model;
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is SqlException sqlEx && sqlEx.Number is 2627 or 2601)
            {
                throw new DuplicatePractitionerException();
            }
        }

        private async Task UpdatePractitionerAsync(Practitioner entity)
        {
            try
            {
                await _practitionerRepository.UpdatePractitionerAsync(entity);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyConflictException();
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is SqlException sqlEx && sqlEx.Number is 2627 or 2601)
            {
                throw new DuplicatePractitionerException();
            }
        }
    }
}
