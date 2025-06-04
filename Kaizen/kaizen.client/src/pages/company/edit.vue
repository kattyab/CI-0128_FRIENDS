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
          <input
            id="company_name"
            type="text"
            class="form-control"
            :class="{ 'is-invalid': errors.form.companyName !== '' }"
            v-model="data.companyName"
          />
          <div class="invalid-feedback">
            {{ errors.form.companyName }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="brand_name" class="form-label">Nombre de Fantasía</label>
        <div class="input-group has-validation">
          <input
            id="brand_name"
            type="text"
            class="form-control"
            :class="{ 'is-invalid': errors.form.brandName }"
            v-model="data.brandName"
          />
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
        <input
          id="foundation_date"
          type="text"
          class="form-control"
          disabled
          :value="formatDate(data.foundationDate)"
        />
      </div>
      <div class="mb-3">
        <label for="max_benefits" class="form-label">Beneficios Máximos</label>
        <div class="input-group has-validation">
          <input
            id="max_benefits"
            type="number"
            min="0"
            max="255"
            class="form-control"
            :class="{ 'is-invalid': errors.form.maxBenefits }"
            v-model="data.maxBenefits"
          />
          <div class="invalid-feedback">
            {{ errors.form.maxBenefits }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="web_page" class="form-label">Página Web</label>
        <div class="input-group has-validation">
          <input
            id="web_page"
            type="text"
            class="form-control"
            :class="{ 'is-invalid': errors.form.webPage }"
            v-model="data.webPage"
          />
          <div class="invalid-feedback">
            {{ errors.form.webPage }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="description" class="form-label">Descripción</label>
        <div class="input-group has-validation">
          <input
            id="description"
            type="text"
            class="form-control"
            :class="{ 'is-invalid': errors.form.description }"
            v-model="data.description"
          />
          <div class="invalid-feedback">
            {{ errors.form.description }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="po" class="form-label">Apartado Postal</label>
        <div class="input-group has-validation">
          <input
            id="po"
            type="text"
            class="form-control"
            :class="{ 'is-invalid': errors.form.po }"
            v-model="data.po"
          />
          <div class="invalid-feedback">
            {{ errors.form.po }}
          </div>
        </div>
      </div>


      <div class="mb-3">
        <label for="phoneNumbers" class="form-label">Numeros de Telefono</label>
        <div class="input-group has-validation">
          <input
            id="phoneNumbers"
            type="text"
            class="form-control"
            :class="{ 'is-invalid': errors.form.phoneNumbers }"
            v-model="data.phoneNumbers"
          />
          <div class="invalid-feedback">
            {{ errors.form.phoneNumbers }}
          </div>
        </div>
      </div>

      <div class="mb-3">
        <label for="emails" class="form-label">Correos Electronicos</label>
        <div class="input-group has-validation">
          <input
            id="emails"
            type="text"
            class="form-control"
            :class="{ 'is-invalid': errors.form.emails }"
            v-model="data.emails"
          />
          <div class="invalid-feedback">
            {{ errors.form.emails }}
          </div>
        </div>
      </div>

      <div class="mb-3">
        <label for="province" class="form-label">Provincia</label>
        <div class="input-group has-validation">
          <select
            id="province"
            class="form-control"
            :class="{ 'is-invalid': errors.form.province }"
            v-model="selectedProvince"
          >
            <option value="" disabled selected>Seleccione una provincia</option>
            <option v-for="province in provinces" :key="province.id" :value="province.id">
              {{ province.nombre }}
            </option>
          </select>
          <div class="invalid-feedback">
            {{ errors.form.province }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="canton" class="form-label">Cantón</label>
        <div class="input-group has-validation">
          <select
            id="canton"
            class="form-control"
            :class="{ 'is-invalid': errors.form.canton }"
            v-model="selectedCanton"
          >
            <option value="" disabled selected>Seleccione un canton</option>
            <option v-for="canton in cantons" :key="canton.id" :value="canton.id">
              {{ canton.nombre }}
            </option>
          </select>
          <div class="invalid-feedback">
            {{ errors.form.canton }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="district" class="form-label">Distrito</label>
        <div class="input-group has-validation">
          <select
            id="district"
            class="form-control"
            :class="{ 'is-invalid': errors.form.distrito }"
            v-model="selectedDistrict"
          >
            <option value="" disabled selected>Seleccione un distrito</option>
            <option v-for="district in districts" :key="district.id" :value="district.id">
              {{ district.nombre }}
            </option>
          </select>
          <div class="invalid-feedback">
            {{ errors.form.distrito }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="other_signs" class="form-label">Otras Señas</label>
        <div class="input-group has-validation">
          <input
            id="other_signs"
            type="text"
            class="form-control"
            :class="{ 'is-invalid': errors.form.otherSigns }"
            v-model="data.otherSigns"
          />
          <div class="invalid-feedback">
            {{ errors.form.otherSigns }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="logo" class="form-label">Logo</label>
        <div class="input-group has-validation">
          <input
            id="logo"
            type="file"
            class="form-control"
            :class="{ 'is-invalid': errors.form.logo }"
            accept="image/*"
            @change="onLogoChange"
          />
          <div class="invalid-feedback">
            {{ errors.form.logo }}
          </div>
        </div>
        <div v-if="data.logo" class="d-flex align-items-center justify-content-between">
          <div class="form-image">
            <img :src="data.logo" />
          </div>
          <button type="button" class="btn btn-danger ms-1" @click.stop="removeLogo">
            <span class="material-icons">delete</span>
          </button>
        </div>
      </div>

      <div class="d-flex justify-content-center pt-3 pb-3">
        <button
          type="button"
          class="btn btn-secondary btn-lg btn-block me-2"
          @click.stop="handleCancel"
        >
          Cancelar
        </button>
        <button type="submit" class="btn btn-primary btn-lg btn-block" :disabled="isSubmitting">
          Guardar
        </button>
      </div>

      <div class="row" v-if="formResult.message">
        <div class="col-4"></div>
        <div class="col-4">
          <div
            class="form-error-message alert mt-3 mb-3"
            :class="{
              'alert-success': formResult.error == false,
              'alert-danger': formResult.error == true,
            }"
          >
            {{ formResult.message }}
          </div>
        </div>
        <div class="col-4"></div>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from "vue";
import { useRouter } from "vue-router";
import axios from "axios";
import { formatDate } from "@/composables/formatDate";
import { isValidUrl } from "@/composables/isValidUrl";
import addressHelper from "@/composables/addressHelper";

const router = useRouter();

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
  phoneNumbers: null,
  emails: null,
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
    phoneNumbers: "",
    emails: "",
  },
});
const isSubmitting = ref(false);
const formResult = ref({
  error: false,
  message: "",
});

const selectedProvince = ref(null);
const selectedCanton = ref(null);
const selectedDistrict = ref(null);

const provinces = ref(addressHelper.getProvinces());
const cantons = ref([]);
const districts = ref([]);

watch(selectedProvince, (newProvince) => {
  if (newProvince) {
    data.value.province = provinces.value.find((p) => p.id == newProvince)?.nombre || null;
    cantons.value = addressHelper.getCantons(selectedProvince.value);
    data.value.canton = null;
    districts.value = [];
  }
});

watch(selectedCanton, (newCanton) => {
  if (newCanton) {
    data.value.canton = cantons.value.find((c) => c.id == newCanton)?.nombre || null;
    districts.value = addressHelper.getDistricts(selectedProvince.value, selectedCanton.value);
    data.value.distrito = null;
  }
});

watch(selectedDistrict, (newDistrict) => {
  if (newDistrict) {
    data.value.distrito = districts.value.find((d) => d.id == newDistrict)?.nombre || null;
  }
});

async function fetchData() {
  try {
    const response = await axios.get(`/api/CompanyDetails/by-email/${email.value}`, {
      withCredentials: true,
    });
    data.value = response.data;

    selectedProvince.value = addressHelper.getProvinceIdByName(data.value.province);
    selectedCanton.value = addressHelper.getCantonIdByName(
      selectedProvince.value,
      data.value.canton
    );
    selectedDistrict.value = addressHelper.getDistrictIdByName(
      selectedProvince.value,
      selectedCanton.value,
      data.value.distrito
    );

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

const removeLogo = () => {
  data.value.logo = null;
  errors.value.form.logo = "";
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
  } else if (data.value.maxBenefits > 255) {
    errors.value.form.maxBenefits = "Los beneficios máximos no pueden exceder 255.";
  } else {
    errors.value.form.maxBenefits = "";
  }

  if (!data.value.webPage || data.value.webPage.trim() === "") {
    errors.value.form.webPage = "";
  } else if (data.value.webPage.length > 200) {
    errors.value.form.webPage = "La página web no puede exceder los 200 caracteres.";
  } else if (!isValidUrl(data.value.webPage)) {
    errors.value.form.webPage = "La página web debe ser una URL válida.";
  } else {
    errors.value.form.webPage = "";
  }

  if (!/^\d+$/.test(data.value.maxBenefits)) {
    errors.value.form.po = "El apartado postal debe ser un número.";
  } else if (data.value.po.length != 5) {
    errors.value.form.po = "El apartado postal debe tener exactamente 5 dígitos.";
  } else {
    errors.value.form.po = "";
  }

  const phoneNumbers = data.value.phoneNumbers.split(",").map((phone) => phone.trim()).filter((phone) => phone !== "");
  const duplicatedPhoneNumbers = phoneNumbers.filter((item, index) => phoneNumbers.indexOf(item) !== index);
  if (phoneNumbers.length < 1) {
    errors.value.form.phoneNumbers = "Al menos un número de teléfono es obligatorio.";
  } else if (duplicatedPhoneNumbers.length > 0) {
    errors.value.form.phoneNumbers = `Los siguientes números de teléfono están duplicados: ${[...new Set(duplicatedPhoneNumbers)].join(", ")}.`;
  } else {
    errors.value.form.phoneNumbers = "";
  }

  for (let i = 0; i < phoneNumbers.length; i++) {
    const phone = phoneNumbers[i];
    if (!/^\d{4}-\d{4}$/.test(phone)) {
      errors.value.form.phoneNumbers = `El número de teléfono ${phone} no es válido. Debe ser de la forma: XXXX-XXXX.`;
    }
  }

  const emails = data.value.emails.split(",").map((email) => email.trim()).filter((email) => email !== "");
  const duplicatedEmails = emails.filter((item, index) => emails.indexOf(item) !== index);
  if (emails.length < 1) {
    errors.value.form.emails = "Al menos un correo electronico es obligatorio.";
  } else if (duplicatedEmails.length > 0) {
    errors.value.form.emails = `Los siguientes correos electronicos están duplicados: ${[...new Set(duplicatedEmails)].join(", ")}.`;
  } else {
    errors.value.form.emails = "";
  }

  for (let i = 0; i < emails.length; i++) {
    const email = emails[i];
    if (!/^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/.test(email)) {
      errors.value.form.emails = `El correo electronico ${email} no es válido. Debe ser de la forma: usuario@dominio.tld.`;
    }
  }

  if (!data.value.province) {
    errors.value.form.province = "La provincia es obligatoria.";
  } else {
    errors.value.form.province = "";
  }

  if (!data.value.canton) {
    errors.value.form.canton = "El cantón es obligatorio.";
  } else {
    errors.value.form.canton = "";
  }

  if (!data.value.distrito) {
    errors.value.form.distrito = "El cantón es obligatorio.";
  } else {
    errors.value.form.distrito = "";
  }

  if (data.value.otherSigns.length > 50) {
    errors.value.form.otherSigns = "Otras señas no puede exceder los 50 caracteres.";
  } else {
    errors.value.form.otherSigns = "";
  }

  return (
    errors.value.form.companyName === "" &&
    errors.value.form.brandName === "" &&
    errors.value.form.maxBenefits === "" &&
    errors.value.form.webPage === "" &&
    errors.value.form.description === "" &&
    errors.value.form.po === "" &&
    errors.value.form.province === "" &&
    errors.value.form.canton === "" &&
    errors.value.form.distrito === "" &&
    errors.value.form.otherSigns === "" &&
    errors.value.form.logo === "" &&
    errors.value.form.phoneNumbers === "" &&
    errors.value.form.emails === ""
  );
}

const handleCancel = () => {
  goToShowPage();
};

const goToShowPage = () => {
  router.push(`/landing-page`);
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
        `${import.meta.env.VITE_API_URL}/api/Companies/user`,
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
          phoneNumbers: data.value.phoneNumbers,
          emails: data.value.emails,
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
