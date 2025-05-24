<template>
  <header>
    <nav class="navbar navbar-expand-md">
      <div class="container-fluid d-flex justify-content-center">
        <div class="d-flex justify-content-between align-items-center w-70">
          <div class="navbar-brand">
            <img src="@/assets/images/logo.png" alt="Kaizen Logo" class="app-logo" />
          </div>

          <div class="d-flex justify-content-center align-items-center">
            <router-link to="/about" class="nav-link about-link me-4">
              Sobre nosotros
            </router-link>

            <router-link to="/auth/login" class="nav-link login-link ">
              Iniciar sesión
            </router-link>

            <div class="vertical-line mx-3"></div>

            <router-link to="/auth/register-company" class="nav-link btn-kaizen py-2 px-4 rounded">
              Registrar empresa
            </router-link>
          </div>
        </div>
      </div>
    </nav>
  </header>

  <div class="container py-5">
    <div class="row align-items-center mb-5">
      <div class="col-lg-6 text-center text-lg-start">
        <h1 class="display-4 fw-bold">Gestiona tu planilla con Kaizen</h1>
        <p class="lead text-muted" style="color: rgba(0, 60, 99, 0.6) !important;">
          Administra horas, sueldos, deducciones y genera reportes de forma sencilla.
        </p>
        <router-link to="/auth/login">
          <button class="btn btn-kaizen btn-lg mt-3">Comenzar ahora</button>
        </router-link>
      </div>
      <div class="col-lg-6">
        <div class="clock-container">
          <P5Clock />
        </div>
      </div>
    </div>

    <div class="row text-center g-4">
      <div v-for="feature in features" :key="feature.title" class="col-sm-6 col-lg-3">
        <div class="card h-100 shadow-sm border-1">
          <div class="card-body" style="color: #003C63;">
            <img :src="feature.icon" :alt="feature.title" class="mb-3" style="width: 48px;" />
            <h5 class="card-title">{{ feature.title }}</h5>
            <p class="card-text text-muted" style="color: rgba(0, 60, 99, 0.6) !important;">
              {{ feature.description }}
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import P5Clock from '@/components/shared/Clock.vue';
import time from "@/assets/icons/time.svg";
import money from "@/assets/icons/money.svg";
import benefits from "@/assets/icons/benefits.svg";
import reports from "@/assets/icons/reports.svg";

const features = [
  {
    title: "Control de Horas",
    description: "Registra las horas laborales para un cálculo preciso de la planilla.",
    icon: time,
  },
  {
    title: "Gestión Salarial",
    description: "Define sueldos base, bonificaciones y deducciones.",
    icon: money,
  },
  {
    title: "Beneficios",
    description: "Asigna y visualiza los beneficios de cada empleado en tiempo real.",
    icon: benefits,
  },
  {
    title: "Reportes",
    description: "Genera reportes de planilla y descárcalos en PDF o CSV.",
    icon: reports,
  },
];
</script>

<style scoped>
  .app-logo {
    max-height: 54px;
  }

  .navbar {
    padding: 15px 0;
  }

  .nav-link {
    font-weight: 500;
    color: #003C63;
    text-decoration: none;
    display: flex;
    align-items: center;
  }

    .nav-link:hover {
      text-decoration: none;
    }

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

  .vertical-line {
    border-left: 1px solid #003C63;
    height: 45px;
    margin: 0 1rem;
  }

  .w-70 {
    width: 70%;
  }

  .clock-container {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100%;
  }

  .d-flex > .d-flex {
    margin-top: 0;
  }
</style>
