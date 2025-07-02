<script setup>
import { ref, onMounted } from 'vue'
import { Pie } from 'vue-chartjs'
import axios from 'axios'
import {
  Chart,
  ArcElement,
  Tooltip,
  Legend
} from 'chart.js'

Chart.register(ArcElement, Tooltip, Legend)

const chartData = ref({
  labels: ['Beneficios', 'Deducciones Obligatorias', 'Cargas Sociales', 'Salarios'],
  datasets: [{
    data: [0, 0, 0, 0],
    backgroundColor: ['#7de2e2', '#0096c7', '#43779f', '#5fd0e6'],
    borderWidth: 1
  }]
})

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  layout: { padding: { top: 10 } },
  plugins: {
    title: {
      display: true,
      text: 'Costo Planilla',
      align: 'start',
      color: '#003c63',
      font: { size: 26, weight: 700, family: 'Inter, Roboto, sans-serif' },
      padding: { bottom: 0 }
    },
    legend: {
      display: true,
      position: 'top',
      align: 'center',
      labels: {
        boxWidth: 12,
        boxHeight: 12,
        usePointStyle: true,
        padding: 20,
        pointStyle: 'circle',
        color: '#003c63',
        font: { size: 13, family: 'Inter, Roboto, sans-serif' }
      }
    },
    tooltip: {
      callbacks: {
        label: ({ label = '', parsed = 0 }) =>
          `${label}: ₡${parsed.toLocaleString('es-CR', { maximumFractionDigits: 0 })}`
      }
    }
  }
}

const toNumber = v => Number.parseFloat(v) || 0
const sanitize = arr => arr.every(v => v === 0) ? [1, 1, 1, 1] : arr

onMounted(async () => {
  let companyPk = localStorage.getItem('companyPk')
  if (!companyPk) {
    try {
      const res = await fetch('/api/login/payroll-info', { credentials: 'include' })
      if (res.ok) {
        const { companyId } = await res.json()
        companyPk = companyId
        if (companyPk) localStorage.setItem('companyPk', companyPk)
      }
    } catch (err) {
      console.error('No se pudo obtener companyPk del backend', err)
    }
  }
  if (!companyPk) return console.warn('Sin companyPk — se aborta el fetch')

  try {
    const { data } = await axios.get(
      '/api/owner-dashboard/payroll-cost-breakdown',
      { params: { companyPk } }
    )
    const dataset = sanitize([
      toNumber(data.benefits),
      toNumber(data.obligatoryDeductions),
      toNumber(data.laborCharges),
      toNumber(data.salaries)
    ])
    chartData.value = {
      ...chartData.value,
      datasets: [{ ...chartData.value.datasets[0], data: dataset }]
    }
  } catch (err) {
    console.error('Error fetching payroll-cost-breakdown:', err)
    chartData.value = {
      ...chartData.value,
      datasets: [{ ...chartData.value.datasets[0], data: [1, 1, 1, 1] }]
    }
  }
})
</script>

<template>
  <div class="pie-chart-container">
    <Pie :data="chartData" :options="chartOptions" />
  </div>
</template>

<style scoped>
.pie-chart-container {
  width: 100%;
  max-width: 650px;
  margin: 1rem auto 0;
  height: 340px;
  display: flex;
  align-items: center;
}
</style>
