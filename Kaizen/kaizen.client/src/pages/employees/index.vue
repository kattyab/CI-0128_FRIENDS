<template>
  <div>
    <h1>Empleados</h1>
    <table class="table table-hover">
      <thead>
        <tr>
          <th scope="col">Nombre</th>
          <th scope="col">Apellidos</th>
          <th scope="col">Cédula</th>
          <th scope="col">Puesto de Trabajo</th>
          <th scope="col">Tipo de Contrato</th>
          <th scope="col">Acciones</th>
        </tr>
      </thead>
      <tbody class="table-group-divider">
        <tr class="position-relative" v-for="employee in data" :key="employee.empID">
          <th scope="row">{{ employee.name }}</th>
          <td>{{ employee.lastName }}</td>
          <td>{{ employee.id }}</td>
          <td>{{ employee.jobPosition }}</td>
          <td>{{ employee.contractType }}</td>
          <td>
            <a :href="`/employees/${employee.empID}`" class="btn btn-primary">
              <span class="material-icons">visibility</span>
            </a>
            <a @click="deleteEmployee(employee.empID)" class="btn btn-danger ms-1">
              <span class="material-icons">delete</span>
            </a>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import axios from "axios";

const data = ref([]);

async function fetchData() {
  try {
    axios
      .get(`${import.meta.env.VITE_API_URL}/api/employees`, {
        withCredentials: true,
      })
      .then((response) => {
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

async function deleteEmployee(id) {
  try {
    await axios.delete(`${import.meta.env.VITE_API_URL}/api/employees/${id}`, {
      withCredentials: true,
    });
    console.log("Employee deleted successfully");
    fetchData(); // Refresh the data after deletion
  } catch (error) {
    console.error("Error deleting employee:", error);
  }
}

onMounted(fetchData);
</script>
