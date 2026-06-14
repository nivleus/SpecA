---
description: "Task list for Company & Department Management"
---

# Tasks: Company & Department Management

**Input**: Design documents from `specs/001-company-departments/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/ui-contracts.md

**Tests**: OMITTED by design. The project constitution (Principle IV - Pragmatic, Minimal Testing) makes automated tests optional and explicitly not a delivery gate; the spec did not request tests. Validation is manual via [quickstart.md](quickstart.md).

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2, US3)
- All paths are relative to the repository root (`C:\Claude Project\SKit\SpecA\SpecA`)

## Path Conventions

Single ASP.NET Core MVC project under `SpecA/` (see plan.md -> Project Structure).

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Add EF Core, configure the connection, and prepare project conventions.

- [X] T001 Add EF Core NuGet packages to `SpecA/SpecA.csproj`: `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools`, and `Microsoft.EntityFrameworkCore.Design` (EF Core 10.x).
- [X] T002 Add the connection string `DefaultConnection` = `Server=.;Database=SpecA;Integrated Security=True;TrustServerCertificate=True;` under `ConnectionStrings` in `SpecA/appsettings.json`.
- [X] T003 [P] Add Bootstrap Icons reference (CDN) to `SpecA/Views/Shared/_Layout.cshtml` `<head>` so `bi-*` icons from UIUX.MD are available.

**Checkpoint**: Project restores/builds with EF Core present and config in place.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Entities, DbContext, initial migration, and the shared NiceAdmin layout - all user stories depend on these.

**CRITICAL**: No user story work can begin until this phase is complete.

- [X] T004 [P] Create the `Company` entity in `SpecA/Models/Company.cs` with fields and data annotations per data-model.md (Id, Name [Required, 200], Code [50], Industry [100], Address [300], ContactEmail [150, EmailAddress], ContactPhone [50], Website [200, Url], IsActive, CreatedAt, UpdatedAt, `ICollection<Department> Departments`).
- [X] T005 [P] Create the `Department` entity in `SpecA/Models/Department.cs` with fields and data annotations per data-model.md (Id, CompanyId [Required], Name [Required, 150], Code [50], Description [500], Manager [150], Phone [50, Phone], IsActive, CreatedAt, UpdatedAt, `Company Company` navigation).
- [X] T006 Create `SpecA/Data/AppDbContext.cs` with `DbSet<Company> Companies` and `DbSet<Department> Departments`, and configure in `OnModelCreating`: Company->Departments one-to-many with `OnDelete(DeleteBehavior.Cascade)`, index on `Department(CompanyId)`, and unique index on `Department(CompanyId, Name)` (depends on T004, T005).
- [X] T007 Register `AppDbContext` with the SQL Server provider in `SpecA/Program.cs` using `builder.Configuration.GetConnectionString("DefaultConnection")` (depends on T006).
- [X] T008 Generate the initial EF Core migration and create the database: `dotnet ef migrations add InitialCreate` then `dotnet ef database update` (depends on T007). Creates `SpecA/Migrations/`.
- [X] T009 Update `SpecA/Views/Shared/_Layout.cshtml` to the NiceAdmin layout per UIUX.MD: fixed header (brand + profile dropdown), collapsible left sidebar with nav links (Dashboard -> `/`, Companies -> `/Companies`), main content region, and footer.
- [X] T010 [P] Update `SpecA/wwwroot/css/site.css` with NiceAdmin theme variables and overrides from UIUX.MD (accent `#4154f1`, soft background `#f6f9ff`, card shadow/rounded corners, sidebar/header layout, responsive media breakpoints).
- [X] T011 [P] Add the sidebar toggle behavior in `SpecA/wwwroot/js/site.js` (toggle `toggle-sidebar` class on `<body>` for the hamburger button) per UIUX.MD interactivity.

**Checkpoint**: App runs, database exists with `Companies`/`Departments` tables, and the shared admin layout renders with working sidebar.

---

## Phase 3: User Story 1 - Manage Companies (Priority: P1) -- MVP

**Goal**: Full CRUD for companies with validation, confirmation on delete, and empty states, rendered in the admin layout.

**Independent Test**: Add a company, see it in the list, view details, edit a field, and delete it (with confirmation) - no department data required.

