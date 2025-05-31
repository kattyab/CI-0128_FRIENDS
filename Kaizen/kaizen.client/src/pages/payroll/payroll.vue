<template>
  <div class="container py-4">
    <h1 class="text-center mb-4">Procesar planilla</h1>

    <!-- === FORMULARIO DE SELECCIÓN === -->
    <form @submit.prevent="handleProcess">
      <!-- Tipo de planilla -->
      <div class="mb-4">
        <label class="form-label fw-bold mb-2">Tipo de planilla</label>
        <div class="d-flex gap-3 flex-wrap">
          <div v-for="opt in payrollOptions"
               :key="opt.value"
               class="form-check form-check-inline">
            <input class="form-check-input"
                   type="radio"
                   :id="`payroll-${opt.value}`"
                   name="payroll_type"
                   :value="opt.value"
                   v-model="payrollType" />
            <label class="form-check-label" :for="`payroll-${opt.value}`">
              {{ opt.label }}
            </label>
          </div>
        </div>
      </div>

      <!-- Selector dinámico de período -->
      <div class="mb-4" v-if="payrollType">
        <label class="form-label fw-bold mb-2">Período</label>

        <!-- Semanal -->
        <div v-if="payrollType === 'semanal'" class="row g-3 align-items-end">
          <div class="col-auto">
            <input type="date"
                   class="form-control"
                   v-model="weeklyDate"
                   :max="maxSunday" />
          </div>
          <div class="col-auto" v-if="weeklyDate && !isSunday(weeklyDate)">
            <span class="text-danger small">Seleccione un domingo</span>
          </div>
        </div>

        <!-- Quincenal -->
        <div v-else-if="payrollType === 'quincenal'"
             class="row g-3 align-items-end">
          <div class="col-auto">
            <select class="form-select" v-model="quincenaOption">
              <option value="">Seleccione quincena</option>
              <option value="primera">1ª quincena (01-15)</option>
              <option value="segunda">2ª quincena (16-fin)</option>
            </select>
          </div>
          <div class="col-auto">
            <input type="month" class="form-control" v-model="quincenaMonth" />
          </div>
        </div>

        <!-- Mensual -->
        <div v-else-if="payrollType === 'mensual'"
             class="row g-3 align-items-end">
          <div class="col-auto">
            <input type="month" class="form-control" v-model="monthlyMonth" />
          </div>
        </div>

        <!-- Vista previa -->
        <div class="mt-2" v-if="periodPreview">
          <span class="badge bg-secondary">{{ periodPreview }}</span>
        </div>
      </div>

      <button type="submit" class="btn btn-primary" :disabled="!formValid">
        Procesar nueva planilla
      </button>
    </form>

    <!-- === HISTORIAL === -->
    <hr class="my-5" />
    <h2 class="h4 mb-3">Historial de planillas</h2>

    <table class="table table-striped align-middle">
      <thead>
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
          <td colspan="6" class="text-center text-muted">
            No hay planillas registradas
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
  import { ref, computed } from 'vue'

  /* ===== ESTADO ===== */
  const payrollType = ref('')
  const weeklyDate = ref('')
  const quincenaOption = ref('')
  const quincenaMonth = ref('')
  const monthlyMonth = ref('')
  const psStartDate = ref('')
  const psEndDate = ref('')

  /* Historial (mock) */
  const payrollHistory = ref([
    {
      id: 1,
      manager: 'Juan Pérez',
      type: 'Quincenal',
      period: '01-05-2025 → 15-05-2025',
      gross: 1_500_000,
      deductions: 230_000,
      net: 1_270_000
    },
    {
      id: 2,
      manager: 'María Gómez',
      type: 'Quincenal',
      period: '16-05-2025 → 31-05-2025',
      gross: 1_650_000,
      deductions: 250_000,
      net: 1_400_000
    },
    {
      id: 3,
      manager: 'Carlos Rojas',
      type: 'Semanal',
      period: '01-06-2025 → 07-06-2025',
      gross: 820_000,
      deductions: 120_000,
      net: 700_000
    }
  ])

  /* Opciones para el radio */
  const payrollOptions = [
    { value: 'semanal', label: 'Semanal' },
    { value: 'quincenal', label: 'Quincenal' },
    { value: 'mensual', label: 'Mensual' },
  ]

  /* ===== UTILIDADES ===== */
  function isSunday(d) {
    return !!d && new Date(d).getDay() === 6
  }
  function lastDayOfMonth(y, m0) {
    return new Date(y, m0 + 1, 0).getDate()
  }
  function formatDMY(dateLike) {
    const d = new Date(dateLike)
    return `${String(d.getDate()).padStart(2, '0')}-${String(
      d.getMonth() + 1
    ).padStart(2, '0')}-${d.getFullYear()}`
  }

  /* ===== VISTA PREVIA DEL PERÍODO ===== */
  const periodPreview = computed(() => {
    const t = payrollType.value
    if (!t) return ''

    // Semanal
    if (t === 'semanal' && weeklyDate.value && isSunday(weeklyDate.value)) {
      const end = formatDMY(new Date(new Date(weeklyDate.value).setDate(
        new Date(weeklyDate.value).getDate() + 1
      )))
      const startDate = new Date(weeklyDate.value)
      startDate.setDate(startDate.getDate() - 5)
      return `${formatDMY(startDate)} → ${end}`
    }

    // Quincenal
    if (t === 'quincenal' && quincenaOption.value && quincenaMonth.value) {
      const [y, m] = quincenaMonth.value.split('-').map(Number)
      const mm = String(m).padStart(2, '0')
      if (quincenaOption.value === 'primera') {
        return `01-${mm}-${y} → 15-${mm}-${y}`
      }
      const last = String(lastDayOfMonth(y, m - 1)).padStart(2, '0')
      return `16-${mm}-${y} → ${last}-${mm}-${y}`
    }

    // Mensual
    if (t === 'mensual' && monthlyMonth.value) {
      const [y, m] = monthlyMonth.value.split('-').map(Number)
      const mm = String(m).padStart(2, '0')
      const last = String(lastDayOfMonth(y, m - 1)).padStart(2, '0')
      return `01-${mm}-${y} → ${last}-${mm}-${y}`
    }

    // Servicios profesionales
    if (
      t === 'servicios_profesionales' &&
      psStartDate.value &&
      psEndDate.value &&
      psStartDate.value <= psEndDate.value
    ) {
      return `${formatDMY(psStartDate.value)} → ${formatDMY(psEndDate.value)}`
    }
    return ''
  })

  /* ===== VALIDACIÓN DEL FORMULARIO ===== */
  const formValid = computed(() => {
    switch (payrollType.value) {
      case 'semanal':
        return isSunday(weeklyDate.value)
      case 'quincenal':
        return quincenaOption.value && quincenaMonth.value
      case 'mensual':
        return !!monthlyMonth.value
      case 'servicios_profesionales':
        return (
          psStartDate.value &&
          psEndDate.value &&
          psStartDate.value <= psEndDate.value
        )
      default:
        return false
    }
  })

  /* Último domingo permitido (no fechas futuras) */
  const maxSunday = (() => {
    const today = new Date()
    const last = new Date(today)
    last.setDate(today.getDate() - today.getDay())
    return last.toISOString().split('T')[0]
  })()

  /* ===== SUBMIT (Mock) ===== */
  function handleProcess() {
    if (!formValid.value) return
    const gross = Math.floor(Math.random() * 600_000 + 800_000)
    const deductions = Math.floor(gross * 0.15)

    payrollHistory.value.unshift({
      id: Date.now(),
      manager: 'Yann Sommer',
      type: payrollOptions.find(o => o.value === payrollType.value).label,
      period: periodPreview.value,
      gross,
      deductions,
      net: gross - deductions
    })

    // Limpia campos
    weeklyDate.value = ''
    quincenaOption.value = ''
    quincenaMonth.value = ''
    monthlyMonth.value = ''
    psStartDate.value = ''
    psEndDate.value = ''
    payrollType.value = ''
  }
</script>

<style scoped>
  .badge {
    font-size: 0.9rem;
  }

  .table td,
  .table th {
    vertical-align: middle;
  }
</style>
