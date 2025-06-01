<template>
  <div class="contenedor-principal">
    <h1 class="text-center">Registro de horas {{ tipoPagoTexto }}</h1>


    <!-- Tabla de registros -->
    <div v-if="registros.length > 0" class="table-responsive" style="max-height: 300px; overflow-y: auto;">
      <table class="table table-hover">
        <thead>
          <tr>
            <th>{{ tipoPagoTextoCorto }}</th>
            <th>Horas Trabajadas</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody class="table-group-divider">
          <tr v-for="(registro, index) in registros" :key="index">
            <td>{{ formatoSemana(registro.fechaInicio, registro.fechaFin) }}</td>
            <td>{{ registro.horas }}</td>
            <td>
              <button v-if="!registro.enRevision"
                      class="btn btn-sm btn-revision me-2"
                      @click="enviarRevision(index)">
                Enviar a revisión
              </button>
              <span v-else
                    class="btn btn-sm btn-
                    disabled">
                En revisión
              </span>

              <button class="btn btn-sm btn-outline-dark"
                      @click="eliminarRegistro(index)"
                      v-show="!registro.enRevision">
                <span class="material-icons">delete</span>
              </button>

            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Mensaje si no hay registros -->
    <div v-else class="text-muted text-center my-4">
      No hay registros pendientes.
    </div>

    <!-- Botón para añadir registro -->
    <div class="text-start mt-3">
      <button class="btn btn-primary" @click="mostrarFormulario = !mostrarFormulario">
        Añadir registro
      </button>
    </div>

    <!-- Formulario para añadir nuevo registro -->
    <div v-if="mostrarFormulario" class="mt-3">
      <div class="row g-3 align-items-end">
        <div class="col-auto">
          <label>{{ tipoPagoTextoCorto }}</label>
          <flat-pickr class="form-control"
                      v-model="nuevaFecha"
                      :config="{
                  locale: Spanish,
                  dateFormat: 'd/m/Y',
                  allowInput: true
                }"
                      @on-change="ajustarSemana" />
        </div>

        <div class="col-auto">
          <label>Horas Trabajadas</label>

          <div v-if="registersHours">
            <input type="number"
                   class="form-control"
                   v-model.number="nuevasHoras"
                   :max="48"
                   :min="1"
                   @keydown="blockArrows"
                   @input="validarHoras" />
          </div>

          <div v-else class="form-control-plaintext">
            {{ nuevasHoras }}
          </div>
        </div>

        <div class="col-auto">
          <button class="btn btn-success" @click="confirmarRegistro">Confirmar</button>
          <div v-if="mensajeAdvertencia" class="text-danger mt-2">
            {{ mensajeAdvertencia }}
          </div>
        </div>

      </div>
      <div class="text-muted mt-1" v-if="fechaInicio && fechaFin">
        Rango ajustado: {{ formatoSemana(fechaInicio, fechaFin) }}
      </div>
    </div>
  </div>
  <div class="mt-3">
    <p><strong>UserPK:</strong> {{ userPK }}</p>
    <p><strong>RegistersHours:</strong> {{ registersHours }}</p>
    <p><strong>PayrollType:</strong> {{ payrollType }}</p>
  </div>
</template>

