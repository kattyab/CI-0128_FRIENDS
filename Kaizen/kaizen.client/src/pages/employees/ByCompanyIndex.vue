<template>
  <div>
    <h1 class="text-center my-4">Empleados</h1>

    <!-- Add horizontal margins to the table container -->
    <div class="mx-5">
      <table class="table table-hover">
        <thead>
          <tr>
            <th scope="col">Nombre</th>
            <th scope="col">Apellidos</th>
            <th scope="col">Cédula</th>
            <th scope="col">Posición</th>
            <th scope="col">Tipo de Contrato</th>
            <th scope="col">Acciones</th>
          </tr>
        </thead>
        <tbody class="table-group-divider">
          <tr v-for="employee in employees" :key="employee.empID">
            <th scope="row">{{ employee.name }}</th>
            <td>{{ employee.lastName }}</td>
            <td>{{ employee.id }}</td>
            <td>{{ employee.jobPosition }}</td>
            <td>{{ employee.contractType }}</td>
            <td>
              <a :href="`/employees/${employee.empID}`" class="btn btn-primary">
                <span class="material-icons">visibility</span>
              </a>
              <a @click="openDeleteModal(employee)" class="btn btn-danger ms-1">
                <span class="material-icons">delete</span>
              </a>
            </td>
          </tr>
        </tbody>
      </table>
      <div>
        <p v-if="error" class="text-danger">{{ error }}</p>
      </div>
      <div id="deleteModal" class="modal fade" tabindex="-1" ref="modalElement">
        <div class="modal-dialog modal-dialog-centered">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">Eliminar Empleado</h5>
              <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
              <p>Confirmación: ¿Está seguro de eliminar el empleado {{ deleteEmployee?.name }} {{
                deleteEmployee?.lastName }}?<br>Esta acción no es reversible.</p>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-primary"
                @click="deleteEmployeeExecute(deleteEmployee.id)">Confirmar</button>
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>


<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { Modal } from "bootstrap";

const modalElement = ref(null);
const modalObject = ref(null);

const deleteEmployee = ref(null);

const employees = ref([]);
const loading = ref(true);
const error = ref(null);
const emailComponent = ref('');

function openDeleteModal(employee) {
  deleteEmployee.value = employee;
  modalObject.value.show();
}

function closeDeleteModal() {
  modalObject.value.hide();
}

const deleteEmployeeExecute = async () => {
  try {
    await axios.delete(`${import.meta.env.VITE_API_URL}/api/employees/${deleteEmployee.value.empID}`, { withCredentials: true });
    employees.value = employees.value.filter(e => e.empID !== deleteEmployee.value.empID);
  } catch (err) {
    console.error('Error deleting employee:', err);
    error.value = 'Failed to delete employee: ' + err.message;
  }
  deleteEmployee.value = null;
  closeDeleteModal();
};

const fetchData = async (email) => {
  try {
    const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/CompanyEmployees/by-owner-email/${email}`, {
      withCredentials: true,
    });
    employees.value = response.data;
  } catch (err) {
    console.error("Error fetching employee data:", err);
    error.value = 'Failed to fetch employees: ' + err.message;
  } finally {
    loading.value = false;
  }
};

onMounted(async () => {
  modalObject.value = new Modal(modalElement.value);

  try {
    const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/login/authenticate`, {
      withCredentials: true,
    });
    emailComponent.value = response.data.email;
    console.log("Email fetched successfully:", emailComponent.value);

    await fetchData(emailComponent.value);
  } catch (err) {
    console.error("Error fetching email:", err);
    error.value = 'Failed to fetch user email: ' + err.message;
    loading.value = false;
  }
});
</script>

<style scoped>
.btn-primary {
  background-color: #003c63;
  border-color: #003c63;
}
</style>
