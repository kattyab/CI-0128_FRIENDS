<template>
  <div class="row">
    <div class="col-1"></div>
    <div class="col-10 justify-content-center">
      <h1 class="text-center my-4">Beneficios</h1>

      <div class="mx-5 mb-3 d-flex justify-content-between align-items-center">
        <div class="text">
          <small><i class="bi bi-info-circle me-1"></i>Máximo de beneficios matriculables: {{ maxBenefits }}</small>
        </div>
        <button class="btn btn-lg btn-primary" @click="handleSubscribeClick">
          Suscribir beneficio
        </button>
      </div>

      <!--TODO: CLEAN OR ELIMINATE COMPLETELY THIS ERROR SHOWCASE-->
      <div class="mx-5 mb-3" v-if="errorMessage">
        <div class="alert alert-danger alert-dismissible fade show" role="alert">
          <strong>Error:</strong> {{ errorMessage }}
          <button type="button" class="btn-close" @click="errorMessage = ''" aria-label="Close"></button>
        </div>
      </div>

      <div class="mx-5 table-responsive">
        <div v-if="isLoadingBenefits" class="text-center py-5">
          <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Cargando beneficios...</span>
          </div>
          <p class="mt-2 text-muted">Cargando beneficios...</p>
        </div>

        <div v-else-if="errorMessage && activeBenefits.length === 0" class="text-center py-5">
          <div class="text-muted">
            <i class="material-icons" style="font-size: 3rem;">error_outline</i>
            <p class="mt-2">Incapaz de cargar los beneficios</p>
            <button class="btn btn-outline-primary" @click="refreshBenefits">
              Inténtelo de nuevo.
            </button>
          </div>
        </div>

        <div v-else-if="!isLoadingBenefits && activeBenefits.length === 0" class="text-center py-5">
          <div class="text-muted">
            <i class="material-icons" style="font-size: 3rem;">inbox</i>
            <p class="mt-2">No se encontraron beneficios.</p>
            <p class="small">No se ha suscrito a ningún beneficio todavía.</p>
          </div>
        </div>

        <table v-else class="table table-hover">
          <thead>
            <tr>
              <th scope="col">Nombre del beneficio</th>
              <th scope="col">Método de descargo</th>
              <th scope="col">Meses mínimos para suscribir</th>
              <th scope="col">Suscripción</th>
            </tr>
          </thead>
          <tbody class="table-group-divider">
            <tr v-for="(benefit, index) in activeBenefits" :key="index">
              <td>{{ benefit.name }}</td>
              <td>
                <span v-if="benefit.method.type === 'percentage'">{{ benefit.method.value }}%</span>
                <span v-else-if="benefit.method.type === 'fixed'">₡{{ benefit.method.value }}</span>
                <span v-else>{{ benefit.method.value }}</span>
              </td>
              <td>{{ benefit.minimumMonths }}</td>
              <td>
                <!--TODO: CLEAN unsubscribeBenefit(index) FUNCTIONALITY AS IT IS NOT SPRINT NEEDED-->
                <button class="btn btn-outline-danger btn-sm"
                        @click="unsubscribeBenefit(index)"
                        :disabled="benefit.state === 'Expired' || isLoadingBenefits">
                  Cancelar Suscripción
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Benefit Selection Principal Modal -->
      <div class="modal fade" :class="{ 'show d-block': showSubscribeModal }" tabindex="-1">
        <div class="modal-dialog modal-lg">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">Suscribir beneficios</h5>
              <button type="button" class="btn-close" @click="closeSubscribeModal"></button>
            </div>
            <div class="modal-body">
              <p>Seleccione un beneficio:</p>

              <div v-if="isLoadingAvailableBenefits" class="text-center py-4">
                <div class="spinner-border text-primary" role="status">
                  <span class="visually-hidden">Cargando beneficios disponibles...</span>
                </div>
                <p class="mt-2 text-muted">Cargando beneficios disponibles...</p>
              </div>

              <!--TODO: CONTROL OR ELIMINATE COMPLETELY ERROR STATE-->
              <div v-else-if="availableBenefitsError" class="alert alert-danger">
                <strong>Error:</strong> {{ availableBenefitsError }}
                <button class="btn btn-outline-primary btn-sm ms-2" @click="loadAvailableBenefits">
                  Reintentar
                </button>
              </div>

              <div v-else-if="availableBenefits.length === 0" class="text-center py-5">
                <div class="text-muted">
                  <i class="bi bi-inbox" style="font-size: 3rem;"></i>
                  <p class="mt-2">No hay beneficios disponibles</p>
                  <p class="small">Actualmente no hay beneficios disponibles para suscribir.</p>
                  <button class="btn btn-outline-primary mt-3" @click="closeSubscribeModal">
                    Volver
                  </button>
                </div>
              </div>

              <table v-else class="table table-hover">
                <thead>
                  <tr>
                    <th scope="col">Seleccionar</th>
                    <th scope="col">Nombre</th>
                    <th scope="col">Método de cálculo</th>
                    <th scope="col">Mínimo de meses</th>
                    <th scope="col">Estado</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(benefit, index) in availableBenefits" :key="benefit.id || index" :class="{
            'table-light': benefit.state === 'Disponible',
            'table-secondary': benefit.state === 'No Disponible'
          }">
                    <td>
                      <div class="form-check">
                        <input class="form-check-input"
                               type="radio"
                               :value="index"
                               v-model="selectedBenefitIndex"
                               :disabled="benefit.state === 'No Disponible'">
                      </div>
                    </td>
                    <td>{{ benefit.name }}</td>
                    <td>
                      <span v-if="benefit.method.type === 'percentage'">{{ benefit.method.value }}%</span>
                      <span v-else-if="benefit.method.type === 'fixed'">₡{{ benefit.method.value }}</span>
                      <span v-else>{{ benefit.method.value }}</span>
                    </td>
                    <td>{{ benefit.minimumMonths }}</td>
                    <td>
                      <span :class="{
                'badge bg-success': benefit.state === 'Disponible',
                'badge bg-secondary': benefit.state === 'No Disponible'
              }">
                        {{ benefit.state }}
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
            <div class="modal-footer justify-content-center">
              <button type="button"
                      class="btn btn-success"
                      @click="proceedToConfirmation"
                      :disabled="!hasBenefitSelected">
                Continuar
              </button>
              <button type="button" class="btn btn-secondary" @click="closeSubscribeModal">Cerrar</button>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-backdrop fade" :class="{ 'show': showSubscribeModal }" v-if="showSubscribeModal"></div>

      <!-- Benefit Confirmation Modal -->
      <div class="modal fade" :class="{ 'show d-block': showConfirmationModal }" tabindex="-1">
        <div class="modal-dialog modal-lg">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">Confirmar Suscripción</h5>
              <button type="button" class="btn-close" @click="closeConfirmationModal"></button>
            </div>
            <div class="modal-body">
              <div v-if="selectedBenefit">
                <div class="alert alert-info">
                  <h6 class="mb-3"><strong>{{ selectedBenefit.name }}</strong></h6>

                  <div v-if="selectedBenefit.method.type === 'fixed'">
                    <p><strong>Valor del Beneficio:</strong> ₡{{ selectedBenefit.method.value }}</p>
                  </div>

                  <div v-else-if="selectedBenefit.method.type === 'percentage'">
                    <p><strong>Porcentaje del Beneficio:</strong> {{ selectedBenefit.method.value }}%</p>
                  </div>

                  <div v-else>
                    <p><strong>Tipo de Beneficio:</strong> {{ selectedBenefit.method.value }}</p>
                  </div>

                  <div class="mt-3">
                    <p><strong>Mínimo de Meses:</strong> {{ selectedBenefit.minimumMonths }}</p>
                  </div>
                </div>

                <!-- Additional Input Section for API Benefits - MOVED HERE -->
                <div v-if="requiresAdditionalInput" class="mb-4">
                  <div class="card">
                    <div class="card-header">
                      <h6 class="mb-0">Información Adicional Requerida</h6>
                    </div>
                    <div class="card-body">
                      <!-- Asociación Solidarista Input-->
                      <div v-if="selectedBenefit && selectedBenefit.apiId === 2">
                        <label for="assocName" class="form-label">
                          <strong>Nombre de la Asociación Solidarista:</strong>
                        </label>
                        <input type="text"
                               class="form-control"
                               id="assocName"
                               v-model="assocName"
                               placeholder="Ingrese el nombre de la asociación solidarista"
                               :class="{ 'is-invalid': inputValidationError && selectedBenefit.apiId === 2 }" />
                        <div v-if="inputValidationError && selectedBenefit.apiId === 2" class="invalid-feedback">
                          {{ inputValidationError }}
                        </div>
                      </div>

                      <!-- MediSeguro Dependents Input-->
                      <div v-if="selectedBenefit && selectedBenefit.apiId === 3">
                        <label for="dependents" class="form-label">
                          <strong>Información de Dependientes:</strong>
                        </label>
                        <textarea class="form-control"
                                  id="dependents"
                                  v-model="dependents"
                                  rows="3"
                                  placeholder="Ingrese información sobre sus dependientes"
                                  :class="{ 'is-invalid': inputValidationError && selectedBenefit.apiId === 3 }"></textarea>
                        <div v-if="inputValidationError && selectedBenefit.apiId === 3" class="invalid-feedback">
                          {{ inputValidationError }}
                        </div>
                      </div>
                    </div>
                  </div>

                  <!--TODO: CHECK FOR ERRORS OR ELIMINATE COMPLETELY-->
                  <div v-if="inputValidationError" class="alert alert-warning mt-3">
                    <strong>Atención:</strong> {{ inputValidationError }}
                  </div>
                </div>

                <div class="alert alert-warning">
                  <strong>Confirmación:</strong> ¿Está seguro de que desea suscribirse a este beneficio?
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button"
                      class="btn btn-primary"
                      @click="confirmFinalSubscription"
                      :disabled="isProcessingSubscription || !isInputValid">
                <span v-if="isProcessingSubscription" class="spinner-border spinner-border-sm me-2" role="status"></span>
                {{ isProcessingSubscription ? 'Procesando...' : 'Confirmar Suscripción' }}
              </button>
              <button type="button" class="btn btn-outline-secondary" @click="goBackToSelection">
                Volver
              </button>
              <button type="button" class="btn btn-secondary" @click="closeConfirmationModal">
                Cancelar
              </button>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-backdrop fade" :class="{ 'show': showConfirmationModal }" v-if="showConfirmationModal"></div>

      <!-- Success Modal -->
      <div class="modal fade" :class="{ 'show d-block': showSuccessModal }" tabindex="-1">
        <div class="modal-dialog">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">Suscripción Exitosa</h5>
              <button type="button" class="btn-close" @click="closeSuccessModal"></button>
            </div>
            <div class="modal-body">
              <div class="alert alert-success">
                <h6><strong>¡Felicitaciones!</strong></h6>
                <p class="mb-0">Se ha suscrito exitosamente al beneficio: <strong>{{ subscribedBenefitName }}</strong></p>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-success" @click="closeSuccessModal">
                Aceptar
              </button>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-backdrop fade" :class="{ 'show': showSuccessModal }" v-if="showSuccessModal"></div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import axios from 'axios';

