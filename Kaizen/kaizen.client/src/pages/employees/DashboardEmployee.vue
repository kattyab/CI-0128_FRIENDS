<template>
  <div class="contenedor-principal">
    <h1 class="text-2xl font-bold mb-4 text-center">Dashboard Empleado</h1>
    <div v-if="salaries.length > 0">
      <div class="mb-4">
        <p class="h4">{{ employee.name }}</p>
        <div class="row px-3">
          <div class="col text-start">{{ employee.contractType }}</div>
          <div class="col text-center">Fecha inicio: {{ employee.startDate }}</div>
          <div class="col text-end">{{ employee.role }}</div>
        </div>
      </div>




      <div class="row g-4">

        <div class="col-md-6">
          <div class="bg-light p-4 rounded shadow">
            <h2 class="h5 mb-3">Beneficios Inscritos</h2>
            <div v-if="benefits.length > 0" class="d-flex flex-wrap gap-2">
              <span v-for="benefit in benefits"
                    :key="benefit.name"
                    class="badge bg-success text-light p-2">
                {{ benefit.name }}: ₡{{ benefit.cost.toLocaleString() }}
              </span>
            </div>
            <p v-else class="text-muted fst-italic">No se han seleccionado beneficios</p>
            <div class="mt-3 fw-bold">Total: ₡{{ totalBenefits.toLocaleString() }}</div>
          </div>
        </div>


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




      <div class="row g-4 mt-4 mb-6">


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

      <hr class="my-8 border-t border-gray-300" />


      <div class="row g-4">

        <div class="grid grid-cols-2 gap-4">

          <div>
            <p class="h4 text-start mb-2">Distribución Salarial</p>
            <SalaryChart :gross="salary.gross" :deductions="totalBenefits + totalDeductions" />
          </div>

          <hr class="my-8 border-t border-gray-300" />


          <div>
            <p class="h4 text-start mb-2">Historial Salarial</p>
            <SalaryHistoryChart :salaries="salaries" />
          </div>
        </div>
      </div>
    </div>
    <div v-else class="text-center mt-6">
      <p class="text-danger fw-bold fs-5">No hay planillas recientes</p>
    </div>
  </div>
</template>

<script>
  import axios from 'axios';
  import SalaryChart from '../../components/charts/SalaryChart.vue';
  import SalaryHistoryChart from '../../components/charts/SalaryHistoryChart.vue';

  export default {
    name: 'EmployeeDashboard',

    components: {
      SalaryChart,
      SalaryHistoryChart,
    },

    data() {
      return {
        employee: {
          name: '',
          contractType: '',
          startDate: '',
          role: '',
        },
        benefits: [],
        deductions: [],
        salary: {
          gross: 0,
          net: 0,
        },
        salaries: [],

        userPK: null,
        empID: null,
        fechaInicioTrabajo: '',
        nombreEmpleado: '',
        apellidoEmpleado: '',
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

    methods: {
      formatDate(isoDate) {
        const fecha = new Date(isoDate);
        const dia = String(fecha.getDate()).padStart(2, '0');
        const mes = String(fecha.getMonth() + 1).padStart(2, '0');
        const anio = fecha.getFullYear();
        return `${dia}-${mes}-${anio}`;
      },

      async getEmployeeDashboard() {
        try {
          const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/EmployeeDashboard`);
          const data = response.data;


          this.userPK = data.userPK;
          this.empID = data.empID;
          this.fechaInicioTrabajo = this.formatDate(data.startDate);

          this.nombreEmpleado = data.name;
          this.apellidoEmpleado = data.lastName;

          this.employee.name = `${data.name} ${data.lastName}`;
          this.employee.contractType = data.contractType;
          this.employee.startDate = this.fechaInicioTrabajo;
          this.employee.role = data.jobPosition;


          const latestPayroll = data.recentPayrolls[0];
          if (latestPayroll) {
            this.salary.gross = latestPayroll.brutePaid;
            this.salary.net = latestPayroll.netPaid;
          }


          this.salaries = data.recentPayrolls.map(p => ({
            gross: p.brutePaid,
            net: p.netPaid,
          }));


          this.benefits = data.optionalDeductions.map(od => ({
            name: od.optionalDeductionName,
            cost: od.optionalDeductionAmount,
          }));

          if (latestPayroll) {
            this.deductions = [
              { name: 'Renta', cost: latestPayroll.incomeTax },
              { name: 'CCSS', cost: latestPayroll.ccss },
            ];
          }

        } catch (error) {
          console.error("Error al obtener el dashboard del empleado:", error);
        }
      },
    },

    mounted() {
      this.getEmployeeDashboard();
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
