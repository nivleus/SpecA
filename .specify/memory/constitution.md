<!--
SYNC IMPACT REPORT
==================
Version change: (template / unversioned) → 1.0.0
Bump rationale: Initial ratification of a concrete constitution from the
  placeholder template. MAJOR baseline (1.0.0) per semantic versioning.

Modified principles:
  - [PRINCIPLE_1_NAME] → I. Clean MVC Architecture
  - [PRINCIPLE_2_NAME] → II. Entity Framework Data Access (Code-First)
  - [PRINCIPLE_3_NAME] → III. Bootstrap 5 Responsive UI
  - [PRINCIPLE_4_NAME] → IV. Pragmatic, Minimal Testing
  - [PRINCIPLE_5_NAME] → V. Simplicity & Convention Over Configuration

Added sections:
  - Technology Stack & Standards (was [SECTION_2_NAME])
  - Development Workflow & Quality Gates (was [SECTION_3_NAME])

Removed sections: none

Templates requiring updates:
  - .specify/templates/plan-template.md ✅ aligned (Constitution Check gate
    references this file; no structural change required)
  - .specify/templates/spec-template.md ✅ aligned (testing already optional)
  - .specify/templates/tasks-template.md ✅ aligned (tests already marked
    OPTIONAL, consistent with Principle IV)
  - .specify/templates/checklist-template.md ✅ no changes required

Follow-up TODOs: none
-->

# IT Asset Manager Constitution

## Core Principles

### I. Clean MVC Architecture

The application MUST follow the ASP.NET MVC pattern with strict separation of
concerns. Controllers MUST remain thin: they coordinate requests, invoke
services or the data layer, and return views or results — they MUST NOT contain
business rules or direct presentation logic. Views MUST contain only display
logic and MUST be backed by strongly typed models or view models; raw
domain/EF entities MUST NOT be passed to views when input binding or shaping is
required (use view models). Business logic MUST live in services or model
classes, never in views.

**Rationale**: A clean, predictable MVC layering keeps the codebase navigable,
testable where it matters, and resistant to the "fat controller" rot that makes
asset-management apps unmaintainable as entities multiply.

### II. Entity Framework Data Access (Code-First)

All persistent data access MUST go through Entity Framework. A Code-First
`DbContext` is the single source of truth for the schema, and schema changes
MUST be expressed as EF migrations — never hand-edited against the live
database. Direct ADO.NET or inline SQL is prohibited except for a deliberate,
documented performance exception approved per the amendment process. The
database connection string MUST be
`Server=.;Database=SpecA;Integrated Security=True;TrustServerCertificate=True;`
and MUST be sourced from configuration (e.g., `appsettings.json`), never
hard-coded in controllers, services, or the `DbContext`.

**Rationale**: Centralizing data access in EF Code-First gives one migration
history, consistent change tracking, and a single, configurable connection
point — essential for a SQL Server-backed system of record.

### III. Bootstrap 5 Responsive UI

The user interface MUST be built on Bootstrap 5. Layout, spacing, and
components MUST use Bootstrap's grid and utility classes rather than ad-hoc
custom CSS; custom styles are permitted only where Bootstrap cannot express the
need. All primary screens (asset lists, detail, create/edit forms) MUST be
responsive and usable down to common tablet/mobile widths. A shared `_Layout`
view MUST provide consistent navigation and structure across all pages.

**Rationale**: Standardizing on Bootstrap 5 yields a consistent, responsive,
low-maintenance UI without bespoke styling debt, and keeps contributors
productive with a well-known component vocabulary.

### IV. Pragmatic, Minimal Testing

Testing is intentionally minimized. Automated tests are OPTIONAL and MUST NOT be
treated as a gate that blocks delivery. When tests are written, they SHOULD
target only non-trivial business logic or regression-prone areas — not
framework glue, scaffolded CRUD, or EF/MVC plumbing. Test-first (TDD) is NOT
required for this project. Manual verification of primary user flows is the
accepted baseline for correctness before merge.

**Rationale**: For a focused internal asset-management tool, exhaustive
automated testing costs more than it returns; effort is better spent on a
correct domain model and a clean UI. This principle deliberately overrides any
test-first default in the workflow templates.

### V. Simplicity & Convention Over Configuration

The solution MUST favor framework conventions and the simplest design that
works. Default ASP.NET MVC project structure (`Controllers/`, `Models/`,
`Views/`, `Data/`) MUST be used unless a documented reason exists to deviate.
Additional abstractions, patterns, or libraries (repository wrappers, mediators,
extra layers) MUST NOT be introduced unless a concrete, current need justifies
them — YAGNI applies. Scaffolding MAY be used to generate CRUD where it fits.

**Rationale**: Conventional, minimal structure keeps the app approachable and
fast to evolve, and prevents premature architecture from obscuring a
fundamentally straightforward CRUD-centric domain.

## Technology Stack & Standards

The project is bound to the following stack; deviations require an amendment:

- **Framework**: ASP.NET MVC (server-rendered Razor views).
- **ORM / Data**: Entity Framework, Code-First with migrations.
- **Database**: Microsoft SQL Server.
- **UI**: Bootstrap 5.
- **Connection string** (canonical, configuration-sourced):
  `Server=.;Database=SpecA;Integrated Security=True;TrustServerCertificate=True;`

Standards:

- Configuration and secrets MUST come from configuration files / environment,
  not source code constants.
- EF migrations MUST be committed alongside the model changes that require them.
- View models MUST be used for any form that does not map cleanly to a single
  entity.

## Development Workflow & Quality Gates

- **Constitution Check**: Each plan (`/speckit-plan`) MUST verify alignment with
  these principles before design proceeds. Violations MUST be recorded in the
  plan's Complexity Tracking with justification or be removed.
- **Quality gate**: The build MUST compile and EF migrations MUST apply cleanly
  before merge. Automated tests are NOT a required gate (see Principle IV).
- **Verification**: Primary user flows MUST be manually exercised before a
  feature is considered done.
- **Review**: Changes SHOULD be reviewed for adherence to MVC layering, EF usage,
  and Bootstrap consistency.

## Governance

This constitution supersedes other development practices for the IT Asset
Manager project. Amendments MUST be proposed with a rationale, recorded in this
file, and versioned per the policy below.

- **Versioning policy** (semantic):
  - **MAJOR**: Backward-incompatible governance or principle removals/redefinitions.
  - **MINOR**: New principle or section added, or materially expanded guidance.
  - **PATCH**: Clarifications, wording, and non-semantic refinements.
- **Compliance review**: Plans, specs, and tasks MUST be checked against these
  principles. Any deliberate deviation MUST be justified in the plan's Complexity
  Tracking section.
- **Runtime guidance**: Use the Spec Kit templates under `.specify/templates/`
  for plan, spec, and task generation; they are the operational complement to
  this constitution.

**Version**: 1.0.0 | **Ratified**: 2026-06-14 | **Last Amended**: 2026-06-14
