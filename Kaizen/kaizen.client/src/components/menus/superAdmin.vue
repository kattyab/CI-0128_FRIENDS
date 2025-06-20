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

      <div class="reports-section" ref="reports_section">
        <button class="button reports-toggle" @click="ToggleReports">
          <span class="material-icons">assessment</span>
          <span class="text">Reportes</span>
          <span class="material-icons expand-icon" :class="{ rotated: reports_expanded }" v-show="is_expanded">expand_more</span>
        </button>

        <div class="sub-menu" :class="{ expanded: reports_expanded }" v-show="is_expanded">
          <router-link class="sub-button" :to="`/reports/companieshistoric`">
            <span class="text">Reporte de Planillas Histórico</span>
          </router-link>
        </div>

        <div class="popup-menu" :class="{ show: !is_expanded && reports_expanded }" @mouseleave="CloseReportsPopup">
          <router-link class="popup-button" :to="'/reports/companieshistoric'" @click="CloseReportsPopup">
            <span class="text">Reporte de Planillas Histórico</span>
          </router-link>
        </div>
      </div>
    </div>
  </aside>
</template>

<script setup>
  import { ref, onMounted, onBeforeUnmount } from 'vue'

  const is_expanded = ref(localStorage.getItem("is_expanded") === "true")
  const reports_expanded = ref(localStorage.getItem("reports_expanded") === "true")
  const reports_section = ref(null)
  const minimum_resolution = 768

  const ToggleMenu = () => {
    if (window.innerWidth > minimum_resolution) {
      is_expanded.value = !is_expanded.value
      localStorage.setItem("is_expanded", is_expanded.value)
      if (!is_expanded.value) {
        reports_expanded.value = false
        localStorage.setItem("reports_expanded", reports_expanded.value)
      }
    }
  }

  const ToggleReports = () => {
    reports_expanded.value = !reports_expanded.value
    localStorage.setItem("reports_expanded", reports_expanded.value)
  }

  const CloseReportsPopup = () => {
    if (!is_expanded.value) {
      reports_expanded.value = false
      localStorage.setItem("reports_expanded", reports_expanded.value)
    }
  }

  const handleResize = () => {
    if (window.innerWidth <= minimum_resolution) {
      is_expanded.value = false
      reports_expanded.value = false
      localStorage.setItem("is_expanded", is_expanded.value)
      localStorage.setItem("reports_expanded", reports_expanded.value)
    }
  }

  const handleClickOutside = (event) => {
    if (!is_expanded.value && reports_expanded.value && reports_section.value && !reports_section.value.contains(event.target)) {
      CloseReportsPopup()
    }
  }

  onMounted(() => {
    window.addEventListener('resize', handleResize)
    document.addEventListener('click', handleClickOutside)
    handleResize()
  })

  onBeforeUnmount(() => {
    window.removeEventListener('resize', handleResize)
    document.removeEventListener('click', handleClickOutside)
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
    overflow: visible;
    background-color: #f4f6f8;
    color: var(--light);
    position: relative;

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
          flex: 1;
          text-align: left;
        }

        .expand-icon {
          font-size: 1.5rem;
          margin-right: 0.5rem;
          transition: transform 0.3s ease;

          &.rotated {
            transform: rotate(180deg);
          }
        }

        &:hover,
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

      .reports-section {
        position: relative;

        .reports-toggle {
          justify-content: flex-start;

          &:hover {
            .material-icons,
            .text,
            .expand-icon {
              color: #5AB779;
            }
          }
        }

        .sub-menu {
          max-height: 0;
          overflow: hidden;
          transition: max-height 0.3s ease;
          background-color: rgba(0, 60, 99, 0.05);

          &.expanded {
            max-height: 400px;
          }

          .sub-button {
            height: 2.5rem;
            display: flex;
            align-items: center;
            text-decoration: none;
            padding-left: 1rem;

            .text {
              margin: 0.5rem;
              color: #003c63;
              font-size: 0.9rem;
            }

            &:hover,
            &.router-link-exact-active {
              background-color: rgba(90, 183, 121, 0.1);

              .text {
                color: #5AB779;
              }
            }

            &.router-link-exact-active {
              border-right: 3px solid #5AB779;
            }
          }
        }

        .popup-menu {
          position: absolute;
          left: 100%;
          top: 0;
          background: #f4f6f8;
          border: 1px solid #e0e6ed;
          border-radius: 8px;
          box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
          min-width: 200px;
          z-index: 1000;
          opacity: 0;
          visibility: hidden;
          transform: translateX(-10px);
          transition: all 0.2s ease;
          padding: 1px;

          &.show {
            opacity: 1;
            visibility: visible;
            transform: translateX(0);
          }

          .popup-button {
            height: 2.5rem;
            display: flex;
            align-items: center;
            text-decoration: none;
            padding: 0 1rem;
            border-bottom: 1px solid #f0f0f0;
            margin: 2px 0;

            &:last-child {
              border-bottom: none;
            }

            .text {
              color: #003c63;
              font-size: 0.85rem;
              margin: 0;
              flex: 1;
            }

            &:hover,
            &.router-link-exact-active {
              background-color: rgba(90, 183, 121, 0.1);

              .text {
                color: #5AB779;
              }
            }

            &.router-link-exact-active {
              border-left: 3px solid #5AB779;
            }
          }
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
