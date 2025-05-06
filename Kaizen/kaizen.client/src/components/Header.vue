<template>
  <header>
    <nav class="navbar navbar-expand-md bg-body-secondary">
      <div class="container-fluid">
        <a class="navbar-brand" href="#">
          <img src="@/assets/images/logo.png" alt="Kaizen Logo" class="app-logo" />
        </a>
        <button
          class="navbar-toggler"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target="#navbarCollapse"
          aria-controls="navbarCollapse"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarCollapse">
          <a class="navbar-brand" href="#">
            <img
              src="https://place-hold.it/200x50&text=Company"
              alt="Kaizen Logo"
              class="company-logo"
            />
          </a>
          <ul class="navbar-nav mb-2 mb-md-0 ms-auto">
            <li class="nav-item">
              <div class="nav-link">
                <button type="button" class="btn" data-bs-toggle="dropdown" aria-expanded="false">
                  <Bell class="icon"></Bell>
                </button>
                <ul class="dropdown-menu dropdown-menu-end">
                  <li v-for="notification in data" :key="notification.id">
                    <button class="dropdown-item" type="button">{{ notification.description }}</button>
                  </li>
                </ul>
              </div>
            </li>
            <li class="nav-item">
              <div class="nav-link">
                <button type="button" class="btn" data-bs-toggle="dropdown" aria-expanded="false">
                  <Gear class="icon"></Gear>
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
                  <User class="icon"></User>
                </button>
                <ul class="dropdown-menu dropdown-menu-end">
                  <li><button class="dropdown-item" type="button">Información de usuario</button></li>
                  <li><button class="dropdown-item" type="button">Cerrar sesión</button></li>
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
import axios from "axios";

const data = ref(null);

async function fetchData() {
  try {
    axios
      .get("https://localhost:7153/api/notifications", {
        withCredentials: true,
      })
      .then((response) => {
        console.log("Data fetched successfully:", response.data);
        data.value = response.data;
      })
      .catch((error) => {
        console.error("Error fetching data:", error);
        throw error;
      });
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
  methods: {},
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
}

.navbar .navbar-nav {
  align-items: start;
}
@media (min-width: 768px) {
  .navbar .navbar-nav {
    align-items: center;
  }
}
</style>
