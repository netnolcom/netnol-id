using Netnol.Identity.Service.Domain.ValueObjects;

namespace Netnol.Identity.Service.Application.Contracts.Outputs;

public record RotateSeedOutput(Identification Id, Username Username, byte[] PublicKey);