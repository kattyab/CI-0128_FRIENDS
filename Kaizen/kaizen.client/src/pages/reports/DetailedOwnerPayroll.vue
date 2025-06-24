<template>
  <div class="container" ref="exportContainer">
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

      <div class="employee-info" ref="headerInfo">
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

    <div class="payroll-content" v-if="currentPayrollData" ref="payrollContent">
      <div class="payroll-section">
        <h3 class="section-title" @click="toggleSection('salaries')" style="cursor: pointer;">
          Salarios
          <span class="toggle-arrow">{{ collapsedSections.salaries ? '▶' : '▼' }}</span>
        </h3>
        <table v-if="!collapsedSections.salaries" class="payroll-table">
          <tbody>
            <tr class="payroll-row total-row">
              <td class="payroll-cell">Total salarios</td>
              <td class="payroll-cell amount total-amount bold">
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

      <div class="payroll-section">
        <h3 class="section-title" @click="toggleSection('legalDeductions')" style="cursor: pointer;">
          Deducciones Legales
          <span class="toggle-arrow">{{ collapsedSections.legalDeductions ? '▶' : '▼' }}</span>
        </h3>
        <table v-if="!collapsedSections.legalDeductions" class="payroll-table">
          <tbody>
            <tr class="payroll-row total-row">
              <td class="payroll-cell">Total pagos de ley</td>
              <td class="payroll-cell amount total-amount bold">
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

      <div class="payroll-section">
        <h3 class="section-title" @click="toggleSection('totalCost')" style="cursor: pointer;">
          Costo total empleador
          <span class="toggle-arrow">{{ collapsedSections.totalCost ? '▶' : '▼' }}</span>
        </h3>
        <table v-if="!collapsedSections.totalCost" class="payroll-table">
          <tbody>
            <tr class="payroll-row total-row">
              <td class="payroll-cell">Costo total empleador</td>
              <td class="payroll-cell amount total-amount bold">
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

    <div v-if="showExportModal" class="modal-overlay" @click="closeExportModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3 class="modal-title">Opciones de Exportación</h3>
          <button class="modal-close" @click="closeExportModal">×</button>
        </div>

        <div class="modal-body">
          <p class="modal-description">
            Seleccione cómo desea exportar el reporte de planilla para el período {{ formatPeriodDisplay(selectedPeriod) }}:
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
            Cerrar
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed, watch, onMounted } from 'vue'
  import axios from 'axios'
  import html2pdf from 'html2pdf.js'

  const userEmail = ref('')

  const DECIMAL_PLACES = 2
  const THOUSANDS_SEPARATOR = '.'
  const DECIMAL_SEPARATOR = ','
  const DEFAULT_AMOUNT = '0,00'
  const PDF_MARGIN = 10
  const PDF_SCALE = 2
  const PDF_QUALITY = 0.98
  const PDF_FORMAT = 'a4'
  const PDF_ORIENTATION = 'portrait'
  const PDF_UNIT = 'mm'
  const PDF_IMAGE_TYPE = 'jpeg'
  const CONTAINER_PADDING = '20px'
  const CONTAINER_COLOR = '#003c63'
  const FONT_FAMILY = 'Arial, sans-serif'
  const HEADER_MARGIN_BOTTOM = '20px'
  const HEADER_INFO_MARGIN_BOTTOM = '30px'
  const LOCALE_ES_CR = 'es-CR'
  const DATE_FORMAT_OPTIONS = {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  }
  const FALLBACK_DATE = 'N/A'

  const MONTH_NAMES = [
    'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
    'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
  ]

  const GUID_PATTERN = /[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}/i

  const API_ENDPOINT_TEMPLATE = '/api/Reports/company/'

  const ERROR_MESSAGES = {
    GUID_NOT_FOUND: 'Company GUID not found in URL',
    FETCH_DEFAULT: 'Error fetching payroll data'
  }

  const SALARY_ITEM_CONFIGS = [
    {
      key: 'porHorasAmount',
      label: 'Salario por horas'
    },
    {
      key: 'tiempoCompletoAmount',
      label: 'Salario tiempo completo'
    },
    {
      key: 'serviciosProfesionalesAmount',
      label: 'Salario servicios profesionales'
    }
  ]

  const LEGAL_DEDUCTION_CONFIGS = [
    { key: 'sem', label: 'SEM' },
    { key: 'ivm', label: 'IVM' },
    {
      key: 'cuotaPatronalBancoPopular',
      label: 'Cuota Patronal Banco Popular'
    },
    {
      key: 'asignacionesFamiliares',
      label: 'Asignaciones Familiares'
    },
    { key: 'imas', label: 'IMAS' },
    { key: 'ina', label: 'INA' },
    {
      key: 'aporteBancoPopular',
      label: 'Aporte Banco Popular'
    },
    { key: 'fcl', label: 'FCL' },
    {
      key: 'fondoPensionesComplementarias',
      label: 'Fondo de Pensiones Complementarias'
    },
    { key: 'ins', label: 'INS' }
  ]

  const SECTION_NAMES = {
    SALARIES: 'salaries',
    LEGAL_DEDUCTIONS: 'legalDeductions',
    TOTAL_COST: 'totalCost'
  }

  const selectedPeriod = ref('')
  const payrollDataList = ref([])
  const loading = ref(false)
  const error = ref(null)
  const pdfDownloaded = ref(false)
  const emailSent = ref(false)
  const showExportModal = ref(false)
  const companyName = ref('')
  const companyGuid = ref('')

  const collapsedSections = ref({
    [SECTION_NAMES.SALARIES]: false,
    [SECTION_NAMES.LEGAL_DEDUCTIONS]: false,
    [SECTION_NAMES.TOTAL_COST]: false
  })

  const exportContainer = ref(null)
  const headerInfo = ref(null)
  const payrollContent = ref(null)

  const API_BASE_URL = import.meta.env.VITE_API_URL

  const availablePeriods = computed(() => {
    return payrollDataList.value
      .map(item => item.period)
      .sort((a, b) => {
        const dateA = new Date(a.split('-')[1], parseInt(a.split('-')[0]) - 1)
        const dateB = new Date(b.split('-')[1], parseInt(b.split('-')[0]) - 1)
        return dateB - dateA
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
    return SALARY_ITEM_CONFIGS.map(config => ({
      key: config.key,
      label: config.label,
      amount: data[config.key]
    }))
  })

  const legalDeductions = computed(() => {
    if (!currentPayrollData.value) return []

    const data = currentPayrollData.value
    return LEGAL_DEDUCTION_CONFIGS.map(config => ({
      key: config.key,
      label: config.label,
      amount: data[config.key]
    }))
  })

  function extractGuidFromUrl() {
    const currentUrl = window.location.href
    const match = currentUrl.match(GUID_PATTERN)

    if (match) {
      return match[0]
    }

    const pathParts = window.location.pathname.split('/')
    const guidIndex = pathParts.findIndex(part => GUID_PATTERN.test(part))

    if (guidIndex !== -1) {
      return pathParts[guidIndex]
    }

    return null
  }

  function formatAmount(amount) {
    if (typeof amount !== 'number') return DEFAULT_AMOUNT

    const parts = amount.toFixed(DECIMAL_PLACES).split('.')
    const integerPart = parts[0]
    const decimalPart = parts[1]

    const formattedInteger = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, THOUSANDS_SEPARATOR)
    return `${formattedInteger}${DECIMAL_SEPARATOR}${decimalPart}`
  }

  function formatPeriodDisplay(period) {
    const [month, year] = period.split('-')
    return `${MONTH_NAMES[parseInt(month) - 1]} ${year}`
  }

  function formatExecutionDate(dateString) {
    if (!dateString) return FALLBACK_DATE

    try {
      const date = new Date(dateString)
      return date.toLocaleDateString(LOCALE_ES_CR, DATE_FORMAT_OPTIONS)
    } catch (e) {
      return FALLBACK_DATE
    }
  }

  async function fetchAuthenticatedUser() {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/login/authenticate`, {
        withCredentials: true
      })

      userEmail.value = response.data.email
    } catch (err) {
      console.error('No se pudo obtener el usuario autenticado', err)
    }
  }

  async function fetchPayrollData() {
    try {
      loading.value = true
      error.value = null

      const guid = extractGuidFromUrl()

      if (!guid) {
        throw new Error(ERROR_MESSAGES.GUID_NOT_FOUND)
      }

      companyGuid.value = guid

      const apiEndpoint = `${API_BASE_URL}${API_ENDPOINT_TEMPLATE}${guid}`

      const response = await axios.get(apiEndpoint)

      payrollDataList.value = response.data

      if (response.data.length > 0) {
        selectedPeriod.value = availablePeriods.value[0]
        companyName.value = response.data[0].companyName
      }

    } catch (err) {
      error.value = err.response?.data?.message || err.message || ERROR_MESSAGES.FETCH_DEFAULT
      console.error('Error fetching payroll data:', err)
    } finally {
      loading.value = false
    }
  }

  function toggleSection(sectionName) {
    collapsedSections.value[sectionName] = !collapsedSections.value[sectionName]
  }

  function openExportModal() {
    showExportModal.value = true
    pdfDownloaded.value = false
    emailSent.value = false
  }

  function closeExportModal() {
    showExportModal.value = false
  }

  async function handleEmail() {
    if (!exportContainer.value) return

    const pdfExportWrapper = document.createElement('div')
    pdfExportWrapper.style.padding = CONTAINER_PADDING
    pdfExportWrapper.style.color = CONTAINER_COLOR
    pdfExportWrapper.style.fontFamily = FONT_FAMILY

    const periodTitle = document.createElement('h2')
    periodTitle.textContent = `Reporte de Nómina - ${formatPeriodDisplay(selectedPeriod.value)}`
    periodTitle.style.textAlign = 'center'
    periodTitle.style.marginBottom = HEADER_MARGIN_BOTTOM
    pdfExportWrapper.appendChild(periodTitle)

    if (headerInfo.value) {
      const headerClone = headerInfo.value.cloneNode(true)
      headerClone.style.marginBottom = HEADER_INFO_MARGIN_BOTTOM
      pdfExportWrapper.appendChild(headerClone)
    }

    if (payrollContent.value) {
      const payrollClone = payrollContent.value.cloneNode(true)
      pdfExportWrapper.appendChild(payrollClone)
    }

    try {
      const pdfBlob = await html2pdf()
        .set({
          margin: PDF_MARGIN,
          image: { type: PDF_IMAGE_TYPE, quality: PDF_QUALITY },
          html2canvas: { scale: PDF_SCALE },
          jsPDF: { unit: PDF_UNIT, format: PDF_FORMAT, orientation: PDF_ORIENTATION }
        })
        .from(pdfExportWrapper)
        .outputPdf('blob')

      const reader = new FileReader()
      reader.onloadend = async () => {
        const base64Data = reader.result.split(',')[1]

        const emailPayload = {
          to: [userEmail.value],
          subject: `Reporte de Nómina - ${formatPeriodDisplay(selectedPeriod.value)}`,
          body: 'Adjunto encontrará el reporte de planilla en formato PDF.',
          fileName: `reporte_planilla_${selectedPeriod.value}.pdf`,
          base64Content: base64Data
        }

        try {
          await axios.post(`${API_BASE_URL}/api/emails/send-pdf`, emailPayload)
          emailSent.value = true
          showExportModal.value = false
        } catch (err) {
          console.error('Error sending email:', err)
          alert('Error al enviar el correo.')
        }
      }

      reader.readAsDataURL(pdfBlob)
    } catch (err) {
      console.error('Error generating PDF:', err)
      alert('Error al generar el PDF.')
    }
  }

  function handleDownloadPDF() {
    if (!exportContainer.value) return

    const pdfExportWrapper = document.createElement('div')
    pdfExportWrapper.style.padding = CONTAINER_PADDING
    pdfExportWrapper.style.color = CONTAINER_COLOR
    pdfExportWrapper.style.fontFamily = FONT_FAMILY

    const periodTitle = document.createElement('h2')
    periodTitle.textContent = `Reporte de Nómina - ${formatPeriodDisplay(selectedPeriod.value)}`
    periodTitle.style.textAlign = 'center'
    periodTitle.style.marginBottom = HEADER_MARGIN_BOTTOM
    pdfExportWrapper.appendChild(periodTitle)

    if (headerInfo.value) {
      const headerClone = headerInfo.value.cloneNode(true)
      headerClone.style.marginBottom = HEADER_INFO_MARGIN_BOTTOM
      pdfExportWrapper.appendChild(headerClone)
    }

    if (payrollContent.value) {
      const payrollClone = payrollContent.value.cloneNode(true)
      pdfExportWrapper.appendChild(payrollClone)
    }

    const pdfOptions = {
      margin: PDF_MARGIN,
      filename: `reporte_planilla_${selectedPeriod.value}.pdf`,
      image: { type: PDF_IMAGE_TYPE, quality: PDF_QUALITY },
      html2canvas: { scale: PDF_SCALE },
      jsPDF: { unit: PDF_UNIT, format: PDF_FORMAT, orientation: PDF_ORIENTATION }
    }

    html2pdf()
      .set(pdfOptions)
      .from(pdfExportWrapper)
      .save()
      .then(() => {
        pdfDownloaded.value = true
      })
      .catch(err => {
        console.error('Error generating PDF:', err)
      })
  }

  watch(selectedPeriod, (newPeriod) => {
    pdfDownloaded.value = false
    emailSent.value = false
  })

  onMounted(() => {
    fetchPayrollData()
    fetchAuthenticatedUser()
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

  .pdf-export-wrapper {
    background-color: white;
    color: #003c63;
    padding: 30px;
  }

  .pdf-period-title {
    font-size: 20px;
    font-weight: bold;
    text-align: center;
    margin-bottom: 20px;
  }

  .payroll-content {
  }

  .payroll-section {
    margin-top: 10px;
    margin-bottom: 10px;
  }

  .section-title {
    font-size: 18px;
    font-weight: 700;
    color: #003c63;
    margin-bottom: 8px;
    padding-bottom: 8px;
    border-bottom: 2px solid #e2e8f0;
    user-select: none;
  }

  .toggle-arrow {
    font-weight: 700;
    font-size: 18px;
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

  .payroll-cell {
    padding: 10px 15px;
    font-size: 15px;
    color: #003c63;
  }

    .payroll-cell.amount {
      text-align: right;
    }

  .total-amount {
    font-weight: 700;
  }

  .currency {
    font-weight: 600;
    margin-right: 6px;
  }

  .status-section {
    margin-top: 20px;
    display: flex;
    gap: 20px;
    justify-content: center;
  }

  .status-item {
    display: flex;
    align-items: center;
    gap: 8px;
    color: #006400;
    font-weight: 600;
  }

  .status-icon {
    font-weight: 700;
    font-size: 20px;
  }

  .loading-section,
  .error-section {
    text-align: center;
    padding: 40px 20px;
    font-weight: 600;
    font-size: 18px;
    color: #a00000;
  }

  .modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
  }

  .modal-content {
    background: white;
    border-radius: 12px;
    max-width: 450px;
    width: 90%;
    box-shadow: 0 0 10px rgb(0 0 0 / 0.3);
    display: flex;
    flex-direction: column;
  }

  .modal-header {
    padding: 20px;
    border-bottom: 1px solid #e2e8f0;
    font-weight: 700;
    font-size: 20px;
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .modal-close {
    background: transparent;
    border: none;
    font-size: 28px;
    cursor: pointer;
    color: #003c63;
    font-weight: 700;
    line-height: 1;
  }

  .modal-body {
    padding: 20px;
  }

  .modal-description {
    margin-bottom: 20px;
    font-size: 14px;
    font-weight: 500;
    color: #003c63;
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
    border-radius: 8px;
    padding: 12px 16px;
    border: 1px solid #003c63;
    cursor: pointer;
    background: white;
    transition: all 0.3s ease;
    text-align: left;
  }

    .export-option-btn:hover {
      background: #003c63;
      color: white;
      border-color: #003c63;
    }

  .option-icon {
    font-size: 28px;
    flex-shrink: 0;
  }

  .option-content {
    display: flex;
    flex-direction: column;
  }

  .option-title {
    font-weight: 700;
    font-size: 16px;
  }

  .option-description {
    font-size: 13px;
  }

  .modal-footer {
    padding: 16px 20px;
    border-top: 1px solid #e2e8f0;
    display: flex;
    justify-content: flex-end;
  }

  .modal-cancel-btn {
    background: #f44336;
    color: white;
    border: none;
    font-weight: 600;
    font-size: 14px;
    padding: 10px 18px;
    border-radius: 6px;
    cursor: pointer;
    transition: background-color 0.2s ease;
  }

    .modal-cancel-btn:hover {
      background: #d32f2f;
    }
</style>
