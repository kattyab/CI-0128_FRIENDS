<template>
  <section class="dashboard-grid">
    <EmployeeCountChart />
    <!-- aquí irán los demás gráficos / tablas -->
    <div class="dashboard-btn-wrapper">
      <router-link to="/payroll" class="dashboard-btn">
        Ir a Pago de Planilla
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
</style>
