<template>
  <section class="bg-white pb-5">
    <h2 class="text-center titulo fw-bold mt-4 mb-5">Registro de empresa</h2>

    <form class="needs-validation mx-auto row justify-content-center"
          style="max-width: 800px;"
          novalidate
          @submit.prevent="registerCompany">

      <!-- Brand Name -->
      <div class="mb-3 col-10">
        <label for="brandName" class="form-label label-kaizen mb-1">Nombre de Fantasía</label>
        <input v-model="brandName"
               id="brandName"
               type="text"
               maxlength="100"
               required
               class="form-control campo shadow-sm"
               :class="{ 'is-invalid': attemptedSubmit && errorBrandName }" />
        <div class="invalid-feedback">{{ errorBrandName }}</div>
      </div>

      <!-- Company ID -->
      <div class="mb-3 col-10">
        <label for="cedulaJuridica" class="form-label label-kaizen mb-1">Cédula jurídica</label>
        <input v-model="cedulaJuridica"
               id="cedulaJuridica"
               type="text"
               placeholder="3-102-242458"
               required
               class="form-control campo shadow-sm"
               :class="{ 'is-invalid': attemptedSubmit && errorCedula }" />
        <div class="invalid-feedback">{{ errorCedula }}</div>
      </div>

      <!-- Company Name -->
      <div class="mb-3 col-10">
        <label for="nombreEmpresa" class="form-label label-kaizen mb-1">Nombre de la empresa</label>
        <input v-model="nombreEmpresa"
               id="nombreEmpresa"
               type="text"
               placeholder="Nombre legal"
               required
               class="form-control campo shadow-sm"
               :class="{ 'is-invalid': attemptedSubmit && errorNombreEmpresa }" />
        <div class="invalid-feedback">{{ errorNombreEmpresa }}</div>
      </div>

      <!-- Address: Province -->
      <div class="mb-3 col-10">
        <label for="province" class="form-label label-kaizen mb-1">Provincia</label>
        <input v-model="province"
               id="province"
               type="text"
               placeholder="Ej: San José"
               required
               class="form-control campo shadow-sm"
               :class="{ 'is-invalid': attemptedSubmit && errorProvince }" />
        <div class="invalid-feedback">{{ errorProvince }}</div>
      </div>

      <!-- Address: Canton -->
      <div class="mb-3 col-10">
        <label for="canton" class="form-label label-kaizen mb-1">Cantón</label>
        <input v-model="canton"
               id="canton"
               type="text"
               placeholder="Ej: Curridabat"
               required
               class="form-control campo shadow-sm"
               :class="{ 'is-invalid': attemptedSubmit && errorCanton }" />
        <div class="invalid-feedback">{{ errorCanton }}</div>
      </div>

      <!-- Address: District -->
      <div class="mb-3 col-10">
        <label for="district" class="form-label label-kaizen mb-1">Distrito</label>
        <input v-model="district"
               id="district"
               type="text"
               placeholder="Ej: Granadilla"
               required
               class="form-control campo shadow-sm"
               :class="{ 'is-invalid': attemptedSubmit && errorDistrict }" />
        <div class="invalid-feedback">{{ errorDistrict }}</div>
      </div>

      <!-- Address: Additional Signs -->
      <div class="mb-3 col-10">
        <label for="additionalSigns" class="form-label label-kaizen mb-1">Señas adicionales</label>
        <input v-model="additionalSigns"
               id="additionalSigns"
               type="text"
               placeholder="Casa azul, junto al parque..."
               required
               class="form-control campo shadow-sm"
               :class="{ 'is-invalid': attemptedSubmit && errorAdditionalSigns }" />
        <div class="invalid-feedback">{{ errorAdditionalSigns }}</div>
      </div>

      <!-- Company Phone Number -->
      <div class="mb-3 col-10">
        <label for="telefonoEmpresa" class="form-label label-kaizen mb-1">Teléfono</label>
        <input v-model="telefonoEmpresa"
               id="telefonoEmpresa"
               type="text"
               placeholder="8888-1234"
               required
               class="form-control campo shadow-sm"
               :class="{ 'is-invalid': attemptedSubmit && errorTelefonoEmpresa }" />
        <div class="invalid-feedback">{{ errorTelefonoEmpresa }}</div>
      </div>

      <!-- Company Legal Description -->
      <div class="mb-3 col-10">
        <label for="razonSocial" class="form-label label-kaizen mb-1">Descripción</label>
        <input v-model="razonSocial"
               id="razonSocial"
               type="text"
               placeholder="Añade descripción a tu empresa"
               required
               class="form-control campo shadow-sm"
               :class="{ 'is-invalid': attemptedSubmit && errorRazonSocial }" />
        <div class="invalid-feedback">{{ errorRazonSocial }}</div>
      </div>

      <!-- —— Owner Information —— -->
      <h3 class="text-center titulo fw-bold mt-5 mb-3">Información del dueño</h3>

      <!-- Owner ID -->
      <div class="mb-3 col-10">
        <label for="ownerId" class="form-label label-kaizen mb-1">Cédula del dueño</label>
        <input v-model="ownerId"
               id="ownerId"
               type="text"
               placeholder="01-0111-0111"
               required
               class="form-control campo shadow-sm"
               :class="{ 'is-invalid': attemptedSubmit && errorOwnerId }" />
        <div class="invalid-feedback">{{ errorOwnerId }}</div>
      </div>

      <!-- Owner Name -->
      <div class="mb-3 col-10">
        <label for="ownerName" class="form-label label-kaizen mb-1">Nombre del dueño</label>
        <input v-model="ownerName"
               id="ownerName"
               type="text"
               placeholder="Nombre"
               required
               class="form-control campo shadow-sm"
               :class="{ 'is-invalid': attemptedSubmit && errorOwnerName }" />
        <div class="invalid-feedback">{{ errorOwnerName }}</div>
      </div>

      <!-- Owner Last Name -->
      <div class="mb-3 col-10">
        <label for="ownerLastName" class="form-label label-kaizen mb-1">Apellidos del dueño</label>
        <input v-model="ownerLastName"
               id="ownerLastName"
               type="text"
               placeholder="Apellidos"
               required
               class="form-control campo shadow-sm"
               :class="{ 'is-invalid': attemptedSubmit && errorOwnerLastName }" />
        <div class="invalid-feedback">{{ errorOwnerLastName }}</div>
      </div>

      <!-- Owner Sex -->
      <div class="mb-3 col-10">
        <label class="form-label label-kaizen mb-1">Sexo del dueño</label>
        <div class="mt-2">
          <div class="form-check" v-for="opt in ownerSexOptions" :key="opt.value">
            <input class="form-check-input"
                   type="radio"
                   :id="`ownerSex-${opt.value}`"
                   :value="opt.value"
                   v-model="ownerSex"
                   name="ownerSex"
                   :class="{ 'is-invalid': attemptedSubmit && errorOwnerSex }" />
            <label class="form-check-label ms-2" :for="`ownerSex-${opt.value}`">{{ opt.label }}</label>
          </div>
          <div class="invalid-feedback d-block">{{ errorOwnerSex }}</div>
        </div>
      </div>

      <!-- Owner Birth Date -->
      <div class="mb-3 col-10">
        <label for="ownerBirthDate" class="form-label label-kaizen mb-1">Fecha de nacimiento</label>
        <input v-model="ownerBirthDate"
               id="ownerBirthDate"
               type="date"
               required
               class="form-control campo shadow-sm"
               :class="{ 'is-invalid': attemptedSubmit && errorOwnerBirthDate }" />
        <div class="invalid-feedback">{{ errorOwnerBirthDate }}</div>
      </div>

      <!-- Owner Email -->
      <div class="mb-3 col-10">
        <label for="ownerEmail" class="form-label label-kaizen mb-1">Correo electrónico</label>
        <input v-model="ownerEmail"
               id="ownerEmail"
               type="email"
               placeholder="dueño@dominio.cr"
               required
               class="form-control campo shadow-sm"
               :class="{ 'is-invalid': attemptedSubmit && errorOwnerEmail }" />
        <div class="invalid-feedback">{{ errorOwnerEmail }}</div>
      </div>

      <!-- Owner Password -->
      <div class="mb-3 col-10">
        <label for="ownerPassword" class="form-label label-kaizen mb-1">Contraseña</label>
        <input v-model="ownerPassword"
               id="ownerPassword"
               type="password"
               placeholder="Mín. 8 caracteres"
               required
               class="form-control campo shadow-sm"
               :class="{ 'is-invalid': attemptedSubmit && errorOwnerPassword }" />
        <div class="invalid-feedback">{{ errorOwnerPassword }}</div>
      </div>

      <!-- Submit Button and Messages -->
      <div class="mb-3 col-10 text-center">
        <button type="submit" class="btn boton-kaizen fw-semibold text-white px-5 py-2">Registrar</button>
        <p v-if="generalError" class="text-danger text-center small mt-2">{{ generalError }}</p>
        <p v-if="success" class="text-success text-center small mt-2">{{ success }}</p>
      </div>
    </form>
  </section>