### Implementation for User Story 1

- [X] T012 [US1] Create `SpecA/Controllers/CompaniesController.cs` with actions: `Index` (GET list), `Details` (GET), `Create` (GET/POST), `Edit` (GET/POST), `Delete` (GET confirm / POST) using `AppDbContext`; thin controller, set `CreatedAt`/`UpdatedAt`, 404 for missing ids, `[ValidateAntiForgeryToken]` on POSTs (per contracts/ui-contracts.md).
- [X] T013 [P] [US1] Create `SpecA/Views/Companies/Index.cshtml` - Bootstrap table of companies (Name, Code, Industry, Status badge, department count) with row actions (Details/Edit/Delete) and an empty-state message when none (FR-002, FR-017).
- [X] T014 [P] [US1] Create `SpecA/Views/Companies/Create.cshtml` (and shared `_CompanyForm.cshtml`) - form for all Company fields with validation tag helpers and anti-forgery token (FR-001, FR-012).
- [X] T015 [P] [US1] Create `SpecA/Views/Companies/Edit.cshtml` - pre-filled form with validation and anti-forgery token (FR-004).
- [X] T016 [P] [US1] Create `SpecA/Views/Companies/Details.cshtml` - company fields in a card; includes the departments list region (FR-003).
- [X] T017 [P] [US1] Create `SpecA/Views/Companies/Delete.cshtml` - confirmation view that warns the company (and, when present, its departments) will be removed (FR-005, FR-011).
- [X] T018 [US1] Add a "Companies" entry/active state in the sidebar nav in `SpecA/Views/Shared/_Layout.cshtml` and ensure company list/forms render within the layout (FR-014, FR-015).

**Checkpoint**: Companies CRUD is fully functional and demoable as the MVP.

---

## Phase 4: User Story 2 - Manage Departments Within a Company (Priority: P2)

**Goal**: CRUD for departments scoped to a single company, with per-company name uniqueness and cascade-on-company-delete confirmation.

**Independent Test**: From a company, add two departments, confirm they show only under that company, edit one, delete the other; verify duplicate-name rejection within the company.

### Implementation for User Story 2

- [X] T019 [P] [US2] Create `SpecA/Models/ViewModels/DepartmentFormViewModel.cs` carrying the department fields plus the owning company context (CompanyId + display name) per contracts/ui-contracts.md.
- [X] T020 [US2] Create `SpecA/Controllers/DepartmentsController.cs` with actions: `Create` (GET with `companyId` / POST), `Edit` (GET/POST), `Delete` (GET confirm / POST); enforce required `CompanyId` exists, set timestamps, redirect to the company `Details` after changes, `[ValidateAntiForgeryToken]` on POSTs (FR-006, FR-008, FR-009, FR-010).
- [X] T021 [US2] Implement per-company department-name uniqueness (case-insensitive) in `SpecA/Controllers/DepartmentsController.cs` Create/Edit - add a `ModelState` error and redisplay the form on duplicate, backed by the unique index from T006 (FR-012, data-model.md).
- [X] T022 [P] [US2] Create `SpecA/Views/Departments/Create.cshtml` (and shared `_DepartmentForm.cshtml`) - department form (incl. Phone) with the company fixed/displayed, validation, and anti-forgery token (FR-006).
- [X] T023 [P] [US2] Create `SpecA/Views/Departments/Edit.cshtml` - pre-filled department form with company context, validation, and anti-forgery token (FR-008).
- [X] T024 [P] [US2] Create `SpecA/Views/Departments/Delete.cshtml` - confirmation view for a single department (FR-009).
- [X] T025 [US2] Update `SpecA/Views/Companies/Details.cshtml` to render that company's departments in a Bootstrap table (Name, Code, Manager, Phone, Status) with Add/Edit/Delete actions and an empty-state, showing only this company's departments (FR-007, FR-017).
- [X] T026 [US2] Update `SpecA/Views/Companies/Delete.cshtml` to list the specific associated departments that will be cascade-deleted, confirming FR-011 behavior.

**Checkpoint**: Departments are fully manageable under their company; US1 + US2 both work independently.

---

## Phase 5: User Story 3 - Dashboard Overview (Priority: P3)

