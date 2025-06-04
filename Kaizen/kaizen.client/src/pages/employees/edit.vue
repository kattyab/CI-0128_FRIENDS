<template>
  <div class="container py-4">
    <div v-if="isLoading" class="text-center mt-5">
      <div class="spinner-border text-primary mb-3" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
      <p class="fw-bold">Cargando datos de empleado...</p>
    </div>

    <div v-else-if="notFound" class="alert alert-danger text-center">
      Empleado no encontrado.
    </div>

    <div v-else>
      <h1 class="text-center mb-4 mt-4 pt-4" style="color: #003c63;">
        Editar Empleado
      </h1>
      <div class="row custom-gap">
        <div class="col-1"></div>
        <div class="col-10">
          <div class="p-3 border shadow-sm custom-box">
            <div class="mb-3">
              <strong>Nombre</strong>
              <div>
                <input v-model="editableEmployee.firstName"
                       type="text"
                       class="form-control "
                       :class="{ 'is-invalid': validationErrors.firstName }"
                       placeholder="Nombre" />
                <div v-if="validationErrors.firstName" class="invalid-feedback">
                  {{ validationErrors.firstName }}
                </div>
              </div>
            </div>
            <div class="mb-3">
              <strong>Apellidos</strong>
              <div>
                <input v-model="editableEmployee.lastName"
                       type="text"
                       class="form-control "
                       :class="{ 'is-invalid': validationErrors.lastName }"
                       placeholder="Apellidos" />
                <div v-if="validationErrors.lastName" class="invalid-feedback">
                  {{ validationErrors.lastName }}
                </div>
              </div>
            </div>
            <div class="mb-3">
              <strong>Salario</strong>
              <div>
                <input v-model="editableEmployee.grossSalary"
                       type="number"
                       step="0.01"
                       class="form-control "
                       :class="{ 'is-invalid': validationErrors.grossSalary }"
                       placeholder="Salario" />
                <div v-if="validationErrors.grossSalary" class="invalid-feedback">
                  {{ validationErrors.grossSalary }}
                </div>
              </div>
            </div>
            <div class="mb-3">
              <strong>Tipo de Contrato</strong>
              <select v-model="editableEmployee.contractType"
                      class="form-select ">
                <option value="Tiempo Completo">Tiempo Completo</option>
                <option value="Medio Tiempo">Medio Tiempo</option>
                <option value="Por Horas">Por Horas</option>
                <option value="Servicios Profesionales">Servicios Profesionales</option>
              </select>
            </div>
            <div class="mb-3">
              <strong>Puesto</strong>
              <div>
                <input v-model="editableEmployee.jobPosition"
                       type="text"
                       class="form-control "
                       :class="{ 'is-invalid': validationErrors.jobPosition }"
                       placeholder="Puesto" />
                <div v-if="validationErrors.jobPosition" class="invalid-feedback">
                  {{ validationErrors.jobPosition }}
                </div>
              </div>
            </div>
            <div class="mb-3">
              <strong>¿Registra horas?</strong>
              <select v-model="editableEmployee.registersHours"
                      class="form-select ">
                <option :value="true">Sí</option>
                <option :value="false">No</option>
              </select>
            </div>
            <div class="mb-3">
              <strong>Rol</strong>
              <select v-model="editableEmployee.role"
                      class="form-select ">
                <option value="Empleado">Empleado</option>
                <option value="Supervisor">Supervisor</option>
                <option value="Administrador">Administrador</option>
              </select>
            </div>
            <div class="mb-3">
              <strong>Ciclo de pago</strong>
              <select v-model="editableEmployee.payCycle"
                      class="form-select ">
                <option value="Semanal">Semanal</option>
                <option value="Quincenal">Quincenal</option>
                <option value="Mensual">Mensual</option>
              </select>
            </div>
            <div class="mb-3">
              <strong>Cuenta Bancaria</strong>
              <div>
                <input v-model="editableEmployee.bankAccount"
                       type="text"
                       class="form-control "
                       :class="{ 'is-invalid': validationErrors.bankAccount }"
                       placeholder="Cuenta Bancaria (CR + 20 números)" />
                <div v-if="validationErrors.bankAccount" class="invalid-feedback">
                  {{ validationErrors.bankAccount }}
                </div>
              </div>
            </div>

            <div class="mb-3">
              <strong>Fecha de Contratación</strong>
              <input v-model="editableEmployee.startDate"
                     type="date"
                     class="form-control"
                     readonly/>
            </div>
            <div class="mb-3">
              <strong>Teléfonos</strong>
              <div>
                <input v-model="editableEmployee.phoneNumbersStr"
                       type="text"
                       class="form-control "
                       :class="{ 'is-invalid': validationErrors.phoneNumbers }"
                       placeholder="Teléfonos (formato: xxxx-xxxx, separados por comas)" />
                <div v-if="validationErrors.phoneNumbers" class="invalid-feedback">
                  {{ validationErrors.phoneNumbers }}
                </div>
              </div>
            </div>
            <div class="mb-3">
              <strong>Cédula</strong>
              <div>
                <input v-model="editableEmployee.id"
                       type="text"
                       class="form-control "
                       :class="{ 'is-invalid': validationErrors.id }"
                       placeholder="Cédula (formato: x-xxxx-xxxx o xxxxxxxxxxxx)" />
                <div v-if="validationErrors.id" class="invalid-feedback">
                  {{ validationErrors.id }}
                </div>
              </div>
            </div>
            <div class="mb-3">
              <strong>Sexo</strong>
              <select v-model="editableEmployee.sex"
                      class="form-select ">
                <option value="Hombre">Hombre</option>
                <option value="Mujer">Mujer</option>
              </select>
            </div>
            <div class="mb-3">
              <strong>Correo</strong>
              <div>
                <input v-model="editableEmployee.email"
                       type="email"
                       class="form-control "
                       :class="{ 'is-invalid': validationErrors.email }"
                       placeholder="Correo electrónico" />
                <div v-if="validationErrors.email" class="invalid-feedback">
                  {{ validationErrors.email }}
                </div>
              </div>
            </div>
            <div class="mb-3">
              <strong>Provincia</strong>
              <div>
                <input v-model="editableEmployee.province"
                       type="text"
                       class="form-control "
                       :class="{ 'is-invalid': validationErrors.province }"
                       placeholder="Provincia" />
                <div v-if="validationErrors.province" class="invalid-feedback">
                  {{ validationErrors.province }}
                </div>
              </div>
            </div>
            <div class="mb-3">
              <strong>Cantón</strong>
              <div>
                <input v-model="editableEmployee.canton"
                       type="text"
                       class="form-control "
                       :class="{ 'is-invalid': validationErrors.canton }"
                       placeholder="Cantón" />
                <div v-if="validationErrors.canton" class="invalid-feedback">
                  {{ validationErrors.canton }}
                </div>
              </div>
            </div>
            <div class="mb-3">
              <strong>Otras señas</strong>
              <textarea v-model="editableEmployee.otherSigns"
                        class="form-control "
                        rows="3"
                        placeholder="Otras señas"></textarea>
            </div>
          </div>

          <div class="d-flex justify-content-center mt-4 mb-4 button-container">
            <button @click="saveChanges"
                    class="btn btn-success me-3"
                    :disabled="isSaving">
              {{ isSaving ? 'Guardando...' : 'Guardar' }}
            </button>
            <button @click="cancelEdit"
                    class="btn btn-secondary">
              Cancelar
            </button>
          </div>

          <div class="row">
            <div class="col-2"></div>
            <div class="col-8">
              <div v-if="showErrorMessage" class="alert alert-danger mt-3 mb-3">
                {{ errorMessage }}
              </div>
              <div v-if="showSuccessMessage" class="alert alert-success mt-3 mb-3">
                {{ successMessage }}
              </div>
            </div>
            <div class="col-2"></div>
          </div>

        </div>
        <div class="col-1"></div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue';
  import axios from 'axios';
  import { useRoute } from 'vue-router';

  const route = useRoute();
  const isLoading = ref(true);
  const isSaving = ref(false);
  const notFound = ref(false);
  const employee = ref(null);
  const editableEmployee = ref({});
  const originalEmployee = ref({});
  const validationErrors = ref({});

  // Notification states
  const showErrorMessage = ref(false);
  const showSuccessMessage = ref(false);
  const errorMessage = ref('');
  const successMessage = ref('');

  // Function to show success notification
  function showSuccess(message) {
    successMessage.value = message;
    showSuccessMessage.value = true;
    showErrorMessage.value = false;

    // Auto-hide after 5 seconds
    setTimeout(() => {
      showSuccessMessage.value = false;
    }, 5000);
  }

  // Function to show error notification
  function showError(message) {
    errorMessage.value = message;
    showErrorMessage.value = true;
    showSuccessMessage.value = false;

    // Auto-hide after 7 seconds
    setTimeout(() => {
      showErrorMessage.value = false;
    }, 7000);
  }

  // Function to hide all notifications
  function hideNotifications() {
    showErrorMessage.value = false;
    showSuccessMessage.value = false;
  }

  async function fetchEmployeeData() {
    isLoading.value = true;
    notFound.value = false;
    const empID = route.params.id;

    try {
      const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/EmployeeDetails/by-id/${empID}`);
      const data = response.data;
      if (data) {
        const allBenefits = [];
        if (Array.isArray(data.chosenBenefitNames) && data.chosenBenefitNames.length > 0) {
          allBenefits.push(...data.chosenBenefitNames);
        }
        if (Array.isArray(data.chosenApiNames) && data.chosenApiNames.length > 0) {
          allBenefits.push(...data.chosenApiNames);
        }

        employee.value = {
          id: data.id,
          firstName: data.firstName,
          lastName: data.lastName,
          sex: data.sex,
          salary: `₡${data.grossSalary.toLocaleString()}`,
          contractType: data.contractType,
          status: data.status ? 'Activo' : 'Inactivo',
          payCycle: data.payCycle,
          jobPosition: data.jobPosition,
          registersHours: data.registersHours ? 'Sí' : 'No',
          role: data.role,
          benefits: allBenefits.length > 0 ? allBenefits.join(', ') : 'N/A',
          startDate: new Date(data.startDate).toLocaleDateString('es-CR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
          }),
          phoneNumbers: Array.isArray(data.phoneNumbers) ? data.phoneNumbers.join(', ') : 'N/A',
          email: data.email,
          province: data.province,
          district: data.district,
          canton: data.canton,
          otherSigns: data?.otherSigns,
          bankAccount: data.bankAccount,
        };

        editableEmployee.value = {
          id: data.id,
          firstName: data.firstName,
          lastName: data.lastName,
          sex: data.sex,
          birthDate: data.birthDate,
          workHours: data.workHours,
          startDate: data.startDate,
          status: data.status,
          grossSalary: data.grossSalary,
          contractType: data.contractType,
          status: data.status,
          payCycle: data.payCycle,
          jobPosition: data.jobPosition,
          registersHours: data.registersHours,
          role: data.role,
          startDate: data.startDate ? data.startDate.split('T')[0] : '',
          phoneNumbersStr: Array.isArray(data.phoneNumbers) ? data.phoneNumbers.join(', ') : '',
          email: data.email,
          province: data.province,
          district: data.district,
          canton: data.canton,
          otherSigns: data.otherSigns || '',
          bankAccount: data.bankAccount,
          chosenBenefitNames: data.chosenBenefitNames || [],
          chosenApiNames: data.chosenApiNames || [],
        };

        originalEmployee.value = { ...editableEmployee.value };
      } else {
        console.warn('No data received from API.');
        notFound.value = true;
      }
    } catch (error) {
      console.error('Error fetching employee data:', error);
      notFound.value = true;
    } finally {
      isLoading.value = false;
    }
  }

  function validateForm() {
    console.log(route.params.id);
    const empId = route.params.id;

    validationErrors.value = {};
    let isValid = true;

    if (!editableEmployee.value.firstName || editableEmployee.value.firstName.trim() === '') {
      validationErrors.value.firstName = 'El nombre es requerido';
      isValid = false;
    }

    if (!editableEmployee.value.lastName || editableEmployee.value.lastName.trim() === '') {
      validationErrors.value.lastName = 'Los apellidos son requeridos';
      isValid = false;
    }

    const salaryPattern = /^\d+(\.\d{1,2})?$/;
    if (!editableEmployee.value.grossSalary || editableEmployee.value.grossSalary <= 0) {
      validationErrors.value.grossSalary = 'El salario debe ser mayor a 0';
      isValid = false;
    } else if (!salaryPattern.test(editableEmployee.value.grossSalary.toString())) {
      validationErrors.value.grossSalary = 'El salario debe tener máximo 2 decimales';
      isValid = false;
    }

    if (!editableEmployee.value.jobPosition || editableEmployee.value.jobPosition.trim() === '') {
      validationErrors.value.jobPosition = 'El puesto es requerido';
      isValid = false;
    }

    const bankAccountPattern = /^CR\d{20}$/;
    if (!editableEmployee.value.bankAccount || !bankAccountPattern.test(editableEmployee.value.bankAccount)) {
      validationErrors.value.bankAccount = 'Formato requerido: CRXXXXXXXXXXXXXXXXXXXX';
      isValid = false;
    }

    const phonePattern = /^\d{4}-\d{4}$/;
    if (editableEmployee.value.phoneNumbersStr && editableEmployee.value.phoneNumbersStr.trim() !== '') {
      const phones = editableEmployee.value.phoneNumbersStr.split(',').map(phone => phone.trim());
      const invalidPhones = phones.filter(phone => !phonePattern.test(phone));
      if (invalidPhones.length > 0) {
        validationErrors.value.phoneNumbers = 'Cada teléfono debe tener formato XXXX-XXXX, separados por comas';
        isValid = false;
      }
    }

    const cedulaPattern1 = /^\d-\d{4}-\d{4}$/;
    const cedulaPattern2 = /^\d{12}$/;
    if (!editableEmployee.value.id || (!cedulaPattern1.test(editableEmployee.value.id) && !cedulaPattern2.test(editableEmployee.value.id))) {
      validationErrors.value.id = 'Formato requerido: X-XXXX-XXXX o XXXXXXXXXXXX';
      isValid = false;
    }

    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!editableEmployee.value.email || !emailPattern.test(editableEmployee.value.email)) {
      validationErrors.value.email = 'Formato de correo inválido';
      isValid = false;
    }

    if (!editableEmployee.value.province || editableEmployee.value.province.trim() === '') {
      validationErrors.value.province = 'La provincia es requerida';
      isValid = false;
    }

    if (!editableEmployee.value.canton || editableEmployee.value.canton.trim() === '') {
      validationErrors.value.canton = 'El cantón es requerido';
      isValid = false;
    }

    return isValid;
  }

  function cancelEdit() {
    validationErrors.value = {};
    hideNotifications();
    editableEmployee.value = { ...originalEmployee.value };
  }

  async function saveChanges() {
    hideNotifications();

    if (!validateForm()) {
      return;
    }

    const empID = route.params.id;
    isSaving.value = true;

    try {
      const updateData = {
        empId: empID,
        ...editableEmployee.value,

        phoneNumbers: editableEmployee.value.phoneNumbersStr.split(',').map(phone => phone.trim()),
        startDate: editableEmployee.value.startDate
      };

      const response = await axios.put(
        `${import.meta.env.VITE_API_URL}/api/EmployeeDetails/${empID}`,
        updateData
      );

      if (response.status === 200) {
        originalEmployee.value = { ...editableEmployee.value };

        showSuccess('Datos actualizados correctamente');
      }
    } catch (error) {
      console.error('Error updating employee:', error);
      showError('Error al actualizar los datos. Por favor, intente nuevamente.');
    } finally {
      isSaving.value = false;
    }
  }
  onMounted(() => {
    fetchEmployeeData();
  });
</script>

<style scoped>
  .container {
    color: #003c63;
    margin: 0 auto;
    overflow-x: hidden;
  }




  .row.custom-gap {
    display: flex;
    flex-wrap: wrap;
    gap: 4rem;
  }

  .custom-gap .col-10 {
    flex: 1 1 48%;
    padding-left: 4rem;
    padding-right: 4rem;
    min-width: 0;
  }

  @media (max-width: 768px) {
    .custom-gap .col-10 {
      flex: 1 1 100%;
      padding-left: 1rem;
      padding-right: 1rem;
    }
  }

  .col-1 {
    flex: 1 1 48%;
    min-width: 0;
  }

  @media (max-width: 768px) {
    .col-1 {
      flex: 1 1 100%;
    }
  }

  .btn {
    font-weight: bold;
  }

  .btn-success {
    background-color: #003c63;
    border-color: #003c63;
    color: white;
  }

  .highlight-box {
    background-color: #f2f2f2;
    padding: 5px;
    border-radius: 10px;
    word-break: break-word;
    text-indent: 5px;
  }

  .custom-btn-blue {
    background-color: #003c63;
    color: white;
    border: none;
  }

  .custom-box {
    border-color: #d1d5db;
    border-radius: 10px;
  }

    .custom-box h5,
    .custom-box p {
      color: #003c63;
    }

  .form-control,
  .form-select,
  .form-control:focus,
  .form-select:focus {
    border-color: #f2f2f2;
    border-radius: 10px;
    padding: 4px;
    background-color: #f2f2f2;
    text-indent: 5px;
  }

    .form-control.is-invalid,
    .form-select.is-invalid {
      border-color: #dc3545;
      background-color: #fff5f5;
    }

      .form-control.is-invalid:focus,
      .form-select.is-invalid:focus {
        border-color: #dc3545;
        box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25);
      }

  .invalid-feedback {
    display: block;
    width: 100%;
    margin-top: 0.25rem;
    font-size: 0.875em;
    color: #dc3545;
  }

  .button-container {
    padding: 1rem 0;
  }

    .button-container .btn {
      min-width: 120px;
      padding: 0.5rem 1.5rem;
    }
</style>
