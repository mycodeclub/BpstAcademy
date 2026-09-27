# BPST Academy Admin Panel: Plan

**Status:** M1–M7 built (all screens, demo data). Waiting for owner review; M8 QA checks already pass at 375–3440 px.
**Date:** 25 Sep 2026
**Sibling project:** `../Landing Page UI` (public website, edu.bitprosofttech.com). This portal is deployed at `/portal/` on the same domain.

---

## 1. Goal

This is one rich, interactive HTML + Bootstrap **clickable prototype** of the full business-management system for BPST Software Education, a single training center in Lucknow. It covers:

- **CRM:** leads, enquiries, counselling, applications, with a 1-hour follow-up SLA.
- **Admissions & academics:** enrolment, batches, timetable, parallel courses with no timing clash, attendance.
- **Fees & accounts:** upfront or flexible payments, fines, waivers, discounts, GST receipts and invoices.
- **Verification:** document upload, a manual office verification session, and HR approval.
- **ID cards & certificates.**
- **HR & payroll:** offer letters, salary, payslips, leave, resignation.
- **Self-service portals** for trainers, students and employees.

### In scope (this template phase)
- Static HTML pages with realistic **fake** demo data, working navigation, modals, tabs, filters, live SLA timers and charts.
- Every screen that the backend phase will later make real.

### Not in scope (backend phase)
- Real login, permissions, database, email/SMS/WhatsApp sending, payment gateway, PDF generation.
- These screens are *designed* here (for example email previews and print layouts). Nothing is wired up.

---

## 2. Roles

All roles live in **one template**. A demo **"View as" role switcher** in the top bar changes the sidebar and the dashboard. Real role separation comes with the backend.

The switcher also offers two **gated demo states**, so the onboarding gates in §3.2/§3.3 can be shown: **Student (not yet verified)**, which sees only the onboarding checklist, profile and fees, and **Employee (onboarding)**, which sees only the onboarding checklist and profile.

| Role group | Who | Main job |
|---|---|---|
| **Admin / Management** | Owner, center head | Full CRM, all reports, approvals, settings |
| **Counsellor / Sales** | Counsellors, front desk | Leads, follow-ups, counselling, applications, conversions (business growth) |
| **HR + Accounts** | HR executive, accountant | Document verification, employee onboarding, payroll, fee collection, GST |
| **Trainer** | Faculty | Batches, attendance, assignments, student questions |
| **Student** | Enrolled learners | Own classes, fees, attendance, assignments, ID card, certificates |
| **Employee** | All staff (trainers included) | Offer letter, salary, payslips, attendance, leave, resignation |

### Module access (✓ full, ◐ limited/own data, blank = no access)

| Module | Admin | Counsellor | HR+Accounts | Trainer | Student | Employee |
|---|---|---|---|---|---|---|
| Owner dashboard & reports | ✓ | ◐ own funnel | ◐ fees/HR | | | |
| Leads, counselling, applications | ✓ | ✓ | | | | |
| Institutions (B2B) | ✓ | ✓ | | | | |
| Placement cell | ✓ | ✓ | | | ◐ own drives | |
| Admissions & enrolment | ✓ | ✓ | ◐ | | | |
| Batches & timetable | ✓ | ◐ view | ◐ view | ◐ own | ◐ own | |
| Attendance | ✓ | | ◐ staff | ◐ own batches | ◐ own | ◐ own |
| Fees & GST | ✓ | ◐ collect, view dues | ✓ | | ◐ own | |
| Expenses & P&L | ✓ | | ✓ | | | |
| Assessments & marks | ✓ | | | ◐ own batches | ◐ own | |
| Announcements | ✓ | | ◐ | ◐ own batches | ◐ read | ◐ read |
| Feedback (NPS) | ✓ | | | ◐ own results | ◐ give | |
| Discount / waiver approval | ✓ | request | request | | | |
| Document verification | ✓ | | ✓ | | ◐ upload | ◐ upload |
| ID cards & certificates | ✓ | | ✓ | | ◐ download | |
| HR & payroll | ✓ | | ✓ | | | ◐ own |
| Leave approval | ✓ | | ✓ | | | apply |
| Settings, users, audit log | ✓ | | | | | |

