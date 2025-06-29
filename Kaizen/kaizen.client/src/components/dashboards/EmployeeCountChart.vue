<!-- src/components/dashboards/EmployeeCountChart.vue -->
<script setup>
import { Bar } from 'vue-chartjs'
import {
  Chart,
  BarElement,
  CategoryScale,
  LinearScale,
  Title,
  Legend
} from 'chart.js'

Chart.register(BarElement, CategoryScale, LinearScale, Title, Legend)

/* ── DATOS (ejemplo) ── */
const chartData = {
  labels: ['Mayo', 'Abril', 'Marzo'],
  datasets: [
    { label: 'Tiempo completo', data: [3, 8, 16], backgroundColor: '#7de2e2' },
    { label: 'Horas',           data: [6, 14, 18], backgroundColor: '#0096c7' },
    { label: 'Servicios Profesionales', data: [5, 6, 6], backgroundColor: '#43779f' }
  ]
}

/* ── OPCIONES ── */
const chartOptions = {
  responsive: true,
  maintainAspectRatio: true,  // respeta aspectRatio
  aspectRatio: 1.6,           // ancho = 1.6 × alto
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
