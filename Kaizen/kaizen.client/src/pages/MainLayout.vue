<!-- layouts/MainLayout.vue -->
<script setup>
import Header         from '../components/Header.vue'
import Footer         from '../components/Footer.vue'
import EmpleadoMenu   from '../components/menus/empleadoMenu.vue'
import DuenoMenu      from '../components/menus/duenoMenu.vue'
import SupervisorMenu from '../components/menus/supervisorMenu.vue'
import SuperAdminMenu from '../components/menus/superAdminMenu.vue'
import AdminMenu from '../components/menus/adminMenu.vue'
import RegisterEmployee from '../components/RegisterEmployee.vue'
import { computed } from 'vue'
import { useRoute }  from 'vue-router'

const route = useRoute()
const role  = computed(() => localStorage.getItem('role') ?? 'Empleado')
</script>

<template>
  <!--  <Header />  -->
  <div class="d-flex flex-grow-1 main-content">
    <div class="me-2">
      <EmpleadoMenu v-if="role === 'Empleado'" />
      <DuenoMenu v-else-if="role === 'Dueño'" />
      <SupervisorMenu v-else-if="role === 'Supervisor'" />
      <SuperAdminMenu v-else-if="role === 'SuperAdmin'" />
      <AdminMenu v-else-if="role === 'Administrador'" />
    </div>

    <main class="flex-grow-1">
      <RouterView />
    </main>
  </div>
  <Footer />
</template>
