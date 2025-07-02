<template>
  <div class="container">
    <div v-if="data">
      <h1 class="text-center">{{ data?.companyName || "N/A" }}</h1>
      <form>
        <div class="mb-3">
          <label for="company_id" class="form-label">Cédula Jurídica</label>
          <input id="company_id" type="text" class="form-control" disabled :value="data?.companyID" />
        </div>
        <div class="mb-3">
          <label for="owner" class="form-label">Dueño</label>
          <input id="owner" type="text" class="form-control" disabled :value="data?.ownerName" />
        </div>
        <div class="mb-3">
          <label for="company_name" class="form-label">Nombre de Empresa</label>
          <input id="company_name"
                 type="text"
                 class="form-control"
                 disabled
                 :value="data?.companyName" />
        </div>
        <div class="mb-3">
          <label for="brand_name" class="form-label">Nombre de Fantasía</label>
          <input id="brand_name" type="text" class="form-control" disabled :value="data?.brandName" />
        </div>
        <div class="mb-3">
          <label for="type" class="form-label">Tipo</label>
          <input id="type" type="text" class="form-control" disabled :value="data?.type" />
        </div>
        <div class="mb-3">
          <label for="foundation_date" class="form-label">Fecha de Fundación</label>
          <input id="foundation_date"
                 type="text"
                 class="form-control"
                 disabled
                 :value="
            data?.foundationDate ? new Date(data?.foundationDate).toISOString().split('T')[0] : ''
          " />
        </div>
        <div class="mb-3">
          <label for="max_benefits" class="form-label">Beneficios Máximos</label>
          <input id="max_benefits"
                 type="text"
                 class="form-control"
                 disabled
                 :value="data?.maxBenefits" />
        </div>
        <div class="mb-3">
          <label for="web_page" class="form-label">Página Web</label>
          <input id="web_page" type="text" class="form-control" disabled :value="data?.webPage" />
        </div>
        <div class="mb-3">
          <label for="description" class="form-label">Descripción</label>
          <input id="description"
                 type="text"
                 class="form-control"
                 disabled
                 :value="data?.description" />
        </div>
        <div class="mb-3">
          <label for="po" class="form-label">Apartado Postal</label>
          <input id="po" type="text" class="form-control" disabled :value="data?.po" />
        </div>
        <div class="mb-3">
          <label for="province" class="form-label">Provincia</label>
          <input id="province" type="text" class="form-control" disabled :value="provinceValue" />
        </div>
        <div class="mb-3">
          <label for="canton" class="form-label">Cantón</label>
          <input id="canton" type="text" class="form-control" disabled :value="cantonValue" />
        </div>
        <div class="mb-3">
          <label for="other_signs" class="form-label">Otras Señas</label>
          <input id="other_signs"
                 type="text"
                 class="form-control"
                 disabled
                 :value="otherSignsValue" />
        </div>
        <div class="mb-3">
          <label for="logo_path" class="form-label">Logo</label>
          <div class="form-image">
            <img v-if="data?.logo" :src="data?.logo" />
          </div>
        </div>
        <div class="d-flex justify-content-center gap-3 pt-3 pb-3">
          <a type="submit" class="btn btn-primary btn-lg" href="/company/edit"> Editar </a>
          <button type="button" class="btn btn-danger btn-lg" @click="showDeleteModal = true">
            Borrar empresa
          </button>
        </div>
      </form>
    </div>
    <div v-else class="text-center mt-5">
      <h2>No tienes una compañía asociada</h2>
      <p class="text-muted">Tu compañía ya no existe o no se pudo cargar la información.</p>
    </div>

    <div class="modal fade" :class="{ show: showDeleteModal }" :style="{ display: showDeleteModal ? 'block' : 'none' }" tabindex="-1" aria-labelledby="deleteModalLabel" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="deleteModalLabel">Confirmar eliminación</h5>
            <button type="button" class="btn-close" @click="showDeleteModal = false" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <p>¿Estás seguro de que deseas eliminar la empresa <strong>{{ data?.companyName }}</strong>?</p>
            <p class="text-muted">Esta acción no se puede deshacer.</p>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="showDeleteModal = false">
              Cancelar
            </button>
            <button type="button" class="btn btn-danger" @click="confirmDeleteCompany" :disabled="isDeleting">
              <span v-if="isDeleting" class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
              {{ isDeleting ? 'Eliminando...' : 'Eliminar empresa' }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal backdrop -->
    <div v-if="showDeleteModal" class="modal-backdrop fade show" @click="showDeleteModal = false"></div>
  </div>
</template>

<script setup>
  import { ref, onMounted, computed } from "vue";
  import axios from "axios";
  import { useLogout } from '@/composables/useLogout';

  const data = ref(null);
  const emailComponent = ref(null);
  const showDeleteModal = ref(false);
  const isDeleting = ref(false);
  const { logout } = useLogout();

  const provinceValue = computed(() => data.value?.province || "N/A");
  const cantonValue = computed(() => data.value?.canton || "N/A");
  const otherSignsValue = computed(() => data.value?.otherSigns || "N/A");

  async function fetchData(email) {
    try {
      const response = await axios.get(
        `${import.meta.env.VITE_API_URL}/api/CompanyDetails/by-email/${email}`,
        {
          withCredentials: true,
        }
      );
      data.value = response.data;
    } catch (error) {
      console.error("Error fetching company data:", error);
      data.value = null;
    }
  }

  const confirmDeleteCompany = async () => {
    isDeleting.value = true;
    try {
      const response = await axios.delete(`${import.meta.env.VITE_API_URL}/api/Companies/delete`, {
        withCredentials: true,
      });
      console.log("deleting", response);

      showDeleteModal.value = false;

      logout();

    } catch (error) {
      console.error("Error deleting company:", error);
      alert("Error al eliminar la empresa. Por favor, inténtalo de nuevo.");
    } finally {
      isDeleting.value = false;
    }
  };

  onMounted(async () => {
    try {
      const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/login/authenticate`, {
        withCredentials: true,
      });
      emailComponent.value = response.data.email;

      await fetchData(emailComponent.value);
    } catch (error) {
      console.error("Error fetching email:", error);
    }
  });
</script>

<style scoped>
  .form-image {
    padding-left: 2rem;
    max-width: 300px;
    max-height: 300px;
    overflow: hidden;
  }

    .form-image img {
      width: 100%;
      height: auto;
    }

  .modal {
    background-color: rgba(0, 0, 0, 0.5);
  }

  .gap-3 {
    gap: 1rem;
  }
</style>
