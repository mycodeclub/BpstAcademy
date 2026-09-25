# BPST Edu: HTML + Bootstrap 5 Template

This is a plain HTML/CSS/JS template. It needs no build step and no Node.js: edit the `.html` files directly and upload the folder to any web host.

## How to view the UI
- **Quickest:** double-click `index.html`. It opens in your browser and all pages link to each other.
- **Recommended (same behaviour as the live site):** in VS Code, install the "Live Server" extension, right-click `index.html` and choose "Open with Live Server".
  - Or run `npx http-server . -p 8080` in this folder and open http://localhost:8080
- **Check mobile and tablet:** press F12 in Chrome, then Ctrl+Shift+M (device toolbar). Choose iPhone, iPad or a custom width.
  - Check 375, 768, 1024 and 1440px.
- **Preview the site after the offer ends:** add `?now=2026-11-09` to any URL, e.g. `index.html?now=2026-11-09`.

## Folder structure
```
index.html                 Homepage (LoveDesigne design, ₹49 Diwali pre-booking)
courses.html               All 58 courses with search + category filter
<category>.html            14 category pages (mega menu → tab layout), e.g. industrial-training.html
courses/<course>.html      58 course pages (syllabus, roadmap, projects, FAQs, pre-book form)
prebook.html               ₹49 pre-booking page (form → Razorpay / UPI QR → WhatsApp confirmation)
about.html, contact.html, online-courses.html, locations.html
terms.html, refund-policy.html, privacy.html, 404.html
assets/css/bpst-base.css   Original BpstEduLoveDesigne stylesheet (colours, fonts, components)
assets/css/bpst.css        New layer: mega menu, course tabs, Diwali offer, responsive fixes
assets/js/bpst.js          Countdown, menus, tabs, matcher, filters, booking flow
assets/js/config.js        ← Payment and lead settings (Razorpay, UPI ID, form endpoint)
assets/img/                Optimized images, tech logos (SVG), icons, OG image
assets/vendor/             Bootstrap 5.3.3 + Bootstrap Icons 1.11.3 (local, no CDN)
assets/docs/               Brochure PDFs
sitemap.xml, robots.txt, llms.txt, site.webmanifest   SEO / AI-search files
```

## Common edits
- **Phone, address or offer text:** they appear on every page, so use your editor's "Replace in files" (Ctrl+Shift+H in VS Code).
- **Payment:** set `razorpayKeyId` (or `razorpayPaymentLink`) and `upiId` in `assets/js/config.js`.
- **Receiving bookings:** set `leadEndpoint` in `assets/js/config.js`. Until then the forms run in demo mode and only log to the browser console.
- **Offer deadline:** it is stored in the `data-offer-deadline` attribute on each page's `<body>`, currently `2026-11-08T23:59:59+05:30`. Replace it in all files to change it.
- **Header, footer and mega menu:** they are repeated in every page, the same as in BpstEduLikeCourseListing/Template. Use "Replace in files" to change them everywhere.
