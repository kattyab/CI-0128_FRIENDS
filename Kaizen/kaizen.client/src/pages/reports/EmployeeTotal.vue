<template>
  <div>
    <h1 class="text-center my-4">Reporte Histórico Pago Planilla</h1>
    <div class="mx-4 my-4 d-flex justify-content-between align-items-center">
      <div>
        <div class="mb-3 d-flex gap-4">
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
        <div class="mb-2"><span class="fw-bold">Empresa:</span> Kaizen</div>
        <div class="mb-2"><span class="fw-bold">Empleado:</span> Juan Perez</div>
      </div>
      <div>
        <button class="btn btn-lg btn-primary self-align-end" :disabled="payrollData.length == 0" @click="openExportModal">
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
          <tr v-for="(item, index) in payrollData" :key="index">
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
            <td class="fw-bold">₡{{ formatNumber(payrollData.reduce((a, c) => a + c.salarioBruto, 0)) }}</td>
            <td class="fw-bold">₡{{ formatNumber(payrollData.reduce((a, c) => a + c.cargasSociales, 0)) }}</td>
            <td class="fw-bold">₡{{ formatNumber(payrollData.reduce((a, c) => a + c.deduccionesVoluntarias, 0)) }}</td>
            <td class="fw-bold">₡{{ formatNumber(payrollData.reduce((a, c) => a + c.costoEmpleador, 0)) }}</td>
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
import { ref, onMounted } from "vue";
import { Modal } from "bootstrap";

const modalElement = ref(null);
const modalObject = ref(null);

const searchData = ref({
  start: "",
  end: "",
});

// Mock data for the table, as in the screenshot
const payrollData = ref([
  {
    employeeName: "Karla Jiménez",
    cedula: "112252021",
    tipoEmpleado: "Tiempo completo",
    periodoPago: "01/12/2025\n31/12/2025",
    fechaPago: "31/12/2025",
    salarioBruto: 100000,
    cargasSociales: 100000,
    deduccionesVoluntarias: 100000,
    costoEmpleador: 100000,
  },
  {
    employeeName: "Pedro Rojas",
    cedula: "117352251",
    tipoEmpleado: "Tiempo completo",
    periodoPago: "01/12/2025\n15/12/2025",
    fechaPago: "15/12/2025",
    salarioBruto: 100000,
    cargasSociales: 100000,
    deduccionesVoluntarias: 100000,
    costoEmpleador: 100000,
  },
]);

function formatNumber(num) {
  return Number(num).toLocaleString("es-CR", { maximumFractionDigits: 0 });
}

function openExportModal() {
  modalObject.value.show();
}
function closeExportModal() {
  modalObject.value.hide();
}
function exportEmail() {
  alert("Correo enviado (mock)");
  closeExportModal();
}
function exportDownload() {
  alert("Descarga iniciada (mock)");
  closeExportModal();
}
function search() {
  alert("Búsqueda simulada (mock)");
}

onMounted(() => {
  modalObject.value = new Modal(modalElement.value);
});
</script>
