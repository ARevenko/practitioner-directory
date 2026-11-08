namespace PractitionerDirectory.Services.CustomExceptions
{
    public class DuplicatePractitionerException : Exception
    {
        public DuplicatePractitionerException() :
            base("Запись с такими данными уже существует")
        {
        }
    }
}
