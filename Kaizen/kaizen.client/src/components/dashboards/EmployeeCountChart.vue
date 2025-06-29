<!-- src/components/dashboards/EmployeeCountChart.vue -->
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
  Legend
} from 'chart.js'

Chart.register(BarElement, CategoryScale, LinearScale, Title, Legend)

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
        boxWidth: 12,
        boxHeight: 12,
        usePointStyle: true,
        padding: 20,
        pointStyle: 'circle',
        color: '#003c63',
        font: { size: 13, family: 'Inter, Roboto, sans-serif' }
      }
    }
  },
  scales: {
    x: {
      grid: { display: false },
      ticks: { color: '#003c63', font: { size: 12 } }
    },
    y: {
      beginAtZero: true,
      suggestedMax: 20,
      ticks: { stepSize: 5, color: '#003c63', font: { size: 12 } },
      grid: { color: '#e2e8f0' }
    }
  },
  elements: { bar: { borderRadius: 4, borderSkipped: false } }
}

function getMonthName(month, year) {
  // Devuelve el nombre del mes en español
  return new Date(year, month - 1, 1).toLocaleString('es-ES', { month: 'long' })
}

onMounted(async () => {
  // Obtén el companyPk dinámicamente igual que en payroll.vue
  let companyPk = localStorage.getItem('companyPk');
  if (!companyPk) {
    // Intenta obtenerlo desde el backend si no está en localStorage
    try {
      const pay = await fetch('/api/login/payroll-info', { credentials: 'include' });
      if (pay.ok) {
        const { companyId } = await pay.json();
        companyPk = companyId;
        if (companyPk) localStorage.setItem('companyPk', companyPk);
      }
    } catch {
      console.error('No se pudo obtener companyPk del backend');
    }
  }
  if (!companyPk) {
    console.error('No se encontró companyPk');
    return;
  }
  const res = await axios.get('/api/owner-dashboard/contract-counts-last-3-months', {
    params: { companyPk }
  })
  const data = res.data
  // Determinar los meses y tipos de contrato únicos
  const months = [...new Set(data.map(d => `${d.year}-${d.month}`))]
  const monthLabels = data.length > 0 ? months.map(m => {
    const [y, mo] = m.split('-')
    return getMonthName(Number(mo), Number(y))
  }) : []
  const contractTypes = [...new Set(data.map(d => d.contractType))]
  // Construir datasets
  const datasets = contractTypes.map((type, idx) => {
    const colorList = ['#7de2e2', '#0096c7', '#43779f', '#fbbf24', '#ef4444']
    return {
      label: type,
      data: months.map(m => {
        const found = data.find(d => d.contractType === type && `${d.year}-${d.month}` === m)
        return found ? found.count : 0
      }),
      backgroundColor: colorList[idx % colorList.length]
    }
  })
  chartData.value = { labels: monthLabels, datasets }
})
</script>

<template>
  <!-- wrapper limita el ancho y centra la gráfica -->
  <div class="chart-wrapper">
    <Bar :data="chartData" :options="chartOptions" />
  </div>
</template>

<style scoped>
.chart-wrapper {
  width: 100%;
  max-width: 520px;   /* ajusta este valor para más/menos anchura */
  margin: 0 auto;     /* centrado horizontal */
}

/* Si quisieras fijar alto exacto: descomenta ↓ y elimina aspectRatio
.employee-chart {
  height: 260px;
}
*/
</style>