</template>


<script>
import axios from 'axios';
export default {
  name: 'RegisterCompany',
  data() {
    return {
      attemptedSubmit: false,
      brandName: '',
      cedulaJuridica: '',
      nombreEmpresa: '',
      emailEmpresa: '',
      province: '',
      canton: '',
      district: '',
      additionalSigns: '',
      telefonoEmpresa: '',
      razonSocial: '',
      /* — Owner — */
      ownerId: '',
      ownerName: '',
      ownerLastName: '',
      ownerSex: '',
      ownerSexOptions: [
        { label: 'Hombre', value: 'Hombre' },
        { label: 'Mujer', value: 'Mujer' }
      ],
      ownerBirthDate: '',
      ownerEmail: '',
      ownerPassword: '',
      /* — Messages — */
      success: '',
      generalError: '',
      errorBrandName: '',
      errorType: '',
      errorCedula: '',
      errorNombreEmpresa: '',
      errorEmailEmpresa: '',
      errorProvince: '',
      errorCanton: '',
      errorDistrict: '',
      errorAdditionalSigns: '',
      errorTelefonoEmpresa: '',
      errorRazonSocial: '',
      errorOwnerId: '',
      errorOwnerName: '',
      errorOwnerLastName: '',
      errorOwnerSex: '',
      errorOwnerBirthDate: '',
      errorOwnerEmail: '',
      errorOwnerPassword: ''
    };
  },
  methods: {
    async registerCompany() {
      this.attemptedSubmit = true;
      this.generalError = '';

      Object.keys(this)
        .filter(k => k.startsWith('error'))
        .forEach(k => (this[k] = ''));

      let hasError = false;
      const emailRX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      const today = new Date().toISOString().split('T')[0];

      if (!this.brandName.trim()) { this.errorBrandName = 'Requerido (máx 100 caracteres).'; hasError = true; }
      if (!/^[0-9]-[0-9]{3}-[0-9]{6}$/.test(this.cedulaJuridica)) { this.errorCedula = 'Formato: 3-102-242458'; hasError = true; }
      if (!this.nombreEmpresa.trim()) { this.errorNombreEmpresa = 'Requerido.'; hasError = true; }
      if (!/^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]{1,50}$/.test(this.province)) { this.errorProvince = 'Sólo letras, 1–50 caracteres.'; hasError = true; }
      if (!/^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]{1,50}$/.test(this.canton)) { this.errorCanton = 'Sólo letras, 1–50 caracteres.'; hasError = true; }
      if (!/^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]{1,50}$/.test(this.district)) { this.errorDistrict = 'Sólo letras, 1–50 caracteres.'; hasError = true; }
      if (!/^[0-9]{4}-[0-9]{4}$/.test(this.telefonoEmpresa)) { this.errorTelefonoEmpresa = 'Ej: 8888-1234'; hasError = true; }
      if (!/^\d{2}-\d{4}-\d{4}$/.test(this.ownerId)) { this.errorOwnerId = 'Formato: 01-0111-0111'; hasError = true; }
      if (!/^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]{1,75}$/.test(this.ownerName)) { this.errorOwnerName = 'Sólo letras, 1–75 caracteres.'; hasError = true; }
      if (!/^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]{1,75}$/.test(this.ownerLastName)) { this.errorOwnerLastName = 'Sólo letras, 1–75 caracteres.'; hasError = true; }
      if (!['Hombre','Mujer'].includes(this.ownerSex)) { this.errorOwnerSex = 'Seleccione sexo.'; hasError = true; }
      if (!this.ownerBirthDate) {
        this.errorOwnerBirthDate = 'Requerido.'; hasError = true;
      } else if (this.ownerBirthDate > today) {
        this.errorOwnerBirthDate = 'La fecha no puede ser futura.'; hasError = true;
      }
      if (!emailRX.test(this.ownerEmail)) { this.errorOwnerEmail = 'Correo inválido.'; hasError = true; }
      if (this.ownerPassword.length < 8) { this.errorOwnerPassword = 'Mínimo 8 caracteres.'; hasError = true; }

      if (hasError) {
        this.$nextTick(() => {
          const el = this.$el.querySelector('.is-invalid');
          if (el) {
            el.scrollIntoView({ behavior: 'smooth', block: 'center' });
            el.focus({ preventScroll: true });
          }
        });
        return;
      }
      try{

      await axios.post('/api/registerCompany', {
        companyID: this.cedulaJuridica,
        companyName: this.nombreEmpresa,
        brandName: this.brandName,
        type: "Juridico",
        foundationDate: new Date().toISOString().split('T')[0],
        maxBenefits: 5,
        webPage: "",
        logo: "",
        description: this.razonSocial,
        po: "",
        province: this.province,
        canton: this.canton,
        distrito: this.district,
        otherSigns: this.additionalSigns,

        owner: {
          id: this.ownerId,
          name: this.ownerName,
          lastName: this.ownerLastName,
          sex: this.ownerSex,
          birthDate: this.ownerBirthDate,
          province: this.province,
          canton: this.canton,
          otherSigns: this.additionalSigns
        },

        user: {
          email: this.ownerEmail,
          passwordHash: this.ownerPassword,
          active: true,
          role: "Dueño"
        }
      });

        this.success = 'Empresa y dueño registrados correctamente.';
      } catch (e) {
        this.generalError = e.response?.status === 409
          ? 'Ya existe una empresa con esa cédula.'
          : 'Error al registrar la empresa.';
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

  input:focus {
    outline: none;
    background-color: #f2f2f2;
  }

  input:-webkit-autofill,
  textarea:-webkit-autofill,
  select:-webkit-autofill {
    -webkit-box-shadow: 0 0 0px 1000px #f2f2f2 inset !important;
    box-shadow: 0 0 0px 1000px #f2f2f2 inset !important;
    transition: background-color 5000s ease-in-out 0s;
  }
</style>
