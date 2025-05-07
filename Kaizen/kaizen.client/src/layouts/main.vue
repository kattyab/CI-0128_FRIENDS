<script setup>
  import { ref, computed, onMounted, onUnmounted } from 'vue'
  import { useRoute } from 'vue-router'
  import axios from 'axios'

  import Header from '../components/Header.vue'
  import Footer from '../components/Footer.vue'
  import EmpleadoMenu from '../components/menus/empleadoMenu.vue'
  import DuenoMenu from '../components/menus/duenoMenu.vue'
  import SupervisorMenu from '../components/menus/supervisorMenu.vue'
  import SuperAdminMenu from '../components/menus/superAdminMenu.vue'
  import AdminMenu from '../components/menus/adminMenu.vue'

  const route = useRoute()
  const role = ref(null)
  const isSidebarOpen = ref(false)

  const roleMenuMap = {
    'Empleado': EmpleadoMenu,
    'Dueño': DuenoMenu,
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
  <button class="hamburger-toggle d-md-none" v-if="shouldRenderHeaderAndHamburger" @click="toggleSidebar">
    ☰
  </button>

  <Header v-if="shouldRenderHeaderAndHamburger" />

  <div class="d-flex flex-column min-vh-100 layout-container">
    <div class="d-flex flex-grow-1 main-content">
      <div v-if="showMenu" :class="['sidebar', { 'sidebar-open': isSidebarOpen }]">
        <component :is="CurrentMenu" />
      </div>

      <div v-if="isSidebarOpen" class="overlay d-md-none" @click="closeSidebar"></div>

      <main class="flex-grow-1 content-area">
        <RouterView />
      </main>
    </div>

    <Footer />
  </div>
</template>

<style scoped>
  .layout-container {
    min-height: 100vh;
  }

  .sidebar {
    width: 250px;
    min-width: 250px;
    transition: transform 0.3s ease, background-color 0.3s ease;
    height: 100vh; 
  }

    .sidebar.sidebar-open {
      background-color: #f8f9fa;
    }

  .main-content {
    display: flex;
    flex-grow: 1;
    min-height: 100vh;
  }

  .content-area {
    flex-grow: 1;
    padding: 1rem;
    overflow-y: auto;
  }

  @media (max-width: 768px) {
    .sidebar {
      position: absolute;
      top: 0;
      left: 0;
      height: 100vh;
      transform: translateX(-100%);
      z-index: 1050;
      transition: transform 0.3s ease, background-color 0.3s ease;
    }

      .sidebar.sidebar-open {
        transform: translateX(0);
        background-color: #f8f9fa;
      }

    .overlay {
      position: fixed;
      top: 0;
      left: 0;
      width: 100vw;
      height: 100vh;
      background: rgba(0, 0, 0, 0.3);
      z-index: 1040;
    }

    .hamburger-toggle {
      position: fixed;
      top: 18px;
      left: 12px;
      z-index: 1060;
      font-size: 1.5rem;
      padding: 0.25rem 0.75rem;
      background-color: #003c63;
      color: white;
      border: none;
      border-radius: 4px;
    }
  }
</style>