---

## 3. Lifecycles (status flows)

Every status gets a colour chip, defined in BRAND.md.

### 3.1 Lead → Admission
```
New ──▶ Contacted ──▶ Counselling booked ──▶ Counselled ──▶ Applied ──▶ Admitted
  │          │                 │                   │            │
  └──────────┴─────────────────┴───────────────────┴────────────┴──▶ Lost (reason required)
```
- Lead sources include **Website pre-book ₹49** from the landing site's `prebook.html` form, which shows its paid status. Other sources: website enquiry, walk-in, phone call, WhatsApp, referral, Google/Meta ads, JustDial, school/college camp.
- The 1-hour SLA clock (§5) runs from **New** until the first logged contact.

#### Website → lead mapping
The landing site has two forms. Both post to `leadEndpoint` in the landing site's `assets/js/config.js`, which is empty for now, so the forms run in demo mode. There is **no separate website Apply form**. Applications are created by counsellors from a lead (§4.3).

| Lead field | Pre-book form (`prebook.html`, `index.html`) | Enquiry form (`contact.html`) |
|---|---|---|
| Source | `Website pre-book ₹49` | `Website enquiry` |
| Name, mobile | `name`, `mobile` | `name`, `mobile` |
| Email | `email` | — |
| City / district | `city` | — |
| Qualification | `qualification` | — |
| Course interest | `course` (course slug) | `course` (also pre-filled from `?course=` on the URL) |
| Mode (classroom / online) | `mode` | — |
| Preferred duration | `duration` | — |
| Message | `message` | `message` |
| Consent (DPDP) | `consent` | `consent` |
| Payment | ₹49 paid / failed + gateway reference | — |
| Campaign | `utm_source`, `utm_medium`, `utm_campaign` and page URL, captured by the landing script (to add) | same |
| Honeypot | `website` (spam trap; dropped, never stored) | same |

- Duplicate check by mobile (§8.3) runs on every incoming lead.
- A paid pre-book shows a **"₹49 paid"** chip in the inbox and on the Lead 360 page.

### 3.1a Institution (B2B) lead → MoU → bulk batch
```
Enquiry (college / school / company) ─▶ Proposal sent ─▶ MoU / PO signed ─▶ Bulk batch created ─▶ Completed
                                                │
                                                └──▶ Lost (reason required)
```
Used for college tracks, industrial / summer-winter training and school coding programmes. Students added to a bulk batch go through the normal onboarding (§3.2). Billing can go to the institution (one GST invoice) or to each student.

### 3.2 Student onboarding (the verification gate)
```
Registered ─▶ Credentials emailed ─▶ Documents pending ─▶ Submitted ─▶ Verification session booked
 (default ID + password;                (govt ID + education                    │
  forced password change                 documents upload)                      ▼
  at first login)                                                 HR verifies originals in office
                                                                         │               │
                                                                      Rejected ◀── (remarks) ──▶ Verified
                                                                   (re-upload)                   │
                                                                                                 ▼
                                                                           ACTIVE: ID card issued, full portal unlocked
```
- Before verification the student portal shows **only** the onboarding checklist, profile and fee payment.
- After verification it unlocks classes, batch, attendance, assignments, the ID card and messaging with trainers and management.

### 3.3 Employee onboarding
The document gate is the same, followed by **HR approval → offer letter issued → accepted → Active**. After that the full employee portal unlocks: offer letter, salary breakdown, payslips, attendance, leave and resignation.

### 3.4 Fees
```
Fee plan set ─▶ Due ─▶ Part-paid ─▶ Paid
                  │         │
                  └─────────┴──▶ Overdue (+ late fine) ─▶ Fine waived (approval) / Paid
```
Section 6 covers the rules.

### 3.5 Leave and resignation
- **Leave:** Applied → Approved / Rejected (by HR or Admin), and the leave balance updates.
- **Resignation:** Submitted → Accepted → Notice period → Handover checklist → Full & final settlement → Exited.

---

## 4. Page inventory (~75 pages)

The sidebar is grouped by the sections below. `(P)` = print layout.

