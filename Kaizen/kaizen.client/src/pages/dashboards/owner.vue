<template>
  <section class="dashboard-grid">
    <div class="charts-row">
      <EmployeeCountChart />
      <PayrollCostPieChart />
    </div>
    <div class="ultimos-pagos-wrapper">
      <h2 class="ultimos-pagos-title">Ultimos Pagos</h2>
      <table class="ultimos-pagos-table">
        <thead>
          <tr>
            <th>Planilla</th>
            <th>Fecha</th>
            <th>Costo Total</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(pago, i) in lastPayrolls" :key="i" :class="{ 'row-alt': i % 2 === 1 }">
            <td>{{ pago.period }}</td>
            <td>{{ new Date(pago.executedOn).toLocaleDateString('es-CR') }}</td>
            <td><b>${{ pago.totalMoneyPaid.toLocaleString('es-CR') }}</b></td>
          </tr>
          <tr v-if="lastPayrolls.length === 0">
            <td colspan="3">No hay pagos recientes</td>
          </tr>
        </tbody>
      </table>
    </div>
    <div class="dashboard-btn-wrapper">
      <router-link to="/payroll" class="dashboard-btn">
        Ejecutar Planilla
      </router-link>
    </div>
  </section>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import EmployeeCountChart from '@/components/dashboards/EmployeeCountChart.vue'
import PayrollCostPieChart from '@/components/dashboards/PayrollCostPieChart.vue'
import axios from 'axios'

const lastPayrolls = ref([])

onMounted(async () => {
  let companyPk = localStorage.getItem('companyPk')
  if (!companyPk) {
    try {
      const pay = await fetch('/api/login/payroll-info', { credentials: 'include' })
      if (pay.ok) {
        const { companyId } = await pay.json()
        companyPk = companyId
        if (companyPk) localStorage.setItem('companyPk', companyPk)
      }
    } catch {
      //
    }
  }
  if (!companyPk) return
  try {
    const res = await axios.get('/api/owner-dashboard/last-3-payrolls', { params: { companyPk } })
    lastPayrolls.value = res.data
  } catch  {
    lastPayrolls.value = []
  }
})
</script>

<style scoped>
.dashboard-grid {
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 2rem;
}
.charts-row {
  width: 100%;
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  align-items: flex-start;
  gap: 2.5rem;
  margin-bottom: 1.5rem;
}
.ultimos-pagos-wrapper {
  max-width: 520px;
  width: 100%;
  margin: 0 auto 0 auto;
  text-align: center;
}
.ultimos-pagos-title {
  color: #003c63;
  font-size: 1.6rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
}
.ultimos-pagos-table {
  width: 100%;
  border-collapse: collapse;
  background: #fff;
  font-size: 1.08rem;
  margin-bottom: 1.5rem;
}
.ultimos-pagos-table th {
  background: #f3f6f9;
  color: #003c63;
  font-weight: 600;
  padding: 0.5rem 0.7rem;
  text-align: left;
}
.ultimos-pagos-table td {
  padding: 0.5rem 0.7rem;
  color: #003c63;
}
.ultimos-pagos-table .row-alt {
  background: #f3f6f9;
}
.dashboard-btn-wrapper {
  display: flex;
  justify-content: center;
  margin-top: 2rem;
  width: 100%;
  margin-bottom: 3rem;
}
.dashboard-btn {
  background: #003c63;
  color: #fff;
  padding: 0.75rem 2rem;
  border-radius: 8px;
  font-size: 1.1rem;
  font-weight: 600;
  text-decoration: none;
  transition: background 0.2s;
}
.dashboard-btn:hover {
  background: #00508a;
}
</style>
