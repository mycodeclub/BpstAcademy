/* Fake demo data only — invented names, masked phones, example.com emails. Never put real people here.
   Lead times are minutes before page load, so every SLA state is always visible. */
window.DEMO = {
  user: { name: 'Demo Admin', initials: 'DA', email: 'admin@example.com' },
  /* the signed-in person for each demo role (top-bar avatar) */
  users: {
    counsellor: { name: 'Neha Sharma', initials: 'NS', email: 'neha.s@example.com' },
    hr: { name: 'Sunita Pal', initials: 'SP', email: 'sunita.p@example.com' },
    trainer: { name: 'Arjun Mehta', initials: 'AM', email: 'arjun.m@example.com' },
    student: { name: 'Harsh Kumar', initials: 'HK', email: 'harsh.k@example.com' },
    employee: { name: 'Neha Sharma', initials: 'NS', email: 'neha.s@example.com' },
    'student-pending': { name: 'Aditi Verma', initials: 'AV', email: 'aditi.v@example.com' },
    'employee-onboarding': { name: 'Tushar Rawat', initials: 'TR', email: 'tushar.r@example.com' }
  },

  leads: [
    { id: 'L-2419', name: 'Aarav Mishra', phone: '98XXXXXX14', course: 'Full Stack Development', source: 'Website pre-book ₹49', paid: true, owner: 'Neha S.', minutesAgo: 97 },
    { id: 'L-2421', name: 'Simran Kaur', phone: '97XXXXXX32', course: 'Data Analyst', source: 'Google Ads', owner: 'Rahul T.', minutesAgo: 71 },
    { id: 'L-2423', name: 'Mohd. Faizan', phone: '90XXXXXX07', course: 'Python Programming', source: 'Walk-in', owner: 'Neha S.', minutesAgo: 63 },
    { id: 'L-2425', name: 'Priya Yadav', phone: '99XXXXXX58', course: 'Software Testing', source: 'Website enquiry', owner: 'Rahul T.', minutesAgo: 52 },
    { id: 'L-2426', name: 'Kunal Srivastava', phone: '88XXXXXX21', course: 'Cyber Security', source: 'Instagram', owner: 'Unassigned', minutesAgo: 47 },
    { id: 'L-2427', name: 'Ananya Gupta', phone: '96XXXXXX90', course: 'AI & Machine Learning', source: 'Website pre-book ₹49', paid: true, owner: 'Neha S.', minutesAgo: 38 },
    { id: 'L-2428', name: 'Rohan Pandey', phone: '93XXXXXX45', course: 'Class 6–12 Coding', source: 'Referral', owner: 'Rahul T.', minutesAgo: 31 },
    { id: 'L-2429', name: 'Sana Rizvi', phone: '91XXXXXX63', course: 'Cloud & DevOps', source: 'JustDial', owner: 'Neha S.', minutesAgo: 18 },
    { id: 'L-2430', name: 'Vivek Chauhan', phone: '82XXXXXX19', course: 'Java Programming', source: 'Website enquiry', owner: 'Rahul T.', minutesAgo: 9 },
    { id: 'L-2431', name: 'Isha Tiwari', phone: '70XXXXXX88', course: 'Data Analyst', source: 'WhatsApp', owner: 'Unassigned', minutesAgo: 2 }
  ],

  kpis: {
    leadsToday: 14, leadsYesterday: 11,
    conversion30d: 18.4, conversionPrev: 16.3,
    collectedToday: 48500, collectedYesterday: 36200,
    monthTarget: 900000,
    dues: 345600, overdue: 92400,
    attendancePresent: 212, attendanceTotal: 246,
    verifications: 7, approvals: 5
  },

  /* ₹ collected per day for the last 28 days, ending today (Sundays are low). Month-to-date = last 25 entries. */
  collections: [
    26400, 4200, 30100, 44800, 21700, 33600, 29500, 38500, 3500, 27800, 39900, 25300, 47600, 31800,
    22800, 0, 35400, 28600, 19800, 51200, 38500, 24000, 9000, 31200, 27500, 42000, 36200, 48500
  ],

  sources: [
    { label: 'Website pre-book ₹49', value: 64 },
    { label: 'Website enquiry', value: 48 },
    { label: 'Walk-in', value: 31 },
    { label: 'Google Ads', value: 27 },
    { label: 'Referral', value: 22 },
    { label: 'Instagram', value: 15 },
    { label: 'JustDial', value: 9 }
  ],

  funnel: [
    { label: 'Leads', value: 216 },
    { label: 'Contacted', value: 188 },
    { label: 'Counselled', value: 121 },
    { label: 'Applied', value: 62 },
    { label: 'Admitted', value: 40 }
  ],

  approvals: [
    { id: 'AP-311', type: 'Discount', icon: 'bi-ticket-perforated', text: 'Upfront 10% on Full Stack (₹3,500)', who: 'Aditi Verma · requested by Neha S.' },
    { id: 'AP-312', type: 'Fine waiver', icon: 'bi-slash-circle', text: 'Waive late fine ₹650', who: 'Harsh Kumar · requested by Accounts' },
    { id: 'AP-313', type: 'Sibling discount', icon: 'bi-people', text: '₹2,000 off Class 6–12 Coding', who: 'Meera & Kabir Singh · requested by Rahul T.' },
    { id: 'AP-314', type: 'Leave', icon: 'bi-calendar-x', text: 'Casual leave · 2 days (29–30 Sep)', who: 'Pooja Nigam, Trainer' },
    { id: 'AP-315', type: 'Coupon', icon: 'bi-ticket', text: 'DIWALI49 on Data Analyst (₹1,500)', who: 'Nikhil Arora · requested by Neha S.' }
  ],

  counsellors: [
    { name: 'Neha S.', initials: 'NS', leads: 92, admitted: 21, avgResponse: '14 min', breaches: 2 },
    { name: 'Rahul T.', initials: 'RT', leads: 81, admitted: 14, avgResponse: '23 min', breaches: 5 },
    { name: 'Front desk', initials: 'FD', leads: 43, admitted: 5, avgResponse: '31 min', breaches: 4 }
  ],

  classesToday: [
    { time: '9:00 AM', batch: 'Python · PY-M1', trainer: 'Pooja Nigam', room: 'Lab 1', strength: '18/20', status: 'Done' },
    { time: '11:00 AM', batch: 'Full Stack · FS-M2', trainer: 'Arjun Mehta', room: 'Lab 2', strength: '22/24', status: 'Done' },
    { time: '2:00 PM', batch: 'Data Analyst · DA-A1', trainer: 'Ritu Saxena', room: 'Lab 1', strength: '16/20', status: 'Live' },
    { time: '4:00 PM', batch: 'Class 6–12 · SC-E1', trainer: 'Pooja Nigam', room: 'Room 3', strength: '12/15', status: 'Upcoming' },
    { time: '6:00 PM', batch: 'Cyber Security · CS-E1', trainer: 'Vikas Dubey', room: 'Lab 2', strength: '14/18', status: 'Upcoming' }
  ],

  dues: [
    { name: 'Harsh Kumar', initials: 'HK', course: 'Full Stack', due: 12500, days: 34, fine: 1000 },
    { name: 'Nidhi Awasthi', initials: 'NA', course: 'Data Analyst', due: 8000, days: 21, fine: 700 },
    { name: 'Aman Khan', initials: 'AK', course: 'Python', due: 5000, days: 12, fine: 250 },
    { name: 'Riya Bajpai', initials: 'RB', course: 'Software Testing', due: 7500, days: 9, fine: 100 },
    { name: 'Sahil Verma', initials: 'SV', course: 'Cloud & DevOps', due: 15000, days: 3, fine: 0 }
  ],

  verifications: [
    { name: 'Aditi Verma', initials: 'AV', kind: 'Student', docs: 'Aadhaar, 12th marksheet, photo', status: 'Session booked', when: 'Today 4:30 PM' },
    { name: 'Tushar Rawat', initials: 'TR', kind: 'Employee', docs: 'PAN, Aadhaar, degree, experience letter', status: 'Submitted', when: 'Waiting 2 days' },
    { name: 'Meera Singh', initials: 'MS', kind: 'Student', docs: 'Aadhaar, school ID', status: 'Submitted', when: 'Waiting 1 day' },
    { name: 'Nikhil Arora', initials: 'NA', kind: 'Student', docs: '2 of 4 uploaded', status: 'Documents pending', when: 'Reminder sent' },
    { name: 'Farah Naqvi', initials: 'FN', kind: 'Student', docs: 'Aadhaar, graduation marksheet', status: 'Session booked', when: 'Tomorrow 11:00 AM' }
  ],

  notifications: [
    { icon: 'bi-alarm', tone: 'danger', text: '<b>Aarav Mishra</b> breached the 1-hour SLA. Manager alerted.', time: '37 min ago', unread: true },
    { icon: 'bi-cash-coin', tone: 'success', text: 'Payment ₹12,000 received from <b>Farah Naqvi</b> (UPI).', time: '52 min ago', unread: true },
    { icon: 'bi-ticket-perforated', tone: 'warning', text: 'Discount approval requested for <b>Aditi Verma</b>.', time: '1 h ago', unread: true },
    { icon: 'bi-shield-check', tone: 'info', text: '<b>Tushar Rawat</b> submitted documents for verification.', time: '2 d ago', unread: false },
    { icon: 'bi-calendar-x', tone: 'neutral', text: '<b>Pooja Nigam</b> applied for casual leave.', time: '3 h ago', unread: false }
  ],

  /* global search index (Ctrl+K) */
  search: [
    { group: 'Leads', label: 'Aarav Mishra', meta: 'L-2419 · Full Stack · 98XXXXXX14', icon: 'bi-inbox', href: 'pages/crm/lead.html' },
    { group: 'Leads', label: 'Simran Kaur', meta: 'L-2421 · Data Analyst · 97XXXXXX32', icon: 'bi-inbox', href: 'pages/crm/lead.html' },
    { group: 'Leads', label: 'Isha Tiwari', meta: 'L-2431 · Data Analyst · 70XXXXXX88', icon: 'bi-inbox', href: 'pages/crm/lead.html' },
    { group: 'Students', label: 'Harsh Kumar', meta: 'BPST26S0042 · Full Stack · FS-M2', icon: 'bi-mortarboard', href: 'pages/admissions/student.html' },
    { group: 'Students', label: 'Nidhi Awasthi', meta: 'BPST26S0051 · Data Analyst · DA-A1', icon: 'bi-mortarboard', href: 'pages/admissions/student.html' },
    { group: 'Students', label: 'Farah Naqvi', meta: 'BPST26S0063 · Python · PY-M1', icon: 'bi-mortarboard', href: 'pages/admissions/student.html' },
    { group: 'Receipts', label: 'BPST/RC/2026-27/0412', meta: '₹12,000 · Farah Naqvi · 25 Sep 2026', icon: 'bi-receipt', href: 'print/receipt.html' },
    { group: 'Receipts', label: 'BPST/RC/2026-27/0411', meta: '₹8,500 · Aman Khan · 24 Sep 2026', icon: 'bi-receipt', href: 'print/receipt.html' },
    { group: 'Employees', label: 'Pooja Nigam', meta: 'BPST26E004 · Trainer · Python, School coding', icon: 'bi-person-badge', href: 'pages/hr/employee.html' },
    { group: 'Employees', label: 'Neha Sharma', meta: 'BPST26E002 · Senior counsellor', icon: 'bi-person-badge', href: 'pages/hr/employee.html' },
    { group: 'Batches', label: 'FS-M2 · Full Stack', meta: 'Mon–Fri 11:00 AM · Lab 2 · Arjun Mehta', icon: 'bi-collection', href: 'pages/admissions/batch.html' },
    { group: 'Pages', label: 'Owner dashboard', meta: 'Overview', icon: 'bi-grid-1x2', href: 'pages/management/dashboard.html' },
    { group: 'Pages', label: 'Lead inbox', meta: 'Leads & CRM', icon: 'bi-inbox', href: 'pages/crm/leads.html' },
    { group: 'Pages', label: 'Enrol student', meta: 'Admissions', icon: 'bi-person-plus', href: 'pages/admissions/enrol.html' },
    { group: 'Pages', label: 'Collections', meta: 'Fees & Accounts', icon: 'bi-cash-stack', href: 'pages/fees/collections.html' },
    { group: 'Pages', label: 'Verification queue', meta: 'Verification', icon: 'bi-shield-check', href: 'pages/verification/queue.html' },
    { group: 'Pages', label: 'Payroll', meta: 'HR & Payroll', icon: 'bi-wallet2', href: 'pages/hr/payroll.html' },
    { group: 'Pages', label: 'Reports', meta: 'Overview', icon: 'bi-bar-chart', href: 'pages/management/reports.html' },
    { group: 'Pages', label: 'Settings', meta: 'System', icon: 'bi-gear', href: 'pages/management/settings.html' },
    { group: 'Pages', label: 'UI kit', meta: 'Design reference', icon: 'bi-palette', href: 'pages/system/ui-kit.html' }
  ]
};