// Backend-connected active benefits
const activeBenefits = ref([]);
const isLoadingBenefits = ref(false);
const errorMessage = ref('');

// Available benefits for subscription
const availableBenefits = ref([]);
const isLoadingAvailableBenefits = ref(false);
const availableBenefitsError = ref('');

const userData = ref(null);

// Modal states
const showSubscribeModal = ref(false);
const showConfirmationModal = ref(false);
const showSuccessModal = ref(false);
const showUnsubscribeModal = ref(false);
const isProcessingSubscription = ref(false);

// Selection states
const selectedBenefitIndex = ref(null);
const benefitToUnsubscribe = ref(null);
const benefitIndexToUnsubscribe = ref(null);
const subscribedBenefitName = ref('');
const maxBenefits = ref(0);
  

// Benefit calculation states
const calculatedBenefitValue = ref(null);
const isCalculatingBenefit = ref(false);
const calculationError = ref('');

// API Benefit specific input states
const assocName = ref('');
const dependents = ref('');
  const inputValidationError = ref('');

const hasReachedMaxBenefits = computed(() => {
  return activeBenefits.value.length >= maxBenefits.value && maxBenefits.value > 0;
});

const selectedBenefit = computed(() => {
  return selectedBenefitIndex.value !== null ? availableBenefits.value[selectedBenefitIndex.value] : null;
});

