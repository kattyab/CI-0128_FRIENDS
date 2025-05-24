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
              <a :href="`/employees/${employee.empID}/edit`" class="btn btn-secondary ms-1">
                <span class="material-icons">edit</span>
              </a>
              <a @click="deleteEmployee(employee.empID)" class="btn btn-danger ms-1">
                <span class="material-icons">delete</span>
              </a>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>


<script setup>
  import { ref, onMounted } from 'vue';
  import axios from 'axios';

  const employees = ref([]);
  const loading = ref(true);
  const error = ref(null);
  const emailComponent = ref('');

  const deleteEmployee = async (empID) => {
    try {
      await axios.delete(`${import.meta.env.VITE_API_URL}/api/CompanyEmployees/${empID}`, { withCredentials: true });
      employees.value = employees.value.filter(e => e.empID !== empID);
    } catch (err) {
      console.error('Error deleting employee:', err);
      error.value = 'Failed to delete employee: ' + err.message;
    }
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
