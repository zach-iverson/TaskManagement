# Lessons from Day 1

## Summary of Today’s Sessions

### What You Asked
- Implement versioned Swagger support.
- Troubleshoot why Swagger UI was not accessible.
- Diagnose CORS issues blocking Swagger requests.
- Explain what CORS is.
- Create a `README.md` covering Docker Compose usage, endpoints, and migrations.
- Provide a consolidated recap to reinforce learning.

### What Was Implemented / Clarified
- Added API versioning explorer and dynamic Swagger generation (plan included creating a `ConfigureSwaggerOptions` to register one doc per version).
- Confirmed expected Swagger access paths (e.g., `https://localhost:7114/swagger` and per-version JSON).
- Identified common causes for Swagger access issues: port mismatch, HTTPS cert trust, missing versioned doc registration.
- Introduced broad development CORS policy (AllowAnyOrigin/Method/Header) to eliminate browser blocking during local testing.
- Delivered a README including: overview, Docker Compose flow, endpoints (CRUD + health), migrations workflow, config notes, and dev vs. prod considerations.

### Key Concepts Learned
- **API Versioning**: `IApiVersionDescriptionProvider` enables enumerating versions for Swagger grouping.
- **Swagger Version Docs**: Each version requires explicit `SwaggerDoc` registration or a configuration class to iterate versions.
- **CORS Basics**: Browser security model (same-origin), preflight (OPTIONS), required headers (`Access-Control-Allow-*`).
- **Typical CORS Pitfalls**: Mixed protocol (HTTP vs HTTPS), mismatched ports, absent headers, blocked preflight.
- **Local Dev Strategy**: Use permissive CORS; tighten origins for production.
- **Migrations Workflow**: `add` → `update` → optionally `remove` for rollback before applying.
- **Operational Flow**: Start containers (`docker-compose up --build`), verify health, explore via Swagger, run EF migrations if schema changes.

### Reinforcement Points
- Swagger issues often stem from missing doc registration or environment (HTTPS trust) rather than code defects.
- CORS errors originate in the browser; server logs may appear normal.
- Versioned APIs future-proof contracts; Swagger grouping makes consumer discovery clearer.
- Documentation (README) accelerates onboarding and reduces repeated setup questions.

### Suggested Mental Model
Request Flow: Browser Swagger UI → Fetch `/swagger/{version}/swagger.json` → If cross-origin: CORS preflight → Server returns policy headers → UI renders operations.

End of summary.
