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

const data = ref([]);

async function fetchData() {
  try {
    axios
      .get("https://localhost:7153/api/employees", {
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

async function deleteEmployee(id) {
  try {
    await axios.delete(`https://localhost:7153/api/employees/${id}`, {
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
