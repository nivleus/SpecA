# Phase 1 Data Model: Company & Department Management

**Feature**: 001-company-departments | **Date**: 2026-06-14

Two entities with a one-to-many relationship: a **Company** has many **Departments**;
a **Department** belongs to exactly one **Company**. Mapped via EF Core Code-First to
SQL Server database `SpecA`.

## Entity: Company

Represents an organization tracked by the application.

| Field | Type | Required | Constraints / Notes |
|-------|------|----------|---------------------|
| `Id` | int | yes | Primary key, identity |
| `Name` | string | yes | Max 200. Company name. Not required to be unique. |
| `Code` | string | no | Max 50. Short code / registration reference. |
| `Industry` | string | no | Max 100. Category/industry. |
| `Address` | string | no | Max 300. |
| `ContactEmail` | string | no | Max 150. Email-format validation when present. |
| `ContactPhone` | string | no | Max 50. |
| `Website` | string | no | Max 200. URL-format validation when present. |
| `IsActive` | bool | yes | Status flag; defaults to `true`. |
| `CreatedAt` | DateTime | yes | Set on create (UTC). |
| `UpdatedAt` | DateTime | no | Set on edit (UTC). |
| `Departments` | ICollection&lt;Department&gt; | — | Navigation; zero or more. |

**Relationships**: One Company → many Departments. Cascade delete (deleting a
Company deletes its Departments — see [research.md](research.md) Decision 4).

**Validation rules** (from FR-001, FR-012):
- `Name` is required and non-empty after trim.
- `ContactEmail`, when provided, must be a valid email format.
- `Website`, when provided, must be a valid URL format.
- String fields enforce the max lengths above.

## Entity: Department

Represents an organizational unit belonging to a single company.

| Field | Type | Required | Constraints / Notes |
|-------|------|----------|---------------------|
| `Id` | int | yes | Primary key, identity |
| `CompanyId` | int | yes | Foreign key → Company.Id. |
| `Name` | string | yes | Max 150. Unique **within** the parent company. |
| `Code` | string | no | Max 50. |
| `Description` | string | no | Max 500. |
| `Manager` | string | no | Max 150. Department head / manager name. |
| `Phone` | string | no | Max 50. Department contact phone number. Phone-format validation when present. |
| `IsActive` | bool | yes | Status flag; defaults to `true`. |
| `CreatedAt` | DateTime | yes | Set on create (UTC). |
| `UpdatedAt` | DateTime | no | Set on edit (UTC). |
| `Company` | Company | — | Navigation to owning company. |

**Relationships**: Many Departments → one Company (`CompanyId`, required).

**Validation rules** (from FR-006, FR-007, FR-010, FR-012):
- `CompanyId` is required and must reference an existing Company.
- `Name` is required and non-empty after trim.
- `Name` must be unique within its `CompanyId` (case-insensitive); enforced in
  the controller/service before save, backed by a filtered unique index
  `(CompanyId, Name)` as defense-in-depth.
- `Phone`, when provided, must be a valid phone-number format.
- String fields enforce the max lengths above.

## Indexes

- `Department (CompanyId)` — non-unique index for fast per-company listing (FR-007).
- `Department (CompanyId, Name)` — unique index enforcing per-company name uniqueness.

## State / Lifecycle

No workflow state machine. Records are created → optionally edited → deleted.
`IsActive` is a simple status flag (active/inactive) with no transition rules.

## Mapping Notes (EF Core)

- Single `AppDbContext` with `DbSet<Company> Companies` and `DbSet<Department> Departments`.
- Relationship and cascade configured via Fluent API in `OnModelCreating`
  (`HasMany(c => c.Departments).WithOne(d => d.Company).OnDelete(DeleteBehavior.Cascade)`).
- Code-First migrations create the `SpecA` schema; the initial migration produces
  `Companies` and `Departments` tables with the indexes above.
