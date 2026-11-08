using Microsoft.EntityFrameworkCore;
using PractitionerDirectory.Repositories.Models;

namespace PractitionerDirectory.Repositories
{
    public static class DbInitializer
    {
        public static async Task UpsertAsync(PractitionerDirectoryDbContext context)
        {
            await context.Database.MigrateAsync();

            var isDataExist = await context.Practitioners.AnyAsync();

            if (isDataExist)
            {
                return;
            }

            var date = DateTime.UtcNow;

            context.Practitioners.AddRange(
                new Practitioner
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Иван",
                    LastName = "Петров",
                    Speciality = "Терапевт",
                    Email = "i.petrov@clinic.example",
                    Phone = "+375291112233",
                    PlaceOfWork = "Поликлиника №1",
                    IsActive = true,
                    CreatedOn = date,
                    UpdatedOn = date
                },
                new Practitioner
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Мария",
                    LastName = "Сидорова",
                    Speciality = "Кардиолог",
                    Email = "m.sidorova@clinic.example",
                    Phone = "+375292223344",
                    PlaceOfWork = "Городская больница №2",
                    IsActive = true,
                    CreatedOn = date,
                    UpdatedOn = date
                },
                new Practitioner
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Александр",
                    LastName = "Ковалёв",
                    Speciality = "Хирург",
                    Email = "a.kovalev@clinic.example",
                    Phone = "+375293334455",
                    PlaceOfWork = "Городская больница №2",
                    IsActive = true,
                    CreatedOn = date,
                    UpdatedOn = date
                }, new Practitioner
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Елена",
                    LastName = "Новикова",
                    Speciality = "Педиатр",
                    Email = "e.novikova@clinic.example",
                    Phone = "+375294445566",
                    PlaceOfWork = "Детская поликлиника №3",
                    IsActive = true,
                    CreatedOn = date,
                    UpdatedOn = date
                },
                new Practitioner
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Дмитрий",
                    LastName = "Морозов",
                    Speciality = "Невролог",
                    Email = "d.morozov@clinic.example",
                    Phone = "+375295556677",
                    PlaceOfWork = "Поликлиника №1",
                    IsActive = true,
                    CreatedOn = date,
                    UpdatedOn = date
                },
                new Practitioner
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Ольга",
                    LastName = "Васильева",
                    Speciality = "Офтальмолог",
                    Email = "o.vasileva@clinic.example",
                    Phone = "+375296667788",
                    PlaceOfWork = "Медицинский центр «Здоровье»",
                    IsActive = true,
                    CreatedOn = date,
                    UpdatedOn = date
                },
                new Practitioner
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Сергей",
                    LastName = "Захаров",
                    Speciality = "Стоматолог",
                    Email = "s.zakharov@clinic.example",
                    Phone = "+375297778899",
                    PlaceOfWork = "Стоматология №5",
                    IsActive = false,   // неактивный — для проверки фильтров
                    CreatedOn = date,
                    UpdatedOn = date
                },
                new Practitioner
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Анна",
                    LastName = "Козлова",
                    Speciality = "Терапевт",
                    Email = "a.kozlova@clinic.example",
                    Phone = null,       // без телефона — проверка nullable-поля
                    PlaceOfWork = "Поликлиника №4",
                    IsActive = true,
                    CreatedOn = date,
                    UpdatedOn = date
                },
                new Practitioner
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Павел",
                    LastName = "Григорьев",
                    Speciality = "Рентгенолог",
                    Email = "p.grigorev@clinic.example",
                    Phone = "+375298889900",
                    PlaceOfWork = null,  // без места работы — тоже проверка
                    IsActive = true,
                    CreatedOn = date,
                    UpdatedOn = date
                },
                new Practitioner
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Наталья",
                    LastName = "Фёдорова",
                    Speciality = "Кардиолог",
                    Email = "n.fedorova@clinic.example",
                    Phone = "+375299990011",
                    PlaceOfWork = "Городская больница №2",
                    IsActive = false,   // вторая неактивная
                    CreatedOn = date,
                    UpdatedOn = date
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
