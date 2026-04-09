using Netnol.Identity.Service.Domain.ValueObjects;

namespace Netnol.Identity.Service.Application.Contracts.Outputs;


public record CheckExistenceOutput(Identification Id, Username Username);