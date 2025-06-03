<template>
  <div class="contenedor-principal">
    <h1 class="text-center">Registro de horas {{ payTypeText }}</h1>


    <!-- Register table -->
    <div v-if="registers.length > 0" class="table-responsive" style="max-height: 300px; overflow-y: auto;">
      <table class="table table-hover">
        <thead>
          <tr>
            <th>{{ payTypeTextShort }}</th>
            <th>Horas Trabajadas</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody class="table-group-divider">
          <tr v-for="(register, index) in registers" :key="index">
            <td>{{ periodFormatting(register.fechaInicio, register.fechaFin) }}</td>
            <td>{{ register.horas }}</td>
            <td>
              <button v-if="!register.enRevision"
                      class="btn btn-sm btn-revision me-2"
                      @click="sendReviewing(index)">
                Enviar a revisión
              </button>
              <span v-else
                    class="btn btn-sm"
                    :class="{
                            'btn-success': register.status === 'Approved',
                            'btn-danger': register.status === 'Rejected',
                            'btn btn-sm': register.status === 'Waiting'
                          }">
                                        {{
                            register.status === 'Approved'
                              ? 'Aprobado'
                              : register.status === 'Rejected'
                                ? 'Rechazado'
                                : 'En revisión'
                                        }}
              </span>




              <button class="btn btn-sm btn-outline-dark"
                      @click="eliminarRegistro(index)"
                      v-show="!register.enRevision">
                <span class="material-icons">delete</span>
              </button>

            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Message if there are no registers -->
    <div v-else class="text-muted text-center my-4">
      No hay registros pendientes.
    </div>

    <!-- Button to add a new register -->
    <div class="text-start mt-3">
      <button class="btn btn-primary" @click="mostrarFormulario = !mostrarFormulario">
        Añadir registro
      </button>
    </div>

    <!-- Form to add a new register -->
    <div v-if="mostrarFormulario" class="mt-3">
      <div class="row g-3 align-items-end">
        <div class="col-auto">
          <label>{{ payTypeTextShort }}</label>
          <flat-pickr class="form-control"
                      v-model="nuevaFecha"
                      :config="{
                  locale: Spanish,
                  dateFormat: 'd/m/Y',
                  allowInput: true
                }"
                      @on-change="adjustDate" />
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
          <button class="btn btn-success" @click="confirmRegistry">Confirmar</button>
          <div v-if="warningMessage" class="text-danger mt-2">
            {{ warningMessage }}
          </div>
        </div>


      </div>
      <div class="text-muted mt-1" v-if="fechaInicio && fechaFin">
        Rango ajustado: {{ periodFormatting(fechaInicio, fechaFin) }}
      </div>
    </div>
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
        registers: [],
        approvalID: null,
        status: null,
        mostrarFormulario: false,
        nuevaFecha: null,
        nuevasHoras: null,
        fechaInicio: null,
        fechaFin: null,

        userPK: null,
        registersHours: null,
        payrollType: null,
        empID: null,
        fechaInicioTrabajo: null,
        nombreEmpleado: null,
        apellidoEmpleado: null,
        warningMessage: null,


      };
    },
    components: {
      flatPickr,
    },
    async mounted() {
      await this.obtainUserInfo();
      if (this.empID) {
        await this.obtainApprovedRegisters();
      }
    },

    computed: {
      payTypeText() {
        switch (this.payrollType) {
          case 'W':
            return 'semanal';
          case 'B':
            return 'quincenal';
          case 'M':
            return 'mensual';
          default:
            return 'semanal';
        }
      },

       payTypeTextShort() {
        switch (this.payrollType) {
          case 'W':
            return 'Semana';
          case 'B':
            return 'Quincena';
          case 'M':
            return 'Mes';
          default:
            return 'Semana'; 
        }
      }
    },
    methods: {
      adjustDate(selectedDates) {
        const seleccion = selectedDates[0];
        if (!seleccion) return;

        const año = seleccion.getFullYear();
        const mes = seleccion.getMonth();
        const dia = seleccion.getDate();

        if (this.payrollType === 'M') {
          const inicio = new Date(año, mes, 1);
          const fin = new Date(año, mes + 1, 0);

          this.fechaInicio = inicio.toISOString().split('T')[0];
          this.fechaFin = fin.toISOString().split('T')[0];

          if (this.registersHours === false) {
            let horas = 0;
            const inicioReal = new Date(inicio);
            const finReal = new Date(fin);
            const ingreso = new Date(this.fechaInicioTrabajo);

            if (ingreso > inicioReal && ingreso <= finReal) {
              inicioReal.setTime(ingreso.getTime());
            }

            for (let d = new Date(inicioReal); d <= finReal; d.setDate(d.getDate() + 1)) {
              if (d.getDay() !== 0) horas += 8;
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
            const inicioReal = new Date(inicio);
            const finReal = new Date(fin);
            const ingreso = new Date(this.fechaInicioTrabajo);


            if (ingreso > inicioReal && ingreso <= finReal) {
              inicioReal.setTime(ingreso.getTime());
            }

            for (let d = new Date(inicioReal); d <= finReal; d.setDate(d.getDate() + 1)) {
              if (d.getDay() !== 0) horas += 8;
            }
            this.nuevasHoras = horas;
          }

        } else {
          const day = seleccion.getDay();
          const monday = new Date(seleccion);
          monday.setDate(seleccion.getDate() - ((day + 6) % 7));
          const sunday = new Date(monday);
          sunday.setDate(monday.getDate() + 6);

          this.fechaInicio = monday.toISOString().split('T')[0];
          this.fechaFin = sunday.toISOString().split('T')[0];

          if (this.registersHours === false) {
            let horas = 0;
            const inicioReal = new Date(inicio);
            const finReal = new Date(fin);
            const ingreso = new Date(this.fechaInicioTrabajo);

            if (ingreso > inicioReal && ingreso <= finReal) {
              inicioReal.setTime(ingreso.getTime());
            }

            for (let d = new Date(inicioReal); d <= finReal; d.setDate(d.getDate() + 1)) {
              if (d.getDay() !== 0) horas += 8; 
            }
            this.nuevasHoras = horas;
          }
        }
      },
      async confirmRegistry() {
        if (!this.fechaInicio || !this.nuevasHoras) return;

        const fechaActual = new Date(this.fechaInicio);


        const yaExiste = this.registers.some(reg => reg.fechaInicio === this.fechaInicio);
        if (yaExiste) {
          this.showWarning('Ya existe un registro para este período.');
          return;
        }

        const fechasRegistradas = this.registers.map(r => new Date(r.fechaInicio));
        if (fechasRegistradas.length > 0) {
          const maxFechaRegistrada = new Date(Math.max(...fechasRegistradas));
          if (fechaActual <= maxFechaRegistrada) {
            this.showWarning('No puedes registrar períodos anteriores al último registrado.');
            return;
          }
        }

        const fechaInicioTrabajo = new Date(this.fechaInicioTrabajo);
        const fechaFinSeleccionada = new Date(this.fechaFin);

        if (fechaFinSeleccionada < fechaInicioTrabajo) {
          this.showWarning('No puedes registrar horas para un período anterior a tu fecha de ingreso.');
          return;
        }



        try {
          const payload = {
            empID: this.empID,
            startDate: this.fechaInicio,
            endDate: this.fechaFin,
            hoursWorked: this.nuevasHoras,
            isSentForApproval: false
          };
          console.log('Payload enviado:', payload);

          await axios.post(`${import.meta.env.VITE_API_URL}/api/ApprovedHours`, payload);

          this.registers.push({
            fechaInicio: this.fechaInicio,
            fechaFin: this.fechaFin,
            horas: this.nuevasHoras,
            enRevision: false,
          });

          this.obtainApprovedRegisters();

          this.resetForm();
        } catch (error) {
          console.error("Error al guardar en el backend:", error);
          this.showWarning("Hubo un error al guardar los datos.");
        }
      }
,
      async sendReviewing(index) {
        const register = this.registers[index];

        try {
          const payload = {
            status: "Waiting",
            isSentForApproval: true
          };


          await axios.patch(`${import.meta.env.VITE_API_URL}/api/ApprovedHours/${register.approvalID}`, payload);

          this.registers[index].enRevision = true;
        } catch (error) {
          console.error("❌ Error al enviar a revisión:", error);
          this.showWarning("No se pudo enviar a revisión.");
        }
      },
      eliminarRegistro(index) {
        if (!this.registers[index].enRevision) {
          this.registers.splice(index, 1);
        }
      },
      resetForm() {
        this.nuevaFecha = null;
        this.nuevasHoras = null;
        this.fechaInicio = null;
        this.fechaFin = null;
        this.mostrarFormulario = false;
      },
      periodFormatting(inicio, fin) {

        const fechaIngreso = new Date(this.fechaInicioTrabajo);
        const fechaInicioPeriodo = new Date(inicio);
        const fechaFinPeriodo = new Date(fin);

        let texto = `${this.dateFormatting(inicio)} - ${this.dateFormatting(fin)}`;

        if (fechaIngreso > fechaInicioPeriodo && fechaIngreso <= fechaFinPeriodo) {
          texto += ` (se contabiliza desde el ingreso: ${this.dateFormatting(this.fechaInicioTrabajo)})`;
        }

        return texto;
      },
      dateFormatting(fecha) {

        const soloFecha = fecha.split('T')[0];
        const [a, m, d] = soloFecha.split('-');
        return `${d}/${m}/${a.slice(2)}`;
      }
,
      blockArrows(event) {
        if (event.key === 'ArrowUp' || event.key === 'ArrowDown') {
          event.preventDefault();
        }
      },

      validarHoras(event) {
        const valor = parseInt(event.target.value, 10);

        let maxHoras = 48;

        if (this.payrollType === "M") {
          maxHoras = 192;
        } else if (this.payrollType === "B") {
          maxHoras = 96;
        } else if (this.payrollType === "W") {
          maxHoras = 48;
        }

        if (valor > maxHoras) {
          this.nuevasHoras = maxHoras;
          this.warningMessage = `El máximo permitido para tipo ${this.payrollType} es ${maxHoras} horas.`;
        } else if (valor < 1) {
          this.nuevasHoras = 1;
          this.warningMessage = "El mínimo de horas permitidas es 1.";
        } else {
          this.nuevasHoras = valor;
          this.warningMessage = null;
        }
      },

      showWarning(msg) {
        this.warningMessage = msg;
        setTimeout(() => {
          this.warningMessage = null;
        }, 5000);
      },
      async obtainUserInfo() {
        try {
          const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/Auth/userinfo`);
          const data = response.data;


          this.userPK = data.userPK;
          this.registersHours = data.registersHours;
          this.payrollType = data.payrollType;
          this.empID = data.empID; 
          this.fechaInicioTrabajo = data.startDate.split('T')[0];
          this.nombreEmpleado = data.name;
          this.apellidoEmpleado = data.lastName;


        } catch (error) {
          console.error("Error al obtener información del usuario:", error);
        }
      },
      async obtainApprovedRegisters() {
        try {
          const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/ApprovedHours/approved-hours/${this.empID}`);
          const data = response.data;


          this.registers = data.map(r => ({
            approvalID: r.approvalID, 
            fechaInicio: r.startDate.split('T')[0],
            fechaFin: r.endDate.split('T')[0],
            horas: r.hoursWorked,
            enRevision: r.isSentForApproval,
            status: r.status

          }));

        } catch (error) {
          console.error("Error al obtener registros aprobados:", error);
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
