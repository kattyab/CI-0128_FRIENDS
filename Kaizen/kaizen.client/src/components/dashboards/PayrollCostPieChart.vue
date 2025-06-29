<script setup>
import { ref } from 'vue'
import { Pie } from 'vue-chartjs'
import {
  Chart,
  ArcElement,
  Tooltip,
  Legend
} from 'chart.js'

Chart.register(ArcElement, Tooltip, Legend)

const chartData = ref({
  labels: [
    'Cargas Sociales',
    'Salario Tiempo Completo',
    'Servicios Profesionales',
    'Salarios por Horas'
  ],
  datasets: [
    {
      data: [47.6, 19, 9.5, 23.8],
      backgroundColor: [
        '#7de2e2',
        '#0096c7',
        '#43779f',
        '#5fd0e6' // celeste claro en vez de amarillo
      ],
      borderWidth: 1
    }
  ]
})

const chartOptions = {
  responsive: true,
  maintainAspectRatio: true,
  aspectRatio: 1.6,
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
        label: function(ctx) {
          const label = ctx.label || ''
          const value = ctx.parsed || 0
          return `${label}: ${value}%`
        }
      }
    }
  }
}
</script>

<template>
  <div class="pie-chart-container">
    <div class="pie-chart-wrapper">
      <Pie :data="chartData" :options="chartOptions" />
    </div>
  </div>
</template>

<style scoped>
.pie-chart-container {
  width: 100%;
  max-width: 520px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-top: 1rem;
}
.pie-chart-wrapper {
  width: 100%;
  max-width: 520px;
  margin: 0 auto;
}
</style>
