<script setup>
  import { ref, computed, onMounted } from 'vue'
  import { useRoute } from 'vue-router'
  import axios from 'axios'

  import Footer from '../components/Footer.vue'
  import EmpleadoMenu from '../components/menus/empleadoMenu.vue'
  import DuenoMenu from '../components/menus/duenoMenu.vue'
  import SupervisorMenu from '../components/menus/supervisorMenu.vue'
  import SuperAdminMenu from '../components/menus/superAdminMenu.vue'
  import AdminMenu from '../components/menus/adminMenu.vue'

  const route = useRoute()
  const role = ref(null)

  const roleMenuMap = {
    'Empleado': EmpleadoMenu,
    'Dueño': DuenoMenu,
    'Supervisor': SupervisorMenu,
    'Superadmin': SuperAdminMenu,
    'Administrador': AdminMenu
  }

  const CurrentMenu = computed(() => roleMenuMap[role.value] || null)
  const showMenu = computed(() => !['/', '/about'].includes(route.path))

  onMounted(async () => {
    try {
      const { data } = await axios.get('https://localhost:7153/api/Login/authenticate', {
        withCredentials: true
      })
      role.value = data.role
    } catch (error) {
      console.error('Failed to fetch user role:', error)
      role.value = null
    }
  })
</script>

<template>
  <div class="d-flex flex-grow-1 main-content">
    <div class="me-2" v-if="showMenu && CurrentMenu">
      <component :is="CurrentMenu" />
    </div>
    <main class="flex-grow-1">
      <RouterView />
    </main>
  </div>
  <Footer />
</template>
