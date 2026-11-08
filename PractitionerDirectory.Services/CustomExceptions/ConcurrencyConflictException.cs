namespace PractitionerDirectory.Services.CustomExceptions
{
    public class ConcurrencyConflictException : Exception
    {
        public ConcurrencyConflictException() :
            base("Запись была изменена другим запросом. Обновите данные и повторите.")
        { }
    }
}
