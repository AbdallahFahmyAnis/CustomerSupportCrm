namespace Crm.BuildingBlocks.Security;

/// <summary>SDD CRM-038 deferred / specs/045 — minimal Swagger UI for external OpenAPI.</summary>
public static class ExternalApiSwaggerUi
{
    public const string Html =
        """
        <!DOCTYPE html>
        <html lang="en">
        <head>
          <meta charset="utf-8" />
          <title>CRM External API</title>
          <link rel="stylesheet" href="https://unpkg.com/swagger-ui-dist@5.17.14/swagger-ui.css" />
          <style>body{margin:0}#swagger-ui{max-width:1200px;margin:0 auto}</style>
        </head>
        <body>
          <div id="swagger-ui"></div>
          <script src="https://unpkg.com/swagger-ui-dist@5.17.14/swagger-ui-bundle.js"></script>
          <script>
            window.ui = SwaggerUIBundle({
              url: '/api/external/v1/openapi.yaml',
              dom_id: '#swagger-ui',
              presets: [SwaggerUIBundle.presets.apis],
              layout: 'BaseLayout',
              tryItOutEnabled: true,
              persistAuthorization: true
            });
          </script>
        </body>
        </html>
        """;
}
