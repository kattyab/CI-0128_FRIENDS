<template>
  <div>
    <h1 class="text-center my-4">Reporte de planillas por rango</h1>
    <div class="mx-4 my-4 d-flex justify-content-between align-items-center">
      <div>
        <div class="mb-3"><span class="fw-bold">Empresa: </span>&nbsp;{{ data.companyName }}</div>
        <div class="mb-3 d-flex align-items-center gap-2">
          <label for="employee" class="fw-bold">Empleado</label>
          <select id="employee" class="form-select" v-model="searchData.employeeId">
            <option value="" disabled>Seleccione un empleado</option>
            <option v-for="employee in data.employees" :key="employee.id" :value="employee.id">
              {{ employee.name }} {{ employee.lastName }}
            </option>
          </select>
        </div>
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
        <button
          class="btn btn-lg btn-primary self-align-end"
          :disabled="payrollData.length == 0"
          @click="openExportModal"
        >
          Exportar
        </button>
      </div>
    </div>
    <div class="mx-4">
      <table class="table table-hover">
        <thead>
          <tr>
            <th scope="col">Tipo de contrato</th>
            <th scope="col">Posición</th>
            <th scope="col">Fecha de pago</th>
            <th scope="col">Salario Bruto</th>
            <th scope="col">Deducciones obligatorias</th>
            <th scope="col">Deducciones voluntarias</th>
            <th scope="col">Salario neto</th>
          </tr>
        </thead>
        <tbody class="table-group-divider">
          <tr class="" v-for="(item, index) in payrollData" :key="index">
            <td>{{ item.contractType }}</td>
            <td>{{ item.jobPosition }}</td>
            <td>{{ formatDate(item.payrollDate) }}</td>
            <td>{{ item.bruteSalary }}</td>
            <td>{{ item.obligatoryDeductions }}</td>
            <td>{{ item.optionalDeductions }}</td>
            <td>{{ item.netSalary }}</td>
          </tr>
          <tr class="">
            <td></td>
            <td></td>
            <td></td>
            <td class="fw-bold">
              {{
                payrollData.reduce((a, c) => {
                  return a + c.bruteSalary;
                }, 0)
              }}
            </td>
            <td class="fw-bold">
              {{
                payrollData.reduce((a, c) => {
                  return a + c.obligatoryDeductions;
                }, 0)
              }}
            </td>
            <td class="fw-bold">
              {{
                payrollData.reduce((a, c) => {
                  return a + c.optionalDeductions;
                }, 0)
              }}
            </td>
            <td class="fw-bold">
              {{
                payrollData.reduce((a, c) => {
                  return a + c.netSalary;
                }, 0)
              }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div id="exportModal" class="modal fade" tabindex="-1" ref="modalElement">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Opciones de Exportación</h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
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
const payrollHeaders = ref([
  "Tipo de contrato",
  "Posición",
  "Fecha de pago",
  "Salario Bruto",
  "Deducciones obligatorias",
  "Deducciones voluntarias",
  "Salario neto",
]);
const payrollData = ref([]);

function openExportModal() {
  modalObject.value.show();
}

function closeExportModal() {
  modalObject.value.hide();
}

function exportEmail() {
  try {
    axios
      .post(`${import.meta.env.VITE_API_URL}/api/reports/historicrange/email`, {
        employeeId: searchData.value.employeeId,
        start: searchData.value.start,
        end: searchData.value.end,
      })
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

function exportDownload() {
  const empleado = data.value.employees.find((x) => x.EmpId == searchData.employeeId);
  const empleadoName = `${empleado.name}_${empleado.lastName}`;

  const csvContent =
    "data:text/csv;charset=utf-8," +
    Object.values(payrollHeaders.value).join(",") +
    "\n" +
    payrollData.value
      .map((e) => {
        return `${e.contractType},${e.jobPosition},${formatDate(e.payrollDate)},${e.bruteSalary},${
          e.obligatoryDeductions
        },${e.optionalDeductions},${e.netSalary}`;
      })
      .join("\n") +
    "\n" +
    `,,,${payrollData.value.reduce((a, c) => {
      return a + c.bruteSalary;
    }, 0)},${payrollData.value.reduce((a, c) => {
      return a + c.obligatoryDeductions;
    }, 0)},${payrollData.value.reduce((a, c) => {
      return a + c.optionalDeductions;
    }, 0)},${payrollData.value.reduce((a, c) => {
      return a + c.netSalary;
    }, 0)}`;

  const encodedUri = encodeURI(csvContent);
  const link = document.createElement("a");
  link.setAttribute("href", encodedUri);
  link.setAttribute(
    "download",
    `reporte_planilla_historico_${empleadoName}_${formatDate(searchData.value.start)}_${formatDate(
      searchData.value.end
    )}.csv`
  );
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  closeExportModal();
}

function search() {
  if (searchData.value.selectedEmployee === "") {
    alert("Por favor, seleccione un empleado.");
    return;
  }
  if (!searchData.value.start || !searchData.value.end) {
    alert("Por favor, seleccione un rango de fechas.");
    return;
  }
  if (new Date(searchData.value.start) > new Date(searchData.value.end)) {
    alert("La fecha de inicio no puede ser posterior a la fecha de fin.");
    return;
  }

  try {
    axios
      .get(`${import.meta.env.VITE_API_URL}/api/reports/historicrange/search`, {
        params: {
          employeeId: searchData.value.employeeId,
          start: searchData.value.start,
          end: searchData.value.end,
        },
        withCredentials: true,
      })
      .then((response) => {
        console.log("Datos obtenidos:", response.data);
        payrollData.value = response.data;
      })
      .catch((error) => {
        console.error("Error al obtener los datos:", error);
        alert("Error al obtener los datos. Por favor, inténtelo de nuevo más tarde.");
      });
  } catch (e) {
    console.error(e);
    alert("Ocurrió un error al realizar la búsqueda. Por favor, inténtelo de nuevo más tarde.");
  }
}

async function fetchData() {
  try {
    axios
      .get(`${import.meta.env.VITE_API_URL}/api/reports/historicrange/data`, {
        withCredentials: true,
      })
      .then((response) => {
        console.log("Data fetched successfully:", response.data);
        data.value = response.data;
      })
      .catch((error) => {
        console.error("Error fetching data:", error);
        throw error;
      });
  } catch (e) {
    console.log(e);
  }
}

onMounted(() => {
  fetchData();

  modalObject.value = new Modal(modalElement.value);
});
</script>
