<template>
  <div class="contenedor-principal">
    <h1 class="text-2xl font-bold mb-4 text-center">Dashboard Empleado</h1>

    <div class="mb-4">
      <p class="h4">{{ employee.name }}</p>
      <div class="row px-3">
        <div class="col text-start">{{ employee.contractType }}</div>
        <div class="col text-center">Fecha inicio: {{ employee.startDate }}</div>
        <div class="col text-center">Tiempo inscrito: {{ employee.subscriptionDuration }}</div>
        <div class="col text-end">{{ employee.role }}</div>
      </div>
    </div>




    <div class="row g-4">
      <!-- Beneficios -->
      <div class="col-md-6">
        <div class="bg-light p-4 rounded shadow">
          <h2 class="h5 mb-3">Beneficios Inscritos</h2>
          <div class="d-flex flex-wrap gap-2">
            <span v-for="benefit in benefits"
                  :key="benefit.name"
                  class="badge bg-success text-light p-2">
              {{ benefit.name }}: ₡{{ benefit.cost.toLocaleString() }}
            </span>
          </div>
          <div class="mt-3 fw-bold">Total: ₡{{ totalBenefits.toLocaleString() }}</div>
        </div>
      </div>

      <!-- Deducciones -->
      <div class="col-md-6">
        <div class="bg-light p-4 rounded shadow">
          <h2 class="h5 mb-3">Deducciones</h2>
          <div class="d-flex flex-wrap gap-2">
            <span v-for="deduction in deductions"
                  :key="deduction.name"
                  class="badge bg-danger text-light p-2">
              {{ deduction.name }}: ₡{{ deduction.cost.toLocaleString() }}
            </span>
          </div>
          <div class="mt-3 fw-bold">Total: ₡{{ totalDeductions.toLocaleString() }}</div>
        </div>
      </div>
    </div>



    <div class="row g-4 mt-4">
      <!-- Tarjeta izquierda: Rebajos -->
      <div class="col-md-6">
        <div class="bg-light p-4 rounded shadow">
          <p class="fw-semibold mb-3">Resumen de Rebajos</p>

          <div class="d-flex justify-content-between mb-2">
            <span>Porcentaje Retenido:</span>
            <span class="text-danger">
              {{ ((totalBenefits + totalDeductions) / salary.gross * 100).toFixed(2) }}%
            </span>
          </div>

          <div class="d-flex justify-content-between">
            <span>Total Rebajos:</span>
            <span>₡{{ (totalBenefits + totalDeductions).toLocaleString() }}</span>
          </div>
        </div>
      </div>

      <!-- Tarjeta derecha: Salarios -->
      <div class="col-md-6">
        <div class="bg-light p-4 rounded shadow">
          <p class="fw-semibold mb-3">Resumen Salarial</p>

          <div class="d-flex justify-content-between mb-2">
            <span>Salario Bruto:</span>
            <span>₡{{ salary.gross.toLocaleString() }}</span>
          </div>

          <div class="d-flex justify-content-between">
            <span>Salario Neto:</span>
            <span>₡{{ salary.net.toLocaleString() }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Link al detalle 
    <div class="text-center mt-3">
      <router-link to="/detalle" class="text-decoration-underline text-primary">
        Ir al detalle
      </router-link>
    </div>-->




    <div class="grid grid-cols-2 gap-4 mt-6">
      <!-- Gráfico horizontal existente -->
      <SalaryChart :gross="salary.gross" :deductions="totalBenefits + totalDeductions" />

      <!-- Nuevo gráfico de historial -->
      <SalaryHistoryChart :salaries="salaries" />
    </div>



  </div>
</template>

<script>
  import SalaryChart from '../../components/charts/SalaryChart.vue'
  import SalaryHistoryChart from '../../components/charts/SalaryHistoryChart.vue'

  export default {
    name: 'EmployeeDashboard',

    components: {
      SalaryChart,
      SalaryHistoryChart,
    },

    data() {
      return {
        employee: {
          name: 'Erick S.',
          contractType: 'Tiempo Completo',
          startDate: '07/01/2025',
          subscriptionDuration: '8 meses',
          role: 'Drip Master',
        },
        benefits: [
          { name: 'GYM', cost: 5000 },
          { name: 'HBO', cost: 5000 },
        ],
        deductions: [
          { name: 'Renta', cost: 5000 },
          { name: 'CCSS', cost: 5000 },
        ],
        salary: {
          gross: 100000,
          net: 100000,
        },
        salaries: [
          { gross: 100000, net: 80000 },
          { gross: 100000, net: 80000 },
          { gross: 100000, net: 80000 },
        ],
      };
    },

    computed: {
      totalBenefits() {
        return this.benefits.reduce((sum, b) => sum + b.cost, 0);
      },
      totalDeductions() {
        return this.deductions.reduce((sum, d) => sum + d.cost, 0);
      },
    },
  };
</script>


<style scoped>
  ul {
    list-style: none;
    padding: 0;
  }

  .employee-details span {
    text-align: center;
  }

    .employee-details span:first-child {
      text-align: left;
    }

    .employee-details span:last-child {
      text-align: right;
    }

  .contenedor-principal {
    max-width: 1500px;
    margin: 0 auto;
    padding: 25px;
  }
</style>
