<template>
  <aside :class="{ is_expanded: is_expanded }" class="d-flex flex-column p-3">
    <div class="menu-toggle-wrap">
      <button class="menu-toggle" @click="ToggleMenu">
        <span class="material-icons">menu</span>
      </button>
    </div>

    <h5>Menu</h5>

    <div class="menu d-flex flex-column">
      <router-link class="button" to="/landing-page">
        <span class="material-icons">home</span>
        <span class="text">Inicio</span>
      </router-link>
      <router-link class="button" to="/employees">
        <span class="material-icons">group</span>
        <span class="text">Empleados</span>
      </router-link>
      <router-link class="button" to="/companieslist">
        <span class="material-icons">corporate_fare</span>
        <span class="text">Lista de Empresas</span>
      </router-link>
    </div>
  </aside>
</template>

<script setup>

  import { ref, onMounted, onBeforeUnmount } from 'vue'

  const is_expanded = ref(localStorage.getItem("is_expanded") === "true")

  const ToggleMenu = () => {
    if (window.innerWidth > 768) {
      is_expanded.value = !is_expanded.value
      localStorage.setItem("is_expanded", is_expanded.value)
    }
  }

  const handleResize = () => {
    if (window.innerWidth <= 768) {
      is_expanded.value = false
      localStorage.setItem("is_expanded", is_expanded.value)
    }
  }

  onMounted(() => {
    window.addEventListener('resize', handleResize)
    handleResize()
  })

  onBeforeUnmount(() => {
    window.removeEventListener('resize', handleResize)
  })
</script>


<style lang="scss" scoped>
  .material-icons {
    font-size: 2rem;
    color: #003c63;
    transition: 0.2s ease-out;
  }

  aside {
    width: 4rem;
    height: 100%;
    overflow: hidden;
    background-color: #f4f6f8;
    color: var(--light);

    .menu-toggle-wrap {
      margin-left: -0.35rem;

      .menu-toggle {
        .material-icons {
          transition: 0.2s ease-out;
        }

        &:hover .material-icons {
          transform: translateX(0.5rem);
        }
      }
    }

    h5,
    .button .text {
      opacity: 0;
    }

    h5 {
      color: #003c63;
      margin: 0.5rem 0;
    }

    .menu {
      margin: 0 -1rem;
      padding: 0 0 0 1rem;

      .button {
        height: 3rem;
        display: flex;
        align-items: center;
        text-decoration: none;

        .material-icons,
        .text {
          color: #003c63;
        }

        .text {
          margin: 0.75rem;
        }

        &.hover,
        &.router-link-exact-active {
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

      h5,
      .button .text {
        opacity: 1;
      }
    }
  }
</style>

