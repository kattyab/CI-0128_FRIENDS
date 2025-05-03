<template>
  <div class="page container-fluid">
    <h1>Registrar Empleado</h1>
    <form class="has-validation" @submit.prevent="saveEmployee">
      <div class="row">
        <div class="mb-3 col-10">
          <label for="name" class="form-label">Nombre</label>
          <input type="text" class="form-control" id="name" placeholder="Ingrese su nombre" v-model="name" />
        </div>
      </div>
      <div class="row">
        <div class="mb-3 col-10">
          <label for="lastname" class="form-label">Apellidos</label>
          <input type="text" class="form-control" id="lastname" placeholder="Ingrese sus apellidos"
                 v-model="lastname" />
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="personid" class="form-label">Cédula de Identidad</label>
          <input type="text" class="form-control" id="personid" placeholder="Ingrese el número de cédula"
                 v-model="personid" />
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
        </div>
      </div>

      <!--include birthdate space-->
      <div class="row">
        <div class="mb-3 col-10">
          <label for="birthdate" class="form-label">Fecha de Nacimiento</label>
          <input type="date" class="form-control" id="birthdate" v-model="birthdate" :class="birthdateClass"
                 @change="birthdateTouched = true" />
          <div class="invalid-feedback" v-if="birthdateTouched && !isBirthdateValid">Seleccione una fecha válida.</div>
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="province" class="form-label">Provincia</label>
          <input type="text" class="form-control" id="province" placeholder="Ingrese la provincia de residencia"
                 v-model="province" />
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="canton" class="form-label">Cantón</label>
          <input type="text" class="form-control" id="canton" placeholder="Ingrese el cantón de residencia"
                 v-model="canton" />
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="othersigns" class="form-label">Otras señales</label>
          <input type="text" class="form-control" id="othersigns" placeholder="Opcional: Ingrese otras señales de residencia"
                 v-model="othersigns" />
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
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="jobposition" class="form-label">Puesto</label>
          <input type="text" class="form-control" id="jobposition" placeholder="Ingrese el nombre del puesto"
                 v-model="jobposition" />
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label class="form-label">Tipo de Contrato</label>
          <div class="row">
            <div class="col form-check d-flex
                        align-items-center
                        justify-content-center" v-for="option in contractOptions" :key="option.value">
              <input class="form-check-input" type="radio" :id="option.value" :value="option.value" v-model="contract"
                     name="contractOptions" />
              <label class="form-check-label" :for="option.value">
                {{ option.label }}
              </label>
            </div>
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
              <input class="form-check-input" type="radio" :id="option.value" :value="option.value" v-model="paycicle"
                     name="payCycleOptions" />
              <label class="form-check-label" :for="option.value">
                {{ option.label }}
              </label>
            </div>
          </div>
        </div>
      </div>

      <div class="row has-validation">
        <div class="mb-3 col-10">
          <label for="brutesalary" class="{ 'is-invalid': brutesalary && !isBruteSalaryValid } form-label">
            Salario
            Bruto
          </label>
          <input type="text" class="form-control" id="brutesalary" placeholder="Ingrese el salario bruto"
                 v-model="brutesalary" :class="{ 'is-invalid': brutesalary && !isBruteSalaryValid }" required />
          <div class="invalid-feedback" v-if="brutesalary && !isBruteSalaryValid">
            El salario debe ser un número entero sin decimales.
          </div>
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="startdate" class="form-label">Fecha de Inicio</label>
          <input type="date" class="form-control" id="startdate" v-model="startdate" :class="startdateClass"
                 @change="startdateTouched = true" />
          <div class="invalid-feedback" v-if="startdateTouched && !isStartDateValid">Seleccione una fecha válida.</div>
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="bankaccount" class="form-label">Cuenta Bancaria</label>
          <input type="text" class="form-control" id="bankaccount" placeholder="Ingrese el número de cuenta bancaria"
                 v-model="bankaccount" />
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col-10">
          <label for="email" class="form-label">Correo</label>
          <input type="text" class="form-control" id="email" placeholder="Ingrese el correo electrónico" v-model="email" />
        </div>
      </div>

      <div class="row">
        <div class="mb-3 col">
          <button type="submit" class="btn btn-primary btn-lg btn-block">
            Completar Registro
          </button>
        </div>
      </div>
    </form>
  </div>
</template>

<!-- <script setup>

// export default {
//   name: 'AddEmployeeComponent',
//   data() {
//     return {

//     };
//   },
// };

import { ref, computed } from 'vue';
import axios from axios;



const name = ref('');
const lastname = ref('');
const personid = ref('');
const role = ref('');
const jobposition = ref('');
const contract = ref('');
const paycicle = ref('');
const brutesalary = ref('');
const startdate = ref('');
const bankaccount = ref('');
const email = ref('');

const startdateTouched = ref(false);

const roleOptions = [
  { label: 'Empleado', value: 'employee' },
  { label: 'Administrador', value: 'administrator' },
  { label: 'Supervisor', value: 'supervisor' }
];

const contractOptions = [
  { label: 'Tiempo Completo', value: 'full-time' },
  { label: 'Medio Tiempo', value: 'part-time' },
  { label: 'Por Horas', value: 'by-hours' },
  { label: 'Servicios Profesionales', value: 'profesional-services' }
]

const payCycleOptions = [
  { label: 'Mensual', value: 'monthly' },
  { label: 'Bisemanal', value: 'biweekly' },
  { label: 'Semanal', value: 'weekly' }
]

const isStartDateValid = computed(() => {
  if (!startdate.value) return false;
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  const selectedDate = new Date(startdate.value);
  selectedDate.setHours(0, 0, 0, 0);
  return selectedDate <= today;
})

const startdateClass = computed(() => {
  return {
    'is-invalid': !isStartDateValid.value && startdateTouched.value,
    'text-muted': startdate.value === '',
  };
});

const isBruteSalaryValid = computed(() => {
  // eslint-disable-next-line
  const regex = /^\d+(\,\d{1,2})?$/;
  return regex.test(brutesalary.value);
});
</script> -->

<script>
  /* eslint-disable */
   import axios from 'axios';

  export default {
    name: 'RegisterEmployee',

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
        role: '',
        jobposition: '',
        contract: '',
        paycicle: '',
        brutesalary: '',
        startdate: '',
        bankaccount: '',
        email: '',
        startdateTouched: false,
        birthdateTouched: false,

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
        ]
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

      isBruteSalaryValid() {
        // eslint-disable-next-line
        const regex = /^\d+$/;
        return regex.test(this.brutesalary);
      }
    },

    methods: {
      saveEmployee() {
        axios.post('https://localhost:7214/api/RegisterEmployee/registerEmployee', {
          name: this.name,
          lastname: this.lastname,
          personid: this.personid,
          sex: this.sex,
          birthdate: this.birthdate,
          province: this.province,
          canton: this.canton,
          othersigns: this.othersigns,
          role: this.role,
          jobposition: this.jobposition,
          contract: this.contract,
          paycicle: this.paycicle,
          brutesalary: this.brutesalary,
          startdate: this.startdate,
          bankaccount: this.bankaccount,
          email: this.email
        })
          .then(response => {
            console.log("Empleado registrado con éxito", response);
            // Optionally redirect
          })
          .catch(error => {
            console.error("Error registrando empleado", error);
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
      }
    }
  };
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
