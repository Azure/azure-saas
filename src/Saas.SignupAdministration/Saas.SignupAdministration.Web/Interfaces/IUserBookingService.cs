namespace Saas.SignupAdministration.Web.Services;

public interface IUserBookingService
{
    Task<Booking> CreateBookingAsync(Booking booking, string partitionKey);
}