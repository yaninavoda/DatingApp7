namespace API.Extensions;

public static class DateTimeExtensions
{   
    public static int CalculateAge(this DateOnly dateOfBirth) 
    {
        return DateTime.UtcNow.Year - dateOfBirth.Year;
    }
}