### 4.1 Shared
| Page | Notes |
|---|---|
| Login | Email/ID + password, "View as" demo role cards |
| Forgot / reset password | |
| First-login password change | Forced for new students and employees |
| Notifications | All alerts, filter by type |
| My profile | Photo, contact, password |
| Global search | Ctrl+K palette: leads, students, receipts, employees |
| UI kit | Every component on one page, which serves as the design reference |
| 404 | |

### 4.2 Management (Admin)
| Page | Notes |
|---|---|
| **Owner dashboard** | KPIs: new leads today, **SLA breaches now**, conversion %, today's and month's collections, dues outstanding, attendance %, pending verifications, pending approvals. Charts: leads by source, funnel, collections trend |
| Reports | Lead funnel, source ROI, counsellor leaderboard, revenue by course, dues ageing, attendance, trainer utilisation. Filter by date, export CSV |
| Approvals inbox | Discounts, fine waivers, leaves, refunds. One place to approve or reject |
| Audit log | Who changed what, when (money and status changes) |
| Settings | Courses, fee plans, batches and timings, rooms, holidays and office hours, users & roles, email/SMS/WhatsApp templates, GST profile (GSTIN, SAC, numbering series), ID card and certificate templates |

### 4.3 CRM / Sales
| Page | Notes |
|---|---|
| **Lead inbox** | Table sorted by urgency, **live SLA countdown per lead**, sticky "breaching now" bar, quick actions (call, WhatsApp, log contact, assign) |
| Pipeline | Kanban by status, drag to move (demo) |
| Lead 360 | Details, interest, timeline (calls, notes, status changes), next follow-up, duplicate-phone warning, convert to application |
| Add lead | Quick form (walk-in / phone) |
| Follow-ups today | Due, overdue, done |
| Counselling calendar | Week view of sessions by counsellor and room |
| Counselling session | Notes, recommended course/batch, fee quoted, outcome |
| Applications | Review queue of applications created by counsellors from a lead: approve → enrol |
| Counsellor dashboard | My leads today, my SLA risk, my follow-ups, my conversions this month |
| Institutions | B2B list and detail page: contact person, programme, proposal, MoU / PO upload, bulk batches, invoices (§3.1a) |

