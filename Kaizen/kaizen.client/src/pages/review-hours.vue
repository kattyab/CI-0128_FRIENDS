<template>
  <div class="container mt-4">
    <h2 class="text-center mb-4">Aprobación de horas</h2>


    <div class="row g-3 mb-3">
      <div class="col-md-3">
        <label>Usuario</label>
        <select class="form-select" v-model="filtros.usuario">
          <option value="">Todos</option>
          <option v-for="user in usuariosUnicos" :key="user">{{ user }}</option>
        </select>
      </div>


      <div class="col-md-3">
        <label>Desde</label>
        <Flatpickr class="form-control"
                   v-model="filtros.desde"
                   :config="flatpickrConfig" />
      </div>


      <div class="col-md-3">
        <label>Hasta</label>
        <Flatpickr class="form-control"
                   v-model="filtros.hasta"
                   :config="flatpickrConfig" />
      </div>

      <div class="col-md-3">
        <label>Estado</label>
        <select class="form-select" v-model="filtros.estado">

          <option value="">Todos</option>
          <option value="Aprobado">Aprobado</option>
          <option value="Pendiente">Pendiente</option>
          <option value="Rechazado">Rechazado</option>
        </select>
      </div>
    </div>


    <div class="table-responsive" style="max-height: 350px; overflow-y: auto;">
      <table class="table table-hover align-middle w-100">
        <thead class="table-light">
          <tr>
            <th class="text-start" style="width: 25%;">Nombre</th>
            <th class="text-center" style="width: 25%;">Rango de fechas</th>
            <th class="text-center" style="width: 15%;">Horas</th>
            <th class="text-end pe-4" style="width: 20%;">Estado</th>
            <th class="text-end pe-4" style="width: 15%;"></th>
          </tr>
        </thead>
        <tbody>
          <template v-for="(registro, index) in registrosFiltrados" :key="index">
            <tr @click="toggleDetalles(index)" style="cursor: pointer;">
              <td class="text-start">{{ registro.nombre }}</td>
              <td class="text-center">{{ dateFormatting(registro.inicio) }} - {{ dateFormatting(registro.fin) }}</td>
              <td class="text-center">{{ registro.horas }}</td>
              <td class="text-end pe-4">
                <span class="btn btn-sm"
                      :class="{
      'btn-success': registro.estado === 'Approved',
      'btn-danger': registro.estado === 'Rejected',
      'btn-secondary': registro.estado === 'Waiting'
    }">
                  {{
      registro.estado === 'Approved'
        ? 'Aprobado'
        : registro.estado === 'Rejected'
          ? 'Rechazado'
          : 'Pendiente'
                  }}
                </span>
              </td>
              <td class="text-end pe-4">
                <input type="checkbox"
                       v-model="registro.seleccionado"
                       :disabled="registro.estado === 'Approved' || registro.estado === 'Rejected'"
                       @click.stop />
              </td>
            </tr>
            <tr v-if="registro.mostrarDetalles" class="detalle-fila">
              <td>
                <small><strong>Fecha Inicio:</strong> {{ dateFormatting(registro.fechaInicioCompleta) }}</small>
              </td>
              <td class="text-center">
                <small><strong>Tipo de pago:</strong> {{ registro.tipoPago }}</small>
              </td>
              <td class="text-center">
                <small><strong>Tipo de contrato:</strong> {{ registro.tipoContrato }}</small>
              </td>
              <td colspan="2"></td>
            </tr>
          </template>
        </tbody>
      </table>
    </div>


    <div class="mt-3 d-flex justify-content-center gap-3">
      <button class="btn btn-primary" @click="() => actualizarEstadoDeSeleccionados('Approved')">Aprobar</button>
      <button class="btn btn-danger" @click="() => actualizarEstadoDeSeleccionados('Rejected')">Rechazar</button>
    </div>


  </div>
</template>

