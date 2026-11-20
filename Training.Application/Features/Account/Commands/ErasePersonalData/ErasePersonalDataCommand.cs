using MediatR;

namespace Training.Application.Features.Account.Commands.ErasePersonalData
{
    public record ErasePersonalDataCommand(string UserId) : IRequest<ErasePersonalDataStatus>;

    public enum ErasePersonalDataStatus
    {
        Erased,
        NotFound,
        LastAdmin
    }
}