### 4.4 Admissions & Academics
| Page | Notes |
|---|---|
| **Enrolment wizard** | 1 Student details (+ parent for minors) → 2 Course & batch with **timing-clash check** → 3 Fee plan (upfront/flexible, discount request) → 4 Documents checklist → 5 Confirm → preview of the **credentials email** sent |
| Students | List with status filters (onboarding / active / completed / dropped) |
| Student 360 | Profile, courses, batches, attendance, fee ledger, documents, ID card, certificates, notes |
| **Add parallel course** | Pick course → available batches shown with conflicts against the student's current timetable greyed out ("Clashes with Python Mon–Wed 5–6 PM") |
| Batches | List: course, trainer, timing, **mode (classroom / online / hybrid)**, room *or* meeting link, seats filled/total, start/end |
| Batch detail | Students, schedule, attendance summary, assignments, assessments |
| Batch operations | Transfer a student to another batch, **freeze / pause** a course (with resume date), mark dropout (reason required), cancel or reschedule a class, add a makeup class. Each change notifies the students and is written to the audit log |
| Assessments | Per batch: module tests, final test, project. Marks entry by the trainer, grade bands, result sheet. Feeds the certificate rule (§4.8) |
| Timetable | Weekly grid by room / trainer / batch, with clash highlighting. Online batches have no room but are still checked for trainer and student clashes |
| Courses | Catalogue with durations and fee plans (mirrors the landing site's 58 courses) |

### 4.5 Attendance
| Page | Notes |
|---|---|
| Mark attendance (trainer) | Pick batch/date → roster with present/absent/late toggles, "mark all present" |
| QR attendance (option) | Class QR on screen; the student scans from their portal |
| Attendance reports | By student, by batch, low-attendance alerts (< 75%) |
| Staff attendance | Check-in/out log, late marks, monthly summary |

### 4.6 Fees & Accounts
| Page | Notes |
|---|---|
| HR + Accounts dashboard | Collections today, dues, pending verifications, leave requests, payroll status |
| **Collections dashboard** | Today, month, dues, overdue, fines, waivers, and GST collected |
| Student ledger | All charges, payments, fines, waivers, discounts, running balance |
| Collect payment | Modal: amount (any amount), mode (cash/UPI/card/bank), reference. The GST split is calculated automatically |
| Receipt (P) | Every receipt shows **GSTIN + tax breakup** |
| GST tax invoice (P) | **On demand**, when a student or parent asks: full professional invoice |
| Dues & overdue | Ageing buckets, send reminder (demo), add fine |
| Discounts & coupons | Coupon codes, sibling, early-bird, upfront. Every use goes to approval |
| GST register | Monthly register of receipts/invoices, CGST/SGST/IGST totals, CSV export for filing |
| Refunds | Request → approval → processed (follows the landing site's refund policy) |
| Expenses | Record rent, salaries paid, marketing spend (by channel), software, utilities. Category, amount, mode, bill upload, GST input (if any) |
| Profit & loss | Monthly income vs expenses, by category. Marketing spend by channel feeds the source-ROI report |

### 4.7 Verification & Documents
| Page | Notes |
|---|---|
| Verification queue | Students and employees awaiting verification, oldest first |
| Session scheduler | Book an office slot and notify the person (demo) |
| Document review | Checklist per person: govt ID (Aadhaar masked to last 4 digits), 10th/12th/degree marksheets, photo. Mark each as "original seen ✓", then Verify or Reject with remarks |

### 4.8 ID Cards & Certificates
| Page | Notes |
|---|---|
| ID cards | Issue for verified students/employees, print sheet (P) with CR80-size cards: photo, name, ID, course/role, validity, QR |
| Certificates | Generate on course completion (attendance + fees cleared + assessment passed), A4 landscape (P), unique number + QR. **Types:** course completion, industrial / summer-winter training certificate, internship / project completion letter, experience letter (employees) |
| Verify certificate | Public page: enter the number or scan the QR to see a valid/invalid result. Linked from the landing site footer |

### 4.9 HR & Payroll
| Page | Notes |
|---|---|
| Employees | List, status, department, role |
| Employee 360 | Profile, documents, salary, attendance, leave, payslips |
| Onboard employee | Details → documents → verification → offer letter |
| Offer letter (P) | Generated from a template |
| Salary structure | Components per employee (basic, HRA, allowances, deductions) |
| **Payroll run** | Monthly: attendance and leave → calculate → review → publish. **Payslips are available by the last working day** of the month |
| Payslip (P) | |
| Leave requests | Approve/reject, leave balances, holiday calendar |
| Resignations & exit | Notice tracking, handover checklist, F&F settlement, experience letter |

### 4.10 Trainer portal
Dashboard (today's classes), my batches, timetable, mark attendance, assignments (create, collect, grade), assessments (marks entry), batch announcements, feedback results, student messages, plus the employee pages (salary, leave).

### 4.11 Student portal
| Page | Notes |
|---|---|
| Onboarding checklist | Gate: change password → upload documents → book verification. Progress bar |
| Dashboard | Next class (with meeting link for online batches), attendance %, fee due, pending assignments, announcements |
| My courses & timetable | |
| Fees | Ledger, receipts, "Pay now" (demo), **Request GST invoice** |
| Assignments | Submit and view grades |
| Attendance | Calendar view, % per course |
| ID card | View / download |
| Certificates | Download, share verification link |
| Messages | To trainer / to management |
| Apply for another course | Shows only batches that don't clash |
| Results | Test and project marks per course |
| Feedback | Short module-end survey (trainer rating 1–5, NPS 0–10, comment) |
| Placement | Eligible drives, apply, upload resume, interview status |

### 4.12 Employee portal
Onboarding checklist, dashboard, offer letter download, salary breakdown, payslips, attendance, apply for leave, leave balance, resign.

### 4.13 Placement cell
| Page | Notes |
|---|---|
| Companies | Hiring partners: contact, roles hired for, past drives |
| Drives | Company, role, eligibility (course, attendance %, marks), date. Shortlist eligible students automatically |
| Drive detail | Applicants → shortlisted → interviewed → **offered / placed** (CTC) |
| Placement report | Placed count and % by course and batch, average CTC. Supports the claims on the landing site's `career-placement.html` |

### 4.14 Announcements & feedback
| Page | Notes |
|---|---|
| Announcements | Create a notice for everyone / a course / a batch / staff, with an optional expiry date. Shown on dashboards |
| Feedback results | Trainer rating and NPS by batch, course and trainer, with comments. Low scores are flagged |

### 4.15 Email / message previews
These are HTML email designs shown as pages:
- Welcome + default ID and password
- Document reminder
- Verification session booked
- Verified / rejected
- Fee receipt
- Fee due / overdue reminder
- Lead assigned to counsellor
- **SLA breach alert to manager**
- Pre-book ₹49 confirmation
- Batch change / class cancelled / makeup class
- Placement drive invite
- Payslip ready
- Leave approved/rejected

---

## 5. Lead SLA: "no lead left behind"

Every new lead must be contacted within **1 hour**. The clock starts at `created_at` and stops when the first contact is logged (call, WhatsApp or meeting).

| Time since lead created | Look | Behaviour |
|---|---|---|
| 0–30 min | 🟢 green chip, `59:12` counting down in seconds | Normal |
| 30–45 min | 🟠 amber chip | Moves to the top of the inbox |
| 45–60 min | 🔴 red chip, pulsing | Toast to the assigned counsellor, row highlighted |
| 60+ min | ⛔ **HIGH RISK** badge, counts **up** the overdue time (`+12:40`) | "Manager alerted" tag, shown on the owner dashboard and in the sticky "breaching now" bar; it stays in the SLA breach report even after contact |

- **Inbox sort:** breached first, then least time remaining.
- **Header badge:** a live count of leads at risk, visible on every page for Admin and Counsellor.
- **Template implementation:** each timer is `<span data-sla data-created-at="ISO-time">`, and `admin.js` updates it every second. Demo data sets times relative to "now" so every state is always visible.
- **Proposal:** office-hours-aware clock (leads at 11 PM start counting at 9 AM). Recorded as an open question, not built by default.

---

## 6. Fee rules (from the owner)

1. **Full payment upfront** is preferred and **gets a discount**. The upfront discount % is set in Settings.
2. **Flexible payments.** Most Indian students and parents pay in parts, so a fee plan can be:
   - fixed EMI (n instalments with due dates),
   - monthly, or
   - **open part-payment**: no fixed schedule. The student pays any amount at any time and the ledger updates.
3. **Late fine.** A fine is added to overdue dues (a fixed amount or per-day, set in Settings). A **waive-off** (full or partial) needs a reason and **manager approval**.
4. **GST.** GST (18%, SAC 999293, CGST+SGST within UP) is charged on every payment. **Every receipt shows the GSTIN and tax breakup.** A full **GST tax invoice** is generated **on demand** when a student or parent asks. Accounts uses the **GST register** for filing.
   *To confirm with the CA:* GSTIN, legal name, whether fees are shown GST-inclusive or exclusive.
5. **Discounts.** Coupon, sibling, early-bird and upfront discounts all need **manager approval** before they apply.
6. **Numbering:** separate series for receipts (`BPST/RC/2026-27/0001`) and invoices (`BPST/INV/2026-27/0001`), reset each financial year.
7. **₹49 pre-booking (from the landing site's refund policy).** The ₹49 is **adjusted in the course fee** at enrolment. It appears in the ledger as an advance credit carried over from the lead. It is refunded in full only if BPST cancels or cannot start the batch.
   *To confirm with the CA:* whether the ₹49 gets its own GST receipt at payment time (default: yes, as an advance receipt).

---

## 7. Parallel courses without timing clash

- Every batch has fixed slots (days + start/end time + room).
- When enrolling or applying for another course, the system checks the student's current active batch slots, including a 15-minute travel/buffer gap (configurable).
- Clashing batches are shown **disabled with the reason**. Non-clashing ones are shown normally.
- The same check runs for trainers (no double booking) and rooms.

---

## 8. Suggested improvements (proposals for the owner to accept or reject)

1. **Auto-assign leads** round-robin to the counsellors on duty, so no lead sits unassigned.
2. **Lost reason is mandatory** (fees too high, distance, joined elsewhere, not interested…) and feeds the reports.
3. **Duplicate check** by phone number: merge or link to an existing lead or student.
4. **Source tracking and ROI:** which channel brings admissions, not just leads.
5. **Referral rewards:** an existing student refers a friend and gets a discount or reward (with approval).
6. **Parent contact** on every minor's record; a parent login can come later.
7. **Maker-checker for money:** the person who requests a waiver, discount or refund cannot approve it.
8. **Audit log** for every money and status change.
9. **Privacy (DPDP Act 2023):** consent checkbox on forms, Aadhaar always masked, documents visible only to HR/Admin.
10. **Daily digest** email to the owner: leads, breaches, admissions, collections, dues.
11. **Certificate QR verification** so employers can check authenticity.
12. **Global search (Ctrl+K)** across leads, students, receipts and employees.
13. **Low-attendance and fee-due nudges** to students (and parents) automatically.
14. **WhatsApp click-to-chat** from any lead or student (it opens WhatsApp; no API needed in phase 1).

---

## 9. Milestones

Each milestone ends with an **owner review on GitHub Pages**. The next one starts only after the owner's go-ahead.

| # | Deliverable |
|---|---|
| **M0** | PLAN.md, BRAND.md, CLAUDE.md (this) |
| M1 | Repo + Pages deploy, app shell (sidebar, top bar, role switcher, dark mode), **UI kit**, login pages, owner dashboard |
| M2 | CRM: counsellor dashboard, lead inbox with SLA timers, pipeline, lead 360, follow-ups, counselling, applications, institutions (B2B) |
| M3 | Admissions: enrolment wizard, students, student 360, parallel course, batches (incl. online mode), batch operations, timetable, courses, attendance, assessments |
| M4 | Fees & GST: HR + Accounts dashboard, collections, ledger, collect payment, receipt, GST invoice, dues, discounts, GST register, refunds, expenses, P&L |
| M5 | Verification queue and review, ID cards, certificates (all types), certificate verify |
| M6 | HR & payroll: employees, onboarding, offer letter, salary, payroll run, payslip, leave, resignation |
| M7 | Trainer, student and employee portals (incl. gated demo states), placement cell, announcements, feedback, email previews, approvals inbox, reports, settings, audit log |
| M8 | QA: every link works, 375/768/1024/1440 widths, keyboard/contrast check, print layouts, final deploy |

---

## 10. Open questions (defaults used until the owner answers)

| # | Question | Default in template |
|---|---|---|
| 1 | How is attendance marked? | Trainer marks per class; staff check in on the portal; QR screen as an option |
| 2 | Payroll components: PF, ESI, TDS, professional tax? | Basic + HRA + allowances − PF − TDS shown; confirm |
| 3 | GSTIN and legal name for receipts/invoices | Placeholder `09XXXXXXXXXXXZX`, "BitProSoftTech (BPST Software Education)" |
| 4 | Certificate wording, signatories, logo placement | Draft design for review |
| 5 | Office hours (for the SLA pause proposal and staff attendance) | Mon–Sat 9:00 AM–7:00 PM |
| 6 | Default student/employee ID and password format | ID `BPST26S0001` / `BPST26E001`, random temporary password, forced change at first login |
| 7 | Late fine rule | ₹50 per day after 7 days' grace, capped at ₹1,000 |
| 8 | Upfront discount % | 10% |
| 9 | Minimum attendance for a certificate | 75% + all fees cleared |
| 10 | Leave types and yearly quota | Casual 12, Sick 6, Unpaid |
| 11 | Branches: one Lucknow classroom + online, or more centres planned? | One classroom (near IIM Tiraha) + live online. No branch field |
| 12 | Expense categories to track | Rent, salaries, marketing (by channel), software, utilities, other |
| 13 | Final assessment needed for a certificate? Pass mark? | Yes: final test or project, pass mark 50% |
| 14 | Feedback survey timing | At the end of each module and at course end |
| 15 | B2B billing: one invoice to the institution, or each student pays? | Configurable per institution; default one invoice to the institution |
| 16 | Does the ₹49 pre-booking get its own GST receipt? | Yes, as an advance receipt (§6.7) |
