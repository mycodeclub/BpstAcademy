# BPST Edu Admin Panel: Brand & UI Rules

The admin panel is a **work tool**, not a marketing page: plain white, Apple-like and calm. The only things carried over from the landing site are the **logo** and **one accent colour** (BPST blue `#0062ff`). Gradients, glows, the Outfit/Plus Jakarta display fonts and the heavy shadows of the landing site are **not** used here.

## 1. Principles
1. **Data first.** Numbers, names and statuses are the hero. Decoration is kept to a minimum.
2. **One accent.** Blue marks the primary action and the current selection. Everything else stays neutral.
3. **Colour means something.** Green/amber/red appear only for status (paid, due, overdue, SLA), never for decoration.
4. **One primary action per screen.** Secondary actions are outline or ghost buttons.
5. **Calm density.** Tables are compact but never cramped, with lots of white space around groups rather than inside rows.
6. **Every page works on a phone**, even though desktop comes first.

## 2. Colour tokens

Defined once on `:root` in `assets/css/admin.css` and mapped onto Bootstrap variables (`--bs-primary`, `--bs-body-bg`, etc.).

| Token | Light | Dark | Use |
|---|---|---|---|
| `--c-bg` | `#ffffff` | `#000000` | Page background |
| `--c-surface` | `#f5f5f7` | `#1c1c1e` | Sidebar, table header, subtle panels |
| `--c-card` | `#ffffff` | `#1c1c1e` | Cards, modals |
| `--c-border` | `#e5e5ea` | `#38383a` | 1px borders, dividers |
| `--c-text` | `#1d1d1f` | `#f5f5f7` | Main text |
| `--c-muted` | `#6e6e73` | `#98989d` | Labels, help text |
| `--c-accent` | `#0062ff` | `#3b82ff` | Primary buttons, links, active nav, focus ring |
| `--c-accent-soft` | `#e8f0ff` | `#0a2a5c` | Selected row, active nav background |
| `--c-success` | `#10b981` | `#34d399` | Paid, verified, present, active |
| `--c-warning` | `#f59e0b` | `#fbbf24` | Due, pending, late, SLA amber |
| `--c-danger` | `#ef4444` | `#f87171` | Overdue, rejected, absent, SLA red / high risk |
| `--c-info` | `#0ea5e9` | `#38bdf8` | New, scheduled, informational |

Each status colour also has a `-soft` background (10–12% tint) for chips.

Dark mode is optional, switched with a toggle in the top bar using Bootstrap `data-bs-theme="dark"`. Light is the default.

## 3. Status chips (shared across all modules)

| Chip | Colour | Used for |
|---|---|---|
| New | info | Lead new, application new |
| Contacted / Scheduled / Submitted | accent | In progress |
| Pending / Due / Part-paid / On leave | warning | Needs attention soon |
| Verified / Paid / Active / Present / Approved | success | Done, good |
| Overdue / Rejected / Absent / Lost / High risk | danger | Problem |
| Draft / Inactive / Exited | neutral (muted) | Inactive |

The chip style is a small pill, 12px medium text, soft background and strong text colour, plus a dot for colour-blind safety (status is never shown by colour alone).

### SLA timer chip
| State | Style |
|---|---|
| 0–30 min | success-soft, countdown `mm:ss` |
| 30–45 min | warning-soft |
| 45–60 min | danger-soft with a gentle pulse (turned off when `prefers-reduced-motion` is set) |
| 60+ min | solid danger, white text, `HIGH RISK +mm:ss` counting up |

