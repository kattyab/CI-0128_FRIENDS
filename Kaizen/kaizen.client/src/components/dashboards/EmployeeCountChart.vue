
<script setup>
import { ref, onMounted } from 'vue'
import { Bar } from 'vue-chartjs'
import axios from 'axios'
import {
  Chart,
  BarElement,
  CategoryScale,
  LinearScale,
  Title,
  Legend,
  Tooltip
} from 'chart.js'

Chart.register(BarElement, CategoryScale, LinearScale, Title, Legend, Tooltip)

const chartData = ref({ labels: [], datasets: [] })

const chartOptions = {
  responsive: true,
  maintainAspectRatio: true,
  aspectRatio: 1.6,
  layout: { padding: { top: 10 } },
  plugins: {
    title: {
      display: true,
      text: 'Cantidad de Empleados',
      align: 'start',
      color: '#003c63',
      font: { size: 26, weight: 700, family: 'Inter, Roboto, sans-serif' }
    },
    legend: {
      display: true,
      position: 'top',
      align: 'center',
      labels: {
        boxWidth: 12, boxHeight: 12, usePointStyle: true, padding: 20,
        pointStyle: 'circle', color: '#003c63',
        font: { size: 13, family: 'Inter, Roboto, sans-serif' }
      }
    },
    tooltip: {
      enabled: true,
      mode: 'index',
      intersect: false,
      callbacks: {
        label(ctx) {
          return `${ctx.dataset.label}: ${ctx.parsed.y} empleados`
        },
        footer(ctx) {
          if (!ctx?.length) return ''
          const i = ctx[0].dataIndex
          const total = ctx[0].chart.data.datasets
            .reduce((s, ds) => s + (ds.data[i] ?? 0), 0)
          return `Total empleados: ${total}`
        }
      }
    }
  },
  scales: {
    x: { grid: { display: false }, ticks: { color: '#003c63', font: { size: 12 } } },
    y: {
      beginAtZero: true, suggestedMax: 20,
      ticks: { stepSize: 5, color: '#003c63', font: { size: 12 } },
      grid: { color: '#e2e8f0' }
    }
  },
  elements: { bar: { borderRadius: 4, borderSkipped: false } }
}

const monthName = (m, y) =>
  new Date(y, m - 1, 1).toLocaleString('es-ES', { month: 'long' })

onMounted(async () => {
  let companyPk = localStorage.getItem('companyPk')
  if (!companyPk) {
    try {
      const r = await fetch('/api/login/payroll-info', { credentials: 'include' })
      if (r.ok) {
        companyPk = (await r.json()).companyId
        if (companyPk) localStorage.setItem('companyPk', companyPk)
      }
    } catch {
    }
  }
  if (!companyPk) return console.error('companyPk no encontrado')

  const { data } = await axios.get(
    '/api/owner-dashboard/contract-counts-last-3-months',
    { params: { companyPk } }
  )

  const months = [...new Set(data.map(d => `${d.year}-${d.month}`))]
  const labels = months.map(m => {
    const [y, mo] = m.split('-'); return monthName(+mo, +y)
  })
  const types = [...new Set(data.map(d => d.contractType))]
  const colors = ['#7de2e2','#0096c7','#43779f','#5fd0e6','#ef4444']
  const datasets = types.map((t,i) => ({
    label: t,
    data: months.map(m => {
      const f = data.find(d => d.contractType===t && `${d.year}-${d.month}`===m)
      return f ? f.count : 0
    }),
    backgroundColor: colors[i % colors.length]
  }))

  chartData.value = { labels, datasets }
})
</script>

<template>
  <div class="chart-wrapper">
    <Bar :data="chartData" :options="chartOptions" />
  </div>
</template>

<style scoped>
.chart-wrapper {
  width: 100%;
  max-width: 520px;
  margin: 0 auto;
  margin-top: 1rem;
}
</style>
