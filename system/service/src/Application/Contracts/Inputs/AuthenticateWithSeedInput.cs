namespace Netnol.Identity.Service.Application.Contracts.Inputs;

public record AuthenticateWithSeedInput(string Username, string SeedHash);