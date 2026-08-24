namespace Crm.Contracts.Identity;

public sealed record DevLoginRequest(string Email, string Password);

public sealed record DevUserDto(string Id, string Email, string Name, string Role);
