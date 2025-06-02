<template>
  <div class="page container-fluid mt-4">
    <div>
      <h1>Editar Beneficios</h1>
    </div>
    <div>
      <div class="row">
        <div class="col-1"></div>
        <div class="col-10">
          <form @submit.prevent="submitForm">
            <div class="mb-3">
              <label for="benefitName" class="form-label">Nombre beneficio</label>
              <input
                type="text"
                class="form-control input-type"
                id="benefitName"
                v-model="formData.benefitName"
                :class="{ 'is-invalid': validationErrors.benefitName }"
                placeholder="Ingrese el nombre del beneficio"
                required
              />
              <div class="invalid-feedback" v-if="validationErrors.benefitName">
                {{ validationErrors.benefitName }}
              </div>
            </div>

            <div class="mb-3">
              <label for="minimumTime" class="form-label">Tiempo mínimo (meses)</label>
              <input
                type="number"
                class="form-control input-type"
                id="minimumTime"
                v-model.number="formData.minimumTime"
                :class="{ 'is-invalid': validationErrors.minimumTime }"
                placeholder="Ingrese el tiempo mínimo para ser elegible"
                min="0"
                required
              />
              <div class="invalid-feedback" v-if="validationErrors.minimumTime">
                {{ validationErrors.minimumTime }}
              </div>
            </div>

            <div class="mb-3">
              <label class="form-label d-block">Contratos elegibles</label>
              <div class="d-flex justify-content-between elegibles-container">
                <div class="form-check elegible-item">
                  <input
                    class="form-check-input"
                    type="checkbox"
                    id="fullTime"
                    v-model="formData.elegibles.fullTime"
                  />
                  <label class="form-check-label ms-2" for="fullTime">Tiempo Completo</label>
                </div>
                <div class="form-check elegible-item">
                  <input
                    class="form-check-input"
                    type="checkbox"
                    id="partTime"
                    v-model="formData.elegibles.partTime"
                  />
                  <label class="form-check-label ms-2" for="partTime">Medio Tiempo</label>
                </div>
                <div class="form-check elegible-item">
                  <input
                    class="form-check-input"
                    type="checkbox"
                    id="byHours"
                    v-model="formData.elegibles.byHours"
                  />
                  <label class="form-check-label ms-2" for="byHours">Por Horas</label>
                </div>
                <div class="form-check elegible-item">
                  <input
                    class="form-check-input"
                    type="checkbox"
                    id="byServices"
                    v-model="formData.elegibles.byServices"
                  />
                  <label class="form-check-label ms-2" for="byServices">Por Servicios</label>
                </div>
              </div>
              <div class="text-danger small mt-1" v-if="validationErrors.elegibles">
                {{ validationErrors.elegibles }}
              </div>
            </div>

            <div class="mb-3">
              <label for="benefitType" class="form-label">Tipo de beneficio</label>
              <select
                class="form-select input-type"
                id="benefitType"
                v-model="formData.benefitType"
                :class="{ 'is-invalid': validationErrors.benefitType }"
                required
              >
                <option value="" disabled>Seleccione un tipo</option>
                <option value="fixedAmount">Monto fijo</option>
                <option value="percentage">Porcentaje</option>
                <option value="api">API</option>
              </select>
              <div class="invalid-feedback" v-if="validationErrors.benefitType">
                {{ validationErrors.benefitType }}
              </div>
            </div>

            <div v-if="formData.benefitType === 'fixedAmount'" class="mb-3">
              <label for="fixedAmount" class="form-label">Monto fijo</label>
              <div class="input-group">
                <span class="input-group-text input-type">₡</span>
                <input
                  type="number"
                  class="form-control input-type"
                  id="fixedAmount"
                  v-model.number="formData.fixedAmount"
                  :class="{ 'is-invalid': validationErrors.fixedAmount }"
                  min="0"
                  placeholder="25000"
                  required
                />
                <div class="invalid-feedback" v-if="validationErrors.fixedAmount">
                  {{ validationErrors.fixedAmount }}
                </div>
              </div>
            </div>

            <div v-if="formData.benefitType === 'percentage'" class="mb-3">
              <label for="percentage" class="form-label">Porcentaje</label>
              <div class="input-group">
                <input
                  type="number"
                  class="form-control input-type"
                  id="percentage"
                  v-model.number="formData.percentage"
                  :class="{ 'is-invalid': validationErrors.percentage }"
                  step="0.01"
                  min="0"
                  max="100"
                  placeholder="100"
                  required
                />
                <span class="input-group-text input-type">%</span>
                <div class="invalid-feedback" v-if="validationErrors.percentage">
                  {{ validationErrors.percentage }}
                </div>
              </div>
            </div>

            <div v-if="formData.benefitType === 'api'">
              <label for="apiUrl" class="form-label">URL de API</label>
              <input
                type="url"
                class="form-control input-type mb-3"
                id="apiUrl"
                v-model="formData.apiUrl"
                :class="{ 'is-invalid': validationErrors.apiUrl }"
                placeholder="http://example.com"
                required
              />
              <div class="invalid-feedback" v-if="validationErrors.apiUrl">
                {{ validationErrors.apiUrl }}
              </div>

              <div>
                <label for="parameterQuantity" class="form-label mt-3"
                  >Cantidad de parámetros</label
                >
                <select
                  class="form-select input-type mb-3"
                  id="parameterQuantity"
                  v-model.number="formData.parameterQuantity"
                  :class="{ 'is-invalid': validationErrors.parameterQuantity }"
                  required
                >
                  <option value="1">1</option>
                  <option value="2">2</option>
                  <option value="3">3</option>
                </select>
                <div class="invalid-feedback" v-if="validationErrors.parameterQuantity">
                  {{ validationErrors.parameterQuantity }}
                </div>
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
              <button
                type="submit"
                class="btn btn-primary btn-lg btn-block"
                :disabled="isSubmitting"
              >
                Guardar
              </button>
            </div>
            <div class="row">
              <div class="col-4"></div>
              <div class="col-4">
                <div v-if="showFormError" class="form-error-message alert alert-danger mt-3 mb-3">
                  {{ formErrorMessage }}
                </div>
                <div
                  v-if="showSuccessMessage"
                  class="success-message alert alert-success mt-3 mb-3"
                >
                  {{ successMessage }}
                </div>
              </div>
              <div class="col-4"></div>
            </div>
          </form>
        </div>
        <div class="col-1"></div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from "vue";
