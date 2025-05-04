<template>
  <div class="page container-fluid">
    <h1>Registrar Empleado</h1>
    <form class="has-validation" @submit.prevent="saveEmployee">
      <div class="row ">
        <div class="mb-3 col-10">
          <label for="name" class="form-label">Nombre</label>
          <input type="text" id="name" placeholder="Ingrese su nombre" v-model="name"
                 :class="['form-control', { 'is-invalid': name && !isNameValid}]" />
          <div class="invalid-feedback" , v-if="name && !isNameValid">
            Ingrese el nombre sin números ni signos.
          </div>
        </div>
      </div>

      <div class="row ">
        <div class="mb-3 col-10">
          <label for="lastname" class="form-label">Apellidos</label>
          <input type="text" id="lastname" placeholder="Ingrese sus apellidos"
                 v-model="lastname" :class="['form-control', { 'is-invalid': lastname && !isLastnameValid}]" />
          <div class="invalid-feedback" v-if="lastname && !isLastnameValid">
            Ingrese el apellido sin números ni signos.
          </div>
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="personid" class="form-label">Cédula de Identidad</label>
          <input type="text" id="personid" placeholder="XX-XXXX-XXXX"
                 v-model="personid" :class="['form-control', { 'is-invalid': personid && !isPersonidValid}]" />
          <div class="invalid-feedback" v-if="personid && !isPersonidValid">
            Ingrese una cédula física válida con guiones. Ejemplo: 01-0111-0111.
          </div>
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label class="form-label">Sexo</label>
          <div class="row">
            <div class="col form-check d-flex
                  align-items-center
                  justify-content-center" v-for="option in sexOptions" :key="option.value">
              <input class="form-check-input" type="radio" :id="option.value" :value="option.value" v-model="sex"
                     name="sexOptions" />
              <label class="form-check-label" :for="option.value">
                {{ option.label }}
              </label>
            </div>
          </div>
          <div class="invalid-feedback" v-if="!isSexValid && attemptedSubmit">
            Seleccione una opción de sexo.
          </div>
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="birthdate" class="form-label">Fecha de Nacimiento</label>
          <input type="date" id="birthdate" v-model="birthdate" :class="[birthdateClass, 'form-control']"
                 @change="birthdateTouched = true" />
          <div class="invalid-feedback" v-if="birthdateTouched && !isBirthdateValid">Seleccione una fecha válida.</div>
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="province" class="form-label">Provincia</label>
          <input type="text" id="province" placeholder="Ingrese la provincia de residencia"
                 v-model="province" :class="['form-control', { 'is-invalid': province && !isProvinceValid}]" />
          <div class="invalid-feedback" v-if="province && !isProvinceValid">
            Ingrese una provincia válida.
          </div>
        </div>

      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="canton" class="form-label">Cantón</label>
          <input type="text" id="canton" placeholder="Ingrese el cantón de residencia"
                 v-model="canton" :class="['form-control', { 'is-invalid': canton && !isCantonValid}]" />
          <div class="invalid-feedback" v-if="canton && !isCantonValid">
            Ingrese un cantón válido.
          </div>
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="othersigns" class="form-label">Otras señales</label>
          <input type="text" id="othersigns" placeholder="Opcional: Ingrese otras señales de residencia"
                 v-model="othersigns" class="form-control" />
        </div>
      </div>

      <!--TODO: Add dynamic selection of phone numbers-->
      <div class="row">
        <div class="mb-3 col-10">
          <label for="phonenumber" class="form-label">Número de teléfono</label>
          <input type="text" id="phonenumber" placeholder="XXXX-XXXX"
                 v-model="phonenumber" :class="['form-control', { 'is-invalid': phonenumber && !isPhonenumberValid}]" />
          <div class="invalid-feedback" v-if="phonenumber && !isPhonenumberValid">
            Ingrese un teléfono costarricense válido, con guión. Ejemplo: 8888-8888.
          </div>
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label class="form-label">Rol</label>
          <div class="row">
            <div class="col form-check d-flex
                        align-items-center
                        justify-content-center" v-for="option in roleOptions" :key="option.value">
              <input class="form-check-input" type="radio" :id="option.value" :value="option.value" v-model="role"
                     name="roleOptions" />
              <label class="form-check-label" :for="option.value">
                {{ option.label }}
              </label>
            </div>
          </div>
          <div class="invalid-feedback" v-if="!isRoleValid && attemptedSubmit">
            Seleccione una opción de rol.
          </div>
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="jobposition" class="form-label">Puesto</label>
          <input type="text" id="jobposition" placeholder="Ingrese el nombre del puesto"
                 v-model="jobposition" :class="['form-control', { 'is-invalid': jobposition && !isJobPositionValid}]" />
          <div class="invalid-feedback" v-if="jobposition && !isJobPositionValid">
            Ingrese un puesto de trabajo válido.
          </div>
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label class="form-label">Tipo de Contrato</label>
          <div class="row">
            <div class="col form-check d-flex align-items-center justify-content-center"
                 v-for="option in contractOptions"
                 :key="option.value">
              <input class="form-check-input" type="radio" :id="option.value" :value="option.value" v-model="contract"
                     name="contractOptions" />
              <label class="form-check-label" :for="option.value">
                {{ option.label }}
              </label>
            </div>
          </div>
          <div class="invalid-feedback" v-if="!isContractValid && attemptedSubmit">
            Seleccione una opción de tipo de contrato.
          </div>
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label class="form-label">Ciclo de Pago</label>
          <div class="row">
            <div class="col form-check d-flex
                        align-items-center
                        justify-content-center" v-for="option in payCycleOptions" :key="option.value">
              <input class="form-check-input" type="radio" :id="option.value" :value="option.value" v-model="paycycle"
                     name="payCycleOptions" />
              <label class="form-check-label" :for="option.value">
                {{ option.label }}
              </label>
            </div>
          </div>
          <div class="invalid-feedback" v-if="!isPayCycleValid && attemptedSubmit">
            Seleccione una opción de ciclo de pago.
          </div>
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="brutesalary" class="form-label">
            Salario Bruto
          </label>
          <input type="text" id="brutesalary" placeholder="Ingrese el salario bruto" v-model="brutesalary"
                 :class="['form-control', { 'is-invalid': brutesalary && !isBruteSalaryValid }]" required />
          <div class="invalid-feedback" v-if="brutesalary && !isBruteSalaryValid">
            El salario debe ser un número entero sin decimales.
          </div>
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="startdate" class="form-label">Fecha de Inicio</label>
          <input type="date" :class="[startdateClass, 'form-control']" id="startdate" v-model="startdate"
                 @change="startdateTouched = true" />
          <div class="invalid-feedback" v-if="startdateTouched && !isStartDateValid">Seleccione una fecha válida.</div>
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="bankaccount" class="form-label">Cuenta Bancaria</label>
          <input type="text" id="bankaccount" placeholder="Ingrese el número de cuenta bancaria"
                 v-model="bankaccount" :class="['form-control', { 'is-invalid': bankaccount && !isBankAccountValid }]" required />
          <div class="invalid-feedback" v-if="bankaccount && !isBankAccountValid">
            Ingrese un número de cuenta IBAN válido.
          </div>
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="email" class="form-label">Correo Electrónico</label>
          <input type="text" id="email" placeholder="Ingrese el correo electrónico" v-model="email"
                 :class="['form-control', { 'is-invalid': email && !isEmailValid}]" required />
          <div class="invalid-feedback" v-if="email && !isEmailValid">Ingrese un correo electrónico válido</div>
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-4">
          <button type="submit" class="btn btn-primary btn-lg btn-block">
            Completar Registro
          </button>
          <div v-if="showFormError" class="form-error-message alert alert-danger mt-3 mb-0">
            {{ formErrorMessage }}
          </div>
        </div>
      </div>
    </form>
  </div>
  <NotificationModal :title="titletext" :text="bodyTextHTML"
                     :visible="showNotification" @close="handleNotificationClose" />
