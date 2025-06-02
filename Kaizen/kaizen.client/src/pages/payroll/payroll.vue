<!-- src/components/Payroll.vue -->
<template>
  <div class="container-lg py-4">
    <h1 class="text-center mb-5 fw-bold">Procesar planilla</h1>

    <!-- ── FORM CARD ───────────────────────────────────── -->
    <div class="card shadow-sm border-0 mb-5">
      <div class="card-body">
        <form @submit.prevent="handleProcess">
          <!-- Payroll type -->
          <div class="mb-4">
            <h2 class="h6 fw-bold mb-3">Tipo de planilla</h2>
            <div class="d-flex flex-wrap gap-4">
              <div v-for="opt in payrollOptions"
                   :key="opt.value"
                   class="form-check form-check-inline">
                <input class="form-check-input"
                       type="radio"
                       :id="`payroll-${opt.value}`"
                       name="payroll_type"
                       :value="opt.value"
                       v-model="payrollType"
                       :disabled="payrollLocked" />
                <label class="form-check-label" :for="`payroll-${opt.value}`">
                  {{ opt.label }}
                </label>
              </div>
            </div>
          </div>

          <!-- Period selector -->
          <div v-if="payrollType">
            <h2 class="h6 fw-bold mb-3">Período</h2>

            <!-- weekly -->
            <div v-if="payrollType === 'weekly'" class="col-auto">
              <input type="date" class="form-control" v-model="weeklyDate" />
            </div>

            <!-- bi-weekly -->
            <div v-else-if="payrollType === 'biweekly'" class="col-auto">
              <input type="date" class="form-control" v-model="fortnightDate" />
            </div>

            <!-- monthly -->
            <div v-else-if="payrollType === 'monthly'" class="col-auto">
              <input type="month" class="form-control" v-model="monthlyMonth" />
            </div>

            <!-- preview -->
            <div v-if="periodPreview" class="mt-3">
              <span class="badge bg-secondary fs-6">{{ periodPreview }}</span>
            </div>
          </div>

          <hr class="my-4" />

          <div class="d-grid">
            <button class="btn btn-primary btn-lg" :disabled="!formValid">
              Procesar nueva planilla
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- ── HISTORY ─────────────────────────────────────── -->
    <h2 class="h4 mb-3 fw-bold">Historial de planillas</h2>

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
          <tr v-for="row in payrollHistory" :key="row.id">
            <td>{{ row.manager }}</td>
            <td>{{ row.type }}</td>
            <td>{{ row.period }}</td>
            <td class="text-end">₡ {{ row.gross.toLocaleString() }}</td>
            <td class="text-end">₡ {{ row.deductions.toLocaleString() }}</td>
            <td class="text-end">₡ {{ row.net.toLocaleString() }}</td>
          </tr>
          <tr v-if="!payrollHistory.length">
            <td colspan="6" class="text-center text-muted py-4">
              No hay planillas registradas
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed, onMounted } from 'vue'

  /* ── state ───────────────────────────────────────────── */
  const payrollType = ref('')
  const payrollLocked = ref(false)
  const weeklyDate = ref('')
  const fortnightDate = ref('')
  const monthlyMonth = ref('')

  /* ── preload payroll type from API ───────────────────── */
  onMounted(async () => {
    try {
      const res = await fetch('/api/Login/payroll-type', { credentials: 'include' })
      if (!res.ok) throw new Error()
      const { letter } = await res.json()        // W | B | M
      const map = { W: 'weekly', B: 'biweekly', M: 'monthly' }
      payrollType.value = map[letter] ?? ''
      payrollLocked.value = true
    } catch {
      console.warn('Unable to preload payroll type')
    }
  })

  /* ── constants ───────────────────────────────────────── */
  const payrollOptions = [
    { value: 'weekly', label: 'Semanal' },
    { value: 'biweekly', label: 'Quincenal' },
    { value: 'monthly', label: 'Mensual' }
  ]

  /* ── helpers ─────────────────────────────────────────── */
  const formatDMY = d => {
    const dt = new Date(d)
    const dd = dt.getDate().toString().padStart(2, '0')
    const mm = (dt.getMonth() + 1).toString().padStart(2, '0')
    return `${dd}-${mm}-${dt.getFullYear()}`
  }
  const mondayOfWeek = d => {
    const date = new Date(d)
    const diff = date.getDay() === 0 ? -6 : 1 - date.getDay()
    date.setDate(date.getDate() + diff)
    return date
  }
  const lastDayOfMonth = (y, m) => new Date(y, m + 1, 0).getDate()

  /* ── computed: preview & form validation ─────────────── */
  const periodPreview = computed(() => {
    switch (payrollType.value) {
      case 'weekly': {
        if (!weeklyDate.value) return ''
        const start = mondayOfWeek(weeklyDate.value)
        const end = new Date(start)
        end.setDate(start.getDate() + 6)
        return `${formatDMY(start)} → ${formatDMY(end)}`
      }
      case 'biweekly': {
        if (!fortnightDate.value) return ''
        const d = new Date(fortnightDate.value)
        const y = d.getFullYear()
        const m = d.getMonth() + 1
        const mm = String(m).padStart(2, '0')
        if (d.getDate() <= 15) return `01-${mm}-${y} → 15-${mm}-${y}`
        const last = String(lastDayOfMonth(y, m - 1)).padStart(2, '0')
        return `16-${mm}-${y} → ${last}-${mm}-${y}`
      }
      case 'monthly': {
        if (!monthlyMonth.value) return ''
        const [y, m] = monthlyMonth.value.split('-').map(Number)
        const mm = String(m).padStart(2, '0')
        const last = String(lastDayOfMonth(y, m - 1)).padStart(2, '0')
        return `01-${mm}-${y} → ${last}-${mm}-${y}`
      }
      default:
        return ''
    }
  })

  const formValid = computed(() => {
    switch (payrollType.value) {
      case 'weekly': return !!weeklyDate.value
      case 'biweekly': return !!fortnightDate.value
      case 'monthly': return !!monthlyMonth.value
      default: return false
    }
  })

  /* ── mock history & submit ───────────────────────────── */
  const payrollHistory = ref([])

  function handleProcess() {
    if (!formValid.value) return

    const gross = Math.floor(Math.random() * 600_000 + 800_000)
    const deductions = Math.floor(gross * 0.15)
    const label = payrollOptions.find(o => o.value === payrollType.value).label

    payrollHistory.value.unshift({
      id: Date.now(),
      manager: 'Yann Sommer',
      type: label,
      period: periodPreview.value,
      gross,
      deductions,
      net: gross - deductions
    })

    weeklyDate.value = ''
    fortnightDate.value = ''
    monthlyMonth.value = ''
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
