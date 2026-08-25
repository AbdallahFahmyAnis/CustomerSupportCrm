namespace Crm.BuildingBlocks.Security;

/// <summary>SDD CRM-038 polish / specs/041 — OpenAPI 3 for external v1.</summary>
public static class ExternalApiOpenApi
{
    public const string Yaml = """
openapi: 3.0.3
info:
  title: CRM External API
  version: "1.0"
  description: Machine API (CRM-038). Authenticate with X-Api-Key or Authorization ApiKey.
servers:
  - url: http://localhost:5000
paths:
  /api/external/v1/tickets:
    post:
      summary: Create ticket
      security:
        - ApiKeyAuth: []
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
      responses:
        "201":
          description: Created
        "401":
          description: Unauthorized
  /api/external/v1/tickets/{id}:
    get:
      summary: Get ticket
      security:
        - ApiKeyAuth: []
      parameters:
        - name: id
          in: path
          required: true
          schema:
            type: string
            format: uuid
      responses:
        "200":
          description: OK
        "401":
          description: Unauthorized
  /api/external/v1/customers:
    get:
      summary: Search customers
      security:
        - ApiKeyAuth: []
      parameters:
        - name: q
          in: query
          schema:
            type: string
      responses:
        "200":
          description: OK
        "401":
          description: Unauthorized
  /api/external/v1/openapi.yaml:
    get:
      summary: OpenAPI document (public)
      responses:
        "200":
          description: YAML
components:
  securitySchemes:
    ApiKeyAuth:
      type: apiKey
      in: header
      name: X-Api-Key
""";
}
