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
</template>
