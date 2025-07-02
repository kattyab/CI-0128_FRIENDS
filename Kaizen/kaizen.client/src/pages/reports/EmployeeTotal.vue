<template>
  <div>
    <h1 class="text-center my-4">Reporte Histórico Pago Planilla</h1>
    <div class="mx-4 my-4 d-flex justify-content-between align-items-center">
      <div>
        <div class="mb-3 d-flex gap-4 align-items-end">
          <div class="d-flex flex-column">
            <label for="searchName" class="fw-bold">Buscar por nombre</label>
            <input id="searchName" type="text" class="form-control" v-model="searchData.name" placeholder="Nombre del empleado" @input="filterByName" />
          </div>
          <div class="d-flex flex-column">
            <label for="startDate" class="fw-bold">Desde</label>
            <input id="startDate" type="date" class="form-control" v-model="searchData.start" />
          </div>
          <div class="d-flex flex-column">
            <label for="endDate" class="fw-bold">Hasta</label>
            <input id="endDate" type="date" class="form-control" v-model="searchData.end" />
          </div>
          <div class="d-flex align-items-end">
            <button class="btn btn-primary" @click="search"><i class="bi bi-search"></i></button>
          </div>
        </div>
      </div>
      <div>
        <button class="btn btn-lg btn-primary self-align-end" :disabled="payrollDataFiltered.length == 0" @click="openExportModal">
          <i class="bi bi-upload"></i> Exportar
        </button>
      </div>
    </div>
    <div class="mx-4">
      <table class="table table-hover">
        <thead>
          <tr>
            <th>Nombre empleado</th>
            <th>Cédula</th>
            <th>Tipo de empleado</th>
            <th>Periodo de pago</th>
            <th>Fecha de pago</th>
            <th>Salario bruto</th>
            <th>Cargas sociales empleador</th>
            <th>Deducciones voluntarias</th>
            <th>Costo empleador</th>
          </tr>
        </thead>
        <tbody class="table-group-divider">
          <tr v-for="(item, index) in payrollDataFiltered" :key="index">
            <td>{{ item.employeeName }}</td>
            <td>{{ item.cedula }}</td>
            <td>{{ item.tipoEmpleado }}</td>
            <td>{{ item.periodoPago }}</td>
            <td>{{ item.fechaPago }}</td>
            <td>₡{{ formatNumber(item.salarioBruto) }}</td>
            <td>₡{{ formatNumber(item.cargasSociales) }}</td>
            <td>₡{{ formatNumber(item.deduccionesVoluntarias) }}</td>
            <td>₡{{ formatNumber(item.costoEmpleador) }}</td>
          </tr>
          <tr>
            <td colspan="5" class="fw-bold text-end">Total</td>
            <td class="fw-bold">₡{{ formatNumber(payrollDataFiltered.reduce((a, c) => a + c.salarioBruto, 0)) }}</td>
            <td class="fw-bold">₡{{ formatNumber(payrollDataFiltered.reduce((a, c) => a + c.cargasSociales, 0)) }}</td>
            <td class="fw-bold">₡{{ formatNumber(payrollDataFiltered.reduce((a, c) => a + c.deduccionesVoluntarias, 0)) }}</td>
            <td class="fw-bold">₡{{ formatNumber(payrollDataFiltered.reduce((a, c) => a + c.costoEmpleador, 0)) }}</td>
          </tr>
        </tbody>
      </table>
    </div>
    <!-- Export modal remains unchanged -->
    <div id="exportModal" class="modal fade" tabindex="-1" ref="modalElement">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Opciones de Exportación</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <div class="card-group">
              <div class="card">
                <div class="card-body">
                  <h5 class="card-title">Correo Electrónico</h5>
                  <p class="card-text">Enviar el reporte por correo electrónico.</p>
                  <button class="btn btn-primary" @click="exportEmail">Enviar</button>
                </div>
              </div>
              <div class="card">
                <div class="card-body">
                  <h5 class="card-title">Descarga</h5>
                  <p class="card-text">Descargar el reporte como archivo CSV.</p>
                  <button class="btn btn-primary" @click="exportDownload">Descargar</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
