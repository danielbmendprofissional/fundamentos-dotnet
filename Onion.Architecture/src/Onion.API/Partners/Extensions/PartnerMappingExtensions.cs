using Onion.API.Common;
using Onion.API.Partners.Models;
using Onion.Application.Common.DTOs.Requests;
using Onion.Application.Partners.DTOs.Requests;

namespace Onion.API.Partners.Extensions
{
    public static class PartnerMappingExtensions
    {
        public static CreatePartnerRequest ToRequest(this CreatePartnerViewModel viewModel)
            => new(
                new FullNameRequest(viewModel.FullName),
                viewModel.Name,
                new EmailRequest(viewModel.Email),
                viewModel.PartnerType,
                viewModel.ExternalPartnerId,
                viewModel.Image);

        public static UpdatePartnerRequest ToRequest(this UpdatePartnerViewModel viewModel, Guid id)
            => new(
                id,
                viewModel.FullName,
                viewModel.Name,
                viewModel.Image);

        public static PartnerFilter ToRequest(this PartnerFilterViewModel viewModel)
            => new(
                viewModel.Page,
                viewModel.PageSize,
                viewModel.Skip,
                viewModel.Take,
                viewModel.Name,
                viewModel.Email,
                viewModel.PartnerType);
        }
    }
}
