<template>
  <div>
    <h1 class="text-center my-4">Reporte de planillas por rango</h1>
    <div class="mx-4 my-4 d-flex justify-content-between align-items-center">
      <div>
        <div class="mb-3 d-flex align-items-end gap-4">
          <div class="d-flex flex-column">
            <label for="startDate" class="fw-bold">Desde</label>
            <input id="startDate" type="date" class="form-control" v-model="searchData.start" />
          </div>
          <div class="d-flex flex-column">
            <label for="endDate" class="fw-bold">Hasta</label>
            <input id="endDate" type="date" class="form-control" v-model="searchData.end" />
          </div>
          <div class="">
            <button class="btn btn-primary" @click="search">Buscar</button>
          </div>
        </div>
      </div>
      <div>
        <button class="btn btn-lg btn-primary self-align-end" :disabled="payrollData.length == 0"
                @click="openExportModal">
          Exportar
        </button>
      </div>
    </div>

    <div class="mx-4">
      <table class="table table-hover">
        <thead>
          <tr>
            <th>Nombre de la empresa</th>
            <th>Frecuencia de pago</th>
            <th>Periodo de pago</th>
            <th>Fecha de pago</th>
            <th>Salario Bruto</th>
            <th>Cargas sociales empleador</th>
            <th>Deducciones voluntarias</th>
            <th>Deducciones obligatorias</th>
            <th>Costo empleador</th>
          </tr>
        </thead>
        <tbody class="table-group-divider">
          <tr v-for="(item, index) in payrollData" :key="index">
            <td>{{ item.companyName }}</td>
            <td>{{ item.payrollMode }}</td>
            <td>{{ item.period }}</td>
            <td>{{ formatDate(item.executedOn) }}</td>
            <td>₡{{ formatNumber(item.totalBrutePaid) }}</td>
            <td>₡{{ formatNumber(item.totalLaborCharges) }}</td>
            <td>₡{{ formatNumber(item.totalDeductionsBenefits) }}</td>
            <td>₡{{ formatNumber(item.totalObligatoryDeductions) }}</td>
            <td>₡{{ formatNumber(item.totalMoneyPaid) }}</td>
          </tr>
        </tbody>
      </table>
    </div>

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
  import axios from "axios";
  import { formatDate } from "@/composables/formatDate";
  import { Modal } from "bootstrap";

  const modalElement = ref(null);
  const modalObject = ref(null);

  const searchData = ref({
    employeeId: "",
    start: "",
    end: "",
  });

  const data = ref({
    companyName: "",
    employees: [],
  });

  const allReports = ref([]);
  const payrollData = ref([]);

  function formatNumber(num) {
    return Number(num).toLocaleString("es-CR", { maximumFractionDigits: 0 });
  }

  function openExportModal() {
    modalObject.value.show();
  }

  function closeExportModal() {
    modalObject.value.hide();
  }

  function search() {
    if (!searchData.value.start || !searchData.value.end) {
      alert("Por favor, seleccione un rango de fechas.");
      return;
    }

    const start = new Date(searchData.value.start);
    const end = new Date(searchData.value.end);

    payrollData.value = allReports.value.filter((item) => {
      const period = item.period.trim();

      // Caso 1: Mensual (ej: "09-2025")
      const isMonthly = /^\d{2}-\d{4}$/.test(period);

      if (isMonthly) {
        const [monthStr, yearStr] = period.split("-");
        const month = parseInt(monthStr) - 1; // JS Date usa 0-index para meses
        const year = parseInt(yearStr);

        const periodStart = new Date(year, month, 1);
        const periodEnd = new Date(year, month + 1, 0); // último día del mes

        // Si el rango seleccionado toca el mes, se incluye
        return (
          end >= periodStart && start <= periodEnd
        );
      }

      // Caso 2: Quincenal (ej: "16-07-2025 → 31-07-2025")
      const match = period.match(/^(\d{2}-\d{2}-\d{4})\s+→\s+(\d{2}-\d{2}-\d{4})$/);

      if (match) {
        const [_, fromStr, toStr] = match;

        const [fd, fm, fy] = fromStr.split("-").map(Number);
        const [td, tm, ty] = toStr.split("-").map(Number);

        const periodStart = new Date(fy, fm - 1, fd);
        const periodEnd = new Date(ty, tm - 1, td);

        // El rango del usuario debe contener todo el periodo para ser válido
        return start <= periodStart && end >= periodEnd;
      }

      // Si no coincide con ninguno de los formatos, se descarta
      return false;
    });
  }



  // 📡 Cargar todos los reportes de planilla
  async function fetchReports() {
    try {
      const res = await axios.get(`${import.meta.env.VITE_API_URL}/api/GeneralPayrollReport`);
      allReports.value = res.data;
      payrollData.value = res.data;
    } catch (error) {
      console.error("Error al obtener los reportes:", error);
      alert("Error al cargar reportes.");
    }
  }

  onMounted(() => {
    fetchReports();
    modalObject.value = new Modal(modalElement.value);
  });</script>
