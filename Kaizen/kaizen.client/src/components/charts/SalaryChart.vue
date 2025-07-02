<template>
  <div class="w-full">
    <Bar :data="chartData" :options="chartOptions" />
  </div>
</template>

<script setup>
  import { computed } from 'vue'
  import { Bar } from 'vue-chartjs'
  import {
    Chart as ChartJS,
    BarElement,
    CategoryScale,
    LinearScale,
    Tooltip,
    Legend
  } from 'chart.js'

  ChartJS.register(BarElement, CategoryScale, LinearScale, Tooltip, Legend)

  const props = defineProps({
    gross: Number,
    deductions: Number
  })


  const chartData = computed(() => ({
    labels: ['Salario Bruto', 'Total Rebajos'],
    datasets: [
      {
        label: 'Colones',
        data: [props.gross, props.deductions],
        backgroundColor: ['#4caf50', '#f44336'],
        barThickness: 200,
        barPercentage: 1,
        categoryPercentage: 1,
      }
    ]
  }))

  const chartOptions = {
    indexAxis: 'y',
    responsive: true,
    plugins: {
      legend: { display: false },
      tooltip: { enabled: true }
    },
    scales: {
      x: { beginAtZero: true },
      y: {
        ticks: {
          font: { size: 14 }
        }
      }
    }
  }
</script>
