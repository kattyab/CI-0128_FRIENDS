<template>
  <div class="container">
    <h1 class="text-center">{{ data?.companyName || 'N/A' }}</h1>
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
        <input
          id="company_name"
          type="text"
          class="form-control"
          disabled
          :value="data?.companyName"
        />
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
        <input
          id="foundation_date"
          type="text"
          class="form-control"
          disabled
          :value="data?.foundationDate ? new Date(data?.foundationDate).toISOString().split('T')[0] : ''"
        />
      </div>
      <div class="mb-3">
        <label for="max_benefits" class="form-label">Beneficios Máximos</label>
        <input
          id="max_benefits"
          type="text"
          class="form-control"
          disabled
          :value="data?.maxBenefits"
        />
      </div>
      <div class="mb-3">
        <label for="web_page" class="form-label">Página Web</label>
        <input id="web_page" type="text" class="form-control" disabled :value="data?.webPage" />
      </div>
      <!--
      <div class="mb-3">
        <label for="logo_path" class="form-label">Logo</label>
        <div class="form-image">
          <img v-if="data?.logo" :src="data?.logo" />
        </div>
      </div>
      -->
      <div class="mb-3">
        <label for="description" class="form-label">Descripción</label>
        <input
          id="description"
          type="text"
          class="form-control"
          disabled
          :value="data?.description"
        />
      </div>
      <div class="mb-3">
        <label for="po" class="form-label">Apartado Postal</label>
        <input id="po" type="text" class="form-control" disabled :value="data?.po" />
      </div>
      <div class="mb-3">
        <label for="province" class="form-label">Provincia</label>
        <input id="province" type="text" class="form-control" disabled :value="data?.province" />
      </div>
      <div class="mb-3">
        <label for="canton" class="form-label">Cantón</label>
        <input id="canton" type="text" class="form-control" disabled :value="data?.canton" />
      </div>
      <div class="mb-3">
        <label for="other_signs" class="form-label">Otras Señas</label>
        <input
          id="other_signs"
          type="text"
          class="form-control"
          disabled
          :value="data?.otherSigns"
        />
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import axios from "axios";

const data = ref(null);
const route = useRoute();

async function fetchData() {
  try {
    axios
      .get(`https://localhost:7153/api/companies/${route.params.id}`, {
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

onMounted(fetchData);
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
</style>
