<script setup>
  import { computed } from 'vue'
  import { useRoute } from 'vue-router'
  import Header from '../components/Header.vue'
  import Footer from '../components/Footer.vue'
  import EmpleadoMenu from '../components/menus/empleadoMenu.vue'
  import DuenoMenu from '../components/menus/duenoMenu.vue'
  import SupervisorMenu from '../components/menus/supervisorMenu.vue'
  import SuperAdminMenu from '../components/menus/superAdminMenu.vue'
  import AdminMenu from '../components/menus/adminMenu.vue'

  const route = useRoute()

  const role = computed(() => localStorage.getItem('userRole'))

  const roleMenuMap = {
    'Empleado': EmpleadoMenu,
    'Dueño': DuenoMenu,
    'Supervisor': SupervisorMenu,
    'Superadmin': SuperAdminMenu,
    'Administrador': AdminMenu
  }

  const CurrentMenu = computed(() => roleMenuMap[role.value])
  const showMenu = computed(() => !['/', '/about'].includes(route.path))
</script>

<template>
  <div class="d-flex flex-grow-1 main-content">
    <div class="me-2" v-if="showMenu">
      <component :is="CurrentMenu" />
    </div>
    <main class="flex-grow-1">
      <RouterView />
    </main>
  </div>
  <Footer />
</template>
