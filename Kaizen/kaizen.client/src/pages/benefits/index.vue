<template>
  <div class="row">
    <div class="col-1"></div>
    <div class="col-10">
      <h1 class="text-center my-4" style="font-weight: bold">Lista de Beneficios</h1>
      <div class="mx-4 my-4 d-flex justify-content-between align-items-center">
        <div></div>
        <a class="btn btn-lg btn-primary self-align-end" style="font-weight: bold" href="/#/benefits/create">
          Crear beneficio
        </a>
      </div>
      <div class="mx-4">
        <table class="table table-hover">
          <thead>
            <tr>
              <th scope="col">Nombre</th>
              <th scope="col">Tiempo mínimo (meses)</th>
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
              <td>{{ item.isFixed ? "Fijo" : (item.isPercentage ? "Porcentaje" : "API") }}</td>
              <td>{{ item.isFixed ? '₡' + item.fixedValue : (item.isPercentage ? item.percentageValue + '%' : 'Calculado con API') }}</td>
              <td>
                <a :href="item.isAPI ? `/benefits/${item.apiID}` : `/benefits/${item.id}`" class="btn btn-primary">
                  <span class="material-icons">visibility</span>
                </a>
                <button @click="openDeleteModal(item)"
                        class="btn btn-danger ms-1"
                        type="button">
                  <span class="material-icons">delete</span>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div v-if="showFormError" class="form-error-message alert alert-danger mt-3 mb-3 justify-content-center">
        {{ formErrorMessage }}
      </div>
      <div v-if="showSuccessMessage" class="success-message alert alert-success mt-3 mb-3 justify-content-center">
        {{ successMessage }}
      </div>

      <div class="col-1"></div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from "vue";
  import axios from "axios";

  const data = ref([]);
  const itemToDelete = ref(null);
  const showDeleteModal = ref(false);

  const showFormError = ref(false);
  const formErrorMessage = ref('');
  const showSuccessMessage = ref(false);
  const successMessage = ref('');

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

  function openDeleteModal(item) {
    itemToDelete.value = item;
    showDeleteModal.value = true;
  }

  function closeDeleteModal() {
    showDeleteModal.value = false;
    itemToDelete.value = null;
  }

  const showError = (message) => {
    formErrorMessage.value = message;
    showFormError.value = true;
  };

  const showSuccess = (message) => {
    successMessage.value = message;
    showSuccessMessage.value = true;
  };

  onMounted(fetchData);
</script>

<style>
  .alert {
    margin-left: 20em;
    margin-right: 20em;
    text-align: center;
  }
</style>
