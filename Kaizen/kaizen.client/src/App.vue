<script setup>
  import Header from './components/Header.vue';
  import Footer from './components/Footer.vue';
  import employeeMenu from './components/employeeMenu.vue';
</script>

<template>
  <Header v-if="$route.meta.hide_header !== true"></Header>
  <main class="app">
    <employeeMenu />
    <RouterView />
  </main>
  <Footer v-if="$route.meta.hide_footer !== true"></Footer>
</template>
