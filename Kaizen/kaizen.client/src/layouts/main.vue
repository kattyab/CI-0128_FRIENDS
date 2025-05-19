<script setup>
  import { ref, computed, onMounted, onUnmounted } from 'vue'
  import { useRoute } from 'vue-router'
  import axios from 'axios'

  import Header from '@/components/shared/Header.vue'
  import Footer from '@/components/shared/Footer.vue'
  import EmpleadoMenu from '@/components/menus/employee.vue'
  import OwnerMenu from '@/components/menus/owner.vue'
  import SupervisorMenu from '@/components/menus/supervisor.vue'
  import SuperAdminMenu from '@/components/menus/superAdmin.vue'
  import AdminMenu from '@/components/menus/admin.vue'

  const route = useRoute()
  const role = ref(null)
  const isSidebarOpen = ref(false)

  const roleMenuMap = {
    'Empleado': EmpleadoMenu,
    'Dueño': OwnerMenu,
    'Supervisor': SupervisorMenu,
    'Superadmin': SuperAdminMenu,
    'Administrador': AdminMenu
  }

  const CurrentMenu = computed(() => roleMenuMap[role.value] || null)
  const showMenu = computed(() => !['/', '/about'].includes(route.path))

  const shouldRenderHeaderAndHamburger = computed(() => route.path !== '/')

  const toggleSidebar = () => {
    isSidebarOpen.value = !isSidebarOpen.value
  }

  const closeSidebar = () => {
    isSidebarOpen.value = false
  }

  const handleResize = () => {
    if (window.innerWidth > 768) {
      closeSidebar()
    }
  }

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

    window.addEventListener('resize', handleResize)
  })

  onUnmounted(() => {
    window.removeEventListener('resize', handleResize)
  })
</script>

<template>

  <Header v-if="shouldRenderHeaderAndHamburger" />

  <div class="d-flex flex-column min-vh-100">
    <div class="d-flex flex-grow-1">
      <div v-if="showMenu">
        <component :is="CurrentMenu" />
      </div>

      <main class="flex-grow-1">
        <RouterView />
      </main>
    </div>

    <Footer />
  </div>
</template>