</template>

<script>
  /* eslint-disable */
  import axios from 'axios';
  import NotificationModal from '../components/NotificationModal.vue';

  export default {
    name: 'RegisterEmployee',

    components: {
      NotificationModal
    },

    data() {
      return {
        name: '',
        lastname: '',
        personid: '',
        birthdate: '',
        sex: '',
        province: '',
        canton: '',
        othersigns: '',
        phonenumber: '',
        role: '',
        jobposition: '',
        contract: '',
        paycycle: '',
        brutesalary: '',
        startdate: '',
        bankaccount: '',
        email: '',
        startdateTouched: false,
        birthdateTouched: false,

        titletext: '',
        showNotification: false,
        attemptedSubmit: false,
        showFormError: false,
        formErrorMessage: 'Por favor rellene todos los espacios correctamente',

        sexOptions: [
          { label: 'Hombre', value: 'Hombre' },
          { label: 'Mujer', value: 'Mujer' }
        ],

        roleOptions: [
          { label: 'Empleado', value: 'Empleado' },
          { label: 'Administrador', value: 'Administrador' },
          { label: 'Supervisor', value: 'Supervisor' }
        ],

        contractOptions: [
          { label: 'Tiempo Completo', value: 'Tiempo Completo' },
          { label: 'Medio Tiempo', value: 'Medio Tiempo' },
          { label: 'Por Horas', value: 'Por Horas' },
          { label: 'Servicios Profesionales', value: 'Servicios Profesionales' }
        ],

        payCycleOptions: [
          { label: 'Mensual', value: 'Mensual' },
          { label: 'Quincenal', value: 'Quincenal' },
          { label: 'Bisemanal', value: 'Bisemanal' },
          { label: 'Semanal', value: 'Semanal' }
        ],
      };
    },

    computed: {
      isStartDateValid() {
        return this.isPastOrToday(this.startdate);
      },

      startdateClass() {
        return this.getDateClass(this.isStartDateValid, this.startdateTouched, this.startdate);
      },

      isBirthdateValid() {
        return this.isPastOrToday(this.birthdate);
      },

      birthdateClass() {
        return this.getDateClass(this.isBirthdateValid, this.birthdateTouched, this.birthdate);
      },
      // TODO: Obtain password from somewhere else, hardcoded for now
      bodyTextHTML() {
        return `<span>El usuario es: <strong>${this.email}</strong></span><br><span>La contraseña es: <strong>changeme</strong></span>`;
      },

      isNameValid() {
        const regex = /^[A-Za-zÁÉÍÓÚáéíóúÑñ ]{1,75}$/;
        return regex.test(this.name);
      },

      isLastnameValid() {
        const regex = /^[A-Za-zÁÉÍÓÚáéíóúÑñ ]{1,75}$/;;
        return regex.test(this.lastname);
      },

      isPersonidValid() {
        const regex = /^\d{2}-\d{4}-\d{4}$/;
        return regex.test(this.personid);
      },

      isProvinceValid() {
        const regex = /^[A-Za-zÁÉÍÓÚáéíóúÑñ ]{0,20}$/;
        return regex.test(this.province);
      },

      isCantonValid() {
        const regex = /^[A-Za-zÁÉÍÓÚáéíóúÑñ ]{0,50}$/;
        return regex.test(this.canton);
      },

      isPhonenumberValid() {
        const regex = /^\d{4}-\d{4}$/;
        return regex.test(this.phonenumber);
      },
      //
      isJobPositionValid() {
        const regex = /^[A-Za-zÁÉÍÓÚáéíóúÑñ ]{0,75}$/;
        return regex.test(this.jobposition);
      },

      isBruteSalaryValid() {
        const regex = /^\d+$/;
        return regex.test(this.brutesalary);
      },

      isBankAccountValid() {
        const regex = /^CR\d{20}$/;
        return regex.test(this.bankaccount);
      },

      isEmailValid() {
        const regex = /^[\w.-]+@([\w-]+\.)+[\w-]{2,4}$/;
        return regex.test(this.email);
      },

      isSexValid() {
        return this.sex !== '';
      },
      isRoleValid() {
        return this.role !== '';
      },
      isContractValid() {
        return this.contract !== '';
      },
      isPayCycleValid() {
        return this.paycycle !== '';
      },
      isFormValid() {
        return (
          this.isNameValid &&
          this.isLastnameValid &&
          this.isPersonidValid &&
          this.isSexValid &&
          this.isBirthdateValid &&
          this.isProvinceValid &&
          this.isCantonValid &&
          (this.phonenumber ? this.isPhonenumberValid : true) &&
          this.isRoleValid &&
          this.isJobPositionValid &&
          this.isContractValid &&
          this.isPayCycleValid &&
          this.isBruteSalaryValid &&
          this.isStartDateValid &&
          this.isBankAccountValid &&
          this.isEmailValid
        );
      },
    },
    methods: {
      saveEmployee() {
        this.attemptedSubmit = true;
        this.showFormError = false;

        if (!this.isFormValid) {
          this.showFormError = true;
          return;
        }

        axios.post('https://localhost:7153/api/RegisterEmployee/registerEmployee', {
          name: this.name,
          lastname: this.lastname,
          personid: this.personid,
          sex: this.sex,
          birthdate: this.birthdate,
          province: this.province,
          canton: this.canton,
          othersigns: this.othersigns,
          phonenumber: this.phonenumber,
          role: this.role,
          jobposition: this.jobposition,
          contract: this.contract,
          paycycle: this.paycycle,
          brutesalary: this.brutesalary,
          startdate: this.startdate,
          bankaccount: this.bankaccount,
          email: this.email
        })
          .then(response => {
            console.log("Empleado registrado con éxito", response);
            this.titletext = "Empleado registrado con éxito";

            this.showNotification = true;
          })
          .catch(error => {
            console.error("Error registrando empleado", error);
            this.showFormError = true;
          });
      },

      isPastOrToday(dateStr) {
        if (!dateStr) return true;
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        const selectedDate = new Date(dateStr);
        selectedDate.setHours(0, 0, 0, 0);
        return selectedDate <= today;
      },

      getDateClass(isValid, isTouched, value) {
        return {
          'is-invalid': !isValid && isTouched,
          'text-muted': value === ''
        };
      },

      handleNotificationClose() {
        this.showNotification = false;
        this.$router.push('/');
      }
    }
  }
