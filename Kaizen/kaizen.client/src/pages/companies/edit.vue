<template>
  <div class="container">
    <h1 class="text-center">{{ data.companyName || "N/A" }}</h1>
    <form @submit.prevent="submitForm" class="needs-validation">
      <div class="mb-3">
        <label for="company_id" class="form-label">Cédula Jurídica</label>
        <input id="company_id" type="text" class="form-control" disabled v-model="data.companyID" />
      </div>
      <div class="mb-3">
        <label for="owner" class="form-label">Dueño</label>
        <input id="owner" type="text" class="form-control" disabled v-model="data.ownerName" />
      </div>
      <div class="mb-3">
        <label for="company_name" class="form-label">Nombre de Empresa</label>
        <div class="input-group has-validation">
          <input id="company_name" type="text" class="form-control" :class="{ 'is-invalid': errors.form.companyName !== '' }" v-model="data.companyName" />
          <div class="invalid-feedback">
            {{ errors.form.companyName }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="brand_name" class="form-label">Nombre de Fantasía</label>
        <div class="input-group has-validation">
          <input id="brand_name" type="text" class="form-control" :class="{ 'is-invalid': errors.form.brandName }" v-model="data.brandName" />
          <div class="invalid-feedback">
            {{ errors.form.brandName }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="type" class="form-label">Tipo</label>
        <input id="type" type="text" class="form-control" disabled v-model="data.type" />
      </div>
      <div class="mb-3">
        <label for="foundation_date" class="form-label">Fecha de Fundación</label>
        <input id="foundation_date" type="text" class="form-control"
          disabled :value="formatDate(data.foundationDate)" />
      </div>
      <div class="mb-3">
        <label for="max_benefits" class="form-label">Beneficios Máximos</label>
        <div class="input-group has-validation">
          <input id="max_benefits" type="number" min="0" max="1000" class="form-control" :class="{ 'is-invalid': errors.form.maxBenefits }" v-model="data.maxBenefits" />
          <div class="invalid-feedback">
            {{ errors.form.maxBenefits }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="web_page" class="form-label">Página Web</label>
        <div class="input-group has-validation">
          <input id="web_page" type="text" class="form-control" :class="{ 'is-invalid': errors.form.webPage }" v-model="data.webPage" />
          <div class="invalid-feedback">
            {{ errors.form.webPage }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="description" class="form-label">Descripción</label>
        <div class="input-group has-validation">
          <input id="description" type="text" class="form-control" :class="{ 'is-invalid': errors.form.description }" v-model="data.description" />
          <div class="invalid-feedback">
            {{ errors.form.description }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="po" class="form-label">Apartado Postal</label>
        <div class="input-group has-validation">
          <input id="po" type="text" class="form-control" :class="{ 'is-invalid': errors.form.po }" v-model="data.po" />
          <div class="invalid-feedback">
            {{ errors.form.po }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="province" class="form-label">Provincia</label>
        <div class="input-group has-validation">
          <input id="province" type="text" class="form-control" :class="{ 'is-invalid': errors.form.province }" v-model="data.province" />
          <div class="invalid-feedback">
            {{ errors.form.province }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="canton" class="form-label">Cantón</label>
        <div class="input-group has-validation">
          <input id="canton" type="text" class="form-control" :class="{ 'is-invalid': errors.form.canton }" v-model="data.canton" />
          <div class="invalid-feedback">
            {{ errors.form.canton }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="district" class="form-label">Distrito</label>
        <div class="input-group has-validation">
          <input id="district" type="text" class="form-control" :class="{ 'is-invalid': errors.form.distrito }" v-model="data.distrito" />
          <div class="invalid-feedback">
            {{ errors.form.distrito }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="other_signs" class="form-label">Otras Señas</label>
        <div class="input-group has-validation">
          <input id="other_signs" type="text" class="form-control" :class="{ 'is-invalid': errors.form.otherSigns }" v-model="data.otherSigns" />
          <div class="invalid-feedback">
            {{ errors.form.otherSigns }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="logo" class="form-label">Logo</label>
        <div class="input-group has-validation">
          <input id="logo" type="file" class="form-control" :class="{ 'is-invalid': errors.form.logo }" accept="image/*" @change="onLogoChange" />
          <div class="invalid-feedback">
            {{ errors.form.logo }}
          </div>
        </div>
        <div class="form-image">
          <img v-if="data.logo" :src="data.logo" />
        </div>
      </div>

      <div class="d-flex justify-content-center pt-3 pb-3">
        <button type="button" class="btn btn-secondary btn-lg btn-block me-2" @click.stop="handleCancel">
          Cancelar
        </button>
        <button type="submit" class="btn btn-primary btn-lg btn-block" :disabled="isSubmitting">
          Guardar
        </button>
      </div>

      <div class="row" v-if="formResult.message">
        <div class="col-4"></div>
        <div class="col-4">
          <div class="form-error-message alert mt-3 mb-3" :class="{
              'alert-success': formResult.error == false,
              'alert-danger': formResult.error == true,
            }">
            {{ formResult.message }}
          </div>
        </div>
        <div class="col-4"></div>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRouter, useRoute } from "vue-router";
import axios from "axios";
import { formatDate } from "@/composables/formatDate";
import { isValidUrl } from "@/composables/isValidUrl";

const router = useRouter();
const route = useRoute();

const email = ref(null);
const data = ref({
  companyPK: null,
  companyID: null,
  ownerName: null,
  companyName: null,
  brandName: null,
  type: null,
  foundationDate: null,
  maxBenefits: 0,
  webPage: null,
  description: null,
  po: null,
  province: null,
  canton: null,
  distrito: null,
  otherSigns: null,
  logo: null,
});
const errors = ref({
  form: {
    companyName: "",
    brandName: "",
    maxBenefits: "",
    webPage: "",
    description: "",
    po: "",
    province: "",
    canton: "",
    distrito: "",
    otherSigns: "",
    logo: "",
  },
});
const isSubmitting = ref(false);
const formResult = ref({
  error: false,
  message: "",
});

async function fetchData() {
  try {
    const response = await axios.get(`/api/Companies/${route.params.id}`, {
      withCredentials: true,
    });
    data.value = response.data;
  } catch (error) {
    console.error("Error fetching company data:", error);
  }
}

const onLogoChange = (event) => {
  const file = event.target.files[0];
  if (file && file.type.startsWith("image/")) {
    const reader = new FileReader();
    reader.onload = (e) => {
      data.value.logo = e.target.result;
    };
    reader.readAsDataURL(file);
    errors.value.form.logo = "";
  } else {
    data.value.logo = null;
    errors.value.form.logo = "Por favor, suba un archivo de imagen válido.";
  }
};

function validateData() {
  formResult.value = {
    error: false,
    message: "",
  };

  if (!data.value.companyName || data.value.companyName.trim() === "") {
    errors.value.form.companyName = "El nombre de la empresa es obligatorio.";
  } else if (data.value.companyName.length > 100) {
    errors.value.form.companyName = "El nombre de la empresa no puede exceder los 100 caracteres.";
  } else {
    errors.value.form.companyName = "";
  }

  if (!data.value.brandName || data.value.brandName.trim() === "") {
    errors.value.form.brandName = "El nombre de fantasía es obligatorio.";
  } else if (data.value.brandName.length > 100) {
    errors.value.form.brandName = "El nombre de fantasía no puede exceder los 100 caracteres.";
  } else {
    errors.value.form.brandName = "";
  }

  if (data.value.maxBenefits < 0) {
    errors.value.form.maxBenefits = "Los beneficios máximos no pueden ser negativos.";
  } else if (data.value.maxBenefits > 1000) {
    errors.value.form.maxBenefits = "Los beneficios máximos no pueden exceder 1000.";
  } else {
    errors.value.form.maxBenefits = "";
  }

  if (data.value.webPage.length > 200) {
    errors.value.form.webPage = "La página web no puede exceder los 200 caracteres.";
  } else if (!isValidUrl(data.value.webPage)) {
    errors.value.form.webPage = "La página web debe ser una URL válida.";
  } else {
    errors.value.form.webPage = "";
  }

  if (data.value.description.length > 1000) {
    errors.value.form.description = "La descripción no puede exceder los 1000 caracteres.";
  } else {
    errors.value.form.description = "";
  }

  if (!/^\d+$/.test(data.value.maxBenefits)) {
    errors.value.form.po = "El apartado postal debe ser un número.";
  } else if (data.value.po.length != 5) {
    errors.value.form.po = "El apartado postal debe tener exactamente 5 dígitos.";
  } else {
    errors.value.form.po = "";
  }

  if (!data.value.province || data.value.province.trim() === "") {
    errors.value.form.province = "La provincia es obligatoria.";
  } else if (data.value.province.length > 20) {
    errors.value.form.province = "La provincia no puede exceder los 20 caracteres.";
  } else {
    errors.value.form.province = "";
  }

  if (!data.value.canton || data.value.canton.trim() === "") {
    errors.value.form.canton = "El cantón es obligatorio.";
  } else if (data.value.canton.length > 50) {
    errors.value.form.canton = "El cantón no puede exceder los 50 caracteres.";
  } else {
    errors.value.form.canton = "";
  }

  if (data.value.distrito.length > 50) {
    errors.value.form.distrito = "El distrito no puede exceder los 20 caracteres.";
  } else {
    errors.value.form.distrito = "";
  }

  if (data.value.otherSigns.length > 200) {
    errors.value.form.otherSigns = "Otras señas no puede exceder los 200 caracteres.";
  } else {
    errors.value.form.otherSigns = "";
  }

  return errors.value.form.companyName === "" &&
    errors.value.form.brandName === "" &&
    errors.value.form.maxBenefits === "" &&
    errors.value.form.webPage === "" &&
    errors.value.form.description === "" &&
    errors.value.form.po === "" &&
    errors.value.form.province === "" &&
    errors.value.form.canton === "" &&
    errors.value.form.distrito === "" &&
    errors.value.form.otherSigns === "";
}

const goToShowPage = () => {
  router.push(`/companieslist`);
};

async function submitForm() {
  isSubmitting.value = true;
  try {
    if (!validateData()) {
      formResult.value = {
        error: true,
        message: "Por favor, complete todos los campos requeridos apropiadamente.",
      };
      isSubmitting.value = false;
      return;
    }

    axios
      .post(
        `${import.meta.env.VITE_API_URL}/api/Companies/${route.params.id}`,
        {
          companyName: data.value.companyName,
          brandName: data.value.brandName,
          maxBenefits: data.value.maxBenefits,
          webPage: data.value.webPage,
          description: data.value.description,
          po: data.value.po,
          province: data.value.province,
          canton: data.value.canton,
          distrito: data.value.distrito,
          otherSigns: data.value.otherSigns,
          logo: data.value.logo,
        },
        {
          withCredentials: true,
        }
      )
      .then((response) => {
        formResult.value = {
            error: false,
            message: "Datos de compañia actualizados correctamente!",
          };
          setTimeout(() => {
            goToShowPage();
          }, 3000);
      })
      .catch((error) => {
        if (error.response) {
          if (error.response.status === 400) {
            formResult.value = {
              error: true,
              message: "Datos inválidos. Por favor verifique la información ingresada.",
            };
          } else if (error.response.status === 401) {
            formResult.value = {
              error: true,
              message: "No se pudo autorizar la acción. Por favor, inicie sesión nuevamente.",
            };
          } else if (error.response.status === 403) {
            formResult.value = {
              error: true,
              message: "No tiene permisos para realizar esta acción.",
            };
          } else if (error.response.status === 500) {
            formResult.value = {
              error: true,
              message: "Error en el servidor. Por favor, intente más tarde.",
            };
          } else {
            formResult.value = {
              error: true,
              message: `Error: ${
                error.response.data.message || "No se pudo completar la operación"
              }`,
            };
          }
        } else {
          showError("Error de conexión. Por favor, verifique su conexión a internet.");
        }

        console.error("Error submitting form:", error);
      });
  } catch (error) {
    console.error("Error submitting form:", error);
  } finally {
    isSubmitting.value = false;
  }
}

onMounted(async () => {
  try {
    const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/login/authenticate`, {
      withCredentials: true,
    });

    email.value = response.data.email;

    await fetchData();
  } catch (error) {
    console.error("Authentication failed", error);
  }
});
</script>

<style scoped>
.form-image {
  margin-top: 1rem;
  margin-left: 2rem;
  max-width: 300px;
  max-height: 300px;
  overflow: hidden;
}

.form-image img {
  width: 100%;
  height: auto;
}
</style>
