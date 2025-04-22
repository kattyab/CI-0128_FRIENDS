<script setup>
import Header from './components/Header.vue';
import Footer from './components/Footer.vue';
</script>

<template>
  <Header v-if="$route.meta.hide_header !== true"></Header>
  <main>
    <RouterView />
  </main>
  <Footer v-if="$route.meta.hide_footer !== true"></Footer>

  <main>
    <TheWelcome />
  </main>
</template>

<style>
  .app {
    font-family: 'DM Sans', sans-serif;
  }
</style>
