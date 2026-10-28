namespace Onion.API.Appointments.Models
{
    public sealed record AddressViewModel(
        string Street,
        string Number,
        string? Complement,
        string City,
        string State,
        string PostalCode,
        string Country);
}