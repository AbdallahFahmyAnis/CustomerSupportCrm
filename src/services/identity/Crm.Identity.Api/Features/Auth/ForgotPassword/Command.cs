using Crm.Contracts.Identity;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.ForgotPassword;

/// <summary>SDD CRM-046 / specs/identity/050-password-reset.</summary>
public sealed record ForgotPasswordCommand(string Email) : IRequest<ForgotPasswordResponse>;
