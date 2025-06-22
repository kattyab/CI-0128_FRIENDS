<template>
  <div class="container">
    <div class="header">
      <button class="export-btn" @click="openExportModal">
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
      <!-- Salaries Section -->
      <div class="payroll-section">
        <h3 class="section-title" @click="toggleSection('salaries')" style="cursor: pointer;">
          Salarios
          <span class="toggle-arrow">{{ collapsedSections.salaries ? '▶' : '▼' }}</span>
        </h3>
        <table v-if="!collapsedSections.salaries" class="payroll-table">
          <tbody>
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
          </tbody>
        </table>
      </div>

      <!-- Legal Deductions Section -->
      <div class="payroll-section">
        <h3 class="section-title" @click="toggleSection('legalDeductions')" style="cursor: pointer;">
          Deducciones Legales
          <span class="toggle-arrow">{{ collapsedSections.legalDeductions ? '▶' : '▼' }}</span>
        </h3>
        <table v-if="!collapsedSections.legalDeductions" class="payroll-table">
          <tbody>
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
          </tbody>
        </table>
      </div>

      <!-- Total Employer Cost Section -->
      <div class="payroll-section">
        <h3 class="section-title" @click="toggleSection('totalCost')" style="cursor: pointer;">
          Costo total empleador
          <span class="toggle-arrow">{{ collapsedSections.totalCost ? '▶' : '▼' }}</span>
        </h3>
        <table v-if="!collapsedSections.totalCost" class="payroll-table">
          <tbody>
            <tr class="payroll-row total-row">
              <td class="payroll-cell">Costo total empleador</td>
              <td class="payroll-cell amount total-amount">
                <span class="currency">₡</span>{{ formatAmount(currentPayrollData.totalMoneyPaid) }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="status-section" v-if="pdfDownloaded || emailSent">
        <div class="status-item" v-if="pdfDownloaded">
          <div class="status-icon">✓</div>
          <div class="status-text">PDF descargado correctamente</div>
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

    <!-- Export Modal -->
    <div v-if="showExportModal" class="modal-overlay" @click="closeExportModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3 class="modal-title">Opciones de Exportación</h3>
          <button class="modal-close" @click="closeExportModal">×</button>
        </div>

        <div class="modal-body">
          <p class="modal-description">
            Seleccione cómo desea exportar el reporte de nómina para el período {{ formatPeriodDisplay(selectedPeriod) }}:
          </p>

          <div class="export-options">
            <button class="export-option-btn email-btn" @click="handleEmail">
              <div class="option-icon">📧</div>
              <div class="option-content">
                <div class="option-title">Enviar por Email</div>
                <div class="option-description">Enviar el reporte por correo electrónico</div>
              </div>
            </button>

            <button class="export-option-btn download-btn" @click="handleDownloadPDF">
              <div class="option-icon">📄</div>
              <div class="option-content">
                <div class="option-title">Descargar PDF</div>
                <div class="option-description">Guardar como archivo PDF</div>
              </div>
            </button>
          </div>
        </div>

        <div class="modal-footer">
          <button class="modal-cancel-btn" @click="closeExportModal">
            Cancelar
          </button>
        </div>
      </div>
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
  const pdfDownloaded = ref(false)
  const emailSent = ref(false)
  const companyName = ref('Kaizen')
  const companyGuid = ref('')
  const showExportModal = ref(false)

  const API_BASE_URL = import.meta.env.VITE_API_URL

  const collapsedSections = ref({
    salaries: false,
    legalDeductions: false,
    totalCost: false,
  })

  function toggleSection(sectionKey) {
    collapsedSections.value[sectionKey] = !collapsedSections.value[sectionKey]
  }

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
    if (typeof amount !== 'number') return '0,00'

    const parts = amount.toFixed(2).split('.')
    const integerPart = parts[0]
    const decimalPart = parts[1]

    const formattedInteger = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, '.')
    return `${formattedInteger},${decimalPart}`
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

  function openExportModal() {
    showExportModal.value = true
  }

  function closeExportModal() {
    showExportModal.value = false
  }

  function handleEmail() {
    console.log('Sending email for period:', selectedPeriod.value)

    // Hide modal and show email success status
    closeExportModal()

    // Here you would implement the actual email sending functionality
    // For now, we'll simulate it
    alert('Enviando por correo electrónico...')

    setTimeout(() => {
      emailSent.value = true
    }, 1000)
  }

  function handleDownloadPDF() {
    console.log('Downloading PDF for period:', selectedPeriod.value)
    console.log('Data:', currentPayrollData.value)

    // Hide modal and show PDF download success status
    closeExportModal()

    // Here you would implement the actual PDF generation and download
    // For now, we'll simulate it
    alert('Generando PDF...')

    setTimeout(() => {
      pdfDownloaded.value = true
    }, 1000)
  }

  // Legacy function for backward compatibility
  function exportData() {
    openExportModal()
  }

  watch(selectedPeriod, (newPeriod) => {
    console.log(`Selected period changed to: ${newPeriod}`)
    pdfDownloaded.value = false
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
    font-size: 16px;
    opacity: 0.9;
    font-weight: 600;
  }

  .info-value {
    font-size: 16px;
    font-weight: 400;
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
    display: flex;
    align-items: center;
    justify-content: space-between;
    user-select: none;
  }

  .toggle-arrow {
    font-weight: 700;
    user-select: none;
    width: 20px;
    text-align: right;
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

      .payroll-row:hover > td:first-child {
        border-top-left-radius: 10px;
        border-bottom-left-radius: 10px;
      }

      .payroll-row:hover > td:last-child {
        border-top-right-radius: 10px;
        border-bottom-right-radius: 10px;
      }

    .payroll-row.total-row {
      background-color: #f2f2f2;
      font-weight: 600;
    }

      .payroll-row.total-row > td:first-child {
        border-top-left-radius: 10px;
        border-bottom-left-radius: 10px;
      }

      .payroll-row.total-row > td:last-child {
        border-top-right-radius: 10px;
        border-bottom-right-radius: 10px;
      }

      .payroll-row.total-row:hover {
        background-color: #e6e6e6;
      }

        .payroll-row.total-row:hover > td:first-child {
          border-top-left-radius: 10px;
          border-bottom-left-radius: 10px;
        }

        .payroll-row.total-row:hover > td:last-child {
          border-top-right-radius: 10px;
          border-bottom-right-radius: 10px;
        }

  .payroll-cell {
    padding: 12px 20px;
    text-align: left;
    color: #003c63;
  }

    .payroll-cell.amount {
      padding: 12px 20px;
      text-align: right;
      font-weight: 600;
      color: #003c63;
    }

    .payroll-cell.total-amount {
      color: #003c63;
      font-size: 16px;
      font-weight: 600;
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

  /* Modal Styles */
  .modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
    backdrop-filter: blur(4px);
  }

  .modal-content {
    background: white;
    border-radius: 12px;
    width: 90%;
    max-width: 500px;
    max-height: 90vh;
    overflow-y: auto;
    box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
    animation: modalSlideIn 0.3s ease-out;
  }

  @keyframes modalSlideIn {
    from {
      opacity: 0;
      transform: translateY(-50px) scale(0.95);
    }

    to {
      opacity: 1;
      transform: translateY(0) scale(1);
    }
  }

  .modal-header {
    padding: 24px 24px 0 24px;
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .modal-title {
    font-size: 20px;
    font-weight: 700;
    color: #003c63;
    margin: 0;
  }

  .modal-close {
    background: none;
    border: none;
    font-size: 24px;
    color: #6b7280;
    cursor: pointer;
    padding: 4px;
    line-height: 1;
    transition: color 0.2s ease;
  }

    .modal-close:hover {
      color: #003c63;
    }

  .modal-body {
    padding: 24px;
  }

  .modal-description {
    font-size: 16px;
    color: #6b7280;
    margin-bottom: 24px;
    line-height: 1.5;
  }

  .export-options {
    display: flex;
    flex-direction: column;
    gap: 12px;
  }

  .export-option-btn {
    display: flex;
    align-items: center;
    gap: 16px;
    padding: 16px;
    border: 2px solid #e2e8f0;
    border-radius: 8px;
    background: white;
    cursor: pointer;
    transition: all 0.2s ease;
    text-align: left;
    width: 100%;
  }

    .export-option-btn:hover {
      border-color: #003c63;
      background-color: #f8fafc;
      transform: translateY(-1px);
    }

  .option-icon {
    font-size: 24px;
    width: 32px;
    text-align: center;
  }

  .option-content {
    flex: 1;
  }

  .option-title {
    font-size: 16px;
    font-weight: 600;
    color: #003c63;
    margin-bottom: 4px;
  }

  .option-description {
    font-size: 14px;
    color: #6b7280;
  }

  .modal-footer {
    padding: 0 24px 24px 24px;
    display: flex;
    justify-content: flex-end;
  }

  .modal-cancel-btn {
    background: none;
    border: 1px solid #d1d5db;
    color: #6b7280;
    padding: 8px 16px;
    border-radius: 6px;
    cursor: pointer;
    font-size: 14px;
    transition: all 0.2s ease;
  }

    .modal-cancel-btn:hover {
      background-color: #f3f4f6;
      color: #374151;
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

    .modal-content {
      width: 95%;
      margin: 20px;
    }

    .modal-header,
    .modal-body,
    .modal-footer {
      padding-left: 16px;
      padding-right: 16px;
    }

    .export-options {
      gap: 8px;
    }

    .export-option-btn {
      padding: 12px;
      gap: 12px;
    }

    .option-icon {
      font-size: 20px;
      width: 24px;
    }
  }
</style>
