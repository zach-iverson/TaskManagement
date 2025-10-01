# Troubleshooting Authorization in ASP.NET Core with JWT

## Issue Summary

**Problem:**
Even after adding the `[Authorize]` attribute to your controller, endpoints were accessible without a JWT token. Instead of returning a `401 Unauthorized`, the API returned a `302 Found` redirecting to `/Account/Login`.

## Root Cause

- By default, `AddIdentity<AppUser, IdentityRole>()` registers cookie authentication as the default scheme.
- Even though JWT Bearer authentication was added, the default scheme remained cookies, causing unauthorized requests to be redirected to the login page instead of returning a 401.

## Solution

- Explicitly set the default authentication and challenge scheme to JWT Bearer in your authentication configuration:

```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
```

- This ensures that all authentication and challenge actions use JWT Bearer, not cookies.

## How to Test

1. **Without Token:**
   ```sh
   curl -i http://localhost:5005/v1/TaskManagement
   ```
   Should return `HTTP/1.1 401 Unauthorized`.

2. **With Invalid Token:**
   ```sh
   curl -i -H "Authorization: Bearer invalidtoken" http://localhost:5005/v1/TaskManagement
   ```
   Should return `HTTP/1.1 401 Unauthorized`.

3. **With Valid Token:**
   - Obtain a valid JWT from your `/api/auth/login` endpoint.
   - Use it in the header:
   ```sh
   curl -i -H "Authorization: Bearer <your-valid-jwt>" http://localhost:5005/v1/TaskManagement
   ```
   Should return `HTTP/1.1 200 OK` and the data.

## Additional Notes

- Ensure `[Authorize]` is present on your controller or actions you want to protect.
- Swagger UI requires you to use the "Authorize" button and enter a valid JWT to test protected endpoints.
- Always restart your application after making changes to authentication configuration.

---

This guide helps you diagnose and resolve issues where JWT authentication is not enforced as expected in ASP.NET Core APIs.