import { useRouter, useRoute } from "vue-router";
import axios from "axios";

export default {
  name: "BenefitCreation",

  setup() {
    const formData = reactive({
      id: null,
      benefitName: "",
      minimumTime: 0,
      elegibles: {
        fullTime: false,
        partTime: false,
        byHours: false,
        byServices: false,
      },
      benefitType: "",
      fixedAmount: null,
      percentage: null,
      apiUrl: "",
      parameterQuantity: 1,
    });

    const router = useRouter();
    const route = useRoute();

    const validationErrors = reactive({});
    const isSubmitting = ref(false);

    const showFormError = ref(false);
    const formErrorMessage = ref("");
    const showSuccessMessage = ref(false);
    const successMessage = ref("");

    onMounted(() => {
      axios
        .get(`${import.meta.env.VITE_API_URL}/api/Benefits/${route.params.id}`, {
          withCredentials: true,
        })
        .then((response) => {
          const data = response.data;
          if (!data) {
            showError("Beneficio no encontrado. Por favor, verifique el ID.");
            return;
          }

          formData.id = data.id;
          formData.benefitName = data.name;
          formData.minimumTime = data.minWorkDurationMonths;
          formData.elegibles.fullTime = data.isFullTime;
          formData.elegibles.partTime = data.isPartTime;
          formData.elegibles.byHours = data.isByHours;
          formData.elegibles.byServices = data.isByService;
          formData.benefitType = data.isFixed
            ? "fixedAmount"
            : data.isPercentage
            ? "percentage"
            : "api";
          formData.fixedAmount = data.fixedValue;
          formData.percentage = data.percentageValue;
          formData.apiUrl = data.path;
          formData.parameterQuantity = data.numParameters || 1;
        })
        .catch((error) => {
          console.error("No se pudieron cargar los datos:", error);
          showError("No se pudieron cargar los datos.");
        });
    });

    const showError = (message) => {
      formErrorMessage.value = message;
      showFormError.value = true;
    };

    const showSuccess = (message) => {
      successMessage.value = message;
      showSuccessMessage.value = true;
    };

    const validateForm = () => {
      const errors = {};
      let hasErrors = false;

      if (!formData.benefitName.trim()) {
        errors.benefitName = "El nombre del beneficio es requerido";
        hasErrors = true;
      }

      if (formData.minimumTime === null || formData.minimumTime < 0) {
        errors.minimumTime = "El tiempo mínimo debe ser un número positivo";
        hasErrors = true;
      }

      if (
        !formData.elegibles.fullTime &&
        !formData.elegibles.partTime &&
        !formData.elegibles.byHours &&
        !formData.elegibles.byServices
      ) {
        errors.elegibles = "Debe seleccionar al menos una opción";
        hasErrors = true;
      }

      if (!formData.benefitType) {
        errors.benefitType = "Debe seleccionar un tipo de beneficio";
        hasErrors = true;
      }

      if (formData.benefitType === "fixedAmount") {
        if (formData.fixedAmount === null || formData.fixedAmount < 0) {
          errors.fixedAmount = "El monto debe ser un número positivo";
          hasErrors = true;
        }
      } else if (formData.benefitType === "percentage") {
        if (formData.percentage === null || formData.percentage < 0 || formData.percentage > 100) {
          errors.percentage = "El porcentaje debe estar entre 0 y 100";
          hasErrors = true;
        }
      } else if (formData.benefitType === "api") {
        const urlPattern = /^(https?:\/\/)?([\da-z.-]+)\.([a-z.]{2,6})([/\w .-]*)*\/?$/;
        if (!formData.apiUrl || !urlPattern.test(formData.apiUrl)) {
          errors.apiUrl = "Debe ingresar una URL válida";
          hasErrors = true;
        }
        if (![1, 2, 3].includes(formData.parameterQuantity)) {
          errors.parameterQuantity = "Debe seleccionar una cantidad válida de parámetros";
          hasErrors = true;
        }
      }

      Object.keys(validationErrors).forEach((key) => {
        delete validationErrors[key];
      });

      Object.keys(errors).forEach((key) => {
        validationErrors[key] = errors[key];
      });

      if (hasErrors) {
        showError("Por favor corrija los errores en el formulario antes de continuar.");
      }

      return !hasErrors;
    };

    const submitForm = async () => {
      await updateBenefit();
    };

    const updateBenefit = async () => {
      if (!validateForm()) {
        return;
      }

      isSubmitting.value = true;

      showFormError.value = false;

      try {
        const benefitData = {
          id: formData.id,
          name: formData.benefitName,
          minWorkDurationMonths: formData.minimumTime,
          isFullTime: formData.elegibles.fullTime,
          isPartTime: formData.elegibles.partTime,
          isByHours: formData.elegibles.byHours,
          isByService: formData.elegibles.byServices,
          isFixed: formData.benefitType === "fixedAmount",
          fixedValue: formData.benefitType === "fixedAmount" ? formData.fixedAmount : null,
          isPercentage: formData.benefitType === "percentage",
          percentageValue: formData.benefitType === "percentage" ? formData.percentage : null,
          isAPI: formData.benefitType === "api",
          apiPath: formData.benefitType === "api" ? formData.apiUrl : null,
          numParameters: formData.benefitType === "api" ? formData.parameterQuantity : null,
        };

        const response = await axios.post(
          `${import.meta.env.VITE_API_URL}/api/Benefits/${route.params.id}`,
          benefitData
        );

        if (response.status === 200) {
          showSuccess("Beneficio editado exitosamente.");
          setTimeout(() => {
            goToShowPage();
          }, 3000);
        } else {
          showError("No se pudo editar el beneficio. Por favor, intente nuevamente.");
        }
      } catch (error) {
        console.error("Error editing benefit:", error);

        if (error.response) {
          if (error.response.status === 400) {
            showError("Datos inválidos. Por favor verifique la información ingresada.");
          } else if (error.response.status === 401) {
            showError("No se pudo autorizar la accion.");
          } else if (error.response.status === 403) {
            showError("No tiene permisos para realizar esta acción.");
          } else if (error.response.status === 500) {
            showError("Error en el servidor. Por favor, intente más tarde.");
          } else {
            showError(
              `Error: ${error.response.data.message || "No se pudo completar la operación"}`
            );
          }
        } else {
          showError("Error de conexión. Por favor, verifique su conexión a internet.");
        }
      } finally {
        isSubmitting.value = false;
      }
    };

    const handleCancel = () => {
      console.log("Cancel button clicked, navigating to show page.");
      goToShowPage();
    };

    const goToShowPage = () => {
      router.push(`/benefits/${route.params.id}`);
    };

    return {
      formData,
      validationErrors,
      isSubmitting,
      submitForm,
      handleCancel,
      showFormError,
      formErrorMessage,
      showSuccessMessage,
      successMessage,
    };
  },
};
</script>

