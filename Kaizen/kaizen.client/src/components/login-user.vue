<template>
  <div class="vh-100 d-flex justify-content-center align-items-center bg-white">
    <div class="card shadow border-0 p-4 d-flex flex-column align-items-stretch gap-3"
         style="width:420px;border-radius:18px;">
      <!-- Logo -->
      <div class="text-center mb-1">
        <img src="/logo.png" alt="Kaizen Logo" width="250" />
      </div>

      <!-- Success message -->
      <div v-if="success" class="text-success small text-center mb-2">
        {{ success }}
      </div>

      <!-- Error message -->
      <div v-if="error" class="text-danger small text-center mb-2">
        {{ error }}
      </div>

      <!-- Login form -->
      <form class="d-flex flex-column gap-4" @submit.prevent="login">
        <!-- Username -->
        <div>
          <label for="username" class="form-label kaizen mb-1">Usuario</label>
          <input v-model="username"
                 id="username"
                 type="text"
                 required
                 class="form-control campo rounded-3 shadow-sm" />
        </div>

        <!-- Password -->
        <div>
          <label for="password" class="form-label kaizen mb-1">Contraseña</label>
          <div class="input-group">
            <input v-model="password"
                   :type="showPassword ? 'text' : 'password'"
                   id="password"
                   required
                   class="form-control campo rounded-3 shadow-sm" />
            <button type="button"
                    class="btn btn-outline-light text-dark border"
                    @click="togglePasswordVisibility"
                    tabindex="-1">
              <i :class="showPassword ? 'bi bi-eye-slash' : 'bi bi-eye'"></i>
            </button>
          </div>
        </div>

        <!-- Submit button -->
        <button type="submit"
                class="btn login-btn fw-semibold text-white rounded-3 py-2 fs-6 shadow-sm">
          INICIAR SESIÓN
        </button>

        <!-- secondary links -->
        <div class="text-center small">
          <a href="#" class="me-3 text-decoration-none kaizen">Registrar empresa</a>
          <a href="#" class="text-decoration-none kaizen">¿Olvidaste tu contraseña?</a>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
  import axios from 'axios';

  export default {
    name: 'LoginComponent',
    data() {
      return {
        username: '',
        password: '',
        showPassword: false,
        error: '',
        success: ''
      };
    },
    methods: {
      togglePasswordVisibility() {
        this.showPassword = !this.showPassword;
      },

      async login() {
        this.error = '';
        this.success = '';
        try {
          const response = await axios.post('/api/login/login', {
            email: this.username,
            password: this.password
          });

          const role = response.data.role;
          localStorage.setItem('userRole', role);
          this.success = 'Inicio de sesión correcto. ¡Bienvenido!';

          // Redirect to the dashboard after a successful login
          this.$router.push('/dashboard');
        }
        catch (err) {
          if (!err.response) {
            this.error = 'Error de red. Intente más tarde.';
          } else if (err.response.status === 401 || err.response.status === 404) {
            this.error = 'Error a la hora de iniciar sesión. Correo o contraseña incorrecta.';
          } else {
            this.error = 'Error desconocido.';
          }
        }
      }
    }
  };
</script>


<style scoped>
  .kaizen {
    color: #003C63;
  }

  .campo {
    background: #f2f2f2;
    border: 0;
  }

  .login-btn {
    background: #003C63;
    transition: background 0.25s ease, transform 0.25s ease;
  }

    .login-btn:hover {
      background: #004c83;
      transform: translateY(-2px);
    }
</style>
