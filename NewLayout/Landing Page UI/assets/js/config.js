/* ==========================================================================
   BPST Edu — site configuration (edit here, no rebuild needed)
   ========================================================================== */
window.BPST_CONFIG = {
  // POST endpoint for leads/bookings (JSON). Empty = demo mode (logs to console, no network).
  // Payload contract matches BpstEduLikeCourseListing/Template/docs/FORMS-API.md plus booking fields.
  leadEndpoint: '',

  // --- Razorpay (card / UPI / netbanking) — use ONE of these ---
  // 1) Standard Checkout: your public Key ID (rzp_live_... or rzp_test_...). Enable auto-capture in the dashboard.
  razorpayKeyId: '',
  // 2) Or a Razorpay Payment Link / Payment Page URL for ₹49 (opens in a new tab).
  razorpayPaymentLink: '',

  // --- UPI QR fallback ---
  upiId: '',            // e.g. 'yourbusiness@okaxis' — the QR and deep link are generated from this
  upiName: 'BPST Edu',
  amount: 49,

  whatsapp: '918299101616',

  // --- Student / staff portal (the AdminTemplate folder, deployed at /portal/ on this domain) ---
  // Header "Login" and footer "Verify a certificate" links use this. Opened from disk, ../AdminTemplate/ is used instead.
  portalUrl: 'https://edu.bitprosofttech.com/portal/',
  // Offer deadline is also embedded in each page (data-offer-deadline) from src/data/site.mjs.
};
