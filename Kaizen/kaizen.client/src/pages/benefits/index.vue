<template>
  <div>
    <h1 class="text-center my-4">Lista de Beneficios</h1>
    <div class="mx-4 my-4 d-flex justify-content-between align-items-center">
      <div></div>
      <button class="btn btn-lg btn-primary self-align-end" @click="router.push('/benefits/create')">
        Crear beneficio
      </button>
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
              <button class="btn btn-primary" @click="router.push(`/benefits/${item.isAPI ? item.apiID : item.id}`)">
                <span class="material-icons">visibility</span>
              </button>
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

    <!-- Delete Confirmation Modal -->
    <div v-if="showDeleteModal"
         class="modal fade show"
         style="display: block;"
         tabindex="-1"
         aria-labelledby="deleteModalLabel">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="deleteModalLabel">Confirmar eliminación</h5>
            <button type="button"
                    class="btn-close"
                    @click="closeDeleteModal"
                    aria-label="Close"></button>
          </div>
          <div class="modal-body">
            ¿Está seguro que desea eliminar el beneficio "{{ itemToDelete?.name }}"?
            <br>
            <small class="text-muted">Esta acción no se puede deshacer.</small>
          </div>
          <div class="modal-footer">
            <button type="button"
                    class="btn btn-secondary"
                    @click="closeDeleteModal">
              Cancelar
            </button>
            <button type="button"
                    class="btn btn-danger"
                    @click="confirmDelete">
              Eliminar
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal backdrop -->
    <div v-if="showDeleteModal"
         class="modal-backdrop fade show"
         @click="closeDeleteModal"></div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from "vue";
  import { useRouter } from 'vue-router';
  import axios from "axios";

  const router = useRouter();

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

  async function deleteBenefit(item) {
    try {
      let endpoint, id;

      if (item.apiID) {
        endpoint = `${import.meta.env.VITE_API_URL}/api/benefits/api/${item.apiID}`;
        id = item.apiID;
      } else {
        endpoint = `${import.meta.env.VITE_API_URL}/api/benefits/${item.id}`;
        id = item.id;
      }

      await axios.delete(endpoint, {
        withCredentials: true,
      });

      console.log(`Benefit deleted successfully (${item.isAPI ? 'API' : 'Regular'} - ID: ${id})`);
      showSuccess("El beneficio fue eliminado exitosamente.");

      await fetchData();
    } catch (error) {
      console.error("Error deleting benefit:", error);
      showError("Hubo un error eliminando el beneficio. Inténtelo más tarde");
      throw error;
    }
  }

  async function confirmDelete() {
    if (itemToDelete.value) {
      try {
        await deleteBenefit(itemToDelete.value);
        closeDeleteModal();
      } catch (error) {
        console.error("Failed to delete benefit:", error);
      }
    }
  }

  onMounted(fetchData);
</script>

<style>
  .alert {
    margin-left: 20em;
    margin-right: 20em;
    text-align: center;
  }
</style>
