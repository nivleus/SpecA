# UI Contracts: Company & Department Management

**Feature**: 001-company-departments | **Date**: 2026-06-14

This is a server-rendered ASP.NET Core MVC application; its "contracts" are the
MVC routes (controller/action), the HTTP verbs, inputs, and resulting views or
redirects. Default route pattern: `{controller}/{action}/{id?}`. **No
authentication** — all routes are publicly reachable within the host.

## Navigation map (sidebar — per UIUX.MD)

- **Dashboard** → `/` (Home/Index)
- **Companies** → `/Companies` (list)
  - A company's **Departments** are reached from the company **Details** page.

## Companies (US1)

| Action | Route | Verb | Input | Result |
|--------|-------|------|-------|--------|
| List | `/Companies` | GET | — | `Companies/Index` view: table of companies (Name, Code, Industry, Status, dept count) with row actions; empty-state when none. |
| Details | `/Companies/Details/{id}` | GET | `id` | `Companies/Details` view: company fields + its departments list with add/edit/delete actions. 404 if not found. |
| Create (form) | `/Companies/Create` | GET | — | `Companies/Create` view with empty form. |
| Create (submit) | `/Companies/Create` | POST | Company form fields (anti-forgery token) | On valid → persist, redirect to `Index`. On invalid → redisplay form with validation messages. |
| Edit (form) | `/Companies/Edit/{id}` | GET | `id` | `Companies/Edit` view pre-filled. 404 if not found. |
| Edit (submit) | `/Companies/Edit/{id}` | POST | `id` + form fields (anti-forgery) | On valid → update `UpdatedAt`, persist, redirect to `Details`/`Index`. On invalid → redisplay with messages. |
| Delete (confirm) | `/Companies/Delete/{id}` | GET | `id` | `Companies/Delete` confirmation view; **warns and lists associated departments** that will also be removed. 404 if not found. |
| Delete (submit) | `/Companies/Delete/{id}` | POST | `id` (anti-forgery) | Cascade-delete company + its departments, redirect to `Index`. |

**Validation contract**: `Name` required; `ContactEmail`/`Website` format-checked when present (see [data-model.md](../data-model.md)). Invalid POST returns the same view with field-level messages and does not persist.

## Departments (US2) — scoped to a company

Departments are always created/edited in the context of a company. `companyId` is
required and carried through the forms; list rendering happens on the company
Details page (FR-007: only that company's departments are shown).

| Action | Route | Verb | Input | Result |
|--------|-------|------|-------|--------|
| Create (form) | `/Departments/Create?companyId={companyId}` | GET | `companyId` | `Departments/Create` view with the owning company fixed/displayed. 404 if company missing. |
| Create (submit) | `/Departments/Create` | POST | `CompanyId` + dept fields (anti-forgery) | On valid → persist, redirect to company `Details`. On invalid (incl. duplicate name in company) → redisplay with messages. |
| Edit (form) | `/Departments/Edit/{id}` | GET | `id` | `Departments/Edit` view pre-filled, company context shown. 404 if not found. |
| Edit (submit) | `/Departments/Edit/{id}` | POST | `id` + fields (anti-forgery) | On valid → update, redirect to company `Details`. On invalid → redisplay with messages. |
| Delete (confirm) | `/Departments/Delete/{id}` | GET | `id` | `Departments/Delete` confirmation view. 404 if not found. |
| Delete (submit) | `/Departments/Delete/{id}` | POST | `id` (anti-forgery) | Delete department, redirect to its company `Details`. |

**Validation contract**: `CompanyId` required and must exist; `Name` required and
**unique within the company** (case-insensitive) — a duplicate returns the form
with a clear validation message and does not persist.

## Dashboard (US3)

| Action | Route | Verb | Input | Result |
|--------|-------|------|-------|--------|
| Dashboard | `/` and `/Home/Index` | GET | — | `Home/Index` view rendered as the NiceAdmin dashboard: KPI cards for **total companies** and **total departments**, and a list of the most recently added/updated records. Counts reflect current data (SC-005). |

## Cross-cutting UI contract (all screens)

- Rendered within the shared `_Layout` (fixed header + collapsible sidebar + footer) per `SpecA/UIUX.MD`; responsive to tablet/mobile widths (SC-007, FR-015).
- All state-changing POSTs include an anti-forgery token.
- Delete actions always require an explicit confirmation step before removal (SC-003, FR-005/FR-009).
- Empty states show a clear "add the first record" message (FR-017).
- Not-found ids return a 404 / friendly error.