</script>

<style scoped>
  .page {
    background-color: #fff;
    text-align: center;
    margin-top: 0.5rem;
    margin-bottom: 1rem;
  }

  h1 {
    text-align: center;
    color: #003c63;
    font-weight: bold;
  }

  label {
    text-align: left;
    color: #003c63;
    display: block;
  }

  .btn-primary {
    background-color: #003c63;
    border-color: #003c63;
    font-weight: bold;
  }

  .form-label-input {
    margin-bottom: 0.5rem;
  }

  .form-check-label {
    margin-left: 0.5rem;
  }

  input[type="date"].text-muted {
    color: #6c757d;
  }

  input[type="date"].is-invalid {
    color: #6c757d;
    padding-right: 2.5rem;
  }

  .mb-3 {
    margin: 0 auto;
    background: white;
    border-radius: 20px;
  }

  .form-control {
    border-radius: 10px;
    padding: 0.75rem;
    background-color: #f2f2f2;
    border: 1px solid #f2f2f2;
    transition: box-shadow 0.2s ease;
    margin-bottom: 1rem;
  }

    .form-control:focus {
      outline: none;
      border-color: #aaa;
      box-shadow: 0 0 0 2px rgba(0, 60, 99, 0.15);
    }

    .form-control.is-invalid {
      border-color: #dc3545;
    }

      .form-control.is-invalid:focus {
        box-shadow: 0 0 0 2px rgba(235, 12, 12, 0.25);
      }
</style>
