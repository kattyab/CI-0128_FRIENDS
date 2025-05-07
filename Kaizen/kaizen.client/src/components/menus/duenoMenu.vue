<template>
  <aside :class="`${is_expanded && 'is_expanded'}`">
    <div class="menu-toggle-wrap">
      <button class="menu-toggle" @click="ToggleMenu">
        <span class="material-icons">menu</span>
      </button>
    </div>

    <h3>Menu</h3>
    <div class="menu">
      <router-link class="button" to="/landing-page">
        <span class="material-icons">home</span>
        <span class="text">Inicio</span>
      </router-link>
      <router-link class="button" to="/company">
        <span class="material-icons">apartment</span>
        <span class="text">Empresa</span>
      </router-link>
      <router-link class="button" to="/companyemployees">
        <span class="material-icons">group</span>
        <span class="text">Empleados</span>
      </router-link>
      <router-link class="button" to="/registeremployee">
        <span class="material-icons">person_add</span>
        <span class="text">Agregar Empleados</span>
      </router-link>
      <router-link class="button" to="/registerbenefits">
        <span class="material-icons">star</span>
        <span class="text">Agregar Beneficios</span>
      </router-link>
    </div>
  </aside>
</template>

<script setup>
  import { ref } from 'vue'

  const is_expanded = ref(localStorage.getItem("is_expanded") === "true")

  const ToggleMenu = () => {
    is_expanded.value = !is_expanded.value
    localStorage.setItem("is_expanded", is_expanded.value)
  }
</script>

<style lang="scss" scoped>
  aside {
    display: flex;
    flex-direction: column;
    width: calc(2rem + 32px);
    min-height: 100vh;
    overflow: hidden;
    padding: 1rem;
    background-color: #f4f6f8;
    color: var(--light);

    .menu-toggle-wrap {
      display: flex;
      margin-bottom: 1rem;
      position: relative;
      top: 0;

      .menu-toggle {
        transition: 0.2s ease-out;
        margin-left: -0.3rem;

        .material-icons {
          font-size: 2rem;
          color: #003c63;
          transition: 0.2s ease-out;
        }

        &:hover .material-icons {
          color: #003c63;
          transform: translateX(0.5rem);
        }
      }
    }

    h3, .button .text {
      opacity: 0;
    }

    h3 {
      color: #003c63;
      font-size: 0.875rem;
      margin-bottom: 0.5rem;
      text-transform: uppercase;
    }

    .menu {
      margin: 0 -1rem;

      .button {
        display: flex;
        align-items: center;
        text-decoration: none;
        padding: 0.5rem 1rem;

        .material-icons {
          font-size: 2rem;
          color: #003c63;
          margin-right: 0rem;
        }

        .text {
          color: #003c63;
          padding: 0.5rem;
        }

        &.hover,
        &.router-link-exact-active {
          background-color: var(--light-alt);

          .material-icons,
          .text {
            color: #5AB779;
          }
        }

        &.router-link-exact-active {
          border-right: 5px solid #5AB779;
        }
      }
    }

    &.is_expanded {
      width: var(--sidebar-width);

      .menu-toggle-wrap .menu-toggle {
        transform: rotate(180deg);
      }

      h3, .button .text {
        opacity: 1;
      }

      .button .material-icons {
        margin-right: 0rem;
      }
    }

    @media (max-width: 768px) {
      position: fixed;
      z-index: 99;
    }
  }
</style>

<style lang="scss">
  body {
    background: var(--light);
  }

  button {
    cursor: pointer;
    appearance: none;
    border: none;
    outline: none;
    background: none;
  }
</style>
