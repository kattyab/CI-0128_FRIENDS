<!-- src/components/Payroll.vue -->
<template>
  <div class="container-lg py-4">
    <h1 class="text-center mb-5 fw-bold">Procesar planilla</h1>

    <!-- ── FORM ─────────────────────────────────────────── -->
    <div class="card shadow-sm border-0 mb-5">
      <div class="card-body">
        <form @submit.prevent="submit">
          <!-- tipo -->
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

          <!-- período -->
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
          <button class="btn btn-primary w-100" :disabled="!valid">
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
            <th class="text-end">Neto</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="r in history" :key="r.id">
            <td>{{ r.manager }}</td>
            <td>{{ r.type }}</td>
            <td>{{ r.period }}</td>
            <td class="text-end">₡ {{ r.gross.toLocaleString() }}</td>
            <td class="text-end">₡ {{ r.deductions.toLocaleString() }}</td>
            <td class="text-end">₡ {{ r.net.toLocaleString() }}</td>
          </tr>
          <tr v-if="!history.length">
            <td colspan="6" class="text-center text-muted py-4">No hay planillas registradas</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed, onMounted } from 'vue'

  /* ── refs ─────────────────────────────────────────────── */
  const currentUser = ref('')      // correo del usuario
  const companyId = ref('')
  const type = ref('')
  const locked = ref(false)

  const weekly = ref('')
  const fortnight = ref('')
  const monthly = ref('')

  const history = ref([])

  /* ── radio options ───────────────────────────────────── */
  const options = [
    { value: 'weekly', label: 'Semanal' },
    { value: 'biweekly', label: 'Quincenal' },
    { value: 'monthly', label: 'Mensual' }
  ]

  /* ── helpers ─────────────────────────────────────────── */
  const iso = d => new Date(d).toISOString().substring(0, 10)                 // yyyy-MM-dd
  const dmy = d => new Date(d).toLocaleDateString('es-CR', { day: '2-digit', month: '2-digit', year: 'numeric' })
  const monday = d => { const dt = new Date(d); dt.setDate(dt.getDate() - ((dt.getDay() + 6) % 7)); return dt }
  const lastOfMonth = (y, m) => new Date(y, m + 1, 0).getDate()

  /* ── load user & payroll info ─────────────────────────── */
  onMounted(async () => {
    // email
    const auth = await fetch('/api/login/authenticate', { credentials: 'include' })
    if (auth.ok) {
      const { email } = await auth.json()
      currentUser.value = email ?? 'usuario@local'
    }
    // company + payroll type
    const pay = await fetch('/api/login/payroll-info', { credentials: 'include' })
    if (pay.ok) {
      const { companyId: id, letter } = await pay.json()
      companyId.value = id
      type.value = { W: 'weekly', B: 'biweekly', M: 'monthly' }[letter] ?? ''
      locked.value = true
    }
  })

  /* ── preview period ──────────────────────────────────── */
  const preview = computed(() => {
    if (type.value === 'weekly' && weekly.value) {
      const s = monday(weekly.value)
      const e = new Date(s); e.setDate(s.getDate() + 6)
      return `${dmy(s)} → ${dmy(e)}`
    }
    if (type.value === 'biweekly' && fortnight.value) {
      const d = new Date(fortnight.value)
      const y = d.getFullYear(); const m = d.getMonth() + 1
      const mm = m.toString().padStart(2, '0')
      if (d.getDate() <= 15) return `01-${mm}-${y} → 15-${mm}-${y}`
      const last = lastOfMonth(y, m - 1).toString().padStart(2, '0')
      return `16-${mm}-${y} → ${last}-${mm}-${y}`
    }
    if (type.value === 'monthly' && monthly.value) {
      const [y, m] = monthly.value.split('-').map(Number)
      const mm = m.toString().padStart(2, '0')
      const last = lastOfMonth(y, m - 1).toString().padStart(2, '0')
      return `01-${mm}-${y} → ${last}-${mm}-${y}`
    }
    return ''
  })

  /* ── validation ──────────────────────────────────────── */
  const valid = computed(() =>
    (type.value === 'weekly' && weekly.value) ||
    (type.value === 'biweekly' && fortnight.value) ||
    (type.value === 'monthly' && monthly.value)
  )

  /* ── submit ──────────────────────────────────────────── */
  async function submit() {
    if (!valid.value) return

    /* build start / end ISO */
    let startISO = '', endISO = ''

    if (type.value === 'weekly') {
      const s = monday(weekly.value)
      const e = new Date(s); e.setDate(s.getDate() + 6)
      startISO = iso(s); endISO = iso(e)
    }

    if (type.value === 'biweekly') {
      const d = new Date(fortnight.value)
      const y = d.getFullYear(); const mPad = (d.getMonth() + 1).toString().padStart(2, '0')
      if (d.getDate() <= 15) {
        startISO = `${y}-${mPad}-01`
        endISO = `${y}-${mPad}-15`
      } else {
        startISO = `${y}-${mPad}-16`
        const last = lastOfMonth(y, +mPad - 1).toString().padStart(2, '0')
        endISO = `${y}-${mPad}-${last}`
      }
    }

    if (type.value === 'monthly') {
      const [y, m] = monthly.value.split('-')
      startISO = `${y}-${m}-01`
      endISO = `${y}-${m}-${lastOfMonth(+y, +m - 1).toString().padStart(2, '0')}`
    }

    /* send */
    const dto = {
      email: currentUser.value,
      companyId: companyId.value,
      start: `${startISO}T00:00:00`,
      end: `${endISO}T23:59:59`,
      type: type.value
    }

    const res = await fetch('/api/payroll/process', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify(dto)
    })
    if (!res.ok) return alert('Error al procesar planilla')
    const data = await res.json()

    history.value.unshift({
      id: Date.now(),
      manager: currentUser.value,
      type: options.find(o => o.value === type.value).label,
      period: data.period,
      gross: data.gross,
      deductions: data.deductions,
      net: data.net
    })

    weekly.value = fortnight.value = monthly.value = ''
  }
</script>

<style scoped>
  .badge {
    font-size: .9rem
  }

  .table td,
  .table th {
    vertical-align: middle
  }
</style>
