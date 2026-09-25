# BPST Edu Admin Panel: HTML + Bootstrap prototype

A clickable prototype of the BPST Software Education admin panel: CRM with a 1-hour lead SLA, admissions, fees & GST, verification, HR & payroll, and trainer, student and employee portals. It uses **demo data only**. There is no backend yet.

- Plan: [PLAN.md](PLAN.md) · Design rules: [BRAND.md](BRAND.md) · Working rules: [CLAUDE.md](CLAUDE.md)
- Status: **all screens built (M1–M7)**: 100+ pages, every menu item for every role opens a real page. Next: owner review, then the backend phase.

## View it
- Double-click `index.html`. It works offline with no install.
- Or run `npx http-server . -p 8080` and open http://localhost:8080
- Use **"Prototype: view as"** on the sign-in page, or the role dropdown in the top bar, to see each role's menu.
- Press **Ctrl+K** (Cmd+K on Mac) to search. The moon icon switches to dark mode.
- Check screen sizes: F12 → Ctrl+Shift+M. Every page is checked at 375, 768, 1024, 1440, 1920, 2560 and 3440 px (phone → ultra-wide) with no sideways scrolling.

## Pages
| Area | Files |
|---|---|
| Sign-in, passwords | `index.html`, `pages/auth/forgot-password.html`, `pages/auth/set-password.html` |
| Management | `pages/management/` dashboard, approvals, reports, announcements, feedback, settings, audit-log |
| Leads & CRM | `pages/crm/` dashboard (counsellor), leads (live 1-hour SLA), pipeline, lead (Lead 360), add-lead, follow-ups, counselling, counselling-session, applications, institutions, institution |
| Admissions | `pages/admissions/` enrol (5-step wizard), students, student (Student 360), parallel-course (clash check), batches, batch, batch-operations, timetable, courses (the 58 website courses), assessments |
| Attendance | `pages/attendance/` mark, qr, reports, staff |
| Fees & Accounts | `pages/fees/` collections, ledger (collect payment, fine, waiver), dues, discounts, gst-register, refunds, expenses, profit-loss · `pages/hr/dashboard.html` (HR + Accounts home) |
| Verification | `pages/verification/` queue, review, id-cards, certificates |
| Placement | `pages/placement/` drives, drive, companies |
| HR & Payroll | `pages/hr/` employees, employee, onboard, salary-structure, payroll, leave, resignations |
| Trainer portal | `pages/trainer/` dashboard, batches, timetable, assignments, messages |
| Student portal | `pages/student/` onboarding (locked state), dashboard, courses, fees, assignments, attendance, id-card, messages, results, feedback, placement, apply |
| Employee portal | `pages/employee/` onboarding (locked state), dashboard, payslips, leave, attendance, documents, resign |
| Account | `pages/account/` notifications, profile |
| Print (A4/A5/CR80) | `print/` receipt, gst-invoice, id-card, certificate, offer-letter, payslip, experience-letter |
| Emails | `emails/*.html` (14 designs) · preview page `pages/system/emails.html` |
| Public (indexed by search) | `pages/public/verify-certificate.html` · try `?no=BPST-C-2026-0187` |
| Design reference | `pages/system/ui-kit.html`, `404.html` |

## Linked with the website
- The website header has a **Login** button and the footer has **Verify a Certificate** and **Student & Staff Login**. They go to `portalUrl` in the website's `assets/js/config.js` (default `https://edu.bitprosofttech.com/portal/`), or to this folder when both are opened from disk.
- The sign-in page links back to the website and to the certificate check.
- `edu.bitprosofttech.com/verify.html` (printed on certificates) forwards to the certificate check.
- Deploy this folder at **`/portal/`** on the website domain. Every page except the certificate check is `noindex`; `robots.txt` in both folders keeps crawlers out of the private portal.

## Where things live
```
assets/css/admin.css     Design tokens and components (see BRAND.md)
assets/js/nav.js         Sidebar menu, roles and milestone per page; the only place to edit the menu
assets/js/admin.js       App shell, role switcher, live SLA timers, search, toasts, dark mode
assets/js/demo-data.js   Fake demo data
assets/vendor/           Bootstrap 5.3.3, Bootstrap Icons 1.11.3, Chart.js 4.4.7 (local, no CDN)
```
