<template>
  <div class="page container-fluid mt-4">
    <div>
      <h1>Ver Beneficio</h1>
    </div>
    <div>
      <div class="row">
        <div class="col-1"></div>
        <div class="col-10">
          <form>
            <div class="mb-3">
              <label for="benefitName" class="form-label">Nombre beneficio</label>
              <input
                type="text"
                class="form-control input-type"
                id="benefitName"
                :value="formData.benefitName"
                readonly
              />
            </div>

            <div class="mb-3">
              <label for="minimumTime" class="form-label">Tiempo mínimo (meses)</label>
              <input
                type="number"
                class="form-control input-type"
                id="minimumTime"
                :value="formData.minimumTime"
                readonly
              />
            </div>

            <div class="mb-3">
              <label class="form-label d-block">Contratos elegibles</label>
              <div class="d-flex justify-content-between elegibles-container">
                <div class="form-check elegible-item">
                  <input
                    class="form-check-input"
                    type="checkbox"
                    id="fullTime"
                    :checked="formData.elegibles.fullTime"
                    disabled
                  />
                  <label class="form-check-label ms-2" for="fullTime">Tiempo Completo</label>
                </div>
                <div class="form-check elegible-item">
                  <input
                    class="form-check-input"
                    type="checkbox"
                    id="partTime"
                    :checked="formData.elegibles.partTime"
                    disabled
                  />
                  <label class="form-check-label ms-2" for="partTime">Medio Tiempo</label>
                </div>
                <div class="form-check elegible-item">
                  <input
                    class="form-check-input"
                    type="checkbox"
                    id="byHours"
                    :checked="formData.elegibles.byHours"
                    disabled
                  />
                  <label class="form-check-label ms-2" for="byHours">Por Horas</label>
                </div>
                <div class="form-check elegible-item">
                  <input
                    class="form-check-input"
                    type="checkbox"
                    id="byServices"
                    :checked="formData.elegibles.byServices"
                    disabled
                  />
                  <label class="form-check-label ms-2" for="byServices">Por Servicios</label>
                </div>
              </div>
            </div>

            <div class="mb-3">
              <label for="benefitType" class="form-label">Tipo de beneficio</label>
              <input
                type="text"
                class="form-control input-type"
                id="fixedAmount"
                :value="formData.benefitType == 'fixedAmount' ? 'Monto Fijo' : 'Porcentaje'"
                readonly
              />
            </div>

            <div v-if="formData.benefitType === 'fixedAmount'" class="mb-3">
              <label for="fixedAmount" class="form-label">Monto fijo</label>
              <div class="input-group">
                <span class="input-group-text input-type">₡</span>
                <input
                  type="number"
                  class="form-control input-type"
                  id="fixedAmount"
                  :value="formData.fixedAmount"
                  readonly
                />
              </div>
            </div>

            <div v-if="formData.benefitType === 'percentage'" class="mb-3">
              <label for="percentage" class="form-label">Porcentaje</label>
              <div class="input-group">
                <input
                  type="number"
                  class="form-control input-type"
                  id="percentage"
                  :value="formData.percentage"
                  readonly
                />
                <span class="input-group-text input-type">%</span>
              </div>
            </div>

            <div class="d-flex justify-content-center pt-3 pb-3">
              <a type="button" class="btn btn-secondary btn-lg btn-block me-2" href="/benefits">
                Atras
              </a>
              <a
                type="button"
                class="btn btn-primary btn-lg btn-block"
                :href="`/benefits/${formData.id}/edit`"
                :disabled="formData.isSubscribed"
              >
                Editar
              </a>
            </div>
            <div class="row">
              <div class="col-4"></div>
              <div class="col-4">
                <div v-if="showFormError" class="form-error-message alert alert-danger mt-3 mb-3">
                  {{ formErrorMessage }}
                </div>
              </div>
              <div class="col-4"></div>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from "vue";
import { useRoute } from "vue-router";
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
      isSubscribed: false,
    });

    const route = useRoute();

    const showFormError = ref(false);
    const formErrorMessage = ref("");

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
          formData.benefitType = data.isFixed ? "fixedAmount" : "percentage";
          formData.fixedAmount = data.fixedValue;
          formData.percentage = data.percentageValue;
          formData.isSubscribed = data.isSubscribed;
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

    return {
      formData,
      showFormError,
      formErrorMessage,
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
