<template>
  <div style="height: 250px;">
    <Bar :data="chartData" :options="chartOptions" />
  </div>
</template>

<script>import { Bar } from 'vue-chartjs'
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
} from 'chart.js'

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale)

export default {
  name: 'SalaryHistoryChart',
  components: {
    Bar,
  },
  props: {
    salaries: {
      type: Array,
      required: true,
    },
  },
  computed: {
    chartData() {
      const labels = ['Pago 1', 'Pago 2', 'Pago 3']
      const grossData = this.salaries.map(s => s.gross)
      const netData = this.salaries.map(s => s.net)

      return {
        labels,
        datasets: [
          {
            label: 'Salario Bruto',
            backgroundColor: '#4caf50',
            data: grossData,
            barThickness: 20,
            barPercentage: 0.9,
            categoryPercentage: 0.4,
          },
          {
            label: 'Salario Neto',
            backgroundColor: '#2196f3',
            data: netData,
            barThickness: 20,
            barPercentage: 0.9,
            categoryPercentage: 0.4,
          },
        ],
      }
    },
    chartOptions() {
      return {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { position: 'top' },
          title: {
            display: true,
            text: 'Historial de Pagos (Bruto vs Neto)',
          },
        },
        scales: {
          y: {
            beginAtZero: true,
            ticks: {
              callback: value => `₡${value.toLocaleString()}`,
            },
          },
        },
      }
    },
  },
}</script>