const hasBenefitSelected = computed(() => {
  return selectedBenefitIndex.value !== null && selectedBenefit.value?.state === 'Disponible';
});

const requiresAdditionalInput = computed(() => {
  if (!selectedBenefit.value || selectedBenefit.value.method.type !== 'specific') return false;
  const apiId = selectedBenefit.value.apiId;
  return apiId === 2 || apiId === 3;
});

const isInputValid = computed(() => {
  if (!selectedBenefit.value || selectedBenefit.value.method.type !== 'specific') return true;

  const apiId = selectedBenefit.value.apiId;

  if (apiId === 2) {
    return assocName.value.trim().length > 0;
  } else if (apiId === 3) {
    return dependents.value.trim().length > 0;
  }

  return true;
});

const loadActiveBenefits = async () => {
  if (!userData.value.email) {
    console.error('User email is required to load benefits');
    return;
  }

  isLoadingBenefits.value = true;
  errorMessage.value = '';

  try {
    const response = await axios.get(
      `${import.meta.env.VITE_API_URL}/api/BenefitEmployeeList/by-email/${encodeURIComponent(userData.value.email)}`,
      { withCredentials: true }
    );

    console.log(userData.value.email)
    console.log(response.data)

    maxBenefits.value = response.data[0].maxBenefits;

    activeBenefits.value = response.data
      .filter(benefit => benefit.benefitId !== null || benefit.apiId !== null)
      .map(benefit => ({
        benefitId: benefit.benefitId,
        apiId: benefit.apiId,
        name: benefit.name,
        type: benefit.type,
        method: transformBenefitMethod(benefit.type, benefit.value),
        minimumMonths: benefit.minMonths,
        state: 'Active'
      }));


  } catch (error) {
    console.error('Error cargando los beneficios suscritos:', error);
    if (error.response?.data) {
      errorMessage.value = 'Hubo un error inesperado.';
    } else {
      errorMessage.value = 'Error de red. Reintentar más tarde.';
    }
  } finally {
    isLoadingBenefits.value = false;
  }
};

  const loadAvailableBenefits = async () => {
    if (!userData.value.email) {
      console.error('User email is required to load available benefits');
      return;
    }

    isLoadingAvailableBenefits.value = true;
    availableBenefitsError.value = '';

    try {
      const response = await axios.get(
        `${import.meta.env.VITE_API_URL}/api/BenefitCompanyOfferedList/by-email/${encodeURIComponent(userData.value.email)}`,
        { withCredentials: true }
      );

      //console.log('Available Benefits API Response:', response.data);

      // Transform the benefits first
      const transformedBenefits = response.data.map(benefit => ({
        benefitId: benefit.benefitId,
        apiId: benefit.apiId,
        name: benefit.name,
        type: benefit.type,
        method: transformBenefitMethod(benefit.type, benefit.value),
        minimumMonths: benefit.minMonths,
        state: benefit.isAvailable ? 'Disponible' : 'No Disponible',
        reasonUnavailable: benefit.reasonUnavailable || null
      }));

      await loadActiveBenefits()
      //console.log(activeBenefits.value)
      // Filter out benefits that are already subscribed
      availableBenefits.value = transformedBenefits.filter(availableBenefit => {
        return !activeBenefits.value.some(activeBenefit => {
          // For API benefits (type === 'isapi' or method.type === 'specific'), compare apiId
          if (availableBenefit.type?.toLowerCase() === 'isapi' || availableBenefit.method.type === 'specific') {
            //console.log('Comparison is api', activeBenefit.benefitId, availableBenefit.benefitId)
            return activeBenefit.apiId === availableBenefit.apiId;
          }
          // For regular benefits, compare benefitId
          else {
            //console.log('Comparison', activeBenefit.benefitId, availableBenefit.benefitId)
            return activeBenefit.benefitId === availableBenefit.benefitId;
          }
        });
      });

      //console.log('Filtered Available Benefits:', availableBenefits.value);

    } catch (error) {
      console.error('Error cargando beneficios disponibles:', error);
      if (error.response?.data) {
        availableBenefitsError.value = 'No se pudo cargar los beneficios disponibles.';
      } else {
        availableBenefitsError.value = 'Error de red. Reintentar más tarde.';
      }
    } finally {
      isLoadingAvailableBenefits.value = false;
    }
  };

