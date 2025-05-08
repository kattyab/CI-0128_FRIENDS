<template>
  <section class="text-center p-5">
    <h1 class="display-4 fw-bold">Acerca de esta aplicación</h1>
    <p class="fs-5 mt-3">
      Esta es una aplicación desarrollada para gestionar planillas de empleados, revisión de horas, registro de empresas y empleados.
    </p>
    <p class="text-muted">
      Desarrollada con el poder de la amistad usando Vue.js, ASP.NET Core y SQL Server.
    </p>
  </section>
</template>

<script>
export default {
  name: "AboutPage",
};
</script>

<style scoped>
  section {
    background-color: #f0f2f5;
    min-height: 100vh;
    display: flex;
    flex-direction: column;
    justify-content: center;
  }
</style>
