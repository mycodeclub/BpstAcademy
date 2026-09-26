# BPST Edu Backend: Plan

**Status:** agreed direction, development starting. Production host not chosen yet (see §6).
**Date:** 26 Sep 2026
**Related:** `NewLayout/Landing Page UI` (public site prototype), `NewLayout/AdminTemplate` (portal prototype; roles, lifecycles and page inventory in its `PLAN.md`).

---

## 1. Stack

| Part | Choice |
|---|---|
| Framework | .NET 10 (LTS), ASP.NET Core MVC |
| Database | PostgreSQL, EF Core 10 + Npgsql |
| Local environment | Docker Compose: Postgres, Adminer (DB browser), Mailpit (catches outgoing email) |
| Login and users | ASP.NET Core Identity (cookie login), permissions stored as role claims |
| Background jobs | Hangfire with Postgres storage (lead SLA, fee reminders, late fines) |
| Validation | FluentValidation |
| PDFs (receipts, GST invoices, certificates, payslips) | QuestPDF |
| Payments | Razorpay (₹49 pre-book, fees), webhook signature checks |
| Logging | Serilog |

## 2. Authorization: permissions, not roles

Areas per role failed before because many pages are shared by several roles. The access table in `AdminTemplate/PLAN.md` §2 shows the real pattern: most pages are shared, and what differs is how much data each role sees.

| Concept | What it is | Example |
|---|---|---|
| **Area** | A business module, never a role | `Crm`, `Academics`, `Finance`, `Hr` |
| **Permission** | What a page or action requires | `attendance.view`, `leads.assign`, `fees.collect` |
| **Scope** | How much data the user sees | `All`, `OwnBatches`, `Own` |

Rules:
- Roles (Admin, Counsellor, HR+Accounts, Trainer, Student, Employee) are named bundles of permissions stored in the database and editable from Settings.
- Controllers and actions check permissions (`[HasPermission(...)]`), never role names.
- Services apply the user's scope to every query, so one page serves every role with the right data.
- A user with several roles gets the union of permissions and the widest scope.
- The sidebar is built from `NavRegistry`: each item declares the permission it needs.
- Single records get resource-based authorization ("can this counsellor edit this lead?").
- Onboarding gates (unverified student, onboarding employee; `AdminTemplate/PLAN.md` §3.2–3.3) are one global filter that redirects to the onboarding checklist.
- An integration test opens every portal page as every role and asserts allowed/blocked against the access table.

Identity details: students and employees sign in with their ID (`BPST26S0001`, `BPST26E001`) and must change the temporary password at first login. Permission claims are added to the login cookie once, not loaded per request.

## 3. Solution structure

```
BpstEdu/
├─ BpstEdu.slnx
├─ docker-compose.yml               # postgres, adminer, mailpit
├─ .env.example                     # connection strings and keys (real values never committed)
├─ Directory.Build.props            # nullable, warnings as errors, target framework
├─ Directory.Packages.props         # central NuGet versions
├─ src/
│  ├─ BpstEdu.Domain/               # entities + status rules, no dependencies
│  │  ├─ Common/  Crm/  Academics/  Finance/  People/  Hr/  Verification/
│  ├─ BpstEdu.Application/          # services, DTOs, validators
│  │  ├─ Security/                  # Permissions, DefaultRoles, DataScope, ICurrentUser
│  │  ├─ Crm/  Academics/  Finance/  Hr/  Verification/  Placement/
│  │  └─ Abstractions/              # IEmailSender, IPaymentGateway, IPdfRenderer, IFileStore, IClock
│  ├─ BpstEdu.Infrastructure/
│  │  ├─ Persistence/               # AppDbContext, Configurations, Migrations, Interceptors, Seed
│  │  ├─ Identity/                  # AppUser, AppRole, PermissionClaimsFactory
│  │  ├─ Payments/Razorpay/  Email/  Pdf/  Storage/  Jobs/
│  │  └─ DependencyInjection.cs
│  └─ BpstEdu.Web/
│     ├─ Controllers/               # public site: Home, Courses, Categories, Prebook, Contact, Verify
│     ├─ Views/
│     ├─ Areas/                     # routed under /portal/{area}/{controller}/{action}
│     │  ├─ Account/  Crm/  Academics/  Finance/  Verification/  Hr/  Placement/  Admin/
│     │  └─ Me/                     # self-service dashboard + "my" pages for students, trainers, employees
│     ├─ Authorization/             # HasPermissionAttribute, PermissionPolicyProvider, OnboardingGateFilter
│     ├─ Navigation/                # NavRegistry
│     └─ wwwroot/                   # landing + portal assets
└─ tests/
   ├─ BpstEdu.UnitTests/
   └─ BpstEdu.IntegrationTests/     # Testcontainers Postgres
```

- Layouts: `_Layout` (public site, SEO/GEO head from `SeoMeta`), `_AdminLayout` (portal shell, `noindex`, set by each area's `_ViewStart`), `_AuthLayout` (sign-in pages). Static files: `wwwroot/lib` (shared vendor), `wwwroot/site` (public), `wwwroot/portal` (portal).
- `Me` replaces separate Student/Trainer/Employee areas; its dashboard shows widgets by permission.
- The portal stays at `/portal/`, so the landing site's Login and Verify links keep working.
- Current landing `.html` URLs get 301 redirects to clean MVC URLs; course and category pages come from the database.

## 4. Database conventions

- `snake_case` names, `timestamptz` in UTC, money as `numeric(12,2)`, `xmin` concurrency tokens.
- Audit fields (created/updated by + at) and soft delete on every table, via EF interceptors; they feed the audit log.
- Migrations run as a separate deploy step (EF migration bundle), never at app startup in production.
- Connection strings and secrets from environment variables / user secrets only.

## 5. Build order

| # | Deliverable |
|---|---|
| B0 | Solution skeleton, Docker Compose, CI (build + test), Postgres + first migration |
| B1 | Identity, permissions/roles/scopes, NavRegistry, login, onboarding gate, per-page permission test |
| B2 | Landing site on MVC: courses from DB, lead and pre-book forms saved, Razorpay ₹49 |
| B3 | CRM: lead inbox, SLA job, pipeline, follow-ups, counselling, applications |
| B4 | Academics: enrolment, batches, timetable with clash check, attendance, assessments |
| B5 | Finance: fee plans, collections, receipts/GST invoices, dues, fines, discount approvals |
| B6 | Verification, ID cards, certificates, public verify page |
| B7 | HR and payroll, then the self-service `Me` pages |
| B8 | Reports, audit log, production deploy |

## 6. Production hosting (open)

Options under review:
- **SharkASP** (existing plan): Windows/IIS shared, .NET 10, PostgreSQL. Concerns: US/EU data centres only (latency from Lucknow), idle app pools stop background jobs, no Docker/SSH, Postgres version and limits unpublished. Owner is asking their support about Postgres version and remote access, app pool idle timeout / always-on, database and memory limits, and backup restore.
- **Recommended alternative:** a 2 GB Linux VPS in India (DigitalOcean Bangalore, AWS Lightsail Mumbai or Hostinger India) running the same Docker Compose + Caddy for HTTPS, nightly off-server database backups, GitHub Actions deploy. Later move to managed Postgres in Mumbai by changing the connection string.

Only B8 depends on this decision.