const transformBenefitMethod = (type, value) => {
  const lowerType = type?.toLowerCase();

  switch (lowerType) {
    case 'fixed':
      return {
        type: 'fixed',
        value: typeof value === 'number' ? value.toFixed(2) : '0.00'
      };
    case 'percentage':
      return {
        type: 'percentage',
        value: typeof value === 'number' ? value.toFixed(2) : '0.00'
      };
    case 'isapi':
      return { type: 'specific', value: 'Calculado externamente' };
    default:
      return { type: 'specific', value: value || 'Unknown' };
  }
};

  const refreshBenefits = async () => {
    await loadActiveBenefits();
    await loadAvailableBenefits();
  };

const validateInput = () => {
  inputValidationError.value = '';

  if (!selectedBenefit.value || selectedBenefit.value.method.type !== 'specific') return true;

  const apiId = selectedBenefit.value.apiId;

  if (apiId === 2 && assocName.value.trim().length === 0) {
    inputValidationError.value = 'Por favor ingrese el nombre de la asociación solidarista';
    return false;
  }

  if (apiId === 3 && dependents.value.trim().length === 0) {
    inputValidationError.value = 'Por favor ingrese la cantidad de beneficiarios';
    return false;
  }

  return true;
};

const handleSubscribeClick = () => {
  if (hasReachedMaxBenefits.value) {
    errorMessage.value = 'Has seleccionado el máximo de beneficios que ofrece la empresa';
    return;
  }
  showSubscribeModal.value = true;
};

