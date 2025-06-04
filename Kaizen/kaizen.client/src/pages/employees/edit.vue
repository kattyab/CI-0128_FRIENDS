<template>
  <div class="container py-4">
    <!-- Spinner -->
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
      <!-- Employee Name -->
      <h1 class="text-center mb-4 mt-4 pt-4" style="color: #003c63;">
        Editar Empleado
      </h1>
      <div class="row custom-gap">
        <div class="col-1"></div>
        <div class="col-10">
          <div class="d-flex justify-content-between align-items-center mb-3">
            <h5 class="fw-bold">Datos Contractuales</h5>
            <div>
              <button v-if="!isEditing"
                      @click="toggleEdit"
                      class="btn custom-btn-blue me-2">
                Editar
              </button>
              <template v-else>
                <button @click="saveChanges"
                        class="btn btn-success me-2"
                        :disabled="isSaving">
                  {{ isSaving ? 'Guardando...' : 'Guardar' }}
                </button>
                <button @click="cancelEdit"
                        class="btn btn-secondary">
                  Cancelar
                </button>
              </template>
            </div>
          </div>
          <div class="p-3 border shadow-sm custom-box">
            <div class="mb-3">
              <strong>Nombre</strong>
              <div v-if="!isEditing" class="highlight-box">{{ employee?.firstName }}</div>
              <input v-else
                     v-model="editableEmployee.firstName"
                     type="text"
                     class="form-control"
                     placeholder="Nombre" />
            </div>
            <div class="mb-3">
              <strong>Apellidos</strong>
              <div v-if="!isEditing" class="highlight-box">{{ employee?.lastName }}</div>
              <input v-else
                     v-model="editableEmployee.lastName"
                     type="text"
                     class="form-control "
                     placeholder="Apellidos" />
            </div>
            <div class="mb-3">
              <strong>Salario</strong>
              <div v-if="!isEditing" class="highlight-box">{{ employee.salary }}</div>
              <input v-else
                     v-model="editableEmployee.grossSalary"
                     type="number"
                     class="form-control "
                     placeholder="Salario" />
            </div>
            <div class="mb-3">
              <strong>Tipo de Contrato</strong>
              <div v-if="!isEditing" class="highlight-box">{{ employee.contractType }}</div>
              <select v-else
                      v-model="editableEmployee.contractType"
                      class="form-select ">
                <option value="Tiempo Completo">Tiempo Completo</option>
                <option value="Medio Tiempo">Medio Tiempo</option>
                <option value="Por Horas">Por Horas</option>
                <option value="Temporal">Temporal</option>
              </select>
            </div>
            <div class="mb-3">
              <strong>Puesto</strong>
              <div v-if="!isEditing" class="highlight-box">{{ employee.jobPosition || 'N/A' }}</div>
              <input v-else
                     v-model="editableEmployee.jobPosition"
                     type="text"
                     class="form-control "
                     placeholder="Puesto" />
            </div>
            <div class="mb-3">
              <strong>¿Registra horas?</strong>
              <div v-if="!isEditing" class="highlight-box">{{ employee.registersHours }}</div>
              <select v-else
                      v-model="editableEmployee.registersHours"
                      class="form-select ">
                <option :value="true">Sí</option>
                <option :value="false">No</option>
              </select>
            </div>
            <div class="mb-3">
              <strong>Rol</strong>
              <div v-if="!isEditing" class="highlight-box">{{ employee.role }}</div>
              <select v-else
                      v-model="editableEmployee.role"
                      class="form-select ">
                <option value="Empleado">Empleado</option>
                <option value="Supervisor">Supervisor</option>
                <option value="Gerente">Gerente</option>
                <option value="Administrador">Administrador</option>
              </select>
            </div>
            <div class="mb-3">
              <strong>Ciclo de pago</strong>
              <div v-if="!isEditing" class="highlight-box">{{ employee.payCycle }}</div>
              <select v-else
                      v-model="editableEmployee.payCycle"
                      class="form-select ">
                <option value="Semanal">Semanal</option>
                <option value="Quincenal">Quincenal</option>
                <option value="Mensual">Mensual</option>
              </select>
            </div>
            <div class="mb-3">
              <strong>Cuenta Bancaria</strong>
              <div v-if="!isEditing" class="highlight-box">{{ employee.bankAccount }}</div>
              <input v-else
                     v-model="editableEmployee.bankAccount"
                     type="text"
                     class="form-control "
                     placeholder="Cuenta Bancaria" />
            </div>

            <div class="mb-3">
              <strong>Fecha de Contratación</strong>
              <div class="highlight-box">{{ employee.startDate }}</div>
            </div>
            <div class="mb-3">
              <strong>Teléfonos</strong>
              <div v-if="!isEditing" class="highlight-box">{{ employee.phoneNumbers }}</div>
              <input v-else
                     v-model="editableEmployee.phoneNumbersStr"
                     type="text"
                     class="form-control "
                     placeholder="Teléfonos (separados por comas)" />
            </div>
            <div class="mb-3">
              <strong>Cédula</strong>
              <div v-if="!isEditing" class="highlight-box">{{ employee.id }}</div>
              <input v-else
                     v-model="editableEmployee.id"
                     type="text"
                     class="form-control "
                     placeholder="Cédula" />
            </div>
            <div class="mb-3">
              <strong>Sexo</strong>
              <div v-if="!isEditing" class="highlight-box">{{ employee.sex }}</div>
              <select v-else
                      v-model="editableEmployee.sex"
                      class="form-select ">
                <option value="Hombre">Hombre</option>
                <option value="Mujer">Mujer</option>
              </select>
            </div>
            <div class="mb-3">
              <strong>Correo</strong>
              <div v-if="!isEditing" class="highlight-box">{{ employee.email }}</div>
              <input v-else
                     v-model="editableEmployee.email"
                     type="email"
                     class="form-control "
                     placeholder="Correo electrónico" />
            </div>
            <div class="mb-3">
              <strong>Provincia</strong>
              <div v-if="!isEditing" class="highlight-box">{{ employee.province }}</div>
              <input v-else
                     v-model="editableEmployee.province"
                     type="text"
                     class="form-control "
                     placeholder="Provincia" />
            </div>
            <div class="mb-3">
              <strong>Cantón</strong>
              <div v-if="!isEditing" class="highlight-box">{{ employee.canton }}</div>
              <input v-else
                     v-model="editableEmployee.canton"
                     type="text"
                     class="form-control "
                     placeholder="Cantón" />
            </div>
            <div class="mb-3">
              <strong>Otras señas</strong>
              <div v-if="!isEditing" class="highlight-box">{{ employee.otherSigns || 'N/A' }}</div>
              <textarea v-else
                        v-model="editableEmployee.otherSigns"
                        class="form-control "
                        rows="3"
                        placeholder="Otras señas"></textarea>
            </div>
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
  const isEditing = ref(false);
  const isSaving = ref(false);
  const notFound = ref(false);
  const employee = ref(null);
  const editableEmployee = ref({});
  const originalEmployee = ref({});

  async function fetchEmployeeData() {
    isLoading.value = true;
    notFound.value = false;

    //const empID = route.params.id;
    const empID = 'C09401AA-9E9D-4ACF-AD16-C094DB0D4512';
    console.log("Employee ID:", empID);

    try {
      const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/EmployeeDetails/by-id/${empID}`);
      const data = response.data;
      console.log("hola", response.data)
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

        // Initialize editable data
        editableEmployee.value = {
          id: data.id,
          firstName: data.firstName,
          lastName: data.lastName,
          sex: data.sex,
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
        };

        // Store original data for cancel functionality
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

  function toggleEdit() {
    isEditing.value = !isEditing.value;
  }

  function cancelEdit() {
    isEditing.value = false;
    // Restore original values
    editableEmployee.value = { ...originalEmployee.value };
  }

  async function saveChanges() {
    isSaving.value = true;

    try {
      // Prepare data for API
      const updateData = {
        ...editableEmployee.value,
        phoneNumbers: editableEmployee.value.phoneNumbersStr.split(',').map(phone => phone.trim()),
        startDate: editableEmployee.value.startDate
      };

      // Make API call to update employee
      const response = await axios.put(
        `${import.meta.env.VITE_API_URL}/api/EmployeeDetails/${editableEmployee.value.id}`,
        updateData
      );

      if (response.status === 200) {
        // Update display data
        employee.value = {
          ...employee.value,
          firstName: editableEmployee.value.firstName,
          lastName: editableEmployee.value.lastName,
          sex: editableEmployee.value.sex,
          salary: `₡${editableEmployee.value.grossSalary.toLocaleString()}`,
          contractType: editableEmployee.value.contractType,
          payCycle: editableEmployee.value.payCycle,
          jobPosition: editableEmployee.value.jobPosition,
          registersHours: editableEmployee.value.registersHours ? 'Sí' : 'No',
          role: editableEmployee.value.role,
          startDate: new Date(editableEmployee.value.startDate).toLocaleDateString('es-CR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
          }),
          phoneNumbers: editableEmployee.value.phoneNumbersStr,
          email: editableEmployee.value.email,
          province: editableEmployee.value.province,
          canton: editableEmployee.value.canton,
          otherSigns: editableEmployee.value.otherSigns || 'N/A',
          bankAccount: editableEmployee.value.bankAccount,
        };

        // Update original data
        originalEmployee.value = { ...editableEmployee.value };

        isEditing.value = false;

        // Show success message (you can add a toast notification here)
        alert('Datos actualizados correctamente');
      }
    } catch (error) {
      console.error('Error updating employee:', error);
      alert('Error al actualizar los datos. Por favor, intente nuevamente.');
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
    /*    max-width: 1200px;*/
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
    padding-left: 4rem;
    padding-right: 4rem;
    min-width: 0;
  }

  @media (max-width: 768px) {
    .col-1 {
      flex: 1 1 100%;
      padding-left: 1rem;
      padding-right: 1rem;
    }
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
    background-color: #f2f2f2;
    padding: 5px;
    border-radius: 10px;
    word-break: break-word;
    text-indent: 5px;
    border-block: thin;
  }

    .form-control:focus,
    .form-select:focus {
      padding: 5px;
      border-radius: 10px;
      word-break: break-word;
      text-indent: 5px;
      border-color: #003c63;
      /*      box-shadow: 0 0 0 0.2rem rgba(0, 60, 99, 0.25);*/
      background-color: #ffffff;
    }
</style>
