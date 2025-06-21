<template>
  <div class="container">
    <div class="header">
      <button class="export-btn" @click="exportData">
        📤 Exportar
      </button>

      <div class="period-selector">
        <span class="period-label">Período:</span>
        <select class="period-dropdown" v-model="selectedPeriod">
          <option v-for="period in availablePeriods" :key="period" :value="period">
            {{ formatPeriodDisplay(period) }}
          </option>
        </select>
      </div>

      <div class="employee-info">
        <div class="info-item">
          <span class="info-label">Nombre de la empresa:</span>
          <span class="info-value">{{ companyName }}</span>
        </div>
        <div class="info-item">
          <span class="info-label">Nombre de empleador:</span>
          <span class="info-value">{{ currentPayrollData?.ownerFullName || 'Cargando...' }}</span>
        </div>
        <div class="info-item">
          <span class="info-label">Fecha de pago:</span>
          <span class="info-value">{{ formatExecutionDate(currentPayrollData?.executedOn) }}</span>
        </div>
      </div>
    </div>

    <div class="payroll-content" v-if="currentPayrollData">
      <div class="payroll-section">
        <h3 class="section-title">Salarios</h3>
        <table class="payroll-table">
          <tr class="payroll-row total-row">
            <td class="payroll-cell">Total salarios</td>
            <td class="payroll-cell amount total-amount">
              <span class="currency">₡</span>{{ formatAmount(totalSalarios) }}
            </td>
          </tr>
          <tr class="payroll-row"
              v-for="salary in salaryItems"
              :key="salary.key">
            <td class="payroll-cell">{{ salary.label }}</td>
            <td class="payroll-cell amount">
              <span class="currency">₡</span>{{ formatAmount(salary.amount) }}
            </td>
          </tr>
        </table>
      </div>

      <div class="payroll-section">
        <h3 class="section-title">Deducciones Legales</h3>
        <table class="payroll-table">
          <tr class="payroll-row total-row">
            <td class="payroll-cell">Total pagos de ley</td>
            <td class="payroll-cell amount total-amount">
              <span class="currency">₡</span>{{ formatAmount(currentPayrollData.totalLaborCharges) }}
            </td>
          </tr>
          <tr class="payroll-row"
              v-for="deduction in legalDeductions"
              :key="deduction.key">
            <td class="payroll-cell">{{ deduction.label }}</td>
            <td class="payroll-cell amount">
              <span class="currency">₡</span>{{ formatAmount(deduction.amount) }}
            </td>
          </tr>
        </table>
      </div>

      <div class="payroll-section">
        <table class="payroll-table">
          <tr class="payroll-row total-row">
            <td class="payroll-cell">Costo total empleador</td>
            <td class="payroll-cell amount total-amount">
              <span class="currency">₡</span>{{ formatAmount(currentPayrollData.totalMoneyPaid) }}
            </td>
          </tr>
        </table>
      </div>

      <div class="status-section" v-if="exportCompleted || emailSent">
        <div class="status-item" v-if="exportCompleted">
          <div class="status-icon">✓</div>
          <div class="status-text">Descarga realizada correctamente</div>
        </div>
        <div class="status-item" v-if="emailSent">
          <div class="status-icon">✓</div>
          <div class="status-text">Enviado por correo correctamente</div>
        </div>
      </div>
    </div>

    <div class="loading-section" v-else-if="loading">
      <div class="loading-text">Cargando datos...</div>
    </div>

    <div class="error-section" v-else-if="error">
      <div class="error-text">Error al cargar los datos: {{ error }}</div>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed, watch, onMounted } from 'vue'
  import axios from 'axios'

  const selectedPeriod = ref('')
  const payrollDataList = ref([])
  const loading = ref(false)
  const error = ref(null)
  const exportCompleted = ref(false)
  const emailSent = ref(false)
  const companyName = ref('Kaizen')
  const companyGuid = ref('')

  const API_BASE_URL = import.meta.env.VITE_API_URL

  const availablePeriods = computed(() => {
    return payrollDataList.value.map(item => item.period).sort((a, b) => {
      return b.localeCompare(a)
    })
  })

  const currentPayrollData = computed(() => {
    return payrollDataList.value.find(item => item.period === selectedPeriod.value)
  })

  const totalSalarios = computed(() => {
    if (!currentPayrollData.value) return 0
    const data = currentPayrollData.value
    return data.serviciosProfesionalesAmount + data.porHorasAmount + data.tiempoCompletoAmount
  })

  const salaryItems = computed(() => {
    if (!currentPayrollData.value) return []

    const data = currentPayrollData.value
    return [
      {
        key: 'porHorasAmount',
        label: 'Salario por horas',
        amount: data.porHorasAmount
      },
      {
        key: 'tiempoCompletoAmount',
        label: 'Salario tiempo completo',
        amount: data.tiempoCompletoAmount
      },
      {
        key: 'serviciosProfesionalesAmount',
        label: 'Salario servicios profesionales',
        amount: data.serviciosProfesionalesAmount
      }
    ]
  })

  const legalDeductions = computed(() => {
    if (!currentPayrollData.value) return []

    const data = currentPayrollData.value
    return [
      { key: 'sem', label: 'SEM', amount: data.sem },
      { key: 'ivm', label: 'IVM', amount: data.ivm },
      {
        key: 'cuotaPatronalBancoPopular',
        label: 'Cuota Patronal Banco Popular',
        amount: data.cuotaPatronalBancoPopular
      },
      {
        key: 'asignacionesFamiliares',
        label: 'Asignaciones Familiares',
        amount: data.asignacionesFamiliares
      },
      { key: 'imas', label: 'IMAS', amount: data.imas },
      { key: 'ina', label: 'INA', amount: data.ina },
      {
        key: 'aporteBancoPopular',
        label: 'Aporte Banco Popular',
        amount: data.aporteBancoPopular
      },
      { key: 'fcl', label: 'FCL', amount: data.fcl },
      {
        key: 'fondoPensionesComplementarias',
        label: 'Fondo de Pensiones Complementarias',
        amount: data.fondoPensionesComplementarias
      },
      { key: 'ins', label: 'INS', amount: data.ins }
    ]
  })

  function extractGuidFromUrl() {
    const currentUrl = window.location.href

    const guidPattern = /[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}/i
    const match = currentUrl.match(guidPattern)

    if (match) {
      return match[0]
    }
    const pathParts = window.location.pathname.split('/')
    const guidIndex = pathParts.findIndex(part => /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(part))

    if (guidIndex !== -1) {
      return pathParts[guidIndex]
    }

    return null
  }

  function formatAmount(amount) {
    if (typeof amount !== 'number') return '0.00'
    return new Intl.NumberFormat('es-CR', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(amount)
  }

  function formatPeriodDisplay(period) {
    const [month, year] = period.split('-')
    const monthNames = [
      'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
      'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
    ]
    return `${monthNames[parseInt(month) - 1]} ${year}`
  }

  function formatExecutionDate(dateString) {
    if (!dateString) return 'N/A'

    try {
      const date = new Date(dateString)
      return date.toLocaleDateString('es-CR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
      })
    } catch (e) {
      return 'N/A'
    }
  }

  async function fetchPayrollData() {
    try {
      loading.value = true
      error.value = null

      const guid = extractGuidFromUrl()

      if (!guid) {
        throw new Error('Company GUID not found in URL')
      }

      companyGuid.value = guid

      const apiEndpoint = `${API_BASE_URL}/api/Reports/company/${guid}`

      const response = await axios.get(apiEndpoint)

      payrollDataList.value = response.data

      if (response.data.length > 0) {
        selectedPeriod.value = availablePeriods.value[0]
      }

    } catch (err) {
      error.value = err.response?.data?.message || err.message || 'Error fetching payroll data'
      console.error('Error fetching payroll data:', err)
    } finally {
      loading.value = false
    }
  }

  function exportData() {
    exportCompleted.value = true

    setTimeout(() => {
      emailSent.value = true
    }, 1000)

    console.log('Exporting data for period:', selectedPeriod.value)
    console.log('Data:', currentPayrollData.value)

    alert('Exportando datos...')
  }

  watch(selectedPeriod, (newPeriod) => {
    console.log(`Selected period changed to: ${newPeriod}`)
    exportCompleted.value = false
    emailSent.value = false
  })

  onMounted(() => {
    fetchPayrollData()
  })
