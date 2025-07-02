<template>
  <div>
    <h1 class="text-center my-4">Reporte de planillas general</h1>
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
            <th v-if="userRole === 'Superadmin'">Nombre de la empresa</th>
            <th v-if="userRole === 'Superadmin'">Frecuencia de pago</th>
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
            <td v-if="userRole === 'Superadmin'">{{ item.companyName }}</td>
            <td v-if="userRole === 'Superadmin'">{{ item.payrollMode }}</td>
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
  import { computed } from "vue";

  const modalElement = ref(null);
  const modalObject = ref(null);
  const userRole = ref(''); 




  const searchData = ref({
    employeeId: "",
    start: "",
    end: "",
  });

  function exportDownload() {

    if (payrollData.value.length === 0) {
      alert("No hay datos para exportar.");
      return;
    }


    const headers = userRole.value === 'Superadmin'
      ? [
        "Nombre de la empresa",
        "Frecuencia de pago",
        "Periodo de pago",
        "Fecha de pago",
        "Salario Bruto",
        "Cargas sociales empleador",
        "Deducciones voluntarias",
        "Deducciones obligatorias",
        "Costo empleador"
      ]
      : [
        "Periodo de pago",
        "Fecha de pago",
        "Salario Bruto",
        "Cargas sociales empleador",
        "Deducciones voluntarias",
        "Deducciones obligatorias",
        "Costo empleador"
      ];


    const rows = payrollData.value.map(item => {
      return userRole.value === 'Superadmin'
        ? [
          item.companyName,
          item.payrollMode,
          item.period,
          formatDate(item.executedOn),
          item.totalBrutePaid,
          item.totalLaborCharges,
          item.totalDeductionsBenefits,
          item.totalObligatoryDeductions,
          item.totalMoneyPaid
        ]
        : [
          item.period,
          formatDate(item.executedOn),
          item.totalBrutePaid,
          item.totalLaborCharges,
          item.totalDeductionsBenefits,
          item.totalObligatoryDeductions,
          item.totalMoneyPaid
        ];
    });

    const csvContent =
      [headers, ...rows]
        .map(row =>
          row
            .map(field => `"${String(field).replace(/"/g, '""')}"`)
            .join(",")
        )
        .join("\n");

    const blob = new Blob([csvContent], { type: "text/csv;charset=utf-8;" });
    const link = document.createElement("a");
    const url = URL.createObjectURL(blob);
    link.setAttribute("href", url);
    link.setAttribute("download", "reporte_planillas.csv");
    link.style.visibility = "hidden";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }



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

  async function exportEmail() {

    if (payrollData.value.length === 0) {
      alert("No hay datos para exportar.");
      return;
    }

    try {
      const headers = userRole.value === 'Superadmin'
        ? [
          "Nombre de la empresa",
          "Frecuencia de pago",
          "Periodo de pago",
          "Fecha de pago",
          "Salario Bruto",
          "Cargas sociales empleador",
          "Deducciones voluntarias",
          "Deducciones obligatorias",
          "Costo empleador"
        ]
        : [
          "Periodo de pago",
          "Fecha de pago",
          "Salario Bruto",
          "Cargas sociales empleador",
          "Deducciones voluntarias",
          "Deducciones obligatorias",
          "Costo empleador"
        ];

      const rows = payrollData.value.map(item => {
        return userRole.value === 'Superadmin'
          ? [
            item.companyName,
            item.payrollMode,
            item.period,
            formatDate(item.executedOn),
            item.totalBrutePaid,
            item.totalLaborCharges,
            item.totalDeductionsBenefits,
            item.totalObligatoryDeductions,
            item.totalMoneyPaid
          ]
          : [
            item.period,
            formatDate(item.executedOn),
            item.totalBrutePaid,
            item.totalLaborCharges,
            item.totalDeductionsBenefits,
            item.totalObligatoryDeductions,
            item.totalMoneyPaid
          ];
      });

      const csvContent =
        [headers, ...rows]
          .map(row =>
            row
              .map(field => `"${String(field).replace(/"/g, '""')}"`)
              .join(",")
          )
          .join("\n");

      await axios.post(`${import.meta.env.VITE_API_URL}/api/reports/companieshistoric/email`, {
        employeeId: searchData.value.employeeId,
        start: searchData.value.start,
        end: searchData.value.end,
        csv: csvContent,
      });

      alert("Correo enviado exitosamente.");
    } catch (error) {
      console.error("Error al enviar correo:", error);
      alert("Error al enviar el correo.");
    }

    closeExportModal();
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


      const isMonthly = /^\d{2}-\d{4}$/.test(period);

      if (isMonthly) {
        const [monthStr, yearStr] = period.split("-");
        const month = parseInt(monthStr) - 1; 
        const year = parseInt(yearStr);

        const periodStart = new Date(year, month, 1);
        const periodEnd = new Date(year, month + 1, 0); 


        return (
          end >= periodStart && start <= periodEnd
        );
      }


      const match = period.match(/^(\d{2}-\d{2}-\d{4})\s+→\s+(\d{2}-\d{2}-\d{4})$/);

      if (match) {
        const [_, fromStr, toStr] = match;

        const [fd, fm, fy] = fromStr.split("-").map(Number);
        const [td, tm, ty] = toStr.split("-").map(Number);

        const periodStart = new Date(fy, fm - 1, fd);
        const periodEnd = new Date(ty, tm - 1, td);


        return start <= periodStart && end >= periodEnd;
      }


      return false;
    });
  }




  async function fetchReports() {
    try {
      const res = await axios.get(`${import.meta.env.VITE_API_URL}/api/GeneralPayrollReport`);
      allReports.value = res.data;
      payrollData.value = res.data;
    } catch (error) {
      console.error("Error al cargar todos los reportes:", error);
      alert("No se pudieron cargar los reportes.");
    }
  }

  async function fetchCompanyReport() {
    try {
      const companyRes = await axios.get(`${import.meta.env.VITE_API_URL}/api/ReportMenu/company-id`, { withCredentials: true });
      const companyPK = companyRes.data;

      const reportRes = await axios.get(`${import.meta.env.VITE_API_URL}/api/GeneralPayrollReport/company/${companyPK}`);
      allReports.value = reportRes.data;
      payrollData.value = reportRes.data;
    } catch (error) {
      console.error("Error al cargar los reportes filtrados por empresa:", error);
      alert("No se pudieron cargar los reportes filtrados.");
    }
  }


  onMounted(async () => {
    try {
      const res = await axios.get(`${import.meta.env.VITE_API_URL}/api/login/authenticate`, { withCredentials: true });
      userRole.value = res.data.role;

      console.log("ROL ACTUAL:", userRole.value); 
      if (userRole.value === 'Superadmin') {
        await fetchReports();
      } else if (userRole.value === 'Dueño') {
        await fetchCompanyReport();
      } else {
        alert("No tiene permisos para acceder a los reportes.");
      }

      modalObject.value = new Modal(modalElement.value);
    } catch (error) {
      console.error("Error al autenticar usuario:", error);
      alert("Error de autenticación.");
    }
  });
</script>
