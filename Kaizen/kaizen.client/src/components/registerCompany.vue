<template>
  <section class="bg-white pb-5">
    <h2 class="text-center titulo fw-semibold mt-4 mb-5">
      Registro de empresa
    </h2>

    <form class="mx-auto d-flex flex-column gap-4"
          style="max-width:800px;"
          @submit.prevent="registerCompany">
      <div>
        <label for="nombre" class="form-label label-kaizen mb-1">Nombre de la empresa</label>
        <input v-model="nombre" id="nombre" required class="form-control campo shadow-sm" />
      </div>
      <div>
        <label for="cedula" class="form-label label-kaizen mb-1">Cédula jurídica</label>
        <input v-model="cedulaJuridica" id="cedula" required class="form-control campo shadow-sm" />
      </div>
      <div>
        <label for="email" class="form-label label-kaizen mb-1">Correo electrónico</label>
        <input v-model="email" id="email" type="email" required class="form-control campo shadow-sm" />
      </div>
      <div>
        <label for="direccion" class="form-label label-kaizen mb-1">Dirección</label>
        <input v-model="direccion" id="direccion" required class="form-control campo shadow-sm" />
      </div>
      <div>
        <label for="telefono" class="form-label label-kaizen mb-1">Teléfono</label>
        <input v-model="telefono" id="telefono" required class="form-control campo shadow-sm" />
      </div>
      <div>
        <label for="razonSocial" class="form-label label-kaizen mb-1">Razón social</label>
        <input v-model="razonSocial" id="razonSocial" required class="form-control campo shadow-sm" />
      </div>

      <button type="submit"
              class="btn boton-kaizen fw-semibold text-white mx-auto mt-3 px-5 py-2">
        Registrar empresa
      </button>

      <p v-if="success" class="text-success text-center small">{{ success }}</p>
      <p v-if="error" class="text-danger text-center small">{{ error }}</p>
    </form>
  </section>
</template>

<script>
  import axios from 'axios';

  export default {
    name: 'RegisterCompany',
    data() {
      return {
        nombre: '',
        cedulaJuridica: '',
        email: '',
        direccion: '',
        telefono: '',
        razonSocial: '',
        success: '',
        error: ''
      };
    },
    methods: {
      async registerCompany() {
        this.error = this.success = '';
        try {
          await axios.post('/api/empresas', {
            nombre: this.nombre,
            cedulaJuridica: this.cedulaJuridica,
            email: this.email,
            direccion: this.direccion,
            telefono: this.telefono,
            razonSocial: this.razonSocial
          });
          this.$router.push({ name: 'LoginUser', query: { registered: 1 } });
        } catch (err) {
          this.error = err.response?.data?.message || 'Error al registrar la empresa.';
        }
      }
    }
  };
</script>

<style scoped>
  .titulo,
  .label-kaizen {
    color: #003C63 !important;
  }

  .campo {
    background: #f2f2f2;
    border: 0;
    border-radius: 8px;
    height: 44px;
  }

  
  .boton-kaizen {
    background-color: #003C63;
    border: none;
    border-radius: 8px;
    transition: background 0.25s ease, transform 0.25s ease;
  }

    .boton-kaizen:hover {
      background-color: #004c83;
      transform: translateY(-2px);
    }
</style>
