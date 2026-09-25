# Session Notes (for resuming on another machine)

**Last updated:** 26 Sep 2026
**Repo:** https://github.com/mycodeclub/BpstEdu (public, branch `main`)

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

## 3. Photos: important honesty rules

- All photos in `Landing Page UI/assets/img/campus/`, plus `hero-students-*`, `team-collaboration-*` and `career-students-*`, are **Unsplash stock images** (free commercial use, no attribution required). Credits and source links: `assets/img/campus/CREDITS.md`.
- They are **not BPST students.** The only real BPST photo is `certified-students-*` (three students holding certificates).
- The 4 success stories on the homepage are **samples** (made-up names, "Sample" badge, a note under them). The owner chose this; replace with real stories, with written consent, and remove the badges.
- The homepage line "No stock photos or inflated claims" was removed because it would no longer be true.
- **Owner to do:** send real classroom/student photos and real success stories.

## 4. Decisions and preferences

- **Authorization:** permission + scope based, Areas by business module, one `Me` self-service area. Never one Area per role; never check role names in code. (Owner's past projects suffered from role-per-area.) Details in `docs/BACKEND_PLAN.md` §2.
- **Stack:** .NET 10 MVC, PostgreSQL + EF Core 10, Docker Compose locally, ASP.NET Core Identity, Hangfire, QuestPDF, Razorpay.
- **Portal address:** assumed `https://edu.bitprosofttech.com/portal/` (not yet confirmed by the owner). The landing site's Login button, footer "Verify a Certificate", `verify.html`, `robots.txt`, `sitemap.xml`, `llms.txt` and the verify page's canonical URL depend on it. If it changes, update `portalUrl` in `Landing Page UI/assets/js/config.js` and those files.
- **Production host: open.** Owner is asking SharkASP support (Postgres version and remote access, app pool idle timeout / always-on, DB and memory limits, backup restore). Recommended alternative: 2 GB Linux VPS in India with Docker Compose + Caddy. Only B8 (deploy) depends on this.

## 5. Next steps

1. Owner: get the SharkASP answers; decide the host.
2. **Start B0:** solution skeleton (`src/BpstEdu.Domain|Application|Infrastructure|Web`, `tests/`), `docker-compose.yml` (Postgres, Adminer, Mailpit), `.env.example`, central package management, CI (build + test), first EF migration.
3. B1: Identity, permissions/roles/scopes, NavRegistry sidebar, login, onboarding gate, per-page permission test.
4. Then B2–B8 as in `docs/BACKEND_PLAN.md` §5.

## 6. Working notes for Claude sessions

- Preview the landing site locally: `cd "NewLayout/Landing Page UI" && python3 -m http.server 8765`, then open http://localhost:8765.
- Chrome caches images and CSS aggressively during previews; fetch with `{cache: 'reload'}` or bump the `?v=` query when checking changes.
- The header, footer and mega menu are repeated in every landing page; change them with a replace across all `.html` files.
- Never commit real secrets (production DB passwords, live Razorpay keys). Local Docker uses throwaway passwords in a git-ignored `.env`.