<script>
  import axios from 'axios';
  import flatPickr from 'vue-flatpickr-component';
  import 'flatpickr/dist/flatpickr.min.css';
  import { Spanish } from 'flatpickr/dist/l10n/es.js';

  export default {
    data() {
      return {
        registros: [],
        mostrarFormulario: false,
        nuevaFecha: null,
        nuevasHoras: null,
        fechaInicio: null,
        fechaFin: null,
        // nuevas variables para mostrar datos del usuario
        userPK: null,
        registersHours: null,
        payrollType: null,
        mensajeAdvertencia: null,

      };
    },
    components: {
      flatPickr,
    },
    mounted() {
      this.obtenerInfoUsuario();
    },
    computed: {
      tipoPagoTexto() {
        switch (this.payrollType) {
          case 'W':
            return 'semanal';
          case 'B':
            return 'quincenal';
          case 'M':
            return 'mensual';
          default:
            return 'semanal'; // o cualquier texto por defecto
        }
      },

       tipoPagoTextoCorto() {
        switch (this.payrollType) {
          case 'W':
            return 'Semana';
          case 'B':
            return 'Quincena';
          case 'M':
            return 'Mes';
          default:
            return 'Semana'; // o cualquier texto por defecto
        }
      }
    },
    methods: {
      ajustarSemana(selectedDates) {
        const seleccion = selectedDates[0];
        if (!seleccion) return;

        const año = seleccion.getFullYear();
        const mes = seleccion.getMonth(); // 0-indexed
        const dia = seleccion.getDate();

        if (this.payrollType === 'M') {
          const inicio = new Date(año, mes, 1);
          const fin = new Date(año, mes + 1, 0); // último día del mes

          this.fechaInicio = inicio.toISOString().split('T')[0];
          this.fechaFin = fin.toISOString().split('T')[0];

          if (this.registersHours === false) {
            let horas = 0;
            for (let d = new Date(inicio); d <= fin; d.setDate(d.getDate() + 1)) {
              if (d.getDay() !== 0) horas += 8; // excluir domingos
            }
            this.nuevasHoras = horas;
          }

        } else if (this.payrollType === 'B') {
          let inicio, fin;
          const ultimoDia = new Date(año, mes + 1, 0);

          if (dia <= 15) {
            inicio = new Date(año, mes, 1);
            fin = new Date(año, mes, 15);
          } else {
            inicio = new Date(año, mes, 16);
            fin = ultimoDia;
          }

          this.fechaInicio = inicio.toISOString().split('T')[0];
          this.fechaFin = fin.toISOString().split('T')[0];

          if (this.registersHours === false) {
            let horas = 0;
            for (let d = new Date(inicio); d <= fin; d.setDate(d.getDate() + 1)) {
              if (d.getDay() !== 0) horas += 8;
            }
            this.nuevasHoras = horas;
          }

        } else {
          // Caso 'W' por defecto (semanal)
          const day = seleccion.getDay(); // domingo = 0
          const monday = new Date(seleccion);
          monday.setDate(seleccion.getDate() - ((day + 6) % 7));
          const sunday = new Date(monday);
          sunday.setDate(monday.getDate() + 6);

          this.fechaInicio = monday.toISOString().split('T')[0];
          this.fechaFin = sunday.toISOString().split('T')[0];

          if (this.registersHours === false) {
            let horas = 0;
            for (let i = 0; i < 7; i++) {
              const dia = new Date(monday);
              dia.setDate(monday.getDate() + i);
              if (dia.getDay() !== 0) horas += 8;
            }
            this.nuevasHoras = horas;
          }
        }
      },
      confirmarRegistro() {
        if (!this.fechaInicio || !this.nuevasHoras) return;

        const fechaActual = new Date(this.fechaInicio);

        // Validación 1: No permitir duplicados (fechaInicio ya existe)
        const yaExiste = this.registros.some(reg => reg.fechaInicio === this.fechaInicio);
        if (yaExiste) {
          this.mostrarAdvertencia('Ya existe un registro para este período.');
          return;
        }

        // Validación 2: No permitir períodos anteriores al último registrado
        const fechasRegistradas = this.registros.map(r => new Date(r.fechaInicio));
        if (fechasRegistradas.length > 0) {
          const maxFechaRegistrada = new Date(Math.max(...fechasRegistradas));
          if (fechaActual <= maxFechaRegistrada) {
            this.mostrarAdvertencia('No puedes registrar períodos anteriores al último registrado.');
            return;
          }
        }

        // Si pasa las validaciones, se agrega el registro
        this.registros.push({
          fechaInicio: this.fechaInicio,
          fechaFin: this.fechaFin,
          horas: this.nuevasHoras,
          enRevision: false,
        });

        this.resetFormulario();
      },
      enviarRevision(index) {
        this.registros[index].enRevision = true;
      },
      eliminarRegistro(index) {
        if (!this.registros[index].enRevision) {
          this.registros.splice(index, 1);
        }
      },
      resetFormulario() {
        this.nuevaFecha = null;
        this.nuevasHoras = null;
        this.fechaInicio = null;
        this.fechaFin = null;
        this.mostrarFormulario = false;
      },
      formatoSemana(inicio, fin) {
        return `${this.formatoFecha(inicio)} - ${this.formatoFecha(fin)}`;
      },
      formatoFecha(fecha) {
        const [a, m, d] = fecha.split('-');
        return `${d}/${m}/${a.slice(2)}`;
      },
      blockArrows(event) {
        // Evita flechas ↑ ↓ del teclado
        if (event.key === 'ArrowUp' || event.key === 'ArrowDown') {
          event.preventDefault();
        }
      },
      validarHoras(event) {
        const valor = parseInt(event.target.value, 10);
        if (valor > 48) {
          this.nuevasHoras = 48;
        } else if (valor < 1) {
          this.nuevasHoras = 1;
        }
      },
      mostrarAdvertencia(msg) {
        this.mensajeAdvertencia = msg;
        setTimeout(() => {
          this.mensajeAdvertencia = null;
        }, 5000);
      },
      async obtenerInfoUsuario() {
        try {
          const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/Auth/userinfo`);
          const data = response.data;

          console.log("User info:", data);

          // Aquí puedes guardarlos en variables reactivas si quieres usarlos en el template
          this.userPK = data.userPK;
          this.registersHours = data.registersHours;
          this.payrollType = data.payrollType;
        } catch (error) {
          console.error("Error al obtener información del usuario:", error);
        }
      }
    },
  };
</script>

<style scoped>
  .material-icons {
    font-size: 20px;
    vertical-align: middle;
  }

  .contenedor-principal {
    max-width: 1300px;
    margin: 0 auto;
    padding: 25px;
  }

  .btn-primary {
    background-color: #003c63;
    border-color: #003c63;
    font-weight: bold;
  }

  .btn-revision {
    background-color: #00796B;
    color: white;
    font-family: 'Arial';
    font-weight: bold;
    border: none;
  }
</style>
