namespace Netnol.Identity.Service.Application.Contracts.Inputs;

public record AuthenticateWithPasswordInput(string Username, string PasswordHash);