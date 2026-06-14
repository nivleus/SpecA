# Implementation Plan: Company & Department Management

**Branch**: `main` (feature dir `001-company-departments`) | **Date**: 2026-06-14 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `specs/001-company-departments/spec.md`

## Summary

Build a server-rendered admin web application that maintains **Companies** and the **Departments** that belong to each company, plus a summary **Dashboard**. The application uses ASP.NET Core MVC with Entity Framework Core (Code-First, SQL Server) and a Bootstrap 5 "NiceAdmin"-style UI (fixed header, collapsible sidebar, card-based content, data tables) per `SpecA/UIUX.MD`. CRUD for companies (P1) is the MVP; departments scoped to a company (P2) and the dashboard (P3) follow. **Authentication and authorization are explicitly out of scope** (per user direction and the spec's assumptions) — all screens are open within the trusted internal environment.

## Technical Context

**Language/Version**: C# 13 on .NET 10 (`net10.0`), ASP.NET Core MVC (server-rendered Razor views)

**Primary Dependencies**: Entity Framework Core 10 with `Microsoft.EntityFrameworkCore.SqlServer` and `Microsoft.EntityFrameworkCore.Tools` (Code-First migrations); Bootstrap 5.3.x and Bootstrap Icons (UI); jQuery + jQuery Validation Unobtrusive (client-side validation, already in `wwwroot/lib`)

**Storage**: Microsoft SQL Server, database `SpecA`. Canonical connection string (from constitution, sourced from `appsettings.json`): `Server=.;Database=SpecA;Integrated Security=True;TrustServerCertificate=True;`

**Testing**: Minimal per constitution (Principle IV). No TDD; manual verification of primary flows via `quickstart.md` is the baseline. Optional, narrowly-scoped unit tests only for non-trivial logic (e.g., uniqueness validation) — not a delivery gate.

**Target Platform**: ASP.NET Core web app hosted on Windows (Kestrel/IIS Express in dev); modern browsers for the UI.

**Project Type**: Web application — single ASP.NET Core MVC project (`SpecA/`).

**Performance Goals**: Standard internal web-app responsiveness; pages render quickly for a small/medium dataset (hundreds–low thousands of companies). No special throughput targets.

**Constraints**: No authentication/authorization. Responsive down to common tablet/mobile widths. Connection string and config must come from `appsettings.json`, never hard-coded.

**Scale/Scope**: Small internal admin tool — 2 core entities, ~3 user-facing areas (dashboard, companies, departments), roughly 8–10 screens (lists, details, create/edit/delete forms).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I. Clean MVC Architecture | ✅ PASS | Thin controllers; business rules (e.g., uniqueness, cascade-confirm) in a small service/model layer; view models for forms; no domain entities bound directly where shaping is needed. |
| II. Entity Framework Data Access (Code-First) | ✅ PASS | Single `AppDbContext`; Code-First migrations; connection string from `appsettings.json`; no inline SQL/ADO.NET. |
| III. Bootstrap 5 Responsive UI | ✅ PASS | Bootstrap 5 grid/utilities, shared `_Layout` with sidebar/header per `UIUX.MD`; responsive at tablet/mobile widths. |
| IV. Pragmatic, Minimal Testing | ✅ PASS | No test gate; manual verification via quickstart; optional unit tests only for non-trivial logic. |
| V. Simplicity & Convention Over Configuration | ✅ PASS | Default MVC structure (`Controllers/`, `Models/`, `Views/`, `Data/`); scaffolded CRUD where it fits; no repository/mediator abstractions (direct `DbContext` use). |

**Result**: PASS — no violations. Complexity Tracking not required.

## Project Structure

### Documentation (this feature)

```text
specs/001-company-departments/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output (UI route/screen contracts)
│   └── ui-contracts.md
├── checklists/
│   └── requirements.md  # Created by /speckit-specify
└── tasks.md             # Phase 2 output (/speckit-tasks - NOT created here)
```

### Source Code (repository root)

The existing single ASP.NET Core MVC project under `SpecA/` is used. New/changed paths:

```text
SpecA/
├── Controllers/
│   ├── HomeController.cs          # existing; Dashboard action added (US3)
│   ├── CompaniesController.cs     # NEW — Company CRUD (US1)
│   └── DepartmentsController.cs   # NEW — Department CRUD scoped to a company (US2)
├── Data/
│   └── AppDbContext.cs            # NEW — EF Core DbContext (Companies, Departments)
├── Migrations/                    # NEW — EF Core Code-First migrations
├── Models/
│   ├── Company.cs                 # NEW — entity
│   ├── Department.cs              # NEW — entity
│   ├── ErrorViewModel.cs         # existing
│   └── ViewModels/
│       ├── DashboardViewModel.cs # NEW — counts + recent records (US3)
│       └── DepartmentFormViewModel.cs # NEW — department form + company context
├── Views/
│   ├── Home/
│   │   └── Index.cshtml           # repurposed as Dashboard (US3)
│   ├── Companies/                 # NEW — Index, Details, Create, Edit, Delete
│   ├── Departments/               # NEW — Create, Edit, Delete (Index shown via company Details)
│   └── Shared/
│       └── _Layout.cshtml         # updated — NiceAdmin header + sidebar nav
├── wwwroot/
│   ├── css/site.css               # updated — NiceAdmin theme variables/overrides
│   └── lib/                       # Bootstrap 5 / jQuery (already present)
├── appsettings.json               # updated — DefaultConnection
└── Program.cs                     # updated — register AppDbContext + SQL Server
```

**Structure Decision**: Single-project ASP.NET Core MVC (Option: web application, server-rendered — no separate frontend). This matches the constitution's "Convention Over Configuration" principle and the existing scaffolded `SpecA` project. EF Code-First lives in `Data/` + `Migrations/`; view models live under `Models/ViewModels/`.

## Complexity Tracking

> No constitution violations — section intentionally empty.
