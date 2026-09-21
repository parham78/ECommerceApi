# ECommerceWeb

Angular frontend for the ecommerce portfolio project.

## Development

Run the ASP.NET Core backend from the repository root:

```powershell
cd C:\csharp-project\OrderManagementApi
dotnet run
```

The backend runs at:

```text
http://localhost:5289
```

Run the Angular frontend in a second terminal:

```powershell
cd C:\csharp-project\OrderManagementApi\ECommerceWeb
npm start
```

The frontend runs at:

```text
http://localhost:4200
```

During development, Angular proxies `/api/**` requests to the ASP.NET Core backend.

## Current frontend scope

The current storefront includes:

- public product catalog
- real backend product data
- responsive product cards
- local product imagery
- loading, empty, and error states
- retry behavior
- backend-driven pagination metadata
- URL-based page and page-size state
- responsive page-size selector
- 404 page
- Canadian currency formatting

Authentication, basket, checkout, customer account, and admin features are planned for later milestones.

## Commands

Production build:

```powershell
npm run build
```

Run tests once:

```powershell
npm test -- --watch=false
```
