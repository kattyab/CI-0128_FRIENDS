<template>
  <div>
    <h1 class="text-center my-4">Lista de Beneficios</h1>
    <div class="mx-4 my-4 d-flex justify-content-between align-items-center">
      <div></div>
      <a class="btn btn-lg btn-primary self-align-end" href="/benefits/create">
          Crear beneficio
      </a>
    </div>
    <div class="mx-4">
      <table class="table table-hover">
        <thead>
          <tr>
            <th scope="col">Nombre</th>
            <th scope="col">Minimum Hours</th>
            <th scope="col">Contratos</th>
            <th scope="col">Tipo</th>
            <th scope="col">Valor</th>
            <th scope="col">Acciones</th>
          </tr>
        </thead>
        <tbody class="table-group-divider">
          <tr class="position-relative" v-for="(item, index) in data" :key="index">
            <th scope="row">{{ item.name }}</th>
            <td>{{ item.minWorkDurationMonths }}</td>
              <td>
                <div v-if="item.isFullTime">Tiempo completo</div>
                <div v-if="item.isPartTime">Medio tiempo</div>
                <div v-if="item.isByHours">Por horas</div>
                <div v-if="item.isByService">Por servicio</div>
              </td>
            <td>{{ item.isFixed ? "Fijo" : "Porcentaje" }}</td>
            <td>{{ item.isFixed ? '₡' + item.fixedValue : item.percentageValue + '%' }}</td>
            <td>
              <a :href="`/benefits/${item.id}`" class="btn btn-primary">
                <span class="material-icons">visibility</span>
              </a>
              <!-- TODO use a post request in this page -->
              <a :href="`/benefits/${item.id}`" class="btn btn-danger ms-1">
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
import { ref, onMounted } from "vue";
import axios from "axios";

const data = ref([]);

async function fetchData() {
  try {
    axios
      .get(`${import.meta.env.VITE_API_URL}/api/benefits`, {
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