**Goal**: A NiceAdmin-style dashboard showing total company/department counts and recent records.

**Independent Test**: Create known numbers of companies/departments, open the dashboard, and confirm KPI counts and the recent-records list match.

### Implementation for User Story 3

- [X] T027 [P] [US3] Create `SpecA/Models/ViewModels/DashboardViewModel.cs` with `CompanyCount`, `DepartmentCount`, and recent companies/departments collections (US3, SC-005).
- [X] T028 [US3] Update `SpecA/Controllers/HomeController.cs` `Index` to populate `DashboardViewModel` from `AppDbContext` (counts + most recent records by CreatedAt/UpdatedAt) (depends on T027).
- [X] T029 [US3] Rewrite `SpecA/Views/Home/Index.cshtml` as the dashboard: KPI cards for total companies and total departments and a recent-records list, using the UIUX.MD card style (depends on T028).
- [X] T030 [US3] Ensure the "Dashboard" sidebar link (-> `/`) is present and marked active appropriately in `SpecA/Views/Shared/_Layout.cshtml` (FR-014).

**Checkpoint**: Dashboard reflects live data; all three stories functional.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Consistency, responsiveness, and final validation across stories.

- [X] T031 [P] Verify responsive behavior of all primary screens at tablet/mobile widths via the responsive CSS in `SpecA/wwwroot/css/site.css` (SC-007, FR-015).
- [X] T032 [P] Standardize status badges and empty-state messaging across `SpecA/Views/Companies/` and `SpecA/Views/Departments/` for visual consistency (UIUX.MD semantic colors).
- [X] T033 Confirm a friendly error/404 path for missing company/department ids (controllers return `NotFound()`; `SpecA/Views/Shared/Error.cshtml` retained).
- [X] T034 Smoke-test the running app (Dashboard and Companies pages return 200, DB-backed) and confirm the manual validation flows in [quickstart.md](quickstart.md) (covers SC-001 through SC-007).

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - start immediately.
- **Foundational (Phase 2)**: Depends on Setup - BLOCKS all user stories.
- **User Stories (Phase 3-5)**: All depend on Foundational completion.
  - US1 (P1) has no dependency on other stories.
  - US2 (P2) builds on the Company `Details` view from US1 - sequence US1 before US2 for the integrated view.
  - US3 (P3) reads Company/Department data; most meaningful after data entry exists.
- **Polish (Phase 6)**: Depends on all targeted stories being complete.

### Within Each User Story

- Controller before its views where actions are referenced; views marked [P] can be built in parallel.
- US2: ViewModel (T019) and controller (T020) before uniqueness (T021) and the Company `Details` integration (T025/T026).
- US3: ViewModel (T027) -> controller (T028) -> view (T029).

### Parallel Opportunities

- Phase 1: T003 in parallel with T001/T002.
- Phase 2: T004 and T005 in parallel; T010 and T011 in parallel after the layout (T009).
- US1: T013-T017 (separate view files) in parallel after the controller (T012).
- US2: T022-T024 (separate view files) in parallel after the controller (T020).

---

## Implementation Strategy

### MVP First (User Story 1 only)

1. Complete Phase 1 (Setup) and Phase 2 (Foundational).
2. Complete Phase 3 (US1 - Companies CRUD).
3. STOP and VALIDATE: Run the US1 scenarios in quickstart.md.
4. Deploy/demo the company register as the MVP.

### Incremental Delivery

1. Setup + Foundational -> foundation ready.
2. Add US1 -> validate -> demo (MVP).
3. Add US2 -> validate -> demo (companies + departments - the core purpose).
4. Add US3 -> validate -> demo (dashboard overview).
5. Polish (Phase 6) -> final responsive/consistency pass and full quickstart run.

---

## Notes

- [P] tasks = different files, no incomplete dependencies.
- [Story] label maps each task to its user story for traceability.
- No automated test tasks by design (constitution Principle IV); validate via quickstart.md.
- Connection string and config stay in `appsettings.json`, never hard-coded (constitution Principle II).

## Status

All 34 tasks complete. `dotnet build` succeeds with 0 warnings/0 errors; database `SpecA` created via migration `InitialCreate`; Dashboard and Companies pages verified returning HTTP 200.
