<template>
  <section class="text-center p-5">
    <h1 class="display-1 fw-bold">404</h1>
    <p class="fs-4">Uy... esta página está más perdida que yo un lunes a las 7 a.m. recibiendo clases de bases de datos.</p>
    <p class="text-muted">Spoiler: tampoco entendí el Trigger.</p>

    <!-- Imagen fija centrada -->
    <img src="@/assets/images/404.jpeg"
         alt="404 divertido"
         class="img-fluid my-4 centered-image" />

    <!-- Botón de regreso -->
    <router-link to="/" class="btn btn-outline-primary mt-3">
      Volver al inicio (¡con café en mano!)
    </router-link>
  </section>
</template>

<script>
  export default {
    name: "Not Found",
  };
</script>

<style scoped>
  section {
    background-color: #f8f9fa;
    min-height: 100vh;
    display: flex;
    flex-direction: column;
    justify-content: center;
  }

  .centered-image {
    max-width: 300px;
    display: block;
    margin-left: auto;
    margin-right: auto;
  }
</style>
