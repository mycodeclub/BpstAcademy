# BPST Edu

Website and business-management system for BPST Software Education (Lucknow), the educational division of BitProSoftTech Software Solutions.

**Start here when resuming:** read `docs/SESSION_NOTES.md` (history, decisions, next steps) and `docs/BACKEND_PLAN.md` (backend architecture and build order).

- `src/BpstEdu.Web/`: the ASP.NET Core MVC app (.NET 10). Public pages use `_Layout`; portal areas use `_AdminLayout`.
- `NewLayout/Landing Page UI/`: public website prototype (HTML + Bootstrap). See its README.
- `NewLayout/AdminTemplate/`: portal prototype. See its PLAN.md, BRAND.md and CLAUDE.md.
- `NewLayout/01_Logos/BPST_Logo_Flat_Transparent.png`: the original logo; every logo and icon derives from it.

Rules:
- Authorization is permission + scope based with Areas per business module. Never one Area per role, never role-name checks.
- Campus photos are Unsplash stock and the homepage success stories are samples; never present them as real BPST students (see `docs/SESSION_NOTES.md` §3).
- Never commit real secrets.