const resetInputs = () => {
  assocName.value = '';
  dependents.value = '';
  inputValidationError.value = '';
};

watch(selectedBenefit, (newBenefit) => {
  calculatedBenefitValue.value = null;
  calculationError.value = '';
  inputValidationError.value = '';
});

watch(showSubscribeModal, (newVal) => {
  if (newVal) loadAvailableBenefits();
  if (!newVal) resetInputs();
});

watch([assocName, dependents], () => {
  if (inputValidationError.value) {
    inputValidationError.value = '';
  }
});

const proceedToConfirmation = () => {
  if (hasBenefitSelected.value) {
    showSubscribeModal.value = false;
    showConfirmationModal.value = true;
  }
};

const goBackToSelection = () => {
  showConfirmationModal.value = false;
  showSubscribeModal.value = true;
};

const closeConfirmationModal = () => {
  showConfirmationModal.value = false;
  closeSubscribeModal();
};

const confirmFinalSubscription = async () => {
  if (!selectedBenefit.value) return;

  if (!validateInput()) {
    return;
  }

  isProcessingSubscription.value = true;

  try {
    if (selectedBenefit.value.method.type === 'specific') {
      // API benefit subscription
      const payload = {
        email: userData.value.email,
        id: selectedBenefit.value.apiId,
        assocName: selectedBenefit.value.apiId === 2 ? assocName.value.trim() : null,
        dependents: selectedBenefit.value.apiId === 3 ? dependents.value.trim() : null
      };

      console.log('API Subscription Payload:', payload);

      await axios.post(`${import.meta.env.VITE_API_URL}/api/BenefitAPISubscription/subscribe`, payload, {
        withCredentials: true
      });
    } else {
      // Regular benefit subscription
      await axios.post(`${import.meta.env.VITE_API_URL}/api/BenefitSubscription/subscribe`, {
        email: userData.value.email,
        benefitId: selectedBenefit.value.benefitId
      }, { withCredentials: true });
    }

    subscribedBenefitName.value = selectedBenefit.value.name;
    await refreshBenefits();

    showConfirmationModal.value = false;
    showSuccessModal.value = true;

  } catch (error) {
    console.error('Error al suscribirse al beneficio:', error);

    let errorMsg = 'No se pudo suscribir al beneficio. Reintentar más tarde.';
    if (error.response?.data) {
      availableBenefitsError.value = 'No se pudo suscribir el beneficio.';
    } else {
      availableBenefitsError.value = 'Error de red. Reintentar más tarde.';
    }
    
    errorMessage.value = errorMsg;
    showConfirmationModal.value = false;
  } finally {
    isProcessingSubscription.value = false;
  }
};

