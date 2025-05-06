<template>
      <employee-detail/>
</template>

<script setup>
  import EmployeeDetail from '../components/EmployeeDetail.vue'
</script>

<style scoped>
    .main-content {
        display: flex;
        justify-content: flex-start;
        gap: 1rem;
        width: 100%;
    }

  .employee-detail-wrapper {
    flex-grow: 1;
    display: flex;
    justify-content: center;
    align-items: center;
    overflow-x: auto;
    padding: 20px;
  }

  .employee-detail {
    min-width: max-content;
  }
</style>
