<template>
  <footer class="footer mt-auto py-3 bg-white">
    <div class="container">
      <div class="text-center fw-bold fs-2">
        <span>F.R.I.E.N.D.S®</span>
      </div>
      <div class="text-center">
        <span>All Rights Reserved {{ currentYear }}</span>
      </div>
    </div>
  </footer>
</template>

<script>
  export default {
    data() {
      return {
        currentYear: new Date().getFullYear(),
      };
    },
  };
</script>

<style scoped>
  .footer {
    border-top: 1px solid #dee2e6;
  }
</style>
