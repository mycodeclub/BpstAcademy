/* BPST Academy portal — shell behaviour, SLA timers, toasts, theme, table helpers, charts.
   Derived from NewLayout/AdminTemplate/assets/js/admin.js without the prototype-only parts
   (role switcher, JS-built sidebar, demo data). The shell markup is rendered by _AdminLayout.cshtml. */
(function () {
  'use strict';

  var body = document.body;

  /* ---------- storage (never throws) ---------- */
  function store(key, val) {
    try {
      if (val === undefined) return localStorage.getItem('bpst.' + key);
      localStorage.setItem('bpst.' + key, val);
    } catch (e) { return null; }
  }

  /* ---------- helpers ---------- */
  var inr = new Intl.NumberFormat('en-IN', { maximumFractionDigits: 0 });
  function fmtINR(n) { return (n < 0 ? '−₹' : '₹') + inr.format(Math.abs(n)); }
  function fmtNum(n) { return inr.format(n); }
  var MONTHS = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
  function fmtDate(d) { d = new Date(d); return d.getDate() + ' ' + MONTHS[d.getMonth()] + ' ' + d.getFullYear(); }
  function fmtTime(d) {
    d = new Date(d);
    var h = d.getHours(), m = d.getMinutes();
    return (h % 12 || 12) + ':' + (m < 10 ? '0' : '') + m + ' ' + (h < 12 ? 'AM' : 'PM');
  }
  function esc(s) { return String(s).replace(/[&<>"']/g, function (c) { return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]; }); }
  function initials(name) { return name.split(/\s+/).filter(Boolean).slice(0, 2).map(function (p) { return p[0]; }).join('').toUpperCase(); }
  function el(html) { var t = document.createElement('template'); t.innerHTML = html.trim(); return t.content.firstChild; }

  /* ---------- theme ---------- */
  function applyTheme(t) {
    document.documentElement.setAttribute('data-bs-theme', t);
    var btn = document.getElementById('themeToggle');
    if (btn) {
      btn.innerHTML = '<i class="bi ' + (t === 'dark' ? 'bi-sun' : 'bi-moon') + '"></i>';
      btn.setAttribute('aria-label', t === 'dark' ? 'Switch to light mode' : 'Switch to dark mode');
    }
    document.dispatchEvent(new CustomEvent('bpst:theme', { detail: t }));
  }
  function currentTheme() { return document.documentElement.getAttribute('data-bs-theme') || 'light'; }

  /* ---------- toasts ---------- */
  var toastBox;
  function toast(msg, tone) {
    if (!toastBox) {
      toastBox = el('<div class="toast-container position-fixed bottom-0 end-0 p-3" aria-live="polite"></div>');
      body.appendChild(toastBox);
    }
    var icons = { success: 'bi-check-circle-fill', danger: 'bi-exclamation-octagon-fill', warning: 'bi-exclamation-triangle-fill', info: 'bi-info-circle-fill' };
    var colors = { success: 'var(--c-success-text)', danger: 'var(--c-danger-text)', warning: 'var(--c-warning-text)', info: 'var(--c-accent)' };
    tone = tone || 'info';
    var t = el('<div class="toast align-items-center" role="status"><div class="d-flex"><div class="toast-body d-flex gap-2 align-items-start">' +
      '<i class="bi ' + icons[tone] + '" style="color:' + colors[tone] + '"></i><span>' + msg + '</span></div>' +
      '<button type="button" class="btn-close me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button></div></div>');
    toastBox.appendChild(t);
    if (window.bootstrap) {
      var bt = new bootstrap.Toast(t, { delay: 4000 });
      t.addEventListener('hidden.bs.toast', function () { t.remove(); });
      bt.show();
    }
  }

  /* ---------- SLA ---------- */
  var SLA_MIN = 60;
  function pad(n) { return (n < 10 ? '0' : '') + n; }
  function dur(sec) {
    sec = Math.max(0, Math.floor(sec));
    var h = Math.floor(sec / 3600), m = Math.floor((sec % 3600) / 60), s = sec % 60;
    return (h ? h + ':' + pad(m) : m) + ':' + pad(s);
  }
  function slaInfo(createdAt, contactedAt) {
    var end = contactedAt ? new Date(contactedAt).getTime() : Date.now();
    var elapsed = (end - new Date(createdAt).getTime()) / 1000;
    var mins = elapsed / 60;
    if (contactedAt) return { state: 'done', elapsed: elapsed, breached: mins >= SLA_MIN };
    if (mins >= SLA_MIN) return { state: 'breach', elapsed: elapsed, over: elapsed - SLA_MIN * 60 };
    return { state: mins >= 45 ? 'hot' : mins >= 30 ? 'warn' : 'ok', elapsed: elapsed, left: SLA_MIN * 60 - elapsed };
  }
  function renderSla(node) {
    var info = slaInfo(node.getAttribute('data-created-at'), node.getAttribute('data-contacted-at'));
    var cls = 'sla sla--' + info.state, html, label;
    if (info.state === 'breach') {
      html = '<i class="bi bi-exclamation-octagon-fill"></i>HIGH RISK +' + dur(info.over);
      label = 'SLA breached, overdue by ' + dur(info.over);
    } else if (info.state === 'done') {
      html = '<i class="bi bi-check2"></i>Contacted in ' + dur(info.elapsed);
      label = html.replace(/<[^>]+>/g, '') + (info.breached ? ' (late)' : '');
      if (info.breached) cls += ' text-decoration-line-through';
    } else {
      html = '<i class="bi bi-stopwatch"></i>' + dur(info.left);
      label = dur(info.left) + ' left to contact';
    }
    if (node.className !== cls) node.className = cls;
    node.innerHTML = html;
    node.setAttribute('aria-label', label);
    node.setAttribute('data-state', info.state);
    return info;
  }
  /* counts the timers on the page; the top-bar badge will use a server count once leads exist (B3) */
  function tickSla() {
    var c = { breach: 0, hot: 0, warn: 0, ok: 0, done: 0 };
    document.querySelectorAll('[data-sla]').forEach(function (n) { c[renderSla(n).state]++; });
    var risk = c.breach + c.hot;
    document.querySelectorAll('[data-count="sla"]').forEach(function (n) { n.textContent = risk; n.hidden = !risk; });
    var badge = document.getElementById('slaBadge');
    if (badge) {
      badge.classList.toggle('is-clear', !risk);
      badge.querySelector('.sla-badge-num').textContent = risk;
      badge.querySelector('.sla-badge-text').textContent = risk ? 'at risk' : 'all on time';
      badge.title = c.breach + ' breached · ' + c.hot + ' breaching in under 15 min';
    }
    document.dispatchEvent(new CustomEvent('bpst:sla', { detail: c }));
  }
  /* ---------- delegated events ---------- */
  document.addEventListener('click', function (e) {
    var t;
    if ((t = e.target.closest('[data-demo-toast]'))) { e.preventDefault(); toast(t.getAttribute('data-demo-toast'), t.getAttribute('data-tone') || 'success'); return; }
    if ((t = e.target.closest('#sidebarToggle'))) {
      if (window.innerWidth < 992) body.classList.toggle('sidebar-open');
      else { body.classList.toggle('sidebar-collapsed'); store('sidebar', body.classList.contains('sidebar-collapsed') ? 'collapsed' : 'open'); }
      return;
    }
    if (e.target.closest('[data-sidebar-close]')) { body.classList.remove('sidebar-open'); return; }
    if (e.target.closest('#themeToggle')) { var nt = currentTheme() === 'dark' ? 'light' : 'dark'; store('theme', nt); applyTheme(nt); return; }
    /* filter chips: <div data-filter-group="target-id"> <button class="filter-chip" data-filter="value|all"> ; rows carry data-filter-value */
    if ((t = e.target.closest('[data-filter]'))) {
      var group = t.closest('[data-filter-group]');
      if (!group) return;
      group.querySelectorAll('[data-filter]').forEach(function (b) { b.classList.toggle('active', b === t); b.setAttribute('aria-pressed', b === t); });
      var v = t.getAttribute('data-filter'), target = document.getElementById(group.getAttribute('data-filter-group'));
      if (target) {
        target.querySelectorAll('[data-filter-value]').forEach(function (r) {
          r.setAttribute('data-hide-chip', v !== 'all' && r.getAttribute('data-filter-value').split(/\s+/).indexOf(v) === -1 ? '1' : '');
        });
        refreshRows(target);
      }
      return;
    }
    /* chart/table view toggle: <div data-view-toggle="id"> buttons data-view="chart|table"; panes data-view-pane */
    if ((t = e.target.closest('[data-view]'))) {
      var vt = t.closest('[data-view-toggle]');
      if (!vt) return;
      vt.querySelectorAll('[data-view]').forEach(function (b) { b.classList.toggle('active', b === t); b.setAttribute('aria-pressed', b === t); });
      var box = document.getElementById(vt.getAttribute('data-view-toggle'));
      if (box) box.querySelectorAll('[data-view-pane]').forEach(function (p) { p.hidden = p.getAttribute('data-view-pane') !== t.getAttribute('data-view'); });
    }
  });
  document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') body.classList.remove('sidebar-open');
  });

  /* ---------- generic page behaviours (all opt-in through data-* attributes) ---------- */
  /* rows are hidden when any of search / chip / select filters hides them */
  function refreshRows(box) {
    var shown = 0;
    box.querySelectorAll('[data-filter-value], [data-search-row]').forEach(function (r) {
      r.hidden = !!(r.getAttribute('data-hide-chip') || r.getAttribute('data-hide-search') || r.getAttribute('data-hide-select'));
      if (!r.hidden) shown++;
    });
    var empty = document.querySelector('[data-empty-for="' + box.id + '"]');
    if (empty) empty.hidden = shown > 0;
    var cnt = document.querySelector('[data-shown-for="' + box.id + '"]');
    if (cnt) cnt.textContent = shown;
  }
  document.addEventListener('input', function (e) {
    var t = e.target, box;
    /* <input data-table-search="tbodyId"> */
    if (t.hasAttribute('data-table-search') && (box = document.getElementById(t.getAttribute('data-table-search')))) {
      var q = t.value.trim().toLowerCase();
      box.querySelectorAll('tr, .list-row, .kanban-card, [data-search-row]').forEach(function (r) {
        if (r.parentNode && r.parentNode.tagName === 'THEAD') return;
        if (!r.hasAttribute('data-search-row') && !r.hasAttribute('data-filter-value')) r.setAttribute('data-search-row', '');
        r.setAttribute('data-hide-search', q && r.textContent.toLowerCase().indexOf(q) === -1 ? '1' : '');
      });
      refreshRows(box);
    }
  });
  document.addEventListener('change', function (e) {
    var t = e.target, box;
    /* <select data-table-filter="tbodyId"> option values match tokens in data-filter-value */
    if (t.hasAttribute('data-table-filter') && (box = document.getElementById(t.getAttribute('data-table-filter')))) {
      var v = t.value;
      box.querySelectorAll('[data-filter-value]').forEach(function (r) {
        r.setAttribute('data-hide-select', v && v !== 'all' && r.getAttribute('data-filter-value').split(/\s+/).indexOf(v) === -1 ? '1' : '');
      });
      refreshRows(box);
    }
    /* <input type="checkbox" data-check-all="tbodyId"> + .row-check + [data-bulk-for="tbodyId"] */
    if (t.hasAttribute('data-check-all') && (box = document.getElementById(t.getAttribute('data-check-all')))) {
      box.querySelectorAll('.row-check').forEach(function (c) { if (!c.closest('[hidden]')) c.checked = t.checked; });
      bulk(box);
    } else if (t.classList.contains('row-check') && (box = t.closest('tbody, [data-bulk-box]'))) bulk(box);
  });
  function bulk(box) {
    var n = box.querySelectorAll('.row-check:checked').length;
    box.querySelectorAll('.row-check').forEach(function (c) { var tr = c.closest('tr'); if (tr) tr.classList.toggle('is-selected', c.checked); });
    var bar = document.querySelector('[data-bulk-for="' + box.id + '"]');
    if (bar) { bar.hidden = !n; var c = bar.querySelector('[data-bulk-count]'); if (c) c.textContent = n; }
  }
  /* sortable columns: <th data-sort> (use data-sort-value on a td for custom values) */
  function sortTable(th) {
    var table = th.closest('table'), tb = table.tBodies[0], idx = Array.prototype.indexOf.call(th.parentNode.children, th);
    var dir = th.classList.contains('asc') ? -1 : 1;
    table.querySelectorAll('th[data-sort]').forEach(function (h) { h.classList.remove('asc', 'desc'); h.removeAttribute('aria-sort'); });
    th.classList.add(dir === 1 ? 'asc' : 'desc');
    th.setAttribute('aria-sort', dir === 1 ? 'ascending' : 'descending');
    var val = function (tr) {
      var td = tr.children[idx]; if (!td) return '';
      var v = td.hasAttribute('data-sort-value') ? td.getAttribute('data-sort-value') : td.textContent.trim();
      var n = parseFloat(v.replace(/[₹,%\s+]/g, '').replace('−', '-'));
      return isNaN(n) || /[a-z]{3,}/i.test(v.replace(/days?|min|pts/gi, '')) ? v.toLowerCase() : n;
    };
    Array.prototype.slice.call(tb.rows).sort(function (a, b) {
      var x = val(a), y = val(b);
      return (typeof x === 'number' && typeof y === 'number' ? x - y : String(x).localeCompare(String(y))) * dir;
    }).forEach(function (r) { tb.appendChild(r); });
  }
  document.addEventListener('keydown', function (e) {
    if ((e.key === 'Enter' || e.key === ' ') && e.target.matches && e.target.matches('th[data-sort]')) { e.preventDefault(); sortTable(e.target); }
  });
  document.addEventListener('click', function (e) {
    var t;
    if ((t = e.target.closest('th[data-sort]'))) { sortTable(t); return; }
    /* attendance: .att-toggle > button[data-att="P|L|A"] */
    if ((t = e.target.closest('.att-toggle [data-att]'))) {
      t.parentNode.querySelectorAll('[data-att]').forEach(function (b) { b.setAttribute('aria-pressed', b === t ? 'true' : 'false'); });
      document.dispatchEvent(new CustomEvent('bpst:att'));
      return;
    }
    if ((t = e.target.closest('[data-mark-all]'))) {
      var box = document.getElementById(t.getAttribute('data-mark-all'));
      if (box) box.querySelectorAll('.att-toggle').forEach(function (g) { g.querySelectorAll('[data-att]').forEach(function (b) { b.setAttribute('aria-pressed', b.getAttribute('data-att') === 'P' ? 'true' : 'false'); }); });
      document.dispatchEvent(new CustomEvent('bpst:att'));
      return;
    }
    if (e.target.closest('[data-print]')) { e.preventDefault(); window.print(); return; }
    /* resolve a row in place: data-resolve="Approved|success|toast text" */
    if ((t = e.target.closest('[data-resolve]'))) {
      e.preventDefault();
      var p = t.getAttribute('data-resolve').split('|'), row = t.closest('tr, .list-row, .kanban-card, .card');
      var slot = row && (row.querySelector('[data-resolve-slot]') || t.parentNode);
      if (slot) slot.innerHTML = '<span class="chip chip--' + (p[1] || 'success') + '">' + esc(p[0]) + '</span>';
      toast(p[2] || (p[0] + '. Recorded in the audit log.'), p[1] === 'danger' ? 'warning' : (p[1] || 'success'));
      return;
    }
    /* whole-row links: <tr data-href="page.html"> */
    if ((t = e.target.closest('[data-href]')) && !e.target.closest('a, button, input, select, label, textarea')) {
      window.location.href = t.getAttribute('data-href');
    }
  });
  /* demo forms: <form data-demo-form="Toast text" [data-then="url"]> — validates, then shows a toast */
  document.addEventListener('submit', function (e) {
    var f = e.target;
    if (!f.hasAttribute('data-demo-form')) return;
    e.preventDefault();
    if (!f.checkValidity()) { f.classList.add('was-validated'); var bad = f.querySelector(':invalid'); if (bad) bad.focus(); return; }
    f.classList.remove('was-validated');
    toast(f.getAttribute('data-demo-form') || 'Saved (demo).', 'success');
    var m = f.closest('.modal'); if (m && window.bootstrap) bootstrap.Modal.getOrCreateInstance(m).hide();
    var then = f.getAttribute('data-then'); if (then) setTimeout(function () { window.location.href = then; }, 900);
  });
  /* kanban: .kanban-card[draggable] dropped on .kanban-col[data-status] */
  var dragCard = null;
  document.addEventListener('dragstart', function (e) { var c = e.target.closest && e.target.closest('.kanban-card'); if (c) { dragCard = c; c.style.opacity = '.5'; } });
  document.addEventListener('dragend', function () { if (dragCard) dragCard.style.opacity = ''; dragCard = null; });
  document.addEventListener('dragover', function (e) { if (dragCard && e.target.closest('.kanban-col')) e.preventDefault(); });
  document.addEventListener('drop', function (e) {
    var col = e.target.closest('.kanban-col');
    if (!dragCard || !col) return;
    e.preventDefault();
    var from = dragCard.closest('.kanban-col');
    col.querySelector('.kanban-body').appendChild(dragCard);
    [from, col].forEach(function (c) { var n = c.querySelector('.kanban-count'); if (n) n.textContent = c.querySelectorAll('.kanban-card').length; });
    if (from !== col) toast('<b>' + esc(dragCard.getAttribute('data-name') || 'Lead') + '</b> moved to <b>' + esc(col.getAttribute('data-status')) + '</b>. Status change logged.' + (col.getAttribute('data-status') === 'Lost' ? ' A lost reason is required.' : ''), 'success');
  });

  /* ---------- chart theme (Chart.js) ---------- */
  function css(name) { return getComputedStyle(document.documentElement).getPropertyValue(name).trim(); }
  function chartTheme() {
    return {
      accent: css('--c-accent'), text: css('--c-text'), muted: css('--c-muted'), grid: css('--c-grid'), surface: css('--c-card'), border: css('--c-border'), font: css('--font-sans'),
      success: css('--c-success'), warning: css('--c-warning'), danger: css('--c-danger'), info: css('--c-info'), neutral: css('--c-neutral-soft')
    };
  }
  /* BPST.chart('canvasId', function (T, base) { return chartConfig; }) — themed, redrawn on theme change */
  var chartRegistry = [];
  function drawOne(entry) {
    if (!window.Chart) return;
    var canvas = document.getElementById(entry.id);
    if (!canvas) return;
    if (entry.inst) entry.inst.destroy();
    var T = chartTheme();
    Chart.defaults.font.family = T.font; Chart.defaults.font.size = 12; Chart.defaults.color = T.muted;
    var base = {
      tooltip: { backgroundColor: T.surface, titleColor: T.text, bodyColor: T.text, borderColor: T.border, borderWidth: 1, padding: 10, cornerRadius: 8, titleFont: { weight: '600' } },
      scales: function (fmt) {
        return {
          x: { grid: { display: false }, border: { color: T.grid }, ticks: { maxRotation: 0, autoSkip: true } },
          y: { beginAtZero: true, grid: { color: T.grid }, border: { display: false }, ticks: { maxTicksLimit: 5, callback: fmt || undefined } }
        };
      },
      shortINR: function (v) { return v >= 100000 ? '₹' + (v / 100000).toFixed(1).replace('.0', '') + 'L' : v >= 1000 ? '₹' + Math.round(v / 1000) + 'k' : '₹' + v; }
    };
    var cfg = entry.fn(T, base);
    cfg.options = cfg.options || {};
    if (cfg.options.maintainAspectRatio === undefined) cfg.options.maintainAspectRatio = false;
    cfg.options.animation = false;
    entry.inst = new Chart(canvas, cfg);
  }
  function chart(id, fn) { var e = { id: id, fn: fn }; chartRegistry.push(e); drawOne(e); return e; }
  document.addEventListener('bpst:theme', function () { requestAnimationFrame(function () { chartRegistry.forEach(drawOne); }); });

  /* ---------- boot ---------- */
  window.BPST = {
    fmtINR: fmtINR, fmtNum: fmtNum, fmtDate: fmtDate, fmtTime: fmtTime, esc: esc, initials: initials,
    toast: toast, slaInfo: slaInfo, renderSla: renderSla, tickSla: tickSla,
    chartTheme: chartTheme, chart: chart, store: store, refreshRows: refreshRows
  };

  if (store('sidebar') === 'collapsed' && window.innerWidth >= 992) body.classList.add('sidebar-collapsed');
  applyTheme(currentTheme());
  tickSla();
  setInterval(tickSla, 1000);
})();