</script>

<style scoped>
  body {
    background-color: #f5f7fa;
    color: #003c63;
    line-height: 1.5;
  }

  .container {
    max-width: 800px;
    margin: 20px auto;
    background: white;
    border-radius: 12px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    overflow: hidden;
  }

  .header {
    background: white;
    color: #003c63;
    padding: 20px;
    position: relative;
    border-bottom: 1px solid #e2e8f0;
  }

  .export-btn {
    position: absolute;
    top: 15px;
    right: 20px;
    background: #003c63;
    border: 1px solid #003c63;
    color: white;
    padding: 8px 16px;
    border-radius: 6px;
    cursor: pointer;
    font-size: 14px;
    display: flex;
    align-items: center;
    gap: 8px;
    transition: all 0.3s ease;
  }

    .export-btn:hover {
      background: #002a4a;
      border-color: #002a4a;
      transform: translateY(-1px);
    }

  .period-selector {
    margin-bottom: 20px;
    display: flex;
    align-items: center;
    gap: 8px;
  }

  .period-label {
    font-size: 16px;
    font-weight: 600;
  }

  .period-dropdown {
    background: white;
    border: 1px solid #e2e8f0;
    color: #003c63;
    padding: 8px 12px;
    border-radius: 6px;
    font-size: 14px;
    min-width: 200px;
  }

  .employee-info {
    display: flex;
    flex-direction: column;
    gap: 10px;
    margin-top: 20px;
  }

  .info-item {
    display: flex;
    align-items: center;
    gap: 8px;
  }

  .info-label {
    font-size: 14px;
    opacity: 0.9;
    font-weight: 500;
  }

  .info-value {
    font-size: 16px;
    font-weight: 600;
  }

  .payroll-content {
    padding: 30px;
  }

  .payroll-section {
    margin-bottom: 30px;
  }

  .section-title {
    font-size: 18px;
    font-weight: 700;
    color: #003c63;
    margin-bottom: 15px;
    padding-bottom: 8px;
    border-bottom: 2px solid #e2e8f0;
  }

  .payroll-table {
    width: 100%;
    border-collapse: collapse;
  }

  .payroll-row {
    border-bottom: 1px solid #e2e8f0;
    transition: background-color 0.2s ease;
  }

    .payroll-row:hover {
      background-color: #f2f2f2;
    }

    .payroll-row.total-row {
      background-color: #f2f2f2;
      font-weight: 600;
    }

      .payroll-row.total-row:hover {
        background-color: #e6e6e6;
      }

  .payroll-cell {
    padding: 12px 0;
    text-align: left;
    color: #003c63;
  }

    .payroll-cell.amount {
      text-align: right;
      font-weight: 600;
      color: #003c63;
    }

    .payroll-cell.total-amount {
      color: #003c63;
      font-size: 16px;
    }

  .currency {
    color: #003c63;
    margin-right: 4px;
    opacity: 0.7;
  }

  .status-section {
    margin-top: 20px;
  }

  .status-item {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-bottom: 10px;
  }

    .status-item:last-child {
      margin-bottom: 0;
    }

  .status-icon {
    color: #00c3b6;
    font-weight: bold;
    font-size: 16px;
  }

  .status-text {
    color: #00c3b6;
    font-weight: 500;
  }

  .loading-section {
    padding: 40px;
    text-align: center;
  }

  .loading-text {
    color: #003c63;
    font-size: 16px;
  }

  .error-section {
    padding: 40px;
    text-align: center;
  }

  .error-text {
    color: #dc3545;
    font-size: 16px;
  }

  @media (max-width: 768px) {
    .container {
      margin: 10px;
    }

    .header {
      padding: 15px;
    }

    .employee-info {
      flex-direction: column;
      gap: 10px;
    }

    .payroll-content {
      padding: 20px;
    }

    .export-btn {
      position: static;
      margin-top: 15px;
      align-self: flex-start;
    }
  }
</style>
