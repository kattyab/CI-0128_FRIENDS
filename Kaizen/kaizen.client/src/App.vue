<script setup>
  import Header from './components/Header.vue';
  import Footer from './components/Footer.vue';
  import empleadoMenu from './components/menus/empleadoMenu.vue';
  import duenoMenu from './components/menus/duenoMenu.vue';
  import supervisorMenu from './components/menus/supervisorMenu.vue';
  import superAdminMenu from './components/menus/superAdminMenu.vue';
  import adminMenu from './components/menus/adminMenu.vue';
  import { ref } from 'vue'

  // Hard coded role for testing
  localStorage.setItem('role', 'Empleado') // or 'employer'
  const role = ref(localStorage.getItem('role') || 'Empleado')

</script>

<template>
  <Header v-if="$route.meta.hide_header !== true"></Header>
  <main class="app">
    <empleadoMenu v-if="role === 'Empleado'" />
    <duenoMenu v-else-if="role === 'Dueño'" />
    <supervisorMenu v-else-if="role === 'Supervisor'" />
    <superAdminMenu v-else-if="role === 'SuperAdmin'" />
    <adminMenu v-else-if="role === 'Administrador'" />
    <RouterView />
  </main>
  <Footer v-if="$route.meta.hide_footer !== true"></Footer>
</template>
