# BPST Edu Admin Panel: HTML + Bootstrap prototype

A clickable prototype of the BPST Software Education admin panel: CRM with a 1-hour lead SLA, admissions, fees & GST, verification, HR & payroll, and trainer, student and employee portals. It uses **demo data only**. There is no backend yet.

- Plan: [PLAN.md](PLAN.md) · Design rules: [BRAND.md](BRAND.md) · Working rules: [CLAUDE.md](CLAUDE.md)
- Status: **M1**: app shell, UI kit, sign-in pages and owner dashboard. Menu items tagged `M2`–`M7` are coming in later milestones.

## View it
- Double-click `index.html`. It works offline with no install.
- Or run `npx http-server . -p 8080` and open http://localhost:8080
- Use **"Prototype: view as"** on the sign-in page, or the role dropdown in the top bar, to see each role's menu.
- Press **Ctrl+K** (Cmd+K on Mac) to search. The moon icon switches to dark mode.
- Check mobile: F12 → Ctrl+Shift+M, and try 375, 768, 1024 and 1440 px.

## Pages in M1
| Page | File |
|---|---|
| Sign in + demo role picker | `index.html` |
| Forgot password | `pages/auth/forgot-password.html` |
| First sign-in / reset password | `pages/auth/set-password.html` (`?mode=reset` for the reset link) |
| Owner dashboard | `pages/management/dashboard.html` |
| UI kit | `pages/system/ui-kit.html` |
| Not found | `404.html` |

## Where things live
```
assets/css/admin.css     Design tokens and components (see BRAND.md)
assets/js/nav.js         Sidebar menu, roles and milestone per page; the only place to edit the menu
assets/js/admin.js       App shell, role switcher, live SLA timers, search, toasts, dark mode
assets/js/demo-data.js   Fake demo data
assets/vendor/           Bootstrap 5.3.3, Bootstrap Icons 1.11.3, Chart.js 4.4.7 (local, no CDN)
```
