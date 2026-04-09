using Netnol.Identity.Service.Domain.ValueObjects;

namespace Netnol.Identity.Service.Application.Contracts.Outputs;

public record RotatePasswordOutput(Identification Id, Username Username, byte[] PublicKey);