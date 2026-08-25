using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Settings.GetBranding;

/// <summary>SDD CRM-044 — public branding for shell (no admin gate).</summary>
public sealed class GetBrandingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/branding", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetBrandingQuery())));
    }
}

public sealed record GetBrandingQuery : IRequest<BrandingDto>;

public sealed class GetBrandingHandler(IdentityDirectory directory)
    : IRequestHandler<GetBrandingQuery, BrandingDto>
{
    public async Task<BrandingDto> Handle(GetBrandingQuery request, CancellationToken cancellationToken)
    {
        var row = await directory.GetOrCreateSettingsAsync(cancellationToken);
        return new BrandingDto(row.ProductTitle, row.PrimaryColor, row.LogoUrl, row.OrganizationName);
    }
}
