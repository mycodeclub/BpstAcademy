# Session Notes (for resuming on another machine)

**Last updated:** 27 Sep 2026
**Repo:** https://github.com/mycodeclub/BpstAcademy (public, branch `main`). The old repo `mycodeclub/BpstEdu` is superseded; the owner may delete it later.

This file summarises the work and decisions from the Claude Code sessions so far, so work can resume from any machine. Read it together with `docs/BACKEND_PLAN.md`.

---

## 1. What is in this repo

| Folder | What it is |
|---|---|
| `NewLayout/Landing Page UI/` | Public website prototype (plain HTML + Bootstrap 5, no build step). 83 pages: homepage, 14 category pages, 58 course pages, pre-book, contact, legal. See its `README.md`. |
| `NewLayout/AdminTemplate/` | Clickable prototype of the portal (admin, CRM, fees, HR, trainer/student/employee portals), ~75 pages with demo data. Roles, lifecycles and page inventory in its `PLAN.md`; UI rules in `BRAND.md`. |
| `NewLayout/01_Logos/` | **Original BPST logo files.** `BPST_Logo_Flat_Transparent.png` is the master for every logo and icon. |
| `NewLayout/02_Print_Collateral/` | Brochure and flyer PDFs. |
| `docs/BACKEND_PLAN.md` | Agreed .NET backend plan. |
| `docs/SESSION_NOTES.md` | This file. |

## 2. History

