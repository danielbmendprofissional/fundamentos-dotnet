using System.Xml.Linq;

namespace Onion.API.Accounts.Models
{
    public sealed record UpdateProfileViewModel(
        string Name,
        string? Image);
}
