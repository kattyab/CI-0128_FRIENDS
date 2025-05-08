<template>
  <footer class="footer mt-auto" style="background-color: #003c63; color: white; padding: 30px 0;">
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
    margin-top: auto;
    border-top: 1px solid #dee2e6;
  }
</style>
