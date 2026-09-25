/* Sidebar config — the single source of truth for navigation.
   href is relative to the template root. ms = milestone that builds the page (PLAN.md §9);
   items with ms > BPST_BUILT_MS render as "coming soon". count = live badge key. */
window.BPST_BUILT_MS = 8;

window.BPST_ROLES = [
  { id: 'admin', label: 'Admin / Management', short: 'Admin', icon: 'bi-building', home: 'pages/management/dashboard.html', desc: 'Full CRM, reports, approvals, settings' },
  { id: 'counsellor', label: 'Counsellor / Sales', short: 'Counsellor', icon: 'bi-headset', home: 'pages/crm/leads.html', desc: 'Leads, follow-ups, counselling, admissions' },
  { id: 'hr', label: 'HR + Accounts', short: 'HR + Accounts', icon: 'bi-briefcase', home: 'pages/fees/collections.html', desc: 'Verification, payroll, fees, GST' },
  { id: 'trainer', label: 'Trainer', short: 'Trainer', icon: 'bi-easel', home: 'pages/trainer/dashboard.html', desc: 'Batches, attendance, assignments' },
  { id: 'student', label: 'Student', short: 'Student', icon: 'bi-mortarboard', home: 'pages/student/dashboard.html', desc: 'Classes, fees, attendance, ID card' },
  { id: 'employee', label: 'Employee', short: 'Employee', icon: 'bi-person-badge', home: 'pages/employee/dashboard.html', desc: 'Offer letter, payslips, leave' },
  /* gated demo states (PLAN.md §2, §3.2, §3.3): portal locked until verification */
  { id: 'student-pending', label: 'Student (not yet verified)', short: 'Student · pending', icon: 'bi-hourglass', home: 'pages/student/onboarding.html', desc: 'Onboarding checklist, profile, fees only' },
  { id: 'employee-onboarding', label: 'Employee (onboarding)', short: 'Employee · onboarding', icon: 'bi-hourglass', home: 'pages/employee/onboarding.html', desc: 'Onboarding checklist and profile only' }
];

