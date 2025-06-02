<template>
  <div class="row">
    <div class="col-1"></div>
    <div class="col-10 justify-content-center">
      <h1 class="text-center my-4">Benefits</h1>

      <!-- Subscribe button at the top with right alignment -->
      <div class="mx-5 mb-3 d-flex justify-content-end">
        <button class="btn btn-primary btn-lg" @click="showSubscribeModal = true">
          <!--<span class="material-icons align-middle me-1">add_circle</span>-->
          Subscribe to another benefit
        </button>
      </div>

      <!-- Add this right after the "Subscribe to another benefit" button -->
      <div class="mx-5 mb-3" v-if="errorMessage">
        <div class="alert alert-danger alert-dismissible fade show" role="alert">
          <strong>Error:</strong> {{ errorMessage }}
          <button type="button" class="btn-close" @click="errorMessage = ''" aria-label="Close"></button>
        </div>
      </div>

      <!-- Add loading spinner for the table -->
      <div class="mx-5">
        <!-- Loading state -->
        <div v-if="isLoadingBenefits" class="text-center py-5">
          <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Loading benefits...</span>
          </div>
          <p class="mt-2 text-muted">Loading your benefits...</p>
        </div>

        <!-- Error state -->
        <div v-else-if="errorMessage && activeBenefits.length === 0" class="text-center py-5">
          <div class="text-muted">
            <i class="material-icons" style="font-size: 3rem;">error_outline</i>
            <p class="mt-2">Unable to load benefits</p>
            <button class="btn btn-outline-primary" @click="refreshBenefits">
              <span class="material-icons align-middle me-1" style="font-size: 16px;">refresh</span>
              Try Again
            </button>
          </div>
        </div>

        <!-- Empty state -->
        <div v-else-if="!isLoadingBenefits && activeBenefits.length === 0" class="text-center py-5">
          <div class="text-muted">
            <i class="material-icons" style="font-size: 3rem;">inbox</i>
            <p class="mt-2">No benefits found</p>
            <p class="small">You haven't subscribed to any benefits yet.</p>
          </div>
        </div>

        <!-- Table with benefits (your existing table) -->
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
                <span v-else-if="benefit.method.type === 'fixed'">${{ benefit.method.value }}</span>
                <span v-else>{{ benefit.method.value }}</span>
              </td>
              <td>{{ benefit.minimumMonths }}</td>
              <td>
                <button class="btn btn-outline-danger btn-sm"
                        @click="unsubscribeBenefit(index)"
                        :disabled="benefit.state === 'Expired' || isLoadingBenefits">
                  <span class="material-icons align-middle me-1" style="font-size: 16px;">cancel</span>
                  Unsubscribe
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Subscribe to Benefits Modal with Single Selection Functionality -->
      <div class="modal fade" :class="{ 'show d-block': showSubscribeModal }" tabindex="-1">
        <div class="modal-dialog modal-lg">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">Subscribe to Benefits</h5>
              <button type="button" class="btn-close" @click="closeSubscribeModal"></button>
            </div>
            <div class="modal-body">
              <p>Select the benefit you want to subscribe to:</p>

              <!-- Loading state for available benefits -->
              <div v-if="isLoadingAvailableBenefits" class="text-center py-4">
                <div class="spinner-border text-primary" role="status">
                  <span class="visually-hidden">Loading available benefits...</span>
                </div>
                <p class="mt-2 text-muted">Loading available benefits...</p>
              </div>

              <!-- Error state for available benefits -->
              <div v-else-if="availableBenefitsError" class="alert alert-danger">
                <strong>Error:</strong> {{ availableBenefitsError }}
                <button class="btn btn-outline-primary btn-sm ms-2" @click="loadAvailableBenefits">
                  <span class="material-icons align-middle me-1" style="font-size: 14px;">refresh</span>
                  Retry
                </button>
              </div>

              <!-- Available benefits table -->
              <table v-else class="table table-hover">
                <thead>
                  <tr>
                    <th scope="col">Select</th>
                    <th scope="col">Benefit Name</th>
                    <th scope="col">Method</th>
                    <th scope="col">Min. Months</th>
                    <th scope="col">State</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(benefit, index) in availableBenefits" :key="benefit.id || index" :class="{
                  'table-light': benefit.state === 'Available',
                  'table-secondary': benefit.state === 'Not Available'
                }">
                    <td>
                      <div class="form-check">
                        <input class="form-check-input"
                               type="radio"
                               :value="index"
                               v-model="selectedBenefitIndex"
                               :disabled="benefit.state === 'Not Available'">
                      </div>
                    </td>
                    <td>{{ benefit.name }}</td>
                    <td>
                      <span v-if="benefit.method.type === 'percentage'">{{ benefit.method.value }}%</span>
                      <span v-else-if="benefit.method.type === 'fixed'">${{ benefit.method.value }}</span>
                      <span v-else>{{ benefit.method.value }}</span>
                    </td>
                    <td>{{ benefit.minimumMonths }}</td>
                    <td>
                      <span :class="{
                      'badge bg-info': benefit.state === 'Available',
                      'badge bg-secondary': benefit.state === 'Not Available'
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
                <span class="material-icons align-middle me-1">check_circle</span>
                Continue
              </button>
              <button type="button" class="btn btn-secondary" @click="closeSubscribeModal">Close</button>
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
              <h5 class="modal-title">Confirm Benefit Subscription</h5>
              <button type="button" class="btn-close" @click="closeConfirmationModal"></button>
            </div>
            <div class="modal-body">
              <div v-if="selectedBenefit">
                <div class="alert alert-info">
                  <h6 class="mb-3"><strong>{{ selectedBenefit.name }}</strong></h6>

                  <!-- Fixed Value Benefits -->
                  <div v-if="selectedBenefit.method.type === 'fixed'">
                    <p><strong>Benefit Value:</strong> ${{ selectedBenefit.method.value }}</p>
                  </div>

                  <!-- Percentage Benefits -->
                  <div v-else-if="selectedBenefit.method.type === 'percentage'">
                    <p><strong>Benefit Percentage:</strong> {{ selectedBenefit.method.value }}%</p>
                  </div>

                  <!-- Specific/Calculated Benefits -->
                  <div v-else>
                    <p><strong>Benefit Type:</strong> {{ selectedBenefit.method.value }}</p>

                    <!-- Loading state for calculated value -->
                    <div v-if="isCalculatingBenefit" class="mt-3 p-3 bg-light rounded text-center">
                      <div class="spinner-border spinner-border-sm text-primary me-2"></div>
                      <span>Calculating benefit value...</span>
                    </div>

                    <!-- Error state for calculation -->
                    <div v-else-if="calculationError" class="mt-3 p-3 bg-danger bg-opacity-10 rounded">
                      <h6 class="text-danger">Calculation Error:</h6>
                      <p class="text-danger mb-2">{{ calculationError }}</p>
                      <button class="btn btn-outline-danger btn-sm" @click="calculateBenefitValue">
                        <span class="material-icons align-middle me-1" style="font-size: 14px;">refresh</span>
                        Retry Calculation
                      </button>
                    </div>

                    <!-- Successfully calculated value -->
                    <div v-else-if="calculatedBenefitValue !== null" class="mt-3 p-3 bg-light rounded">
                      <h6 class="text-success">Calculated Benefit Value:</h6>
                      <p class="h5 text-success mb-0">${{ calculatedBenefitValue.toLocaleString() }}</p>
                      <small class="text-muted">
                        * This value is calculated based on your profile information and benefit parameters
                      </small>
                    </div>
                  </div>

                </div>

                <div class="alert alert-warning">
                  <strong>Please confirm:</strong> Are you sure you want to subscribe to this benefit?
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button"
                      class="btn btn-success"
                      @click="confirmFinalSubscription"
                      :disabled="isProcessingSubscription || (selectedBenefit?.method.type === 'specific' && calculatedBenefitValue === null && !calculationError)">
                <span v-if="isProcessingSubscription" class="spinner-border spinner-border-sm me-2"></span>
                <span class="material-icons align-middle me-1" v-else>check_circle</span>
                {{ isProcessingSubscription ? 'Processing...' : 'Confirm Subscription' }}
              </button>
              <button type="button" class="btn btn-secondary" @click="goBackToSelection">
                <span class="material-icons align-middle me-1">arrow_back</span>
                Back to Selection
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
            <div class="modal-header bg-success text-white">
              <h5 class="modal-title">
                <span class="material-icons align-middle me-2">check_circle</span>
                Subscription Confirmed
              </h5>
            </div>
            <div class="modal-body text-center">
              <div class="py-3">
                <span class="material-icons text-success" style="font-size: 4rem;">done_all</span>
                <h5 class="mt-3">Successfully Subscribed!</h5>
                <p class="text-muted">You have successfully subscribed to <strong>{{ subscribedBenefitName }}</strong>.</p>
              </div>
            </div>
            <div class="modal-footer justify-content-center">
              <button type="button" class="btn btn-primary" @click="closeSuccessModal">
                Continue
              </button>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-backdrop fade" :class="{ 'show': showSuccessModal }" v-if="showSuccessModal"></div>

      <!-- Unsubscribe Confirmation Modal -->
      <div class="modal fade" :class="{ 'show d-block': showUnsubscribeModal }" tabindex="-1">
        <div class="modal-dialog">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">Confirm Unsubscription</h5>
              <button type="button" class="btn-close" @click="closeUnsubscribeModal"></button>
            </div>
            <div class="modal-body">
              <p>Are you sure you want to unsubscribe from <strong>{{ benefitToUnsubscribe?.name }}</strong>?</p>
              <p class="text-muted">This action cannot be undone and you may lose access to this benefit immediately.</p>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-danger" @click="confirmUnsubscription">
                <span class="material-icons align-middle me-1" style="font-size: 16px;">cancel</span>
                Yes, Unsubscribe
              </button>
              <button type="button" class="btn btn-secondary" @click="closeUnsubscribeModal">Cancel</button>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-backdrop fade" :class="{ 'show': showUnsubscribeModal }" v-if="showUnsubscribeModal"></div>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed, onMounted, watch } from 'vue';
  import axios from 'axios';

  // Backend-connected active benefits (replaces the old static array)
  const activeBenefits = ref([]);
  const isLoadingBenefits = ref(false);
  const errorMessage = ref('');

  // Available benefits for subscription - now loaded from backend
  const availableBenefits = ref([]);
  const isLoadingAvailableBenefits = ref(false);
  const availableBenefitsError = ref('');

  // User profile data (this would typically come from your user management system)
  const userProfile = ref({
    gender: "Female",
    salaryRate: 85000,
    yearsOfService: 7,
    department: "Engineering",
    employmentType: "Full-time",
    age: 32
  });

  // You'll need to get the user's email from your authentication system
  // This could come from: auth store, route params, user profile, etc.
  const userEmail = ref('juan.perez@example.com'); // TODO: Replace with actual user email

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

  // Benefit calculation states
  const calculatedBenefitValue = ref(null);
  const isCalculatingBenefit = ref(false);
  const calculationError = ref('');

  const selectedBenefit = computed(() => {
    return selectedBenefitIndex.value !== null ? availableBenefits.value[selectedBenefitIndex.value] : null;
  });

  const hasBenefitSelected = computed(() => {
    return selectedBenefitIndex.value !== null && selectedBenefit.value?.state === 'Available';
  });

  // Function to load active benefits from backend
  const loadActiveBenefits = async () => {
    if (!userEmail.value) {
      console.error('User email is required to load benefits');
      return;
    }

    isLoadingBenefits.value = true;
    errorMessage.value = '';

    try {
      const response = await axios.get(
        `${import.meta.env.VITE_API_URL}/api/EmployeeBenefitList/by-email/${encodeURIComponent(userEmail.value)}`,
        { withCredentials: true }
      );

      console.log('Active Benefits API Response:', response.data);

      // Transform backend DTO to frontend format - using correct property names
      activeBenefits.value = response.data.map(benefit => ({
        benefitId: benefit.BenefitID,
        apiId: benefit.APIId,
        name: benefit.name,
        type: benefit.type,
        method: transformBenefitMethod(benefit.type, benefit.value),
        minimumMonths: benefit.minMonths,
        state: 'Active'
      }));

      console.log('Transformed Active Benefits:', activeBenefits.value);

    } catch (error) {
      console.error('Error loading active benefits:', error);
      if (error.response?.data) {
        errorMessage.value = typeof error.response.data === 'string' 
          ? error.response.data 
          : 'Failed to load benefits';
      } else {
        errorMessage.value = 'Network error. Please try again.';
      }
    } finally {
      isLoadingBenefits.value = false;
    }
  };

  // Function to load available benefits from backend
  const loadAvailableBenefits = async () => {
    if (!userEmail.value) {
      console.error('User email is required to load available benefits');
      return;
    }

    isLoadingAvailableBenefits.value = true;
    availableBenefitsError.value = '';

    try {
      const response = await axios.get(
        `${import.meta.env.VITE_API_URL}/api/OfferedBenefits/available/${encodeURIComponent(userEmail.value)}`,
        { withCredentials: true }
      );

      console.log('Available Benefits API Response:', response.data);

      // Transform backend DTO to frontend format
      availableBenefits.value = response.data.map(benefit => ({
        benefitId: benefit.benefitId,
        apiId: benefit.apiId,
        name: benefit.name,
        type: benefit.type,
        method: transformBenefitMethod(benefit.type, benefit.value),
        minimumMonths: benefit.minMonths,
        state: benefit.isAvailable ? 'Available' : 'Not Available',
        reasonUnavailable: benefit.reasonUnavailable || null
      }));

      console.log('Transformed Available Benefits:', availableBenefits.value);

    } catch (error) {
      console.error('Error loading available benefits:', error);
      if (error.response?.data) {
        availableBenefitsError.value = typeof error.response.data === 'string' 
          ? error.response.data 
          : 'Failed to load available benefits';
      } else {
        availableBenefitsError.value = 'Network error. Please try again.';
      }
    } finally {
      isLoadingAvailableBenefits.value = false;
    }
  };

  // Updated transformation function
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
        return { type: 'specific', value: 'Calculated via API' };
      default:
        return { type: 'specific', value: value || 'Unknown' };
    }
  };

  // Function to calculate benefit value for specific/API benefits
  const calculateBenefitValue = async () => {
    if (!selectedBenefit.value || selectedBenefit.value.method.type !== 'specific') {
      calculatedBenefitValue.value = null;
      return;
    }

    isCalculatingBenefit.value = true;
    calculationError.value = '';
    calculatedBenefitValue.value = null;

    try {
      // TODO: Replace with actual API call to calculate benefit value
      const response = await axios.post(
        `${import.meta.env.VITE_API_URL}/api/BenefitCalculation/calculate`,
        {
          benefitId: selectedBenefit.value.id,
          userEmail: userEmail.value,
          userProfile: userProfile.value
        },
        { withCredentials: true }
      );

      console.log('Benefit calculation response:', response.data);
      
      // Assuming the API returns { calculatedValue: number }
      calculatedBenefitValue.value = response.data.calculatedValue || 0;

    } catch (error) {
      console.error('Error calculating benefit value:', error);
      calculationError.value = error.response?.data?.message || 'Failed to calculate benefit value';
      calculatedBenefitValue.value = null;
    } finally {
      isCalculatingBenefit.value = false;
    }
  };

  // Function to refresh benefits (can be called after subscribe/unsubscribe)
  const refreshBenefits = () => {
    loadActiveBenefits();
    loadAvailableBenefits();
  };

  // Watch for selected benefit changes to trigger calculation
  watch(selectedBenefit, (newBenefit) => {
    if (newBenefit && newBenefit.method.type === 'specific') {
      calculateBenefitValue();
    } else {
      calculatedBenefitValue.value = null;
      calculationError.value = '';
    } 
  });

  watch(showSubscribeModal, (newVal) => {
    if (newVal) loadAvailableBenefits();
  });

  // Event handlers
  const unsubscribeBenefit = (index) => {
    benefitToUnsubscribe.value = activeBenefits.value[index];
    benefitIndexToUnsubscribe.value = index;
    showUnsubscribeModal.value = true;
  };

  const confirmUnsubscription = async () => {
    if (benefitIndexToUnsubscribe.value !== null) {
      try {
        // TODO: Add actual unsubscribe API call here
        // await axios.delete(`${import.meta.env.VITE_API_URL}/api/EmployeeBenefitList/unsubscribe`, {
        //   data: { email: userEmail.value, benefitId: benefitToUnsubscribe.value.id },
        //   withCredentials: true
        // });

        // For now, just remove locally
        activeBenefits.value.splice(benefitIndexToUnsubscribe.value, 1);

        // Optionally refresh from server to ensure sync
        // await refreshBenefits();

      } catch (error) {
        console.error('Error unsubscribing:', error);
        errorMessage.value = 'Failed to unsubscribe. Please try again.';
      }

      closeUnsubscribeModal();
    }
  };

  const closeUnsubscribeModal = () => {
    showUnsubscribeModal.value = false;
    benefitToUnsubscribe.value = null;
    benefitIndexToUnsubscribe.value = null;
  };

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

    isProcessingSubscription.value = true;

    try {
      // TODO: Make actual API call to subscribe
      const subscriptionData = {
        email: userEmail.value,
        benefitId: selectedBenefit.value.id
      };

      // For specific benefits, include the calculated value
      if (selectedBenefit.value.method.type === 'specific' && calculatedBenefitValue.value !== null) {
        subscriptionData.calculatedValue = calculatedBenefitValue.value;
      }

      if (selectedBenefit.value.method.type === 'specific') {
        // API SUBSCRIPTION CALL: IMPLEMENT
      } else {
        console.log(userEmail.value)
        console.log(selectedBenefit.value.benefitId)
        console.log(selectedBenefit.value)

        await axios.post(`${import.meta.env.VITE_API_URL}/api/BenefitSubscription/subscribe`, {
          email: userEmail.value,
          benefitId: selectedBenefit.value.benefitId 
        }, { withCredentials: true });
      }


      // await axios.post(`${import.meta.env.VITE_API_URL}/api/EmployeeBenefitList/subscribe`, 
      //   subscriptionData, 
      //   { withCredentials: true }
      // );

      // Simulate API call delay
      //await new Promise(resolve => setTimeout(resolve, 1500));

      subscribedBenefitName.value = selectedBenefit.value.name;

      // Refresh benefits from server after successful subscription
      await refreshBenefits();

      // Close confirmation modal and show success
      showConfirmationModal.value = false;
      showSuccessModal.value = true;

    } catch (error) {
      console.error('Error subscribing to benefit:', error);
      errorMessage.value = 'Failed to subscribe to benefit. Please try again.';
    } finally {
      isProcessingSubscription.value = false;
    }
  };

  const closeSuccessModal = () => {
    showSuccessModal.value = false;
    closeSubscribeModal();
  };

  const closeSubscribeModal = () => {
    // Reset all modal states
    selectedBenefitIndex.value = null;
    showSubscribeModal.value = false;
    showConfirmationModal.value = false;
    showSuccessModal.value = false;
    subscribedBenefitName.value = '';
    isProcessingSubscription.value = false;
    calculatedBenefitValue.value = null;
    calculationError.value = '';
  };

  // Load available benefits when subscribe modal is opened
  watch(showSubscribeModal, (isOpen) => {
    if (isOpen && availableBenefits.value.length === 0) {
      loadAvailableBenefits();
    }
  });

  // Load benefits when component mounts
  onMounted(() => {
    loadActiveBenefits();
    loadAvailableBenefits();
  });
</script>

<style scoped lang="scss">
  .btn-primary {
    background-color: #003c63;
    border-color: #003c63;
    font-weight: bold;
  }

  .btn-outline-danger {
    border-color: #dc3545;
    color: #dc3545;
  }

  .btn-outline-danger:hover {
    background-color: #dc3545;
    color: white;
  }

  .btn-outline-danger:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .empresa-lista {
    max-width: 900px;
    margin: auto;
    padding: 20px;

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