const closeSuccessModal = () => {
  showSuccessModal.value = false;
  closeSubscribeModal();
};

const closeSubscribeModal = () => {
  selectedBenefitIndex.value = null;
  showSubscribeModal.value = false;
  showConfirmationModal.value = false;
  showSuccessModal.value = false;
  subscribedBenefitName.value = '';
  isProcessingSubscription.value = false;
  calculatedBenefitValue.value = null;
  calculationError.value = '';
  resetInputs();
};

// TODO: Implement unsubscription of benefits
const unsubscribeBenefit = (index) => {
};

const closeUnsubscribeModal = () => {
  showUnsubscribeModal.value = false;
  benefitToUnsubscribe.value = null;
  benefitIndexToUnsubscribe.value = null;
};

  onMounted(() => {
    axios.get(`${import.meta.env.VITE_API_URL}/api/login/authenticate`, { withCredentials: true })
      .then(response => {
        userData.value = response.data;
        // Now that userData is populated, we can safely call these:
        loadActiveBenefits();
        loadAvailableBenefits();
      })
      .catch(error => {
        console.error('Error de autenticación:', error);
        showError('No se pudo autenticar el usuario. Por favor, inicie sesión nuevamente.');
      });
  });
</script>

<style scoped>
  .btn-primary {
    background-color: #003c63;
    border-color: #003c63;
    font-weight: bold;
  }

  .btn-outline-danger {
    border-color: #dc3545;
    color: #dc3545;
    font-weight: bold;
  }

    .btn-outline-danger:hover {
      background-color: #dc3545;
      color: white;
    }

    .btn-outline-danger:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

  /* Contents from empresa-lista maintained */
  h4 {
    color: #043c62;
  }

  th {
    color: #003c63;
    user-select: none;
  }

  .table-wrapper {
    max-height: 300px;
    overflow-y: auto;
    border: 1px solid #dee2e6;
  }

  .table {
    margin-bottom: 0;
  }

    .table th,
    .table td {
      vertical-align: middle;
      white-space: nowrap;
    }

  .sticky-header {
    position: sticky;
    top: 0;
    z-index: 2;
    background-color: #f8f9fa;
  }

  .btn-outline-primary {
    color: #003c63;
    border-color: #003c63;
  }

    .btn-outline-primary:hover {
      background-color: #003c63;
      color: white;
    }

  .table-responsive {
    overflow-x: auto;
    -webkit-overflow-scrolling: touch;
  }
  /* Modal styles */
  .modal-backdrop {
    opacity: 0.5;
  }

  .modal.show {
    background-color: rgba(0, 0, 0, 0.5);
  }

  .spinner-border-sm {
    width: 1rem;
    height: 1rem;
  }

  .bg-light {
    background-color: #f8f9fa !important;
  }
</style>
