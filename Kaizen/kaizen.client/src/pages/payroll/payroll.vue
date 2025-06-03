```vue
<template>
  <div class="container-lg py-4">
    <h1 class="text-center mb-5 fw-bold">Procesar planilla</h1>

    <!-- ── FORM ─────────────────────────────────────────── -->
    <div class="card shadow-sm border-0 mb-5">
      <div class="card-body">
        <form @submit.prevent="submit">
          <!-- Tipo de planilla -->
          <fieldset class="mb-4">
            <legend class="h6 fw-bold mb-3">Tipo de planilla</legend>
            <div class="d-flex flex-wrap gap-4">
              <label v-for="o in options"
                     :key="o.value"
                     class="form-check form-check-inline">
                <input class="form-check-input"
                       type="radio"
                       name="payrollType"
                       :value="o.value"
                       v-model="type"
                       :disabled="locked" />
                <span class="form-check-label">{{ o.label }}</span>
              </label>
            </div>
          </fieldset>

          <!-- Período -->
          <fieldset v-if="type">
            <legend class="h6 fw-bold mb-3">Período</legend>

            <input v-if="type === 'weekly'"
                   type="date"
                   class="form-control col-auto"
                   v-model="weekly" />
            <input v-else-if="type === 'biweekly'"
                   type="date"
                   class="form-control col-auto"
                   v-model="fortnight" />
            <input v-else
                   type="month"
                   class="form-control col-auto"
                   v-model="monthly" />

            <span v-if="preview" class="badge bg-secondary mt-3">
              {{ preview }}
            </span>
          </fieldset>

          <hr class="my-4" />
          <button class="btn btn-primary w-100"
                  :disabled="!valid || periodAlreadyExists">
            Procesar nueva planilla
          </button>
        </form>
      </div>
    </div>

    <!-- ── HISTORIAL ─────────────────────────────────────── -->
    <h2 class="h4 fw-bold mb-3">Historial de planillas</h2>

    <div class="table-responsive shadow-sm">
      <table class="table table-hover align-middle mb-0">
        <thead class="table-light">
          <tr>
            <th>Encargado</th>
            <th>Tipo</th>
            <th>Período</th>
            <th class="text-end">Bruto</th>
            <th class="text-end">Deducciones</th>
            <th class="text-end">Cargas sociales</th>
            <th class="text-end">Neto pagado</th>
            <th class="text-end">Total</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="r in history" :key="r.id">
            <td>{{ r.manager }}</td>
            <td>{{ r.type }}</td>
            <td>{{ r.period }}</td>
            <td class="text-end">₡ {{ formatCRC(r.gross) }}</td>
            <td class="text-end">₡ {{ formatCRC(r.deductions) }}</td>
            <td class="text-end">₡ {{ formatCRC(r.socialCharges) }}</td>
            <td class="text-end">₡ {{ formatCRC(r.net) }}</td>
            <td class="text-end">₡ {{ formatCRC(r.total) }}</td>
          </tr>

          <tr v-if="!history.length">
            <td colspan="8" class="text-center text-muted py-4">
              No hay planillas registradas
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed, onMounted, watch } from "vue";

  /* ── refs ─────────────────────────────────────────────── */
  const currentUser = ref("");
  const companyId = ref("");
  const type = ref("");
  const locked = ref(false);

  const weekly = ref("");
  const fortnight = ref("");
  const monthly = ref("");

  const history = ref([]);

  /* ── opciones de radio ───────────────────────────────── */
  const options = [
    { value: "weekly", label: "Semanal" },
    { value: "biweekly", label: "Quincenal" },
    { value: "monthly", label: "Mensual" },
  ];

  /* ── helpers ─────────────────────────────────────────── */
  const iso = (d) => new Date(d).toISOString().substring(0, 10); // yyyy-MM-dd
  const dmy = (d) =>
    new Date(d).toLocaleDateString("es-CR", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
    });

  const monday = (d) => {
    const dt = new Date(d);
    dt.setDate(dt.getDate() - ((dt.getDay() + 6) % 7));
    return dt;
  };
  const lastOfMonth = (y, m) => new Date(y, m + 1, 0).getDate();

  const formatCRC = (n) =>
    new Intl.NumberFormat("es-CR", {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(+n);

  /* ── refs auxiliares para evitar rechazos ───────────────── */
  const existingPeriods = ref(new Set()); // agrupa periodos existentes
  const periodAlreadyExists = ref(false);

  /* ── carga inicial ───────────────────────────────────── */
  onMounted(async () => {
    // 1) Obtener usuario y compañía
    const auth = await fetch("/api/login/authenticate", { credentials: "include" });
    if (auth.ok) {
      currentUser.value = (await auth.json()).email ?? "usuario@local";
    }

    const pay = await fetch("/api/login/payroll-info", { credentials: "include" });
    if (pay.ok) {
      const { companyId: id, letter } = await pay.json();
      companyId.value = id;
      type.value = { W: "weekly", B: "biweekly", M: "monthly" }[letter] ?? "";
      locked.value = true;
    }

    // 2) Cargar historial de planillas
    const histRes = await fetch("/api/payroll/history", { credentials: "include" });
    if (histRes.ok) {
      history.value = await histRes.json();
      // llenar el Set de existingPeriods
      history.value.forEach((r) => existingPeriods.value.add(r.period));
    }
  });

  /* ── preview del período ─────────────────────────────── */
  const preview = computed(() => {
    if (type.value === "weekly" && weekly.value) {
      const s = monday(weekly.value);
      const e = new Date(s);
      e.setDate(s.getDate() + 6);
      return `${dmy(s)} → ${dmy(e)}`;
    }
    if (type.value === "biweekly" && fortnight.value) {
      const d = new Date(fortnight.value);
      const y = d.getFullYear();
      const m = d.getMonth() + 1;
      const mm = m.toString().padStart(2, "0");
      if (d.getDate() <= 15) return `01-${mm}-${y} → 15-${mm}-${y}`;
      const last = lastOfMonth(y, m - 1).toString().padStart(2, "0");
      return `16-${mm}-${y} → ${last}-${mm}-${y}`;
    }
    if (type.value === "monthly" && monthly.value) {
      const [y, m] = monthly.value.split("-");
      return `${m.padStart(2, "0")}-${y}`;
    }
    return "";
  });

  /* ── avisar si el período ya existe ────────────────── */
  watch(preview, (val) => {
    periodAlreadyExists.value = existingPeriods.value.has(val);
  });

  /* ── validación mínima ───────────────────────────────── */
  const valid = computed(
    () =>
      (type.value === "weekly" && weekly.value) ||
      (type.value === "biweekly" && fortnight.value) ||
      (type.value === "monthly" && monthly.value)
  );

  /* ── submit ─────────────────────────────────────────── */
  async function submit() {
    if (!valid.value || periodAlreadyExists.value) return;

    let startISO = "", endISO = "";

    if (type.value === "weekly") {
      const s = monday(weekly.value);
      const e = new Date(s);
      e.setDate(s.getDate() + 6);
      startISO = iso(s);
      endISO = iso(e);
    } else if (type.value === "biweekly") {
      const d = new Date(fortnight.value);
      const y = d.getFullYear();
      const m = (d.getMonth() + 1).toString().padStart(2, "0");
      if (d.getDate() <= 15) {
        startISO = `${y}-${m}-01`;
        endISO = `${y}-${m}-15`;
      } else {
        const last = lastOfMonth(y, +m - 1).toString().padStart(2, "0");
        startISO = `${y}-${m}-16`;
        endISO = `${y}-${m}-${last}`;
      }
    } else {
      const [y, m] = monthly.value.split("-");
      startISO = `${y}-${m}-01`;
      endISO = `${y}-${m}-${lastOfMonth(+y, +m - 1).toString().padStart(2, "0")}`;
    }

    /* ---- llamada única al endpoint process ---- */
    const res = await fetch("/api/payroll/process", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({
        email: currentUser.value,
        companyId: companyId.value,
        start: `${startISO}T00:00:00`,
        end: `${endISO}T23:59:59`,
        type: type.value,
      }),
    });

    if (!res.ok) {
      const text = await res.text();
      alert(text); // muestra mensaje de “Ya se ejecutó la planilla...”
      return;
    }

    const data = await res.json();

    /* actualizar historial en cliente */
    const newRow = {
      id: Date.now(),
      manager: currentUser.value,
      type: options.find((o) => o.value === type.value).label,
      period: preview.value,
      gross: data.gross,
      deductions: data.deductions,
      socialCharges: data.socialCharges ?? 0,
      net: data.net,
      total: data.totalPaid ?? data.gross + (data.socialCharges ?? 0),
    };

    history.value.unshift(newRow);
    existingPeriods.value.add(preview.value);

    weekly.value = fortnight.value = monthly.value = "";
  }
</script>

<style scoped>
  .badge {
    font-size: 0.9rem;
  }

  .table th,
  .table td {
    vertical-align: middle;
  }
</style>
```
