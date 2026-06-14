# Quickstart & Validation Guide: Company & Department Management

**Feature**: 001-company-departments | **Date**: 2026-06-14

This guide runs the application and manually validates the feature end-to-end.
Per the constitution (minimal testing), manual verification of these flows is the
correctness baseline — there is no automated test gate.

## Prerequisites

- .NET 10 SDK
- SQL Server reachable at the default local instance (`Server=.`) using Windows
  Integrated Security (LocalDB or a local SQL Server instance)
- EF Core tools: `dotnet tool install --global dotnet-ef` (if not already installed)

## One-time setup

From the repository root (`C:\Claude Project\SKit\SpecA\SpecA`):

```powershell
# 1. Restore / build
dotnet build SpecA/SpecA.csproj

# 2. Apply Code-First migrations to create the SpecA database
#    (connection string: ConnectionStrings:DefaultConnection in SpecA/appsettings.json)
dotnet ef database update --project SpecA/SpecA.csproj
```

> If migrations do not yet exist, they are created during implementation with:
> `dotnet ef migrations add InitialCreate --project SpecA/SpecA.csproj`

## Run

```powershell
dotnet run --project SpecA/SpecA.csproj
```

Open the shown URL (e.g., `https://localhost:xxxx`). The app opens on the
**Dashboard**.

## Validation scenarios

Map to user stories and success criteria. See [contracts/ui-contracts.md](contracts/ui-contracts.md)
for routes and [spec.md](spec.md) for acceptance scenarios.

### US1 — Manage Companies (P1, MVP)

1. From the sidebar, open **Companies**. Expect an empty-state message (FR-017).
2. Click **Add Company**, submit with a blank name → expect a validation message, no record created (SC-006).
3. Add a company with a name (and optional details) → it appears in the list (US1-AS1).
4. Open the company's **Details** → all entered fields shown (US1-AS2).
5. **Edit** a field, save → updated value shows in list/details (US1-AS3).
6. **Delete** the company → confirmation required, then it disappears from the list (US1-AS4, SC-003).

### US2 — Manage Departments within a company (P2)

1. Open an existing company's **Details**. Expect an empty departments list with an add prompt.
2. **Add Department** with a name → appears under that company (US2-AS1).
3. Add a second department; create a **different** company and confirm its departments list is empty → no cross-company leakage (US2-AS2, SC-004).
4. Try to add a department whose name duplicates an existing one **in the same company** → expect a validation message, not saved (uniqueness rule).
5. **Edit** a department → changes reflected (US2-AS3).
6. **Delete** a department → confirmation required, then removed (US2-AS4, SC-003).
7. **Delete the company** while it has departments → confirmation warns about associated departments; confirming removes the company and its departments (US2-AS5, FR-011).

### US3 — Dashboard (P3)

1. Create a known number of companies and departments.
2. Open the **Dashboard** → KPI cards show totals matching what you entered (US3-AS1, SC-005).
3. Confirm the recent-records list shows your latest additions (US3-AS2).
4. Use the sidebar to move between Dashboard, Companies, and a company's Departments (US3-AS3, FR-014).

### Cross-cutting

- Resize the browser to tablet/mobile width → sidebar collapses, content stays usable (SC-007, FR-015).
- All delete actions require explicit confirmation (SC-003).

## Expected outcome

All scenarios above pass with no errors, the `SpecA` database contains the
`Companies` and `Departments` tables, and dashboard counts always match stored
data. This constitutes feature acceptance for this iteration.
