
<template>
  <div class="vh-100 d-flex justify-content-center align-items-center bg-white">
    <div
      class="card shadow border-0 p-4 d-flex flex-column align-items-stretch gap-3"
      style="width:420px;border-radius:18px;"
    >
      <!-- Company logo -->
      <div class="text-center mb-1">
        <img src="/logo.png" alt="Kaizen Logo" width="250" />
      </div>

      <!-- Login form container -->
      <form class="d-flex flex-column gap-4" @submit.prevent="login">
        <!-- Username input group -->
        <div>
          <label for="username" class="form-label kaizen mb-1">Usuario</label>
          <input
            v-model="username"
            id="username"
            type="text"
            required
            class="form-control campo rounded-3 shadow-sm"
          />
        </div>

        <!-- Password input group with visibility toggle -->
        <div>
          <label for="password" class="form-label kaizen mb-1">Contraseña</label>
          <div class="input-group">
            <input
              v-model="password"
              :type="showPassword ? 'text' : 'password'"
              id="password"
              required
              class="form-control campo rounded-3 shadow-sm"
            />
            <button
              type="button"
              class="btn btn-outline-light text-dark border"
              @click="togglePasswordVisibility"
              tabindex="-1"
            >
              <!-- Toggle icon switches based on visibility -->
              <i :class="showPassword ? 'bi bi-eye-slash' : 'bi bi-eye'"></i>
            </button>
          </div>
        </div>

        <!-- Submit button -->
        <button
          type="submit"
          class="btn login-btn fw-semibold text-white rounded-3 py-2 fs-6 shadow-sm"
        >
          INICIAR SESIÓN
        </button>

        <!-- Secondary action links -->
        <div class="text-center small">
          <a href="#" class="me-3 text-decoration-none kaizen">Registrarse</a>
          <a href="#" class="text-decoration-none kaizen">¿Olvidaste tu contraseña?</a>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
export default {
  name: 'LoginComponent', // Component name

  data() {
    return {
      /**
       * Holds the username entered by the user.
       * @type {string}
       */
      username: '',

      /**
       * Holds the password entered by the user.
       * @type {string}
       */
      password: '',

      /**
       * Controls whether the password is shown as plain text.
       * @type {boolean}
       */
      showPassword: false
    };
  },

  methods: {
    /**
     * Toggle the password visibility between masked and plain text.
     */
    togglePasswordVisibility() {
      this.showPassword = !this.showPassword;
    },

    /**
     * Handle the login form submission.
     * Currently logs credentials to the console; replace with real auth logic.
     */
    login() {
      console.log(`Username: ${this.username}\nPassword: ${this.password}`);
      // TODO: Replace with actual authentication API call and error handling
    }
  }
};
</script>

<style scoped>
/* Kaizen corporate color */
.kaizen {
  color: #003C63;
}

/* Light gray background for input fields, no border */
.campo {
  background: #f2f2f2;
  border: 0;
}

/* Primary login button styling and hover effect */
.login-btn {
  background: #003C63;
  transition: background 0.25s ease, transform 0.25s ease;
}
.login-btn:hover {
  background: #004c83;
  transform: translateY(-2px);
}
</style>