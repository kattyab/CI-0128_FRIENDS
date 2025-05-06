<template>
  <div class="container">
    <h1 class="text-center">Company Details</h1>
    <form>
      <div class="mb-3">
        <label for="company_id" class="form-label">Company ID</label>
        <input id="company_id" type="text" class="form-control" disabled :value="data?.companyID" />
      </div>
      <div class="mb-3">
        <label for="owner" class="form-label">Owner</label>
        <input id="owner" type="text" class="form-control" disabled :value="data?.ownerName" />
      </div>
      <div class="mb-3">
        <label for="company_name" class="form-label">Company Name</label>
        <input
          id="company_name"
          type="text"
          class="form-control"
          disabled
          :value="data?.companyName"
        />
      </div>
      <div class="mb-3">
        <label for="brand_name" class="form-label">Brand Name</label>
        <input id="brand_name" type="text" class="form-control" disabled :value="data?.brandName" />
      </div>
      <div class="mb-3">
        <label for="type" class="form-label">Type</label>
        <input id="type" type="text" class="form-control" disabled :value="data?.type" />
      </div>
      <div class="mb-3">
        <label for="foundation_date" class="form-label">Foundation Date</label>
        <input
          id="foundation_date"
          type="text"
          class="form-control"
          disabled
          :value="data?.foundationDate ? new Date(data?.foundationDate).toISOString().split('T')[0] : ''"
        />
      </div>
      <div class="mb-3">
        <label for="max_benefits" class="form-label">Max Benefits</label>
        <input
          id="max_benefits"
          type="text"
          class="form-control"
          disabled
          :value="data?.maxBenefits"
        />
      </div>
      <div class="mb-3">
        <label for="web_page" class="form-label">Web Page</label>
        <input id="web_page" type="text" class="form-control" disabled :value="data?.webPage" />
      </div>
      <div class="mb-3">
        <label for="logo_path" class="form-label">Logo</label>
        <div class="form-image">
          <img v-if="data?.logo" :src="data?.logo" />
        </div>
      </div>
      <div class="mb-3">
        <label for="description" class="form-label">Description</label>
        <input
          id="description"
          type="text"
          class="form-control"
          disabled
          :value="data?.description"
        />
      </div>
      <div class="mb-3">
        <label for="po" class="form-label">PO</label>
        <input id="po" type="text" class="form-control" disabled :value="data?.po" />
      </div>
      <div class="mb-3">
        <label for="province" class="form-label">Province</label>
        <input id="province" type="text" class="form-control" disabled :value="data?.province" />
      </div>
      <div class="mb-3">
        <label for="canton" class="form-label">Canton</label>
        <input id="canton" type="text" class="form-control" disabled :value="data?.canton" />
      </div>
      <div class="mb-3">
        <label for="other_signs" class="form-label">Other Signs</label>
        <input
          id="other_signs"
          type="text"
          class="form-control"
          disabled
          :value="data?.otherSigns"
        />
      </div>
      <a class="btn btn-primary" :href="`/companies/${data?.companyPK}/edit`">Edit</a>
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
