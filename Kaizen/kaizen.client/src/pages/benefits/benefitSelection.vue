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

      <!-- Add horizontal margins to the table container -->
      <div class="mx-5">
        <table class="table table-hover">
          <thead>
            <tr>
              <th scope="col">Nombre del beneficio</th>
              <th scope="col">Método de descargo</th>
              <th scope="col">Meses mínimos para suscribir</th>
              <th scope="col">Suscripción</th>
            </tr>
          </thead>
          <tbody class="table-group-divider">
            <tr v-for="(benefit, index) in activeBenefits" :key="index" >
              <td>{{ benefit.name }}</td>
              <td>
                <span v-if="benefit.method.type === 'percentage'">{{ benefit.method.value }}%</span>
                <span v-else-if="benefit.method.type === 'fixed'">${{ benefit.method.value }}</span>
                <span v-else>{{ benefit.method.value }}</span>
              </td>
              <td>{{ benefit.minimumMonths }}</td>
              <td>
                <button 
                  class="btn btn-outline-danger btn-sm" 
                  @click="unsubscribeBenefit(index)"
                  :disabled="benefit.state === 'Expired'">
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

              <table class="table table-hover">
                <thead>
                  <tr>
                    <th scope="col">Select</th>
                    <th scope="col">Benefit Name</th>
                    <th scope="col">Method</th>
                    <th scope="col">Details</th>
                    <th scope="col">Min. Months</th>
                    <th scope="col">State</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(benefit, index) in availableBenefits" :key="index" :class="{
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
                    <td>{{ benefit.details }}</td>
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
                    
                    <!-- Display user parameters used for calculation -->
                    <div class="mt-3">
                      <h6>Calculation Parameters:</h6>
                      <div class="row">
                        <div class="col-md-6">
                          <small class="text-muted">Gender:</small> {{ userProfile.gender }}<br>
                          <small class="text-muted">Salary Rate:</small> ${{ userProfile.salaryRate.toLocaleString() }}/year<br>
                          <small class="text-muted">Years of Service:</small> {{ userProfile.yearsOfService }} years
                        </div>
                        <div class="col-md-6">
                          <small class="text-muted">Department:</small> {{ userProfile.department }}<br>
                          <small class="text-muted">Employment Type:</small> {{ userProfile.employmentType }}<br>
                          <small class="text-muted">Age:</small> {{ userProfile.age }} years
                        </div>
                      </div>
                    </div>
                    
                    <!-- Display calculated value -->
                    <div class="mt-3 p-3 bg-light rounded">
                      <h6 class="text-success">Calculated Benefit Value:</h6>
                      <p class="h5 text-success mb-0">${{ calculatedBenefitValue.toLocaleString() }}</p>
                      <small class="text-muted">
                        <!-- TODO: Replace with actual API call logic -->
                        * This value is calculated based on your profile information
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
                      :disabled="isProcessingSubscription">
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
import { ref, computed } from 'vue';

// Active benefits that the user already has
const activeBenefits = ref([
  {
    name: "Health Insurance Premium Discount",
    method: { type: "percentage", value: 15 },
    minimumMonths: 12
  },
  {
    name: "Wellness Program Rebate",
    method: { type: "fixed", value: 250 },
    minimumMonths: 6
  },
  {
    name: "Dental Coverage Supplement",
    method: { type: "fixed", value: 500 },
    minimumMonths: 24
  },
  {
    name: "Vision Care Package",
    method: { type: "specific", value: "Premium Plan Coverage" },
    minimumMonths: 12
  }
]);

// Available benefits for subscription in the modal
const availableBenefits = ref([
  {
    name: "Retirement Contribution Match",
    method: { type: "percentage", value: 5 },
    minimumMonths: 12,
    state: "Available",
    details: "Employer matches up to 5% of your salary contributions to retirement plan"
  },
  {
    name: "Home Office Stipend",
    method: { type: "fixed", value: 500 },
    minimumMonths: 6,
    state: "Available",
    details: "Monthly stipend for home office equipment and utilities"
  },
  {
    name: "Professional Development",
    method: { type: "fixed", value: 1000 },
    minimumMonths: 12,
    state: "Available",
    details: "Annual budget for courses, certifications, and training"
  },
  {
    name: "Life Insurance",
    method: { type: "specific", value: "Calculated Coverage" },
    minimumMonths: 24,
    state: "Available",
    details: "Life insurance coverage calculated based on your profile and salary"
  },
  {
    name: "Executive Health Program",
    method: { type: "specific", value: "Premium Health Package" },
    minimumMonths: 36,
    state: "Not Available",
    details: "Comprehensive health screening and premium healthcare services"
  },
  {
    name: "Sabbatical Program",
    method: { type: "specific", value: "Paid Leave Benefit" },
    minimumMonths: 60,
    state: "Not Available",
    details: "Extended paid leave program for long-term employees"
  }
]);

// User profile data (this would typically come from your user management system)
const userProfile = ref({
  gender: "Female",
  salaryRate: 85000,
  yearsOfService: 7,
  department: "Engineering",
  employmentType: "Full-time",
  age: 32
});

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

const selectedBenefit = computed(() => {
  return selectedBenefitIndex.value !== null ? availableBenefits.value[selectedBenefitIndex.value] : null;
});

const hasBenefitSelected = computed(() => {
  return selectedBenefitIndex.value !== null && selectedBenefit.value?.state === 'Available';
});

// Calculate benefit value for specific/calculated benefits
const calculatedBenefitValue = computed(() => {
  if (!selectedBenefit.value || selectedBenefit.value.method.type !== 'specific') {
    return 0;
  }
  
  // TODO: Replace this mock calculation with actual API call logic
  // This is where you would make an API call to your backend service
  // The API would receive user profile data and benefit type, then return calculated value
  
  // Mock calculation logic based on benefit type and user profile
  const benefit = selectedBenefit.value;
  let calculatedValue = 0;
  
  if (benefit.name === "Life Insurance") {
    // Example: Life insurance = 2x salary + age factor + years of service bonus
    calculatedValue = (userProfile.value.salaryRate * 2) + 
                     (userProfile.value.age * 100) + 
                     (userProfile.value.yearsOfService * 500);
  } else if (benefit.name === "Executive Health Program") {
    // Example: Health program value based on salary and department
    const baseCost = 5000;
    const salaryFactor = userProfile.value.salaryRate * 0.05;
    const departmentMultiplier = userProfile.value.department === "Engineering" ? 1.2 : 1.0;
    calculatedValue = (baseCost + salaryFactor) * departmentMultiplier;
  } else if (benefit.name === "Sabbatical Program") {
    // Example: Sabbatical value = 3 months of current salary
    calculatedValue = (userProfile.value.salaryRate / 12) * 3;
  } else {
    // Default calculation for other specific benefits
    calculatedValue = userProfile.value.salaryRate * 0.1; // 10% of salary as default
  }
  
  return Math.round(calculatedValue);
});

// API call function placeholder
const calculateBenefitValue = async (benefitData, userProfileData) => {
  // TODO: Implement actual API call here
  // Example API call structure:
  /*
  try {
    const response = await fetch('/api/benefits/calculate', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        benefit: benefitData,
        userProfile: userProfileData
      })
    });
    
    if (!response.ok) {
      throw new Error('Failed to calculate benefit value');
    }
    
    const result = await response.json();
    return result.calculatedValue;
  } catch (error) {
    console.error('Error calculating benefit value:', error);
    throw error;
  }
  */
  
  // For now, return the computed value
  return calculatedBenefitValue.value;
};

const unsubscribeBenefit = (index) => {
  benefitToUnsubscribe.value = activeBenefits.value[index];
  benefitIndexToUnsubscribe.value = index;
  showUnsubscribeModal.value = true;
};

const confirmUnsubscription = () => {
  if (benefitIndexToUnsubscribe.value !== null) {
    activeBenefits.value.splice(benefitIndexToUnsubscribe.value, 1);
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
    // TODO: Here you would make the actual API call to subscribe the user
    // For specific benefits, you might need to calculate the final value via API
    if (selectedBenefit.value.method.type === 'specific') {
      await calculateBenefitValue(selectedBenefit.value, userProfile.value);
    }
    
    // Simulate API call delay
    await new Promise(resolve => setTimeout(resolve, 1500));
    
    // Add the selected benefit to the active benefits list
    activeBenefits.value.push({
      name: selectedBenefit.value.name,
      method: selectedBenefit.value.method,
      minimumMonths: selectedBenefit.value.minimumMonths
    });
    
    subscribedBenefitName.value = selectedBenefit.value.name;
    
    // Close confirmation modal and show success
    showConfirmationModal.value = false;
    showSuccessModal.value = true;
    
  } catch (error) {
    console.error('Error subscribing to benefit:', error);
    // Handle error (show error message, etc.)
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
};
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
