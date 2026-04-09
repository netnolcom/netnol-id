using Netnol.Identity.Service.Domain.ValueObjects;

namespace Netnol.Identity.Service.Application.Contracts.Outputs;

public record ProfileOutput(Identification Id, Username Username, byte[] PublicKey);