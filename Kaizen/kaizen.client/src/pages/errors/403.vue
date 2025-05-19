<template>
  <div class="container py-5 custom-bg text-center">
    <div class="row justify-content-center">
      <div class="col-lg-8">
        <img src="@/assets/images/403.png" alt="Acceso denegado" class="img-fluid mb-4" style="max-height: 550px;" />
        <h1 class="display-5 fw-bold">Acceso denegado</h1>
        <p class="lead text-muted" style="color: rgba(0, 60, 99, 0.6) !important;">
          No tienes los permisos necesarios para acceder a esta página.
        </p>
        <router-link to="/landing-page">
          <button class="btn btn-kaizen btn-lg mt-3">Regresar</button>
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
</script>

<style scoped>
  .btn-kaizen {
    background-color: #003C63;
    color: #fff;
    border: none;
  }

    .btn-kaizen:hover,
    .btn-kaizen:focus {
      background-color: #002944;
      color: #fff;
    }
</style>
