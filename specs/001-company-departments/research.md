# Phase 0 Research: Company & Department Management

**Feature**: 001-company-departments | **Date**: 2026-06-14

The Technical Context had no open `NEEDS CLARIFICATION` items — the constitution
fixes the stack and the spec records the functional assumptions. This document
captures the key technical decisions and the few choices worth recording.

## Decision 1: EF Core (not EF6) for data access

- **Decision**: Use Entity Framework **Core 10** with the SQL Server provider, Code-First with migrations.
- **Rationale**: The project targets `net10.0` (ASP.NET Core, `Microsoft.NET.Sdk.Web`). EF6 is .NET Framework-era; EF Core is the only EF flavor that fits .NET 10. This satisfies the constitution's "Entity Framework, Code-First with migrations" intent on the actual runtime.
- **Alternatives considered**: EF6 (incompatible with .NET 10); Dapper/raw ADO.NET (prohibited by constitution Principle II).

## Decision 2: Connection string & configuration

- **Decision**: Store the connection string as `ConnectionStrings:DefaultConnection` in `appsettings.json` with value `Server=.;Database=SpecA;Integrated Security=True;TrustServerCertificate=True;`; read it in `Program.cs` via `builder.Configuration.GetConnectionString("DefaultConnection")`.
- **Rationale**: Constitution Principle II requires config-sourced, never hard-coded connection strings, and fixes this exact canonical string (DB name `SpecA`).
- **Alternatives considered**: Hard-coded string (prohibited); user secrets/env vars (overkill for an internal tool using integrated security; may be layered later without changing code).

## Decision 3: No authentication / authorization

- **Decision**: Do not add any authentication or authorization. All controllers/actions are open.
- **Rationale**: Explicit user direction for this plan, and the spec's Assumptions already scope auth out (trusted internal tool, single admin role).
- **Alternatives considered**: ASP.NET Core Identity / cookie auth — deferred; would add schema, UI, and middleware not needed for the MVP and contradicting the directive.

## Decision 4: Department deletion semantics on company delete

- **Decision**: Configure the `Company → Departments` relationship with **cascade delete**; the UI requires an explicit confirmation that names the affected departments before deleting a company.
- **Rationale**: Satisfies FR-011 (no orphaned departments) and the edge case in the spec. Cascade at the DB level keeps integrity; the confirmation screen keeps it intentional.
- **Alternatives considered**: Restrict/prevent delete when departments exist (rejected — spec allows delete after confirmation); soft-delete (rejected — spec says no recycle-bin requirement).

## Decision 5: Department uniqueness within a company

- **Decision**: Department name must be unique **within its parent company**; enforce via a server-side validation check before save (and a filtered unique index as defense-in-depth). Company names are not required to be unique.
- **Rationale**: Matches the spec's assumption (FR-012 validation). A scoped uniqueness rule is light business logic that lives in the service/controller, consistent with Principle I.
- **Alternatives considered**: Global department-name uniqueness (wrong — departments named "HR" legitimately repeat across companies); no uniqueness (rejected — spec requires it).

## Decision 6: UI approach — Razor views + Bootstrap 5, NiceAdmin layout

- **Decision**: Use server-rendered Razor views with a shared `_Layout.cshtml` implementing the NiceAdmin pattern from `UIUX.MD` (fixed header, collapsible sidebar, card content, borderless Bootstrap tables). Use the Bootstrap/jQuery assets already in `wwwroot/lib`; add Bootstrap Icons. Custom theme variables in `site.css`.
- **Rationale**: Constitution Principle III (Bootstrap 5) and Principle V (conventions). The sidebar nav maps directly to the app areas: Dashboard, Companies (and a company's Departments).
- **Alternatives considered**: SPA (React/Vue) — rejected (out of stack, violates simplicity); pulling the full NiceAdmin template wholesale — unnecessary; we adopt its layout/visual language only, scoped to real features (sample revenue widgets are style reference only).

## Decision 7: Testing posture

- **Decision**: No automated test gate. Validate via `quickstart.md` manual flows. Optionally add a small unit test for the department-uniqueness rule if it grows non-trivial.
- **Rationale**: Constitution Principle IV (minimal testing) explicitly overrides any test-first default.
- **Alternatives considered**: Full integration/contract test suite — rejected as disproportionate for this internal CRUD tool.

## Resolved Unknowns

| Item | Resolution |
|------|------------|
| EF flavor on .NET 10 | EF Core 10 + SqlServer provider |
| Connection config location | `appsettings.json` → `ConnectionStrings:DefaultConnection` |
| Auth | None (out of scope) |
| Company-delete behavior | Cascade delete + confirmation UI |
| Department name uniqueness | Unique within parent company |
| UI framework | Razor + Bootstrap 5, NiceAdmin layout |
| Testing | Manual via quickstart; optional unit test |
