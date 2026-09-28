/* ==========================================================================
   BPST Academy — site behaviour (vanilla JS, Bootstrap 5 bundle)
   Offer countdown · navbar · tabs · hero/sandbox · matcher · filters ·
   YouTube facade · ₹49 pre-booking with Razorpay + UPI QR · enquiry form
   ========================================================================== */
(function () {
  'use strict';
  const $ = (s, el = document) => el.querySelector(s);
  const $$ = (s, el = document) => Array.from(el.querySelectorAll(s));
  const CFG = window.BPST_CONFIG || {};
  const store = {
    get(k) { try { return sessionStorage.getItem(k); } catch (e) { return null; } },
    set(k, v) { try { sessionStorage.setItem(k, v); } catch (e) { /* storage blocked */ } },
  };
  const params = new URLSearchParams(location.search);

  /* ---------- Offer state & countdown ---------- */
  const deadline = new Date(document.body.dataset.offerDeadline || '2026-11-08T23:59:59+05:30').getTime();
  const nowOverride = params.get('now') ? new Date(params.get('now')).getTime() : null; // QA: ?now=2026-11-09
  const offsetMs = nowOverride ? nowOverride - Date.now() : 0;
  const now = () => Date.now() + offsetMs;
  const offerOpen = () => now() < deadline;
  const pad = (n) => String(n).padStart(2, '0');

  function applyOfferState() {
    document.documentElement.classList.toggle('bpst-offer-closed', !offerOpen());
  }
  function tick() {
    const ms = Math.max(0, deadline - now());
    const d = Math.floor(ms / 864e5), h = Math.floor(ms / 36e5) % 24, m = Math.floor(ms / 6e4) % 60, s = Math.floor(ms / 1e3) % 60;
    $$('[data-countdown]').forEach((el) => {
      const set = (k, v) => { const x = el.querySelector(k); if (x && x.textContent !== v) x.textContent = v; };
      set('[data-d]', String(d)); set('[data-h]', pad(h)); set('[data-m]', pad(m)); set('[data-s]', pad(s));
    });
    if (!ms) applyOfferState();
  }
  applyOfferState();
  tick();
  setInterval(tick, 1000);

  const strip = $('#offerStrip');
  if (strip) {
    if (store.get('bpstOfferHidden') === '1') strip.remove();
    $('[data-dismiss-offer]', strip)?.addEventListener('click', () => { strip.remove(); store.set('bpstOfferHidden', '1'); setNavH(); });
  }

  /* ---------- Navbar ---------- */
  const header = $('#siteHeader');
  const navbar = $('.bpst-navbar');
  function setNavH() { if (header) document.documentElement.style.setProperty('--navh', header.offsetHeight + 'px'); }
  setNavH();
  window.addEventListener('resize', setNavH);
  window.addEventListener('scroll', () => navbar && navbar.classList.toggle('scrolled', window.scrollY > 40), { passive: true });
  $$('.navbar-collapse a:not(.dropdown-toggle)').forEach((a) => a.addEventListener('click', () => {
    const c = $('.navbar-collapse.show');
    if (c && window.bootstrap) bootstrap.Collapse.getOrCreateInstance(c).hide();
  }));

  /* ---------- Floating pill ---------- */
  const pill = $('#desktopFloatingPill');
  if (pill) {
    if (store.get('bpstPillHidden') === '1') pill.remove();
    $('#btnCloseFloatingPill')?.addEventListener('click', () => { pill.remove(); store.set('bpstPillHidden', '1'); });
    const target = $('[data-prebook]') || $('#footer');
    if (target && 'IntersectionObserver' in window) {
      new IntersectionObserver((es) => es.forEach((e) => { pill.style.opacity = e.isIntersecting ? '0' : ''; pill.style.pointerEvents = e.isIntersecting ? 'none' : ''; })).observe(target);
    }
    // The offer strip and hero already show the offer, so the pill waits until the hero is off screen
    const hero = $('.bpst-hero, .bpst-page-hero');
    if (hero && 'IntersectionObserver' in window) {
      pill.classList.add('is-hidden');
      new IntersectionObserver((es) => es.forEach((e) => pill.classList.toggle('is-hidden', e.isIntersecting))).observe(hero);
    }
  }

  /* ---------- Category tabs: deep links (#slug), prev/next ---------- */
  function showTab(id, scroll) {
    const btn = document.getElementById('t-' + id);
    if (!btn || !window.bootstrap) return false;
    bootstrap.Tab.getOrCreateInstance(btn).show();
    const bar = btn.parentElement; bar.scrollLeft = btn.offsetLeft - bar.clientWidth / 2 + btn.clientWidth / 2;
    if (scroll) {
      const tb = btn.closest('.tabbar');
      const y = tb.getBoundingClientRect().top + window.scrollY - (header ? header.offsetHeight : 0);
      window.scrollTo({ top: Math.max(0, y), behavior: 'smooth' });
    }
    return true;
  }
  $$('.tab-pill').forEach((b) => b.addEventListener('shown.bs.tab', () => history.replaceState(null, '', '#' + b.dataset.bsTarget.slice(1))));
  $$('[data-tab-go]').forEach((b) => b.addEventListener('click', () => showTab(b.dataset.tabGo, true)));
  window.addEventListener('load', () => { const h = decodeURIComponent(location.hash.slice(1)); if (h) showTab(h, true); });
  window.addEventListener('hashchange', () => showTab(decodeURIComponent(location.hash.slice(1)), true));

  /* ---------- Hero visual switcher + sandbox ---------- */
  const heroTabs = [['#tabHeroCampus', '#heroCampusView'], ['#tabHeroCertified', '#heroCertifiedView'], ['#tabHeroSandbox', '#heroSandboxView']];
  function switchHero(i) {
    heroTabs.forEach(([t, v], j) => {
      const tb = $(t), vw = $(v); if (!tb || !vw) return;
      tb.classList.toggle('active', i === j); tb.setAttribute('aria-selected', String(i === j));
      vw.classList.toggle('d-none', i !== j);
      if (i === j) vw.classList.add('bpst-fade-in');
    });
    if (i === 1) { // lazy-load the certified photo only when requested
      const p = $('[data-lazy-hero]');
      if (p) { p.querySelectorAll('[data-srcset]').forEach((s) => { s.srcset = s.dataset.srcset; }); const im = p.querySelector('img[data-src]'); if (im) im.src = im.dataset.src; p.removeAttribute('data-lazy-hero'); }
    }
  }
  heroTabs.forEach(([t], i) => $(t)?.addEventListener('click', () => switchHero(i)));
  $('#btnPeekTerminal')?.addEventListener('click', () => switchHero(2));

  const code = $('#heroCodeSnippet');
  const K = (s) => `<span class="bpst-code-keyword">${s}</span>`, T = (s) => `<span class="bpst-code-type">${s}</span>`, S = (s) => `<span class="bpst-code-string">${s}</span>`, F = (s) => `<span class="bpst-code-function">${s}</span>`, C = (s) => `<span class="bpst-code-comment">${s}</span>`;
  const files = {
    cs: code ? code.innerHTML : '',
    py: [`<div>${C('# BPST Capstone: Agentic AI support agent (LangGraph + RAG)')}</div>`, `<div>${K('from')} langgraph.graph ${K('import')} StateGraph</div>`, `<div>${K('async def')} ${F('answer')}(state: ${T('AgentState')}):</div>`, `<div>&nbsp;&nbsp;docs = ${K('await')} retriever.${F('ainvoke')}(state[${S('"question"')}])</div>`, `<div>&nbsp;&nbsp;reply = ${K('await')} llm.${F('ainvoke')}(prompt.${F('format')}(context=docs))</div>`, `<div>&nbsp;&nbsp;${K('return')} {${S('"answer"')}: reply.content, ${S('"sources"')}: docs}</div>`].join(''),
    yml: [`<div>${C('# BPST DevOps: Automated CI/CD &amp; Docker Hosting')}</div>`, `<div>${K('name:')} ${S('Build and Deploy Production Container')}</div>`, `<div>${K('on:')} [push]</div>`, `<div>${K('jobs:')}</div>`, `<div>&nbsp;&nbsp;${K('deploy:')}</div>`, `<div>&nbsp;&nbsp;&nbsp;&nbsp;${K('runs-on:')} ubuntu-latest</div>`, `<div>&nbsp;&nbsp;&nbsp;&nbsp;${K('steps:')} - ${K('run:')} docker build -t bpst/capstone:v1 .</div>`].join(''),
  };
  $$('#heroSandboxTabs .bpst-terminal-tab').forEach((tab) => tab.addEventListener('click', () => {
    $$('#heroSandboxTabs .bpst-terminal-tab').forEach((t) => t.classList.remove('active'));
    tab.classList.add('active');
    if (code && files[tab.dataset.file]) code.innerHTML = files[tab.dataset.file];
  }));
  $('#btnRunHeroCode')?.addEventListener('click', (e) => {
    const con = $('#heroTerminalConsole'); if (!con) return;
    const btn = e.currentTarget; btn.disabled = true;
    con.innerHTML = '<div class="bpst-console-line"><span class="bpst-console-prompt">$</span> <span class="text-white-50">dotnet test &amp;&amp; docker compose up -d</span></div>';
    setTimeout(() => con.insertAdjacentHTML('beforeend', '<div class="bpst-console-line"><span class="bpst-console-badge">BUILD</span> <span class="text-info">42 unit tests passed (100% coverage)</span></div>'), 700);
    setTimeout(() => { con.insertAdjacentHTML('beforeend', '<div class="bpst-console-line"><span class="bpst-console-badge">DEPLOY</span> <span class="bpst-console-success">✔ Live Cloud: https://student-project.bpst.edu (HTTP 200 OK)</span></div>'); btn.disabled = false; }, 1400);
  });

  /* ---------- Academic vs industry toggle ---------- */
  const dual = $('#gapDualView'), bench = $('#gapBenchmarkView');
  $('#btnViewDual')?.addEventListener('click', (e) => { dual?.classList.remove('d-none'); bench?.classList.add('d-none'); e.currentTarget.classList.add('active'); $('#btnViewBenchmark').classList.remove('active'); });
  $('#btnViewBenchmark')?.addEventListener('click', (e) => { bench?.classList.remove('d-none'); dual?.classList.add('d-none'); e.currentTarget.classList.add('active'); $('#btnViewDual').classList.remove('active'); });

  /* ---------- Career matcher ---------- */
  const tracks = {
    'Full Stack': ['full-stack-development', 'Software Development & Full Stack Track', 'C# • ASP.NET Core • React • SQL Server • Cloud REST APIs', 'Master the full enterprise web engineering cycle. Go from basic programming logic to architecting production SaaS microservices with database design and CI/CD.', ['Enterprise Multi-Tenant SaaS Portal with JWT Security', 'Real-Time Order & Inventory Processing API with Docker']],
    'Data & AI': ['agentic-ai-engineering', 'Agentic AI Engineering Track', 'Python • LLM APIs • RAG • LangGraph • CrewAI • MCP', 'Build AI agents that reason, call tools and automate real work — from RAG chatbots to multi-agent workflows with evaluation and guardrails.', ['Customer-support agent with RAG over company docs', 'Multi-agent research & report writer']],
    Data: ['data-analyst-pro', 'Data Analyst Pro Track', 'Excel • SQL • Power BI • Tableau • Python', 'The most accessible data career: turn raw business data into dashboards and decisions. Ideal for any graduate, including non-CS.', ['Sales performance dashboard in Power BI', 'HR attrition analysis with SQL + Tableau']],
    'Cloud & Security': ['devops-sre-terraform', 'DevOps + SRE + Terraform Track', 'Linux • Docker • Kubernetes • Terraform • GitHub Actions • Grafana', 'Learn infrastructure as code, CI/CD pipelines, container orchestration and site reliability engineering on real cloud accounts.', ['Zero-downtime CI/CD pipeline to Kubernetes', 'Terraform multi-environment cloud setup']],
    Testing: ['software-testing-masters', 'Software Testing Masters Track', 'Manual • JIRA • Selenium • Playwright • Postman • Jenkins', 'From manual test design to automation frameworks and API testing — the fastest route into IT for non-CS graduates.', ['Hybrid Selenium framework for an e-commerce site', 'Playwright suite with a CI pipeline']],
    Enterprise: ['sap-s4hana-fico', 'SAP S/4HANA FICO Track', 'SAP S/4HANA • Fiori • GL • AP/AR • Controlling', 'SAP ECC support ends in 2027 — S/4HANA migration projects need finance consultants. Ideal for commerce and finance backgrounds.', ['Configure a company code end-to-end', 'Month-end close simulation']],
    'Frontend Engineering': ['advanced-ui-angular-react', 'Advanced UI / Frontend Engineering Track', 'React • Angular • TypeScript • State Management • Design Systems', 'Craft fast, pixel-perfect web interfaces, reusable component libraries and interactive enterprise dashboards.', ['High-Performance Multi-Device SaaS Dashboard', 'Real-Time Collaborative Drag-and-Drop Workspace']],
    'Mobile & Games': ['mobile-app-development', 'Mobile App & Game Development Track', 'Android • iOS • Flutter • Unity C#', 'Build cross-platform mobile apps published to app stores, or develop 2D and 3D games with physics engines.', ['Cross-Platform Delivery & Tracking Mobile App', 'Interactive 3D Simulation Game with Unity & C#']],
  };
  let mDeg = 'B.Tech (CS / IT / ECE / Other)', mDom = 'Full Stack', mDur = '3-Month Program';
  function updateMatch() {
    const t = tracks[mDom] || tracks['Full Stack'];
    const score = /BCA/.test(mDeg) ? 96 : /Polytechnic/.test(mDeg) ? 95 : /Commerce|Working/.test(mDeg) ? 94 : 98;
    const set = (id, v) => { const el = document.getElementById(id); if (el) el.textContent = v; };
    const badge = $('#matchBadgeScore'); if (badge) badge.innerHTML = `<i class="bi bi-patch-check-fill"></i> ${score}% Compatibility Match`;
    set('matchDegreePill', mDeg.split('(')[0].trim()); set('matchTrackTitle', t[1]); set('matchTrackTagline', t[2]); set('matchTrackDescription', t[3]);
    const ul = $('#matchProjectsList'); if (ul) ul.innerHTML = t[4].map((p) => `<li><i class="bi bi-check2 text-info me-1"></i> ${p}</li>`).join('');
    const root = document.body.dataset.root || '';
    const book = $('#btnBookMatchedTrack'); if (book) book.href = `${root}prebook?course=${t[0]}&duration=${encodeURIComponent(mDur)}`;
    const view = $('#btnViewMatchedTrack'); if (view) view.href = `${root}courses/${t[0]}`;
  }
  const matcherGroup = (sel, attr, fn) => $$(sel + ' .bpst-matcher-btn').forEach((b) => b.addEventListener('click', () => {
    $$(sel + ' .bpst-matcher-btn').forEach((x) => x.classList.remove('active')); b.classList.add('active'); fn(b.getAttribute(attr)); updateMatch();
  }));
  matcherGroup('#matcherDegreeOptions', 'data-degree', (v) => (mDeg = v));
  matcherGroup('#matcherDomainOptions', 'data-domain', (v) => (mDom = v));
  matcherGroup('#matcherDurationOptions', 'data-duration', (v) => (mDur = v));

  /* ---------- Workflow role filter ---------- */
  $$('.bpst-role-filter-btn').forEach((b) => b.addEventListener('click', () => {
    $$('.bpst-role-filter-btn').forEach((x) => x.classList.remove('active')); b.classList.add('active');
    const role = b.dataset.roleFilter;
    $$('.bpst-workflow-step').forEach((s) => s.classList.toggle('is-dim', role !== 'all' && !(s.dataset.roles || '').split(' ').includes(role)));
  }));

  /* ---------- Technology filter (scoped: does not affect course filters) ---------- */
  $$('#techCategoryFilter .bpst-filter-btn').forEach((b) => b.addEventListener('click', () => {
    $$('#techCategoryFilter .bpst-filter-btn').forEach((x) => x.classList.remove('active')); b.classList.add('active');
    const f = b.dataset.techFilter;
    $$('#techShowcaseGrid .bpst-tech-card-col').forEach((c) => {
      const show = f === 'all' || c.dataset.techCat.split(' ').includes(f);
      c.classList.toggle('d-none', !show); if (show) c.classList.add('bpst-fade-in');
    });
  }));

  /* ---------- Catalog filter + search ---------- */
  const search = $('[data-course-search]');
  let catFilter = 'all';
  function filterCatalog() {
    const q = (search?.value || '').trim().toLowerCase();
    let shown = 0;
    $$('#catalogGrid .bpst-course-col').forEach((c) => {
      const ok = (catFilter === 'all' || c.dataset.category === catFilter) && (!q || c.dataset.search.includes(q));
      c.classList.toggle('d-none', !ok); if (ok) shown++;
    });
    $('[data-no-results]')?.classList.toggle('d-none', shown > 0);
  }
  $$('[data-course-filter] .bpst-filter-btn').forEach((b) => b.addEventListener('click', () => {
    $$('[data-course-filter] .bpst-filter-btn').forEach((x) => x.classList.remove('active')); b.classList.add('active');
    catFilter = b.dataset.filter; filterCatalog();
  }));
  search?.addEventListener('input', filterCatalog);
  if (search && params.get('q')) { search.value = params.get('q'); filterCatalog(); }

  /* ---------- Count-up stats ---------- */
  const counters = $$('[data-count]');
  if (counters.length && 'IntersectionObserver' in window && !matchMedia('(prefers-reduced-motion: reduce)').matches) {
    const io = new IntersectionObserver((es) => es.forEach((e) => {
      if (!e.isIntersecting) return; io.unobserve(e.target);
      const el = e.target, end = +el.dataset.count, t0 = performance.now();
      const step = (t) => { const p = Math.min(1, (t - t0) / 1200); el.textContent = Math.round(end * (1 - Math.pow(1 - p, 3))); if (p < 1) requestAnimationFrame(step); };
      requestAnimationFrame(step);
    }), { threshold: 0.6 });
    counters.forEach((c) => io.observe(c));
  }

  /* ---------- YouTube facade ---------- */
  $$('.bpst-yt').forEach((b) => b.addEventListener('click', () => {
    const f = document.createElement('iframe');
    f.src = `https://www.youtube-nocookie.com/embed/${b.dataset.yt}?autoplay=1&rel=0&modestbranding=1`;
    f.title = b.getAttribute('aria-label') || 'Video';
    f.allow = 'accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share';
    f.allowFullscreen = true; f.style.cssText = 'position:absolute;inset:0;width:100%;height:100%;border:0';
    b.replaceWith(f);
  }));

  /* ---------- Links to the portal (student / staff sign-in, certificate check) ----------
     <a data-portal-link="index.html"> → CFG.portalUrl + path. Opened from disk, it uses the sibling AdminTemplate folder. */
  const portalBase = location.protocol === 'file:' ? (CFG.portalLocal || '../AdminTemplate/') : (CFG.portalUrl || '/portal/');
  $$('[data-portal-link]').forEach((a) => {
    let base = portalBase;
    if (location.protocol === 'file:' && /\/courses\/[^/]*$/.test(location.pathname)) base = '../' + base; // course pages sit one folder deeper
    a.href = base + a.dataset.portalLink;
  });

  /* ---------- Lead delivery ---------- */
  /* campaign tags: the first ones seen in this visit are kept, so they survive clicks to other pages */
  const UTM_KEYS = ['utm_source', 'utm_medium', 'utm_campaign', 'utm_term', 'utm_content', 'gclid', 'fbclid'];
  (function keepUtm() {
    const o = {}; UTM_KEYS.forEach((k) => { if (params.get(k)) o[k] = params.get(k); });
    if (Object.keys(o).length && !store.get('bpst.utm')) store.set('bpst.utm', JSON.stringify(Object.assign(o, { landing_page: location.pathname, referrer: document.referrer || '' })));
  })();
  function utm() {
    const o = {}; UTM_KEYS.forEach((k) => { if (params.get(k)) o[k] = params.get(k); });
    if (Object.keys(o).length) return o;
    try { return JSON.parse(store.get('bpst.utm') || '{}'); } catch (e) { return {}; }
  }
  async function sendLead(payload) {
    const body = Object.assign({ page: location.pathname, page_url: location.href.split('#')[0], page_title: document.title, utm: utm(), submitted_at: new Date().toISOString() }, payload);
    if (!CFG.leadEndpoint) { console.info('[BPST demo mode] lead payload:', body); return true; }
    try {
      const ctl = new AbortController(); const to = setTimeout(() => ctl.abort(), 15000);
      const res = await fetch(CFG.leadEndpoint, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body), signal: ctl.signal });
      clearTimeout(to); return res.ok;
    } catch (e) { console.warn('Lead delivery failed', e); return false; }
  }
  const newRef = () => { const a = 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789'; let s = ''; const r = (window.crypto && crypto.getRandomValues) ? crypto.getRandomValues(new Uint8Array(6)) : Array.from({ length: 6 }, () => Math.random() * 255); for (const x of r) s += a[x % a.length]; return 'BPST-D26-' + s; };
  const waLink = (t) => `https://wa.me/${CFG.whatsapp || '918299101616'}?text=${encodeURIComponent(t)}`;
  // Shown when a form could not be saved, so the visitor is never told "received" for data we do not have.
  function saveError(form, show) {
    let el = form.querySelector('[data-save-error]');
    if (!show) { if (el) el.hidden = true; return; }
    if (!el) { el = document.createElement('div'); el.className = 'text-danger small mt-2'; el.setAttribute('role', 'alert'); el.dataset.saveError = ''; form.appendChild(el); }
    el.innerHTML = 'Sorry, we could not send your details. Please try again, or <a href="' + waLink('Hi BPST Academy, I want to enquire about your courses.') + '" target="_blank" rel="noopener">message us on WhatsApp</a>.';
    el.hidden = false;
  }
  const loadScript = (src) => new Promise((ok, fail) => { if ($(`script[src="${src}"]`)) return ok(); const s = document.createElement('script'); s.src = src; s.async = true; s.onload = ok; s.onerror = fail; document.head.appendChild(s); });

  /* ---------- ₹49 pre-booking flow ---------- */
  $$('[data-prebook]').forEach((card) => {
    const form = $('form[data-step="1"]', card);
    const steps = $$('[data-step]', card);
    const inds = $$('[data-step-ind]', card);
    let booking = null;

    const go = (n) => {
      steps.forEach((s) => { s.hidden = s.dataset.step !== String(n); });
      inds.forEach((i) => i.classList.toggle('on', +i.dataset.stepInd <= n));
      if (n > 1) card.scrollIntoView({ behavior: 'smooth', block: 'start' });
    };

    // Prefill from URL (?course=slug&duration=...)
    const courseSel = form.elements.course;
    if (params.get('course') && courseSel && [...courseSel.options].some((o) => o.value === params.get('course'))) courseSel.value = params.get('course');
    if (params.get('duration') && form.elements.duration) { const d = params.get('duration'); if ([...form.elements.duration.options].some((o) => o.value === d)) form.elements.duration.value = d; }
    form.elements.mobile?.addEventListener('input', (e) => { e.target.value = e.target.value.replace(/\D/g, '').slice(0, 10); });

    form.addEventListener('submit', async (e) => {
      e.preventDefault();
      if (form.elements.website.value) return; // honeypot
      if (!form.checkValidity()) { form.classList.add('was-validated'); form.querySelector(':invalid')?.focus(); return; }
      const f = form.elements;
      const courseTitle = f.course.options[f.course.selectedIndex].text;
      booking = {
        booking_ref: newRef(), name: f.name.value.trim(), mobile: '+91' + f.mobile.value, email: f.email.value.trim(), city: f.city.value.trim(),
        course: f.course.value, course_title: courseTitle, duration: f.duration.value, mode: f.mode.value, qualification: f.qualification.value,
        message: f.message ? f.message.value.trim() : '', consent: true,
      };
      const btns = $$('button[type=submit]', form); btns.forEach((b) => { b.disabled = true; });
      if (!offerOpen()) { // After Diwali the same form becomes a plain enquiry
        const saved = await sendLead(Object.assign({ form_type: 'enquiry' }, booking));
        btns.forEach((b) => { b.disabled = false; });
        saveError(form, !saved); if (!saved) return;
        $('[data-done-title]', card).textContent = 'Thank you — enquiry received!';
        $('[data-done-text]', card).innerHTML = 'Our admissions desk will contact you on WhatsApp/phone shortly.';
        return go(3);
      }
      const saved = await sendLead(Object.assign({ form_type: 'prebook_started', amount: CFG.amount || 49 }, booking));
      btns.forEach((b) => { b.disabled = false; });
      saveError(form, !saved); if (!saved) return;
      $$('[data-ref]', card).forEach((x) => { x.textContent = booking.booking_ref; });
      const cn = $('[data-course-name]', card); if (cn) cn.textContent = courseTitle;
      setupUpi();
      go(2);
    });
    $('[data-back]', card)?.addEventListener('click', () => go(1));

    // Payment tabs
    $$('[data-pay-tab]', card).forEach((t) => t.addEventListener('click', () => {
      $$('[data-pay-tab]', card).forEach((x) => { x.classList.toggle('active', x === t); x.setAttribute('aria-selected', String(x === t)); });
      $$('[data-pay-pane]', card).forEach((p) => { p.hidden = p.dataset.payPane !== t.dataset.payTab; });
    }));

    async function done(method, id) {
      const saved = await sendLead(Object.assign({ form_type: 'prebook_paid', payment_method: method, payment_id: id, amount: CFG.amount || 49 }, booking));
      if (!saved) $('[data-done-text]', card).innerHTML = 'We could not record your payment automatically. Please tap <b>Share on WhatsApp</b> below so we can reserve your seat.';
      const msg = `Hello BPST Academy, I pre-booked "${booking.course_title}" for ₹49 (Diwali offer).\nBooking ref: ${booking.booking_ref}\nName: ${booking.name}\nPayment: ${method} ${id}`;
      $('[data-wa-confirm]', card).href = waLink(msg);
      go(3);
    }

    // Razorpay
    const rzBtn = $('[data-razorpay]', card), rzMsg = $('[data-razorpay-msg]', card);
    rzBtn?.addEventListener('click', async () => {
      rzMsg.hidden = true;
      if (CFG.razorpayKeyId) {
        try {
          await loadScript('https://checkout.razorpay.com/v1/checkout.js');
          const rz = new window.Razorpay({
            key: CFG.razorpayKeyId, amount: Math.round((CFG.amount || 49) * 100), currency: 'INR', name: 'BPST Academy',
            description: 'Diwali pre-booking — ' + booking.course_title,
            prefill: { name: booking.name, email: booking.email, contact: booking.mobile },
            notes: { booking_ref: booking.booking_ref, course: booking.course },
            theme: { color: '#0062ff' },
            handler: (resp) => done('razorpay', resp.razorpay_payment_id),
          });
          rz.on('payment.failed', (r) => { rzMsg.textContent = 'Payment failed: ' + (r.error && r.error.description || 'please try again or use UPI QR.'); rzMsg.hidden = false; });
          rz.open();
        } catch (e) { rzMsg.textContent = 'Could not load the payment window. Please use the UPI QR tab.'; rzMsg.hidden = false; }
      } else if (CFG.razorpayPaymentLink) {
        window.open(CFG.razorpayPaymentLink, '_blank', 'noopener');
        rzMsg.innerHTML = 'After paying in the new tab, switch to <b>Scan UPI QR</b> → enter your transaction ID, or share the receipt on WhatsApp.';
        rzMsg.classList.replace('text-danger', 'text-muted'); rzMsg.hidden = false;
      } else {
        rzMsg.innerHTML = 'Online card payment is being set up. Please pay using the <b>Scan UPI QR</b> tab or <a href="' + waLink('Hi BPST Academy, I want to pay ₹49 for booking ' + booking.booking_ref) + '" target="_blank" rel="noopener">message us on WhatsApp</a>.';
        rzMsg.hidden = false;
      }
    });

    // UPI QR + deep link + UTR
    async function setupUpi() {
      const box = $('[data-upi-qr]', card), idEl = $('[data-upi-id]', card), link = $('[data-upi-link]', card);
      if (!CFG.upiId) { idEl.textContent = 'shared on WhatsApp'; box.innerHTML = '<div class="small text-muted p-3" style="width:200px">UPI QR will appear here once the UPI ID is configured.</div>'; link?.classList.add('d-none'); return; }
      const uri = `upi://pay?pa=${encodeURIComponent(CFG.upiId)}&pn=${encodeURIComponent(CFG.upiName || 'BPST Academy')}&am=${(CFG.amount || 49).toFixed(2)}&cu=INR&tn=${encodeURIComponent(booking.booking_ref)}`;
      idEl.textContent = CFG.upiId; if (link) link.href = uri;
      try {
        await loadScript('https://cdnjs.cloudflare.com/ajax/libs/qrcode-generator/1.4.4/qrcode.min.js');
        const qr = window.qrcode(0, 'M'); qr.addData(uri); qr.make();
        box.innerHTML = qr.createSvgTag({ cellSize: 4, margin: 2, scalable: true });
      } catch (e) { box.innerHTML = '<div class="small text-muted p-3">Open your UPI app and pay to ' + CFG.upiId + '</div>'; }
    }
    $('[data-utr-submit]', card)?.addEventListener('click', () => {
      const v = $('[data-utr]', card).value.trim(), m = $('[data-utr-msg]', card);
      if (!/^[A-Za-z0-9]{8,22}$/.test(v)) { m.hidden = false; return; }
      m.hidden = true; done('upi', v);
    });
  });

  /* ---------- Contact enquiry form ---------- */
  const enq = $('[data-enquiry-form]');
  if (enq) {
    if (params.get('course') && enq.elements.course) enq.elements.course.value = params.get('course');
    enq.addEventListener('submit', async (e) => {
      e.preventDefault();
      if (enq.elements.website.value) return;
      if (!enq.checkValidity()) { enq.classList.add('was-validated'); return; }
      const f = enq.elements;
      const btns = $$('button[type=submit]', enq); btns.forEach((b) => { b.disabled = true; });
      const saved = await sendLead({ form_type: 'enquiry', name: f.name.value.trim(), mobile: '+91' + f.mobile.value, course: f.course.value, message: f.message.value.trim(), consent: true });
      btns.forEach((b) => { b.disabled = false; });
      saveError(enq, !saved); if (!saved) return;
      $('[data-enquiry-ok]', enq).classList.remove('d-none'); enq.reset(); enq.classList.remove('was-validated');
    });
  }

  /* ---------- Photo gallery lightbox ---------- */
  const gallery = $('[data-bpst-gallery]');
  const box = $('.bpst-lightbox');
  if (gallery && box && typeof box.showModal === 'function') {
    const boxImg = $('img', box), boxCap = $('.bpst-lightbox-caption', box);
    gallery.addEventListener('click', (e) => {
      const a = e.target.closest('.bpst-gallery-item');
      if (!a) return;
      e.preventDefault();
      const img = $('img', a);
      boxImg.src = a.href; boxImg.alt = img.alt; boxCap.textContent = $('span', a).textContent;
      box.showModal();
    });
    $('.bpst-lightbox-close', box).addEventListener('click', () => box.close());
    box.addEventListener('click', (e) => { if (e.target === box) box.close(); });
  }

  /* ---------- Misc ---------- */
  $$('[data-year]').forEach((y) => { y.textContent = new Date().getFullYear(); });
})();
