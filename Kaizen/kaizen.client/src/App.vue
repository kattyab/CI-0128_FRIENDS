<script setup>
  import Header from './components/Header.vue';
  import Footer from './components/Footer.vue';
  import employeeMenu from './components/employeeMenu.vue';
  import employerMenu from './components/employerMenu.vue';
  import { ref } from 'vue'

  // Hard coded role for testing
  localStorage.setItem('role', 'employer') // or 'employer'
  const role = ref(localStorage.getItem('role') || 'employee')
</script>

<template>
  <Header v-if="$route.meta.hide_header !== true"></Header>
  <main class="app">
    <employeeMenu v-if="role === 'employee'" />
    <employerMenu v-else-if="role === 'employer'" />
    <RouterView />
  </main>
  <Footer v-if="$route.meta.hide_footer !== true"></Footer>
</template>