window.BPST_NAV = [
  {
    title: 'Overview',
    items: [
      { id: 'management.dashboard', label: 'Dashboard', icon: 'bi-grid-1x2', href: 'pages/management/dashboard.html', roles: ['admin'], ms: 1 },
      { id: 'crm.dashboard', label: 'My dashboard', icon: 'bi-grid-1x2', href: 'pages/crm/dashboard.html', roles: ['counsellor'], ms: 2 },
      { id: 'hr.dashboard', label: 'Dashboard', icon: 'bi-grid-1x2', href: 'pages/hr/dashboard.html', roles: ['hr'], ms: 4 },
      { id: 'trainer.dashboard', label: 'Dashboard', icon: 'bi-grid-1x2', href: 'pages/trainer/dashboard.html', roles: ['trainer'], ms: 7 },
      { id: 'student.dashboard', label: 'Dashboard', icon: 'bi-grid-1x2', href: 'pages/student/dashboard.html', roles: ['student'], ms: 7 },
      { id: 'student.onboarding', label: 'Onboarding checklist', icon: 'bi-list-check', href: 'pages/student/onboarding.html', roles: ['student-pending'], ms: 7 },
      { id: 'employee.onboarding', label: 'Onboarding checklist', icon: 'bi-list-check', href: 'pages/employee/onboarding.html', roles: ['employee-onboarding'], ms: 7 },
      { id: 'employee.dashboard', label: 'Dashboard', icon: 'bi-grid-1x2', href: 'pages/employee/dashboard.html', roles: ['employee'], ms: 7 },
      { id: 'management.approvals', label: 'Approvals', icon: 'bi-check2-square', href: 'pages/management/approvals.html', roles: ['admin'], ms: 7, count: 'approvals' },
      { id: 'management.reports', label: 'Reports', icon: 'bi-bar-chart', href: 'pages/management/reports.html', roles: ['admin'], ms: 7 },
      { id: 'management.announcements', label: 'Announcements', icon: 'bi-megaphone', href: 'pages/management/announcements.html', roles: ['admin', 'hr', 'trainer'], ms: 7 },
      { id: 'management.feedback', label: 'Feedback', icon: 'bi-star-half', href: 'pages/management/feedback.html', roles: ['admin', 'trainer'], ms: 7 }
    ]
  },
  {
    title: 'Leads & CRM',
    items: [
      { id: 'crm.leads', label: 'Lead inbox', icon: 'bi-inbox', href: 'pages/crm/leads.html', roles: ['admin', 'counsellor'], ms: 2, count: 'sla' },
      { id: 'crm.pipeline', label: 'Pipeline', icon: 'bi-kanban', href: 'pages/crm/pipeline.html', roles: ['admin', 'counsellor'], ms: 2 },
      { id: 'crm.followups', label: 'Follow-ups today', icon: 'bi-telephone-outbound', href: 'pages/crm/follow-ups.html', roles: ['admin', 'counsellor'], ms: 2 },
      { id: 'crm.counselling', label: 'Counselling', icon: 'bi-calendar2-week', href: 'pages/crm/counselling.html', roles: ['admin', 'counsellor'], ms: 2 },
      { id: 'crm.applications', label: 'Applications', icon: 'bi-file-earmark-text', href: 'pages/crm/applications.html', roles: ['admin', 'counsellor'], ms: 2 },
      { id: 'crm.institutions', label: 'Institutions (B2B)', icon: 'bi-bank', href: 'pages/crm/institutions.html', roles: ['admin', 'counsellor'], ms: 2 }
    ]
  },
  {
    title: 'Admissions',
    items: [
      { id: 'admissions.enrol', label: 'Enrol student', icon: 'bi-person-plus', href: 'pages/admissions/enrol.html', roles: ['admin', 'counsellor'], ms: 3 },
      { id: 'admissions.students', label: 'Students', icon: 'bi-people', href: 'pages/admissions/students.html', roles: ['admin', 'counsellor', 'hr'], ms: 3 },
      { id: 'admissions.batches', label: 'Batches', icon: 'bi-collection', href: 'pages/admissions/batches.html', roles: ['admin', 'counsellor', 'hr'], ms: 3 },
      { id: 'admissions.batchops', label: 'Batch changes', icon: 'bi-arrow-left-right', href: 'pages/admissions/batch-operations.html', roles: ['admin', 'counsellor'], ms: 3 },
      { id: 'admissions.timetable', label: 'Timetable', icon: 'bi-calendar3', href: 'pages/admissions/timetable.html', roles: ['admin', 'counsellor', 'hr'], ms: 3 },
      { id: 'admissions.courses', label: 'Courses', icon: 'bi-journal-code', href: 'pages/admissions/courses.html', roles: ['admin', 'counsellor'], ms: 3 },
      { id: 'admissions.assessments', label: 'Assessments', icon: 'bi-clipboard2-check', href: 'pages/admissions/assessments.html', roles: ['admin', 'trainer'], ms: 3 }
    ]
  },
  {
    title: 'Attendance',
    items: [
      { id: 'attendance.mark', label: 'Mark attendance', icon: 'bi-check2-circle', href: 'pages/attendance/mark.html', roles: ['admin', 'trainer'], ms: 3 },
      { id: 'attendance.reports', label: 'Student attendance', icon: 'bi-clipboard-data', href: 'pages/attendance/reports.html', roles: ['admin', 'hr'], ms: 3 },
      { id: 'attendance.staff', label: 'Staff attendance', icon: 'bi-person-check', href: 'pages/attendance/staff.html', roles: ['admin', 'hr'], ms: 3 }
    ]
  },
  {
    title: 'Fees & Accounts',
    items: [
      { id: 'fees.collections', label: 'Collections', icon: 'bi-cash-stack', href: 'pages/fees/collections.html', roles: ['admin', 'hr'], ms: 4 },
      { id: 'fees.dues', label: 'Dues & overdue', icon: 'bi-hourglass-split', href: 'pages/fees/dues.html', roles: ['admin', 'hr', 'counsellor'], ms: 4 },
      { id: 'fees.discounts', label: 'Discounts & coupons', icon: 'bi-ticket-perforated', href: 'pages/fees/discounts.html', roles: ['admin', 'hr'], ms: 4 },
      { id: 'fees.gst', label: 'GST register', icon: 'bi-receipt', href: 'pages/fees/gst-register.html', roles: ['admin', 'hr'], ms: 4 },
      { id: 'fees.refunds', label: 'Refunds', icon: 'bi-arrow-counterclockwise', href: 'pages/fees/refunds.html', roles: ['admin', 'hr'], ms: 4 },
      { id: 'fees.expenses', label: 'Expenses', icon: 'bi-wallet', href: 'pages/fees/expenses.html', roles: ['admin', 'hr'], ms: 4 },
      { id: 'fees.pnl', label: 'Profit & loss', icon: 'bi-graph-up-arrow', href: 'pages/fees/profit-loss.html', roles: ['admin', 'hr'], ms: 4 }
    ]
  },
  {
    title: 'Verification',
    items: [
      { id: 'verify.queue', label: 'Verification queue', icon: 'bi-shield-check', href: 'pages/verification/queue.html', roles: ['admin', 'hr'], ms: 5, count: 'verifications' },
      { id: 'verify.idcards', label: 'ID cards', icon: 'bi-person-vcard', href: 'pages/verification/id-cards.html', roles: ['admin', 'hr'], ms: 5 },
      { id: 'verify.certificates', label: 'Certificates', icon: 'bi-award', href: 'pages/verification/certificates.html', roles: ['admin', 'hr'], ms: 5 }
    ]
  },
  {
    title: 'Placement',
    items: [
      { id: 'placement.drives', label: 'Placement drives', icon: 'bi-briefcase-fill', href: 'pages/placement/drives.html', roles: ['admin', 'counsellor'], ms: 7 },
      { id: 'placement.companies', label: 'Companies', icon: 'bi-buildings', href: 'pages/placement/companies.html', roles: ['admin', 'counsellor'], ms: 7 }
    ]
  },
  {
    title: 'HR & Payroll',
    items: [
      { id: 'hr.employees', label: 'Employees', icon: 'bi-person-lines-fill', href: 'pages/hr/employees.html', roles: ['admin', 'hr'], ms: 6 },
      { id: 'hr.payroll', label: 'Payroll', icon: 'bi-wallet2', href: 'pages/hr/payroll.html', roles: ['admin', 'hr'], ms: 6 },
      { id: 'hr.leave', label: 'Leave requests', icon: 'bi-calendar-x', href: 'pages/hr/leave.html', roles: ['admin', 'hr'], ms: 6 },
      { id: 'hr.exits', label: 'Resignations', icon: 'bi-box-arrow-right', href: 'pages/hr/resignations.html', roles: ['admin', 'hr'], ms: 6 }
    ]
  },
  {
    title: 'Teaching',
    items: [
      { id: 'trainer.batches', label: 'My batches', icon: 'bi-collection', href: 'pages/trainer/batches.html', roles: ['trainer'], ms: 7 },
      { id: 'trainer.timetable', label: 'My timetable', icon: 'bi-calendar3', href: 'pages/trainer/timetable.html', roles: ['trainer'], ms: 7 },
      { id: 'trainer.assignments', label: 'Assignments', icon: 'bi-journal-check', href: 'pages/trainer/assignments.html', roles: ['trainer'], ms: 7 },
      { id: 'trainer.messages', label: 'Student messages', icon: 'bi-chat-left-text', href: 'pages/trainer/messages.html', roles: ['trainer'], ms: 7 }
    ]
  },
  {
    title: 'My learning',
    items: [
      { id: 'student.courses', label: 'My courses', icon: 'bi-journal-code', href: 'pages/student/courses.html', roles: ['student'], ms: 7 },
      { id: 'student.fees', label: 'Fees & receipts', icon: 'bi-cash-coin', href: 'pages/student/fees.html', roles: ['student', 'student-pending'], ms: 7 },
      { id: 'student.assignments', label: 'Assignments', icon: 'bi-journal-check', href: 'pages/student/assignments.html', roles: ['student'], ms: 7 },
      { id: 'student.attendance', label: 'Attendance', icon: 'bi-calendar-check', href: 'pages/student/attendance.html', roles: ['student'], ms: 7 },
      { id: 'student.idcard', label: 'ID card & certificates', icon: 'bi-person-vcard', href: 'pages/student/id-card.html', roles: ['student'], ms: 7 },
      { id: 'student.messages', label: 'Messages', icon: 'bi-chat-left-text', href: 'pages/student/messages.html', roles: ['student'], ms: 7 },
      { id: 'student.results', label: 'Results', icon: 'bi-clipboard2-check', href: 'pages/student/results.html', roles: ['student'], ms: 7 },
      { id: 'student.feedback', label: 'Feedback', icon: 'bi-star-half', href: 'pages/student/feedback.html', roles: ['student'], ms: 7 },
      { id: 'student.placement', label: 'Placement', icon: 'bi-briefcase', href: 'pages/student/placement.html', roles: ['student'], ms: 7 },
      { id: 'student.apply', label: 'Apply for a course', icon: 'bi-plus-circle', href: 'pages/student/apply.html', roles: ['student'], ms: 7 }
    ]
  },
  {
    title: 'My work',
    items: [
      { id: 'employee.payslips', label: 'Payslips & salary', icon: 'bi-file-earmark-ruled', href: 'pages/employee/payslips.html', roles: ['employee', 'trainer'], ms: 7 },
      { id: 'employee.leave', label: 'Leave', icon: 'bi-calendar-x', href: 'pages/employee/leave.html', roles: ['employee', 'trainer'], ms: 7 },
      { id: 'employee.attendance', label: 'My attendance', icon: 'bi-calendar-check', href: 'pages/employee/attendance.html', roles: ['employee', 'trainer'], ms: 7 },
      { id: 'employee.documents', label: 'Documents & offer letter', icon: 'bi-folder2-open', href: 'pages/employee/documents.html', roles: ['employee', 'trainer'], ms: 7 },
      { id: 'employee.resign', label: 'Resignation', icon: 'bi-box-arrow-right', href: 'pages/employee/resign.html', roles: ['employee', 'trainer'], ms: 7 }
    ]
  },
  {
    title: 'System',
    items: [
      { id: 'management.settings', label: 'Settings', icon: 'bi-gear', href: 'pages/management/settings.html', roles: ['admin'], ms: 7 },
      { id: 'management.audit', label: 'Audit log', icon: 'bi-clock-history', href: 'pages/management/audit-log.html', roles: ['admin'], ms: 7 },
      { id: 'system.emails', label: 'Email previews', icon: 'bi-envelope', href: 'pages/system/emails.html', roles: ['admin'], ms: 7 },
      { id: 'system.uikit', label: 'UI kit', icon: 'bi-palette', href: 'pages/system/ui-kit.html', roles: ['admin', 'counsellor', 'hr', 'trainer', 'student', 'employee'], ms: 1 }
    ]
  }
];