1. **Old repos deleted (25 Sep).** At the owner's request these GitHub repos were permanently deleted without backup: `mycodeclub/BpstEdu2026`, `EduTemplateFreezing`, `BpstEduReNew`, `BpstEdu2026_Sep`, `BpstEdu2024`, `bpstedu`. `mister-magnet/BpstEdu2026` (another account) was left alone.
2. **Fresh repo.** Old local git history was removed (the owner accepted losing the old `app/`, `brand/`, `design/`, `docs/` files that existed only there). A new public repo `mycodeclub/BpstEdu` was created with one fresh commit.
3. **Original logo everywhere.** The favicon, app icons, share (OG) images and the portal's logo files were placeholder "BP" designs; all were rebuilt from the original logo:
   - Vector mark traced from the original: `assets/img/bpst-mark.svg`; lockups `bpst-logo.svg` / `bpst-logo-white.svg` (both sites).
   - `favicon.svg`, `icon-192.png`, `icon-512.png`, `apple-touch-icon.png`, `og-bpst-diwali.png`, `og-bpst-edu.png`.
   - Portal emails use `AdminTemplate/assets/img/bpst-logo-email.png` (email apps don't show SVG).
   - Brand colours sampled from the original: navy `#342f4f`, blue `#1788ca`, gold `#b98428`.
4. **New homepage sections** (between Student Showcase and the ₹49 pre-booking): Classroom & Labs, Certifications, Success Stories, Gallery (with a lightbox).
5. **Layout pass on the homepage** (shared CSS, so all pages benefit):
   - Programs mega menu: was 980px fixed and centred on the link (cut off on the left, taller than the screen). Now centred under the header, 5 columns, max screen width, scrolls inside itself.
   - Header bars slimmer; nav aligned with page content (max 1400px).
   - Floating offer pill hidden until the visitor scrolls past the hero; smaller.
   - Hero stacks below 1200px; hero badge sits under the photo on phones.
   - Technology logos in full colour, single border.
   - `bpst.css` / `bpst.js` links carry `?v=20260925` on all 83 pages for cache busting. Bump it when those files change.
6. **"Course Course" fixed** in titles, meta descriptions, headings and FAQs of the C, C++, Java and Python course pages.
7. **MVC boilerplate replaced (26 Sep).** The `dotnet new mvc` project moved from `Project/BpstEdu` to `src/BpstAcademy.Web` (solution `BpstAcademy.slnx` at repo root). Template pages, jQuery/validation and default Bootstrap removed. Three layouts:
   - `_Layout` (default, public site): landing header/mega menu/footer as partials in `Views/Shared/Site/`; SEO from `SeoMeta` (`ViewData["Seo"]`), canonical/OG built from `Site:BaseUrl`; geo meta + organization JSON-LD site-wide; page JSON-LD in `@section Head`.
   - `_AdminLayout` (portal, after login): server-rendered sidebar/top bar in `Views/Shared/Portal/`; `portal.js` = prototype `admin.js` minus role switcher and demo data. Sidebar is a static list until `NavRegistry` (B1).
   - `_AuthLayout`: login / password pages.
   - `wwwroot/`: `lib/` (Bootstrap 5.3.3, icons, Chart.js), `site/` (landing assets), `portal/` (portal assets).
   - All public pages are ported (28 Sep): homepage in `Views/Home`, 23 site pages + `/verify` in `Views/Pages` (served at `/{slug}`), 58 course pages in `Views/Courses` (served at `/courses/{slug}`) by `PagesController`; unknown slugs get the 404 page. They were generated from the prototype by a script (main content + page JSON-LD; the layout supplies head, header and footer); edit the views directly from now on. `/verify` explains how to confirm a certificate until online verification exists. An integration test crawls every internal link. `wwwroot/sitemap.xml` lists the 82 public pages (static until course pages move to the DB in B2; a test checks every entry exists); `llms.txt` still lists the old `.html` URLs.
   - Portal routes: `/portal/{area}/{controller}/{action}`, all require sign-in. Real sign-in with ASP.NET Core Identity since 26 Sep (see item 9); the demo sign-in button was removed.
   - Razor: literal `@` in copied HTML (JSON-LD `@context`, emails) must be written `@@`.
8. **B0 skeleton (26 Sep, branch `feature/b0-skeleton`).** Projects `src/BpstAcademy.Domain|Application|Infrastructure|Web`, `tests/BpstAcademy.UnitTests|IntegrationTests`; `Directory.Build.props` (net10, nullable, warnings as errors), `Directory.Packages.props` (central versions), `global.json` (Microsoft.Testing.Platform runner for xunit v3).
   - Persistence: `AppDbContext` with snake_case names, `numeric(12,2)` money, soft-delete query filter, `xmin` concurrency on `AuditableEntity`; `AuditingInterceptor` stamps created/updated/deleted by+at, turns deletes into soft deletes and writes `audit_log` rows. First migration `InitialCreate` (the `audit_log` table). Ids are GUID v7.
   - Connection string only from `ConnectionStrings:Default` (user secrets locally, env var on the server). Development applies migrations at startup only when `Database:MigrateOnStartup` is true (set by the Docker DB / Local DB launch profiles; never for Live); production never does. `/health` checks the database.
   - `docker-compose.yml`: Postgres 17, Adminer (:8081), Mailpit (:8025 / SMTP :1025), all bound to 127.0.0.1. `.env.example` → `.env`.
   - CI `.github/workflows/ci.yml`: build, check migrations match the model, tests (Testcontainers Postgres).
   - 11 tests pass (homepage SEO head, 404, portal redirects, login noindex, health, audit/soft delete, concurrency).
   - **This PC: Windows Smart App Control blocks `BpstAcademy.Domain.dll`** (the other DLLs load), so the app/tests/EF can't run natively here. Owner chose to use the .NET SDK Docker container for now (commands in §6). The owner decides about Smart App Control; don't change Windows security settings.
9. **B1 start: Identity + admin seed (26 Sep, on `main`).** `AppUser`/`AppRole` (`IdentityUser<Guid>`, GUID v7) in `Infrastructure/Identity`; `AppDbContext` is an `IdentityDbContext`; tables `users`, `roles`, `user_roles`, `user_claims`, `user_logins`, `user_tokens`, `role_claims` (migration `AddIdentity`). `Application/Security/Permissions` (claim type `permission`; `users.manage`, `roles.manage` so far) and `DefaultRoles.Admin`. Login accepts email or user name (BPST ID), lockout after 5 failures.
   - Seed: `dotnet run --project src/BpstAcademy.Web -- seed` runs `IdentitySeeder` (Admin role with every permission + one admin user) and exits. Idempotent; creates no sample data. Admin email/password come from `Seed:AdminEmail` / `Seed:AdminPassword` in user secrets. Re-run it after adding permissions so Admin gets them.
   - Live DB seeded 26 Sep with only that one admin (credentials in the owner's user secrets); nothing else. **Owner to do:** set a strong admin password once a change-password page exists, and rotate the DB password (it was pushed in commit `d68a00a`).
   - Still to do in B1: `[HasPermission]` + policy provider, scopes, NavRegistry sidebar, onboarding gate, change-password / first-login flow, per-page permission test.
10. **Renamed BpstEdu to BpstAcademy (27 Sep).** Solution, projects, namespaces, CI and docs are now `BpstAcademy.*`, and the public brand text "BPST Edu" is now "BPST Academy". Kept on purpose: `UserSecretsId` `bpstedu-web` (the owner's secrets hold the live connection string), the Postgres/Compose/volume names `bpstedu`, and the public file names `BPST_Edu_*.pdf` and `og-bpst-edu.png`. **Still to do:** regenerate the brochure PDFs, print collateral and OG image, which still show "BPST Edu" inside. Code pushed to the new repo `mycodeclub/BpstAcademy`.

## 3. Photos: important honesty rules

- All photos in `Landing Page UI/assets/img/campus/`, plus `hero-students-*`, `team-collaboration-*` and `career-students-*`, are **Unsplash stock images** (free commercial use, no attribution required). Credits and source links: `assets/img/campus/CREDITS.md`.
- They are **not BPST students.** The only real BPST photo is `certified-students-*` (three students holding certificates).
- The 4 success stories on the homepage are **samples** (made-up names, "Sample" badge, a note under them). The owner chose this; replace with real stories, with written consent, and remove the badges.
- The homepage line "No stock photos or inflated claims" was removed because it would no longer be true.
- **Owner to do:** send real classroom/student photos and real success stories.

## 4. Decisions and preferences

- **Authorization:** permission + scope based, Areas by business module, one `Me` self-service area. Never one Area per role; never check role names in code. (Owner's past projects suffered from role-per-area.) Details in `docs/BACKEND_PLAN.md` §2.
- **Stack:** .NET 10 MVC, PostgreSQL + EF Core 10, Docker Compose locally, ASP.NET Core Identity, Hangfire, QuestPDF, Razorpay.
- **Site address (28 Sep, owner's decision):** `https://www.bpstacademy.com` (`Site:BaseUrl`); canonical, Open Graph, JSON-LD, `robots.txt`, `llms.txt` and `sitemap.xml` use it, and `bpstacademy.com` without www redirects there permanently. The portal is `https://www.bpstacademy.com/portal/`. `edu.bitprosofttech.com` is no longer used by the app (the `NewLayout` prototype still has it).
- **Production host: open.** Owner is asking SharkASP support (Postgres version and remote access, app pool idle timeout / always-on, DB and memory limits, backup restore). Recommended alternative: 2 GB Linux VPS in India with Docker Compose + Caddy. Only B8 (deploy) depends on this.

## 5. Next steps

1. Owner: get the SharkASP answers; decide the host.
2. Owner: change the admin password and rotate the live DB password (see §2 item 9).
3. Rest of B1 (see §2 item 9).
4. Then B2–B8 as in `docs/BACKEND_PLAN.md` §5.

## 6. Working notes for Claude sessions

- **Choosing the database (27 Sep).** `DatabaseConnection.Resolve` (Infrastructure) reads `Database:Target` and uses `ConnectionStrings:{Target}`; with no target it uses `ConnectionStrings:Default`. The launch profiles pick it:
  - `Docker DB` → `ConnectionStrings:Docker` (private Docker Postgres; migrations apply on start).
  - `Local DB` → `ConnectionStrings:Local` (Postgres installed on the machine; migrations apply on start).
  - `Live DB` → `ConnectionStrings:Live` (the live site4now database, a new one created by the owner 27 Sep; never migrated on start).
  - Production (no target) → `ConnectionStrings:Default`, from `src/BpstAcademy.Web/appsettings.Production.json` (git-ignored, exists only on the owner's machines, published with the app) or the server env var `ConnectionStrings__Default`.
  Local strings live in user secrets **or** in `src/BpstAcademy.Web/appsettings.Development.local.json` (git-ignored, loaded only in Development, never copied to build or publish output; it overrides user secrets). The owner's Windows VM opens the same folder from the Mac (UTM share, `M:\`), and user secrets are per machine, so `Live` and the `Seed:*` values are kept in that file; `Docker` (localhost, Mac only) stays in the Mac's user secrets. The startup log shows `Database: <name> (host:port/db)`. Never put a connection string in `appsettings.json`. The old app's 26 tables were dropped 26 Sep; a full backup is at `C:\AllData\Backups\BpstAcademy\live-before-cleanup-20260926-213317.dump` (outside the repo).
- **Deploying (27 Sep).** Production is the SharkASP (site4now) IIS site bpstacademy.com. Merge a pull request `main` → `release` (the branch accepts only PRs; CI must pass); `.github/workflows/deploy.yml` then builds, tests, publishes self-contained `win-x64`, writes `appsettings.Production.json` from the `CONNECTION_STRING` secret, puts up `app_offline.htm`, uploads over FTP(S) and removes it. Settings are in GitHub → Settings → Environments → `production` (secrets `CONNECTION_STRING`, `FTP_USERNAME`, `FTP_PASSWORD`; variables `FTP_SERVER`, `FTP_REMOTE_DIR` ending in `/`, optional `FTP_PROTOCOL` = `ftps`|`ftp`). SharkASP's own Git deployer does not work for this repo (it runs Railpack on the `.csproj` folder only, with .NET 8), so don't use it. Migrations are not applied by the deploy; run `dotnet ef database update` against the live DB first when a release adds one.
- Apply migrations to the live database deliberately, never by starting the app:
  `dotnet ef database update --project src/BpstAcademy.Infrastructure --startup-project src/BpstAcademy.Web --connection "<connection string from user secrets>"`
  (`dotnet ef` uses `DesignTimeDbContextFactory`, which ignores user secrets, so `--connection` is required). Run it after reviewing a new migration, before running the app.
- Docker setup: copy `.env.example` to `.env`, set a password (and `POSTGRES_PORT` if 5432 is taken), `docker compose up -d`, then
  `dotnet user-secrets set "ConnectionStrings:Docker" "Host=localhost;Port=<POSTGRES_PORT>;Database=bpstedu;Username=bpst;Password=<.env password>" --project src/BpstAcademy.Web`.
- "Connection string 'Default' is missing" means the app started without a launch profile (not Development), so user secrets were not loaded.
- Run the app: `dotnet run --project src/BpstAcademy.Web --launch-profile "Docker DB"` (or `"Live DB"`, `"Local DB"`) → https://localhost:7089 (portal: `/portal`, sign in with the seeded admin).
- New migration: `dotnet ef migrations add <Name> --project src/BpstAcademy.Infrastructure --startup-project src/BpstAcademy.Web --output-dir Persistence/Migrations`.
- Tests: `dotnet test --solution BpstAcademy.slnx` (Docker must be running for integration tests).
- Where Smart App Control blocks local DLLs, run the same commands in the SDK container (copy the repo without `bin/obj` so Windows build output isn't reused):
  `MSYS_NO_PATHCONV=1 docker run --rm -v "<repo>:/repo:ro" -v bpstedu-nuget:/root/.nuget/packages -v /var/run/docker.sock:/var/run/docker.sock -e TESTCONTAINERS_HOST_OVERRIDE=host.docker.internal --add-host host.docker.internal:host-gateway mcr.microsoft.com/dotnet/sdk:10.0 bash -c 'mkdir /work && cd /repo && tar --exclude=bin --exclude=obj --exclude=NewLayout --exclude=.git -cf - . | tar -xf - -C /work && cd /work && dotnet test --solution BpstAcademy.slnx'`
  (for `ef migrations add`, mount the repo read-write and copy `Persistence/Migrations` back).
- Preview the landing prototype locally: `cd "NewLayout/Landing Page UI" && python3 -m http.server 8765`, then open http://localhost:8765.
- Chrome caches images and CSS aggressively during previews; fetch with `{cache: 'reload'}` or bump the `?v=` query when checking changes.
- The header, footer and mega menu are repeated in every landing page; change them with a replace across all `.html` files.
- Never commit real secrets (production DB passwords, live Razorpay keys). Local Docker uses throwaway passwords in a git-ignored `.env`.
