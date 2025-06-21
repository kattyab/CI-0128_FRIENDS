<template>
  <div class="container">
    <div class="header">
      <button class="export-btn" @click="exportData">
        📤 Exportar
      </button>

      <div class="period-selector">
        <span class="period-label">Período:</span>
        <select class="period-dropdown" v-model="selectedPeriod">
          <option v-for="period in periods" :key="period" :value="period">
            {{ period }}
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
          <span class="info-value">{{ employerName }}</span>
        </div>
        <div class="info-item">
          <span class="info-label">Fecha de pago:</span>
          <span class="info-value">{{ paymentDate }}</span>
        </div>
      </div>
    </div>

    <div class="payroll-content">
      <div class="payroll-section">
        <h3 class="section-title">Salarios</h3>
        <table class="payroll-table">
          <tr class="payroll-row total-row">
            <td class="payroll-cell">Total salarios</td>
            <td class="payroll-cell amount total-amount">
              <span class="currency">₡</span>{{ formatAmount(payrollData.totalSalarios) }}
            </td>
          </tr>
          <tr
            class="payroll-row"
            v-for="salary in salaryItems"
            :key="salary.key"
          >
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
              <span class="currency">₡</span>{{ formatAmount(payrollData.totalPagosLey) }}
            </td>
          </tr>
          <tr
            class="payroll-row"
            v-for="deduction in legalDeductions"
            :key="deduction.key"
          >
            <td class="payroll-cell">{{ deduction.label }}</td>
            <td class="payroll-cell amount">
              <span class="currency">₡</span>{{ formatAmount(deduction.amount) }}
            </td>
          </tr>
        </table>
      </div >

      <div class="payroll-section">
        <table class="payroll-table">
          <tr class="payroll-row total-row">
            <td class="payroll-cell">Costo total empleador</td>
            <td class="payroll-cell amount total-amount">
              <span class="currency">₡</span>{{ formatAmount(payrollData.costoTotalEmpleador) }}
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
    </div >
  </div >
</template >

<script setup>
import { ref, computed, watch, onMounted } from 'vue'

const selectedPeriod = ref('Mayo 2025')
const periods = [
  'Enero 2025',
  'Febrero 2025',
  'Marzo 2025',
  'Abril 2025',
  'Mayo 2025',
  'Junio 2025'
]

const companyName = ref('Kaizen')
const employerName = ref('Carlos Ramírez')
const paymentDate = ref('31/05/2025')

const exportCompleted = ref(false)
const emailSent = ref(false)

const payrollData = ref({
  totalSalarios: 3700000,
  salarioPorHoras: 600000,
  salarioTiempoCompleto: 1500000,
  salarioServiciosProfesionales: 1600000,
  totalPagosLey: 469000,
  sem: 194250,
  ivm: 110250,
  cuotaPatronalBancoPopular: 5250,
  asignacionesFamiliares: 5250,
  imas: 5250,
  ina: 5250,
  aporteBancoPopular: 5250,
  fcl: 5250,
  fondoPensionesComplementarias: 5250,
  ins: 5250,
  costoTotalEmpleador: 4969000
})

const salaryItems = computed(() => [
  {
    key: 'salarioPorHoras',
    label: 'Salario por horas',
    amount: payrollData.value.salarioPorHoras
  },
  {
    key: 'salarioTiempoCompleto',
    label: 'Salario tiempo completo',
    amount: payrollData.value.salarioTiempoCompleto
  },
  {
    key: 'salarioServiciosProfesionales',
    label: 'Salario servicios profesionales',
    amount: payrollData.value.salarioServiciosProfesionales
  }
])

const legalDeductions = computed(() => [
  { key: 'sem', label: 'SEM', amount: payrollData.value.sem },
  { key: 'ivm', label: 'IVM', amount: payrollData.value.ivm },
  {
    key: 'cuotaPatronalBancoPopular',
    label: 'Cuota Patronal Banco Popular',
    amount: payrollData.value.cuotaPatronalBancoPopular
  },
  {
    key: 'asignacionesFamiliares',
    label: 'Asignaciones Familiares',
    amount: payrollData.value.asignacionesFamiliares
  },
  { key: 'imas', label: 'IMAS', amount: payrollData.value.imas },
  { key: 'ina', label: 'INA', amount: payrollData.value.ina },
  {
    key: 'aporteBancoPopular',
    label: 'Aporte Banco Popular',
    amount: payrollData.value.aporteBancoPopular
  },
  { key: 'fcl', label: 'FCL', amount: payrollData.value.fcl },
  {
    key: 'fondoPensionesComplementarias',
    label: 'Fondo de Pensiones Complementarias',
    amount: payrollData.value.fondoPensionesComplementarias
  },
  { key: 'ins', label: 'INS', amount: payrollData.value.ins }
])

function formatAmount(amount) {
  return new Intl.NumberFormat('es-CR').format(amount)
}

function exportData() {
  exportCompleted.value = true
  
  setTimeout(() => {
    emailSent.value = true
  }, 1000)
  
  alert('Exportando datos...')
}

function fetchPayrollData(period) {
  console.log(`Fetching data for: ${period}`)
  // TODO(ERICK): FETCH DATA HERE WHEN BACKEND IS READY
}

watch(selectedPeriod, (newPeriod) => {
  fetchPayrollData(newPeriod)
})

onMounted(() => {
  fetchPayrollData(selectedPeriod.value)
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
