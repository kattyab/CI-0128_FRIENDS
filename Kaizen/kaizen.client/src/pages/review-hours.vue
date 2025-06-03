<template>
  <div class="container mt-4">
    <h2 class="text-center mb-4">Aprobación de Horas</h2>

    <div class="row g-3 mb-3">
      <div class="col-md-3">
        <label>Usuario</label>
        <select class="form-select" v-model="filters.user">
          <option value="">Todos</option>
          <option v-for="user in uniqueUsers" :key="user">{{ user }}</option>
        </select>
      </div>

      <div class="col-md-3">
        <label>Desde</label>
        <Flatpickr class="form-control"
                   v-model="filters.from"
                   :config="flatpickrConfig" />
      </div>

      <div class="col-md-3">
        <label>Hasta</label>
        <Flatpickr class="form-control"
                   v-model="filters.to"
                   :config="flatpickrConfig" />
      </div>

      <div class="col-md-3">
        <label>Estado</label>
        <select class="form-select" v-model="filters.status">
          <option value="">Todos</option>
          <option value="Approved">Aprobado</option>
          <option value="Waiting">Pendiente</option>
          <option value="Rejected">Rechazado</option>
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
          <template v-for="(record, index) in filteredRecords" :key="index">
            <tr @click="toggleDetails(index)" style="cursor: pointer;">
              <td class="text-start">{{ record.name }}</td>
              <td class="text-center">{{ formatDateSimple(record.start) }} - {{ formatDateSimple(record.end) }}</td>
              <td class="text-center">{{ record.hours }}</td>
              <td class="text-end pe-4">
                <span class="btn btn-sm"
                      :class="{
                        'btn-success': record.status === 'Approved',
                        'btn-danger': record.status === 'Rejected',
                        'btn-secondary': record.status === 'Waiting'
                      }">
                  {{
                    record.status === 'Approved'
                      ? 'Aprobado'
                      : record.status === 'Rejected'
                        ? 'Rechazado'
                        : 'Pendiente'
                  }}
                </span>
              </td>
              <td class="text-end pe-4">
                <input type="checkbox"
                       v-model="record.selected"
                       :disabled="record.status === 'Approved' || record.status === 'Rejected'"
                       @click.stop />
              </td>
            </tr>
            <tr v-if="record.showDetails" class="detalle-fila">
              <td>
                <small><strong>Fecha inicio:</strong> {{ formatDateSimple(record.fullStartDate) }}</small>
              </td>
              <td class="text-center">
                <small><strong>Pago tipo:</strong> {{ record.payrollType }}</small>
              </td>
              <td class="text-center">
                <small><strong>Contrato:</strong> {{ record.contractType }}</small>
              </td>
              <td colspan="2"></td>
            </tr>
          </template>
        </tbody>
      </table>
    </div>

    <div class="mt-3 d-flex justify-content-center gap-3">
      <button class="btn btn-primary" @click="() => updateSelectedStatuses('Approved')">Aprobar</button>
      <button class="btn btn-danger" @click="() => updateSelectedStatuses('Rejected')">Rechazar</button>
    </div>

  </div>
</template>

<script setup>
  import { ref, computed, onMounted } from 'vue'
  import axios from 'axios'
  import Flatpickr from 'vue-flatpickr-component'
  import 'flatpickr/dist/flatpickr.css'

  const filters = ref({
    from: null,
    to: null,
    user: '',
    status: ''
  })

  const flatpickrConfig = {
    dateFormat: 'Y-m-d',
    altInput: true,
    altFormat: 'd-m-Y',
    locale: 'es'
  }

  const records = ref([])

  async function fetchRecords() {
    try {
      const { data } = await axios.get(`${import.meta.env.VITE_API_URL}/api/ApprovedHours/AllHours`)
      records.value = data.map(item => ({
        approvalID: item.approvalID,
        isSentForApproval: item.isSentForApproval,
        name: `${item.name} ${item.lastName}`,
        start: item.startDate,
        end: item.endDate,
        hours: item.hoursWorked,
        status: item.status || 'Waiting',
        fullStartDate: item.employeeStartDate,
        payrollType: item.payrollType,
        contractType: item.contractType,
        showDetails: false,
        selected: false
      }))
    } catch (error) {
      console.error('Error loading ApprovedHours data:', error)
    }
  }

  async function updateSelectedStatuses(newStatus) {
    const selectedRecords = records.value.filter(r =>
      r.selected &&
      r.status === 'Waiting' &&
      r.isSentForApproval === true
    )

    if (selectedRecords.length === 0) {
      alert('No selected records to process.')
      return
    }

    try {
      await Promise.all(selectedRecords.map(async r => {
        await axios.patch(`${import.meta.env.VITE_API_URL}/api/ApprovedHours/${r.approvalID}/status`, {
          status: newStatus
        })
      }))
      await fetchRecords()
    } catch (error) {
      console.error(`Error updating records:`, error)
      alert('An error occurred while updating records.')
    }
  }

  onMounted(fetchRecords)

  const uniqueUsers = computed(() => {
    return [...new Set(records.value.map(r => r.name))]
  })

  function formatDateSimple(date) {
    const onlyDate = date.split('T')[0]
    const [y, m, d] = onlyDate.split('-')
    return `${d}/${m}/${y.slice(2)}`
  }

  function formatDateFull(date) {
    const [year, month, day] = date.split('-')
    return `${day}/${month}/${year}`
  }

  const filteredRecords = computed(() => {
    return records.value.filter(r => {
      const statusCondition = r.isSentForApproval === true

      const userOk = !filters.value.user || r.name === filters.value.user
      const statusOk = !filters.value.status || r.status === filters.value.status


      const start = new Date(r.start)
      const end = new Date(r.end)

      const from = filters.value.from instanceof Date
        ? filters.value.from
        : filters.value.from
          ? new Date(filters.value.from)
          : null

      const to = filters.value.to instanceof Date
        ? filters.value.to
        : filters.value.to
          ? new Date(filters.value.to)
          : null

      let rangeOk = true
      if (from && to) {
        rangeOk = end >= from && start <= to
      } else if (from) {
        rangeOk = end >= from
      } else if (to) {
        rangeOk = start <= to
      }

      return statusCondition && userOk && statusOk && rangeOk
    })
  })

  function translateStatus(status) {
    switch (status) {
      case 'Waiting': return 'Pendiente'
      case 'Rejected': return 'Rechazado'
      case 'Approved': return 'Aprobado'
      default: return status
    }
  }

  function toggleDetails(index) {
    const record = filteredRecords.value[index]
    const original = records.value.find(r =>
      r.name === record.name &&
      r.start === record.start &&
      r.end === record.end
    )
    if (original) {
      original.showDetails = !original.showDetails
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

