<template>
  <div class="app flex">
    <employee-detail class="flex-grow" />
  </div>
</template>

<script>
  import EmployeeDetail from './components/EmployeeDetail.vue';

  export default {
    name: 'App',
    components: {
      EmployeeDetail,
    }
  }
</script>

<style>
  html, body {
    margin: 0;
    padding: 0;
    height: 100%;
    width: 100%;
  }

  .app {
    display: flex;
    flex-direction: column;
    min-height: 100vh;
    width: 100vw;
    overflow: auto;
  }


  .main-content {
    display: flex;
    flex-grow: 1;
    width: 100%;
  }

  * {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
    font-family: Arial, sans-serif;
  }
</style>

