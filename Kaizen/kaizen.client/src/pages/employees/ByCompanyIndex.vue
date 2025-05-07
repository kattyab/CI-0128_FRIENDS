<template>
  <div>
    <h1>Employees</h1>
    <table class="table table-hover">
      <thead>
        <tr>
          <th scope="col">Name</th>
          <th scope="col">Surname</th>
          <th scope="col">Id</th>
          <th scope="col">Job</th>
          <th scope="col">Contract</th>
          <th scope="col">Actions</th>
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
</template>

<script setup>
  import { ref, onMounted } from "vue";
  import axios from "axios";

  const employeeList = ref([]);

  onMounted(async () => {
    try {
      const auth = await axios.get('/api/login/authenticate', { withCredentials: true });
      const email = auth.data.email;

      const res = await axios.get(`/api/CompanyEmployees/by-owner-email/${email}`, { withCredentials: true });
      employeeList.value = res.data;
    } catch (e) {
      console.error("Failed to load employees:", e);
    }
  });
</script>
