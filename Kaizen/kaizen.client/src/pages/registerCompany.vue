<template>
  <section class="bg-white pb-5">
    <h2 class="text-center titulo fw-bold mt-4 mb-5">
      Registro de empresa
    </h2>

    <form class="mx-auto d-flex flex-column gap-4"
          style="max-width:800px;"
          @submit.prevent="registerCompany">

      <!-- Cédula jurídica -->

      <div>
        <label for="cedula" class="form-label label-kaizen mb-1">
          Cédula jurídica
        </label>
        <input v-model="cedulaJuridica"
               id="cedula"
               type="text"
               placeholder="3-102-242458"
               required
               class="form-control campo shadow-sm" />
      </div>

      <!-- Nombre de la empresa -->
      <div>
        <label for="nombre" class="form-label label-kaizen mb-1">
          Nombre de la empresa
        </label>
        <input v-model="nombre"
               id="nombre"
               type="text"
               required
               class="form-control campo shadow-sm" />
      </div>

      <!-- Correo electrónico -->
      <div>
        <label for="email" class="form-label label-kaizen mb-1">
          Correo electrónico
        </label>
        <input v-model="email"
               id="email"
               type="email"
               placeholder="usuario@dominio.cr"
               required
               class="form-control campo shadow-sm" />
      </div>

      <!-- Dirección -->
      <div>
        <label for="direccion" class="form-label label-kaizen mb-1">
          Dirección
        </label>
        <input v-model="direccion"
               id="direccion"
               type="text"
               placeholder="Provincia, Cantón, Distrito, Señas adicionales"
               required
               class="form-control campo shadow-sm" />
      </div>

      <!-- Teléfono -->
      <div>
        <label for="telefono" class="form-label label-kaizen mb-1">
          Teléfono
        </label>
        <input v-model="telefono"
               id="telefono"
               type="text"
               placeholder="8888-1234"
               required
               class="form-control campo shadow-sm" />
      </div>

      <!-- Razón social -->
      <div>
        <label for="razonSocial" class="form-label label-kaizen mb-1">
          Razón social
        </label>
        <input v-model="razonSocial"
               id="razonSocial"
               type="text"
               required
               class="form-control campo shadow-sm" />
      </div>

      <!-- Botón -->
      <button type="submit"
              class="btn boton-kaizen fw-semibold text-white mx-auto mt-3 px-5 py-2">
        Registrar
      </button>

      <!-- Mensajes -->
      <p v-if="error" class="text-danger text-center small mt-2">{{ error }}</p>
      <p v-if="success" class="text-success text-center small mt-2">{{ success }}</p>
    </form>
  </section>
</template>

<script>
  import axios from 'axios';

  export default {
    name: 'RegisterCompany',
    data() {
      return {
        cedulaJuridica: '',
        nombre: '',
        email: '',
        direccion: '',
        telefono: '',
        razonSocial: '',
        error: '',
        success: ''
      };
    },
    methods: {
      async registerCompany() {
        this.error = '';
        this.success = '';

        // 1) Validar Cédula: x-xxx-xxxxxx (10 dígitos + 2 guiones en posiciones fijas)
        const cedulaRegex = /^[0-9]-[0-9]{3}-[0-9]{6}$/;
        if (!cedulaRegex.test(this.cedulaJuridica)) {
          this.error = 'Error: Favor ingresar la información en el formato correcto';
          return;
        }

        // 2) Nombre: requerido, permite letras, números, espacios, &, '
        const nombreRegex = /^[\p{L}\p{N}\s&']+$/u;
        if (!nombreRegex.test(this.nombre.trim())) {
          this.error = 'Error: Favor ingresar la información en el formato correcto';
          return;
        }

        // 3) Correo: simple validación de formato
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(this.email)) {
          this.error = 'Error: Favor ingresar la información en el formato correcto';
          return;
        }

        // 4) Dirección: mínimo 4 segmentos separados por coma
        if (this.direccion.split(',').length < 4) {
          this.error = 'Error: Favor ingresar la información en el formato correcto';
          return;
        }

        // 5) Teléfono: 8 dígitos numéricos
        const telefonoRegex = /^[0-9]{4}-[0-9]{4}$/;
        if (!telefonoRegex.test(this.telefono)) {
          this.error = 'Error: Favor ingresar la información en el formato correcto';
          return;
        }

        // 6) Razón social: requerido (permitimos cualquier caracter salvo control)
        if (!this.razonSocial.trim()) {
          this.error = 'Error: Favor ingresar la información en el formato correcto';
          return;
        }

        // 7) Todo OK → envío al backend
        try {
          await axios.post('/api/empresas', {
            cedulaJuridica: this.cedulaJuridica,
            nombre: this.nombre,
            email: this.email,
            direccion: this.direccion,
            telefono: this.telefono,
            razonSocial: this.razonSocial
          });

          this.success = 'Empresa registrada correctamente.';
        } catch (err) {
          if (err.response?.status === 409) {
            this.error = 'Error: La empresa ya está registrada.';
          } else {
            this.error = err.response?.data?.message ||
              'Error al registrar la empresa.';
          }
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

  /* Botón */
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