## 4. Typography
- **Font:** system stack `-apple-system, BlinkMacSystemFont, "Segoe UI", Inter, Roboto, "Helvetica Neue", Arial, sans-serif`. No web-font download: fast, and native on every device. (SF Pro can't legally be self-hosted.)
- **Scale (px):** 12 caption · 13 table/small · 14 body · 16 emphasis · 20 section title · 24 page title · 32 KPI number.
- **Weights:** 400 body, 500 labels/nav, 600 titles and KPI numbers. No 800/900.
- **Numbers:** `font-variant-numeric: tabular-nums` for money, counts, timers and dates, so columns line up.
- **Money:** `₹1,25,000` (Indian grouping), right-aligned in tables. Negative values in danger colour with a minus sign.
- **Dates:** `25 Sep 2026`. Times: `5:30 PM`. All in IST.

## 5. Layout
- **Sidebar:** 240px, `--c-surface`, collapsible to a 64px icon rail. On phones it becomes an off-canvas drawer. Grouped sections with 11px uppercase muted headings.
- **Top bar:** 56px, white, 1px bottom border. Holds page title/breadcrumb, global search (Ctrl+K), **SLA at-risk badge**, notifications, dark-mode toggle, **"View as" role switcher**, avatar menu.
- **Content:** max width 1440px, 24px padding (16px on phones).
- **Spacing:** 8px grid (4, 8, 12, 16, 24, 32, 48).
- **Cards:** 12px radius, 1px `--c-border`, **no shadow** (or `0 1px 2px rgba(0,0,0,.04)` at most). Card header 16px/600.
- **Breakpoints to check:** 375, 768, 1024, 1440, 1920, 2560, 3440.
- **Wide screens:** content grows to 1560px (≥1600), 1760px centred (≥1920), 2200px with 18px base text (≥2560) and 2800px with 20px text (≥3440). KPI rows and card lists use `.grid-auto`, so they add columns on their own.
- **Touch screens:** larger tap targets for nav items, filter chips and attendance buttons (`pointer: coarse`).

## 6. Components (all shown on the UI kit page)
- KPI tile: label, big number, delta vs last period, optional sparkline
- Data table: sticky header, search, filter chips, sortable columns, bulk-select, row actions menu, pagination, empty state
- Status chip, SLA timer chip, avatar + name cell
- Buttons: primary (accent), secondary (outline), ghost, danger. Sizes sm/md
- Forms: floating labels off; label above the field, help text below, inline validation
- Stepper / wizard (enrolment, onboarding)
- Timeline (lead activity, audit)
- Kanban column and card (pipeline)
- Calendar/week grid (counselling, timetable, attendance)
- Tabs, off-canvas detail panel, modals, toasts (bottom-right), confirm dialog
- Progress bar (onboarding checklist, fee paid %)
- Charts (Chart.js): line, bar, doughnut, funnel-as-bar. Neutral grid, accent first series, status colours only for status series
- **Print layouts:** receipt (A5), GST tax invoice (A4), ID card (CR80, 85.6 × 54 mm), certificate (A4 landscape), offer letter and payslip (A4). All use `@media print` to hide the app shell

## 7. Icons
Bootstrap Icons, stored locally. Outline style, 16px in tables and nav, 20px in KPI tiles. Always paired with a text label in the nav (and a tooltip in collapsed mode).

## 8. Logo
- `assets/img/bpst-logo.svg` on light backgrounds (sidebar top, login, print documents).
- `assets/img/bpst-logo-white.svg` on dark backgrounds.
- Minimum height 28px in the sidebar, with clear space equal to the height of the "B". Never recolour or stretch.

## 9. Voice & words
- English only. Plain, short labels: "Collect payment", not "Initiate fee collection transaction".
- Buttons are verbs: *Save*, *Verify*, *Approve*, *Send reminder*.
- Empty states say what to do next: "No leads waiting. New enquiries from the website appear here."
- Numbers never claim guarantees (placement "support", never "guaranteed").

## 10. Accessibility
- Text contrast ≥ 4.5:1 (muted text is checked against both light and dark backgrounds).
- Visible focus ring: 2px accent outline.
- Status is never shown by colour alone (dot plus text).
- All interactive elements can be reached by keyboard; modals trap focus.
- `prefers-reduced-motion` disables the pulse and transitions.
