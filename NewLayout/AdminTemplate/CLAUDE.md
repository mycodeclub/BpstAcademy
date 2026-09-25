# CLAUDE.md: BPST Edu Admin Panel (HTML + Bootstrap template)

## What this is
A static, clickable **prototype** of the BPST Software Education business-management system. It covers CRM with a 1-hour lead SLA, admissions, batches, attendance, fees & GST, document verification, ID cards & certificates, HR & payroll, and trainer/student/employee portals.
- **Read first:** `PLAN.md` (scope, pages, flows, milestones, open questions) and `BRAND.md` (tokens, components, rules).
- The sibling folder `../Landing Page UI` is the **public website**. It links here through `portalUrl` in its `assets/js/config.js` (Login button, footer links). This portal is deployed at `/portal/` on the same domain.
- The backend comes later. This template must be easy to convert (clean markup, one page per screen, `data-*` hooks).

## Stack rules
- Plain HTML5 + **Bootstrap 5.3.3** + **Bootstrap Icons 1.11.3** + **Chart.js**. All three are stored **locally** in `assets/vendor/`. **No CDN, no build step, no npm dependencies.**
- Every page must work when opened by **double-clicking** (`file://`) and on GitHub Pages. So: relative paths only, no `fetch()` of local files, no ES module imports.
- Fonts: system stack only (see BRAND.md). No web fonts.
- Vanilla JS only (no jQuery, no frameworks).

## Structure
```
index.html                 Login (with "View as" demo role cards); pages/auth/ holds forgot + set password
pages/<section>/<page>.html  e.g. pages/crm/leads.html, pages/fees/ledger.html, pages/student/dashboard.html
pages/system/ui-kit.html   Every component; update it whenever a component is added
print/                     Receipt, GST invoice, ID card, certificate, offer letter, payslip
emails/                    HTML email previews
assets/css/admin.css       Tokens (from BRAND.md) + components; the only custom stylesheet
assets/js/admin.js         Shell, role switcher, SLA timers, filters, toasts, dark mode
assets/js/nav.js           Sidebar config: one array of sections/pages with allowed roles
assets/js/demo-data.js     Fake demo data (window.DEMO = {...})
assets/vendor/             bootstrap/, bootstrap-icons/, chartjs/
assets/img/                Logos (copied from ../EduTemplateFreezing/take2/assets/img), avatars
.github/workflows/pages.yml  GitHub Pages deploy (same as the landing repo)
```

## Conventions
- **The sidebar and top bar are rendered by `admin.js` from `nav.js`**, so there is one source of truth and pages don't repeat the menu. Each page has only `<div id="app-shell">` plus its own `<main>` content.
- Each page's `<body>` declares `data-page="crm.leads"` and `data-roles="admin counsellor"`. The role switcher and the future backend use these.
- The role switcher saves the chosen role in `localStorage` (wrapped in try/catch) and hides nav items that role can't see.
- Bootstrap utilities first. Custom CSS only as tokens and named components (`.kpi`, `.chip`, `.sla`, `.kanban`, …). No inline styles except for demo widths.
- Status chips come from one class map (`.chip--paid`, `.chip--overdue`, …) as listed in BRAND.md §3. Don't invent new colours.
- SLA timers: `<span class="sla" data-sla data-created-at="2026-09-25T10:15:00+05:30"></span>`. `admin.js` ticks every second. Demo data sets times relative to page load so every state shows.
- Money: `₹` with Indian grouping via one helper `fmtINR()`. Dates via `fmtDate()` → `25 Sep 2026`.
- Print pages use `@media print` and hide the app shell.
- SEO: every portal page is `noindex, nofollow` except `pages/public/verify-certificate.html` (canonical, Open Graph, JSON-LD). Keep it that way.
- Accessibility: label every input, keep focus rings visible, never convey status by colour alone, respect `prefers-reduced-motion`.

## Data rules
- **Only fake demo data** (invented Indian names, `98XXXXXX01`-style phones, `example.com` emails). Never put real student/staff personal data, resumes, marksheets or spreadsheets in this repo.
- Aadhaar is always shown masked (`XXXX XXXX 1234`).
- Public contact on documents: phone/WhatsApp **8299101616**. GSTIN is a placeholder until the owner confirms it.

## Working process
- Build **milestone by milestone** as in PLAN.md §9. Stop after each one for owner review on GitHub Pages. Do not start the next milestone without a go-ahead.
- If a page needs a decision that is still open (PLAN.md §10), use the listed default and mention it in the milestone summary.
- **Checks before saying a milestone is done:**
  - Every sidebar link opens a real page (no 404s).
  - No console errors.
  - Layout checked at 375 / 768 / 1024 / 1440 / 1920 / 2560 px (phone → ultra-wide): no sideways page scroll; wide tables scroll inside `.table-wrap`.
  - Print layouts previewed.
  - Light and dark mode both readable.
- Owner is not deeply technical: explain results in plain language, and prefer a short conversation over long multiple-choice question forms.

## Git
- Separate repo `AdminTemplate` with GitHub Pages deploy from `main`.
- Commit or push only when the owner asks.
