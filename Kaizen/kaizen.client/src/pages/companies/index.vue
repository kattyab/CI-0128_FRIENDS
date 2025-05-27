<template>
  <div>
    <h1>Companies</h1>
    <table class="table table-hover">
      <thead>
        <tr>
          <th scope="col">Id</th>
          <th scope="col">Name</th>
          <th scope="col">Brand</th>
          <th scope="col">Type</th>
        </tr>
      </thead>
      <tbody class="table-group-divider">
        <tr class="position-relative" v-for="(company, index) in data" :key="index">
          <th scope="row">{{ company.companyID }}</th>
          <td>{{ company.companyName }}</td>
          <td>{{ company.brandName }}</td>
          <td>
            {{ company.type }}
            <a :href="`/companies/${company.companyPK}`" class="stretched-link"></a>
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
      .get(`${import.meta.env.VITE_API_URL}/api/companies`, {
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

onMounted(fetchData);
</script>
