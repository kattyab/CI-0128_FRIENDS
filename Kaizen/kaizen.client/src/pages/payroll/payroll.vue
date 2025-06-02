<template>
  <div class="container-lg py-4">
    <h1 class="text-center mb-5 fw-bold">Procesar planilla</h1>

    <!-- ── CARD ─────────────────────────────────────────── -->
    <div class="card shadow-sm border-0 mb-5">
      <div class="card-body">
        <form @submit.prevent="handleProcess">
          <!-- Tipo de planilla -->
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

          <!-- Selector de período -->
          <div v-if="payrollType">
            <h2 class="h6 fw-bold mb-3">Período</h2>

            <!-- semanal -->
            <div v-if="payrollType === 'weekly'" class="row g-3 align-items-end">
              <div class="col-auto">
                <input type="date"
                       class="form-control"
                       v-model="weeklyDate"
                       :max="maxSunday" />
              </div>
              <div v-if="weeklyDate && !isSunday(weeklyDate)"
                   class="col text-danger small">
                Seleccione un domingo
              </div>
            </div>

            <!-- quincenal -->
            <div v-else-if="payrollType === 'biweekly'"
                 class="row g-3 align-items-end">
              <div class="col-auto">
                <select class="form-select" v-model="fortnightOption">
                  <option value="">Seleccione quincena</option>
                  <option value="first">1ª quincena (01-15)</option>
                  <option value="second">2ª quincena (16-fin)</option>
                </select>
              </div>
              <div class="col-auto">
                <input type="month" class="form-control" v-model="fortnightMonth" />
              </div>
            </div>

            <!-- mensual -->
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

    <!-- ── HISTORIAL ─────────────────────────────────────── -->
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

  /* ── STATE ─────────────────────────────────────────────────────────────── */
  const payrollType = ref('')
  const payrollLocked = ref(false)          // prevents user change
  const weeklyDate = ref('')
  const fortnightOption = ref('')
  const fortnightMonth = ref('')
  const monthlyMonth = ref('')
  const psStartDate = ref('')
  const psEndDate = ref('')

  /* ── FETCH USER + PAYROLL TYPE ─────────────────────────────────────────── */
  onMounted(async () => {
    try {
      const res = await fetch('/api/Login/payroll-type', { credentials: 'include' })
      if (!res.ok) throw new Error()

      const { letter } = await res.json()            // "W" | "B" | "M"
      const map = { W: 'weekly', B: 'biweekly', M: 'monthly' }
      payrollType.value = map[letter] || ''
      payrollLocked.value = true                     // lock radios
    } catch {
      console.warn('Unable to preload payroll type')
    }
  })

  /* ── CONSTANTS ─────────────────────────────────────────────────────────── */
  const payrollOptions = [
    { value: 'weekly', label: 'Semanal' },
    { value: 'biweekly', label: 'Quincenal' },
    { value: 'monthly', label: 'Mensual' }
  ]

  /* ── UTILITIES ─────────────────────────────────────────────────────────── */
  const isSunday = date => !!date && new Date(date).getDay() === 6
  const lastDayOfMonth = (y, m) => new Date(y, m + 1, 0).getDate()
  const formatDMY = d => {
    const dt = new Date(d)
    return `${dt.getDate().toString().padStart(2, '0')}-${(dt.getMonth() + 1)
      .toString().padStart(2, '0')}-${dt.getFullYear()}`
  }

  /* ── COMPUTEDS ─────────────────────────────────────────────────────────── */
  const periodPreview = computed(() => {
    switch (payrollType.value) {
      case 'weekly':
        if (weeklyDate.value && isSunday(weeklyDate.value)) {
          const start = new Date(weeklyDate.value)
          start.setDate(start.getDate() - 5)
          const end = new Date(weeklyDate.value)
          end.setDate(end.getDate() + 1)
          return `${formatDMY(start)} → ${formatDMY(end)}`
        }
        break
      case 'biweekly':
        if (fortnightOption.value && fortnightMonth.value) {
          const [y, m] = fortnightMonth.value.split('-').map(Number)
          const mm = String(m).padStart(2, '0')
          if (fortnightOption.value === 'first') return `01-${mm}-${y} → 15-${mm}-${y}`
          const last = String(lastDayOfMonth(y, m - 1)).padStart(2, '0')
          return `16-${mm}-${y} → ${last}-${mm}-${y}`
        }
        break
      case 'monthly':
        if (monthlyMonth.value) {
          const [y, m] = monthlyMonth.value.split('-').map(Number)
          const mm = String(m).padStart(2, '0')
          const last = String(lastDayOfMonth(y, m - 1)).padStart(2, '0')
          return `01-${mm}-${y} → ${last}-${mm}-${y}`
        }
        break
      default:
        break
    }
    return ''
  })

  const formValid = computed(() => {
    switch (payrollType.value) {
      case 'weekly': return isSunday(weeklyDate.value)
      case 'biweekly': return fortnightOption.value && fortnightMonth.value
      case 'monthly': return !!monthlyMonth.value
      default: return false
    }
  })

  /* ── OTHER CONSTANTS ───────────────────────────────────────────────────── */
  const maxSunday = (() => {
    const today = new Date()
    const lastSun = new Date(today)
    lastSun.setDate(today.getDate() - today.getDay())
    return lastSun.toISOString().split('T')[0]
  })()

  /* ── MOCK HISTORY + SUBMIT ─────────────────────────────────────────────── */
  const payrollHistory = ref([])

  function handleProcess() {
    if (!formValid.value) return
    const gross = Math.floor(Math.random() * 600_000 + 800_000)
    const deductions = Math.floor(gross * 0.15)
    const optionLabel = payrollOptions.find(o => o.value === payrollType.value).label

    payrollHistory.value.unshift({
      id: Date.now(),
      manager: 'Yann Sommer',
      type: optionLabel,
      period: periodPreview.value,
      gross,
      deductions,
      net: gross - deductions
    })

    weeklyDate.value = ''
    fortnightOption.value = ''
    fortnightMonth.value = ''
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
