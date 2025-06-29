<template>
  <section class="dashboard-grid">
    <EmployeeCountChart />
    <!-- aquí irán los demás gráficos / tablas -->
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
          <tr>
            <td>Mayo</td>
            <td>01/06/2025</td>
            <td><b>$10 000</b></td>
          </tr>
          <tr class="row-alt">
            <td>Abril</td>
            <td>01/05/2025</td>
            <td><b>$9 800</b></td>
          </tr>
          <tr>
            <td>Marzo</td>
            <td>01/04/2025</td>
            <td><b>$9 700</b></td>
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
import EmployeeCountChart from '@/components/dashboards/EmployeeCountChart.vue'
</script>

<style scoped>
.dashboard-btn-wrapper {
  display: flex;
  justify-content: center;
  margin-top: 2rem;
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
.ultimos-pagos-wrapper {
  max-width: 420px;
  margin: 2rem auto 0 auto;
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
</style>
