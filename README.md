# Job Search Assistant

PWA to track job applications, store related documents (CV, cover letters, email PDFs), and get AI-assisted insights on job offers (summary + offer↔CV compatibility).
Deployed on Azure using free-tier friendly services and secured with Auth0 (RBAC).

## Why this project
Job searching quickly becomes hard to manage with spreadsheets and scattered emails. This app focuses on:
- structured tracking of applications and process steps,
- traceability of documents exchanged during hiring processes,
- AI assistance limited to high-value tasks (offer summary + compatibility scoring),
- a cost-aware cloud architecture (Azure serverless + free tiers).

## Key features (MVP scope)
- **Applications tracking**: status, notes, dates, last activity tracking
- **Documents**: 1 CV per application (personalized), optional cover letter, and email PDFs (rejection / in-process / other)
- **AI assistance (strict V1)**:
  - job offer summary
  - compatibility score between offer and the application CV (+ explanation)
- **Response likelihood indicator**: computed from dates to estimate whether a response is still likely
- **Auth & RBAC**: USER vs SUPER_ADMIN (Auth0)

## Tech stack
### Frontend
- Angular (PWA)
- Azure Static Web Apps (Free)

### Backend
- Azure Functions (.NET, Consumption plan)
- Auth0 JWT validation + RBAC

### Data & storage
- Azure SQL Database (serverless free offer)
- Azure Blob Storage (private container) for documents
- Application Insights for monitoring

## High-level architecture
- Angular PWA (Azure Static Web Apps)
  - authenticates users via Auth0
  - calls secured API endpoints
- Azure Functions API (.NET)
  - validates Auth0 JWT
  - enforces RBAC (USER vs SUPER_ADMIN) + ownership (`ownerAuth0UserId`)
  - stores metadata in Azure SQL
  - stores files in Azure Blob Storage
- Application Insights collects logs/telemetry

> Architecture diagram: see `/docs` (added in EPIC 0.3).

## Diagrams
- `architecture.png` — High-level Azure architecture (PWA + Functions + SQL + Blob + Auth0)


## Roles & authorization
- **USER**: can access only resources where `ownerAuth0UserId == token.sub`
- **SUPER_ADMIN**: can access all resources

## Status workflow (applications)
`DRAFT` → `APPLIED` → `IN_PROCESS` → (`INTERVIEW` / `ASSESSMENT`) → (`OFFER` / `REJECTED`)  
`WITHDRAWN` is possible from any non-final state.

Final statuses: `OFFER`, `REJECTED`, `WITHDRAWN`.

## Response likelihood (computed)
A computed indicator (not stored) to estimate if a response is still likely:
- Uses `dateApplied`, `closingDate` (optional) and `lastActivityAt`
- Returns: `HIGH | MEDIUM | LOW | NA`

## Repository structure
/web -> Angular PWA
/api -> Azure Functions (.NET)
/docs -> architecture diagram(s), screenshots, and notes


## Environment variables (planned)
### Frontend (Static Web Apps)
- `AUTH0_DOMAIN`
- `AUTH0_CLIENT_ID`
- `AUTH0_AUDIENCE`
- `API_BASE_URL`

### Backend (Azure Functions)
- `Auth0__Domain`
- `Auth0__Audience`
- `Auth0__RolesClaim`
- `ConnectionStrings__Sql`
- `AzureStorage__ConnectionString`
- `AzureStorage__ContainerName`

## Roadmap
- EPIC 1 — Auth & Security (Auth0)
- EPIC 2 — Applications
- EPIC 3 — Documents (Blob Storage)
- EPIC 4 — AI assistance
- EPIC 5 — PWA & Deployment

## License
MIT

## Contributing
See `CONTRIBUTING.md` for commit conventions and branching guidelines.

