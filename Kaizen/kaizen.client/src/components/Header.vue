<template>
  <header>
    <nav class="navbar navbar-expand-md">
      <div class="container-fluid d-flex align-items-center">
        <button class="btn hamburger-btn d-md-none me-2"
                @click="$emit('toggle-sidebar')">
          ☰
        </button>

        <div>
          <img src="@/assets/images/azul.png" alt="Kaizen Logo" class="app-logo" />
        </div>

        <!--
        <div>
          <img src="https://place-hold.it/200x50&text=Company" alt="Kaizen Logo" class="company-logo" />
        </div>
        -->

        <button class="navbar-toggler"
                type="button"
                data-bs-toggle="collapse"
                data-bs-target="#navbarCollapse"
                aria-controls="navbarCollapse"
                aria-expanded="false"
                aria-label="Toggle navigation">
          ▾
        </button>

        <div class="collapse navbar-collapse" id="navbarCollapse">
          <ul class="navbar-nav mb-2 mb-md-0 ms-auto">
            <li class="nav-item">
              <div class="nav-link">
                <button type="button" class="btn" data-bs-toggle="dropdown" aria-expanded="false">
                  <Bell class="icon" />
                </button>
                <ul class="dropdown-menu dropdown-menu-end">
                  <li v-for="notification in notifications" :key="notification.id">
                    <button class="dropdown-item" type="button">{{ notification.title }}</button>
                  </li>
                </ul>
              </div>
            </li>
            <li class="nav-item">
              <div class="nav-link">
                <button type="button" class="btn" data-bs-toggle="dropdown" aria-expanded="false">
                  <Gear class="icon" />
                </button>
                <ul class="dropdown-menu dropdown-menu-end">
                  <li><button class="dropdown-item" type="button">Setting #1</button></li>
                  <li><button class="dropdown-item" type="button">Setting #2</button></li>
                  <li><button class="dropdown-item" type="button">Setting #3</button></li>
                </ul>
              </div>
            </li>
            <li class="nav-item">
              <div class="nav-link">
                <button type="button" class="btn" data-bs-toggle="dropdown" aria-expanded="false">
                  <User class="icon" />
                </button>
                <ul class="dropdown-menu dropdown-menu-end">
                  <li><button class="dropdown-item" type="button">Información de usuario</button></li>
                  <li><button class="dropdown-item" type="button" @click="logout">Cerrar sesión</button></li>
                </ul>
              </div>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  </header>
</template>

<script setup>
  import { ref, onMounted } from "vue";
  import { useLogout } from '@/composables/useLogout';

  const { logout } = useLogout();

  const notifications = ref(null);

  async function fetchData() {
    try {
      const response = await fetch("https://jsonplaceholder.typicode.com/todos");
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      let data = await response.json();
      data = data.slice(0, 5);
      notifications.value = data.map((item) => ({
        id: item.id,
        title: item.title,
      }));
    } catch (e) {
      console.log(e);
    }
  }

  onMounted(fetchData);
</script>

<script>
  import Bell from "@/assets/icons/bell.vue";
  import Gear from "@/assets/icons/gear.vue";
  import User from "@/assets/icons/user.vue";
  export default {
    components: {
      Bell,
      Gear,
      User,
    },
  };
</script>

<style lang="scss" scoped>
  .app-logo {
    max-height: 54px;
  }

  .company-logo {
    max-height: 44px;
  }

  .user-logo {
    max-height: 54px;
  }

  .icon {
    height: 28px;
    color: #003c63;
    fill: #003c63;
  }

  .btn{
      border: none;
  }

  .navbar {
    background-color: #f4f6f8 !important;
    border-bottom: 1px solid #dee2e6;
  }

  .navbar .navbar-nav {
    align-items: start;
  }

  @media (min-width: 768px) {
    .navbar .navbar-nav {
      align-items: center;
    }
  }

  .hamburger-btn {
    font-size: 0.1rem;
  }

  nav .container-fluid > div {
    margin-right: 15px;
  }
</style>