<script setup>
  import { ref, computed, onMounted } from 'vue'
  import axios from 'axios'
  import Flatpickr from 'vue-flatpickr-component'
  import 'flatpickr/dist/flatpickr.css'


  const filtros = ref({
    desde: null,
    hasta: null,
    usuario: '',
    estado: ''
  })

  const flatpickrConfig = {
    dateFormat: 'Y-m-d',
    altInput: true,
    altFormat: 'd-m-Y',
    locale: 'es'
  }


  const registros = ref([])

  async function fetchRegistros() {
    try {
      const { data } = await axios.get(`${import.meta.env.VITE_API_URL}/api/ApprovedHours/AllHours`)
      registros.value = data.map(item => ({
        approvalID: item.approvalID, 
        isSentForApproval: item.isSentForApproval, 
        nombre: `${item.name} ${item.lastName}`,
        inicio: item.startDate,
        fin: item.endDate,
        horas: item.hoursWorked,
        estado: item.status || 'Pendiente',
        fechaInicioCompleta: item.employeeStartDate,
        tipoPago: item.payrollType,
        tipoContrato: item.contractType,
        mostrarDetalles: false,
        seleccionado: false
      }))
    } catch (error) {
      console.error('Error al cargar datos de ApprovedHours:', error)
    }
  }

  async function actualizarEstadoDeSeleccionados(nuevoEstado) {
    const seleccionados = registros.value.filter(r =>
      r.seleccionado &&
      r.estado === 'Waiting' &&
      r.isSentForApproval === true
    )

    if (seleccionados.length === 0) {
      alert('No hay registros seleccionados para procesar.')
      return
    }

    try {
      await Promise.all(seleccionados.map(async r => {
        await axios.patch(`${import.meta.env.VITE_API_URL}/api/ApprovedHours/${r.approvalID}/status`, {
          status: nuevoEstado
        })

      }))
      await fetchRegistros() 
    } catch (error) {
      console.error(`Error al actualizar registros:`, error)
      alert('Ocurrió un error al actualizar los registros.')
    }
  }



  onMounted(fetchRegistros)


  const usuariosUnicos = computed(() => {
    return [...new Set(registros.value.map(r => r.nombre))]
  })


  function dateFormatting(fecha) {
    const soloFecha = fecha.split('T')[0]
    const [a, m, d] = soloFecha.split('-')
    return `${d}/${m}/${a.slice(2)}`
  }

  function formatearFecha(fecha) {
    const [year, month, day] = fecha.split('-')
    return `${day}/${month}/${year}`
  }



  const registrosFiltrados = computed(() => {
    return registros.value.filter(r => {

      const condicionesEstado = r.isSentForApproval === true



      const usuarioOk = !filtros.value.usuario || r.nombre === filtros.value.usuario
      const estadoOk = !filtros.value.estado || traducirEstado(r.estado) === filtros.value.estado

      const inicio = new Date(r.inicio)
      const fin = new Date(r.fin)

      const desde = filtros.value.desde instanceof Date
        ? filtros.value.desde
        : filtros.value.desde
          ? new Date(filtros.value.desde)
          : null

      const hasta = filtros.value.hasta instanceof Date
        ? filtros.value.hasta
        : filtros.value.hasta
          ? new Date(filtros.value.hasta)
          : null

      let rangoOk = true
      if (desde && hasta) {
        rangoOk = fin >= desde && inicio <= hasta
      } else if (desde) {
        rangoOk = fin >= desde
      } else if (hasta) {
        rangoOk = inicio <= hasta
      }

      return condicionesEstado && usuarioOk && estadoOk && rangoOk
    })
  })

  function traducirEstado(estado) {
    switch (estado) {
      case 'Waiting': return 'Pendiente'
      case 'Rejected': return 'Rechazado'
      case 'Approved': return 'Aprobado'
      default: return estado
    }
  }




  function toggleDetalles(index) {
    const registro = registrosFiltrados.value[index]
    const original = registros.value.find(r =>
      r.nombre === registro.nombre &&
      r.inicio === registro.inicio &&
      r.fin === registro.fin
    )
    if (original) {
      original.mostrarDetalles = !original.mostrarDetalles
    }
  }
</script>


<style scoped>
  input[type="checkbox"] {
    transform: scale(1.2);
  }

  .btn-primary {
    background-color: #003c63;
    border-color: #003c63;
    font-weight: bold;
  }

 
  .detalle-fila td {
    background-color: #f5f5f5 !important;
  }


  .container {
    padding-left: 1rem;
    padding-right: 1rem;
  }

  .table-responsive {
    max-height: 350px;
    overflow-y: auto;
    overflow-x: hidden;
  }

  .detalle-fila td {
    background-color: #f8f9fa; 
    color: #6c757d; 
    font-size: 0.85rem;
    white-space: nowrap;
  }

  .detalle-fila small {
    color: #6c757d !important;
  }

  /* Evita que se rompa el ancho al abrir detalles */
  .table td, .table th {
    table-layout: fixed;
    overflow: hidden;
    text-overflow: ellipsis;
    vertical-align: middle;
  }

  .table {
    table-layout: fixed;
  }
</style>