<style scoped>
.page {
  margin-top: 1rem;
  margin-bottom: 1rem;
}

h1 {
  text-align: center;
  color: #003c63;
  font-weight: bold;
}

.mb-3 {
  padding-bottom: 1.25rem;
}

.input-type {
  border-radius: 10px;
  padding: 0.75rem;
  background-color: #f2f2f2;
  border: 1px solid #f2f2f2;
  transition: box-shadow 0.2s ease;
}

::placeholder {
  color: #6c757d !important;
}

input,
select,
textarea {
  color: #000 !important;
}

.input-type:focus {
  outline: none;
  border-color: #aaa;
  box-shadow: 0 0 0 2px rgba(0, 60, 99, 0.15);
}

.elegibles-container {
  width: 100%;
  padding: 0.5rem 0;
}

.elegible-item {
  display: flex;
  align-items: center;
  margin-right: 10px;
}

.form-check-input {
  margin-top: 0;
}

.form-check-label {
  margin-left: 0.5rem;
}

.form-check-input:focus {
  outline: none;
  border-color: #aaa;
  box-shadow: 0 0 0 2px rgba(0, 60, 99, 0.15);
}

.btn-primary {
  background-color: #003c63;
  border-color: #003c63;
  font-weight: bold;
}

.btn-secondary {
  background-color: #6c757d;
  border-color: #6c757d;
  font-weight: bold;
}

.form-error-message,
.success-message {
  border-radius: 10px;
  text-align: center;
}
</style>