function formatNumber(num) {
  if (num === null || num === undefined) return '';
  return Number(num).toLocaleString('es-CR', { maximumFractionDigits: 0 });
}
function filterByName() {
  applyFilters();
}

function filterByDateRange(list) {
  const start = searchData.value.start;
  const end = searchData.value.end;
  if (!start && !end) return [];
  return list.filter((item) => {
    if (!item.fechaPago) return false;
    const fechaPago = item.fechaPago.length === 10 ? item.fechaPago : String(item.fechaPago).slice(0, 10);
    if (start && fechaPago < start) return false;
    if (end && fechaPago > end) return false;
    return true;
  });
}

function applyFilters() {
  let filtered = [...payrollData.value];
  const name = searchData.value.name.trim().toLowerCase();
  const start = searchData.value.start;
  const end = searchData.value.end;
  if (!name || !start || !end) {
    payrollDataFiltered.value = [];
    return;
  }
  filtered = filtered.filter((item) => (item.employeeName || '').toLowerCase().includes(name));
  filtered = filterByDateRange(filtered);
  payrollDataFiltered.value = filtered;
}
import { ref, onMounted } from "vue";
import { Modal } from "bootstrap";
import axios from "axios";

const modalElement = ref(null);
const modalObject = ref(null);

const searchData = ref({
  start: "",
  end: "",
  name: "",
});

const payrollData = ref([]);
const payrollDataFiltered = ref([]);

async function fetchPayrollData() {
  try {
    const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/reports/employee-total`, { withCredentials: true });
    payrollData.value = response.data;
    applyFilters();
  } catch {
    payrollData.value = [];
    payrollDataFiltered.value = [];
    alert("Error al obtener los datos de planilla");
  }
}

function openExportModal() {
  modalObject.value.show();
}
function closeExportModal() {
  modalObject.value.hide();
}
function exportEmail() {
  try {
    axios
      .post(`${import.meta.env.VITE_API_URL}/api/reports/companieshistoric/email`, {
        Csv: generateCsvForEmail()
      }, { withCredentials: true })
      .then(() => {
        alert("Correo enviado.");
      })
      .catch((error) => {
        console.error("Error al enviar correo:", error);
        alert("Error al enviar correo.");
      });
  } catch (e) {
    console.error(e);
    alert("Ocurrió un error al enviar el correo.");
  }
  closeExportModal();
}

function generateCsvForEmail() {
  const headers = [
    'Nombre empleado', 'Cédula', 'Tipo de empleado', 'Periodo de pago', 'Fecha de pago', 'Salario bruto', 'Cargas sociales empleador', 'Deducciones voluntarias', 'Costo empleador'
  ];
  const rows = payrollDataFiltered.value.map(item => [
    item.employeeName,
    item.cedula,
    item.tipoEmpleado,
    item.periodoPago,
    item.fechaPago,
    item.salarioBruto,
    item.cargasSociales,
    item.deduccionesVoluntarias,
    item.costoEmpleador
  ]);
  const csvContent = [headers, ...rows].map(e => e.join(",")).join("\n");
  return "\uFEFF" + csvContent;
}

function exportDownload() {
  const headers = [
    'Nombre empleado', 'Cédula', 'Tipo de empleado', 'Periodo de pago', 'Fecha de pago', 'Salario bruto', 'Cargas sociales empleador', 'Deducciones voluntarias', 'Costo empleador'
  ];
  const rows = payrollDataFiltered.value.map(item => [
    item.employeeName,
    item.cedula,
    item.tipoEmpleado,
    item.periodoPago,
    item.fechaPago,
    item.salarioBruto,
    item.cargasSociales,
    item.deduccionesVoluntarias,
    item.costoEmpleador
  ]);
  const csvContent = [headers, ...rows].map(e => e.join(",")).join("\n");
  const blob = new Blob(["\uFEFF" + csvContent], { type: "text/csv;charset=utf-8;" });
  const link = document.createElement("a");
  link.href = URL.createObjectURL(blob);
  link.setAttribute("download", "reporte_planilla_total.csv");
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  closeExportModal();
}
function search() {
  applyFilters();
}


onMounted(() => {
  modalObject.value = new Modal(modalElement.value);
  fetchPayrollData();
});
</script>
