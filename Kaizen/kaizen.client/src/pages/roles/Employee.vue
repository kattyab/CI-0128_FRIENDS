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
      <!-- Enployee Name -->
      <h1 class="text-center mb-4 mt-4 pt-4" style="color: #003c63;">
        Bienvenido/a
        {{ employee?.firstName }} {{ employee?.lastName }}
      </h1>

      <div class="row custom-gap">
        <div class="col-md-12">
          <div class="d-flex justify-content-between align-items-center mb-3">
            <h5 class="fw-bold">Datos Personales</h5>
          </div>
          <div class="p-3 border shadow-sm custom-box">
            <div class="mb-3">
              <strong>Rol</strong>
              <div class="highlight-box">{{ employee.role }}</div>
            </div>
            <div>
              <div class="mb-3">
                <strong>Tipo de Contrato</strong>
                <div class="highlight-box">{{ employee.contractType }}</div>
              </div>
              <div class="mb-3">
                <strong>Puesto de Trabajo</strong>
                <div class="highlight-box">{{ employee.jobPosition }}</div>
              </div>
              <div class="mb-3">
                <strong>Teléfonos</strong>
                <div class="highlight-box">{{ employee.phoneNumbers }}</div>
              </div>
              <div class="mb-3">
                <strong>Correo</strong>
                <div class="highlight-box">{{ userData.email}}</div>
              </div>
              <div class="mb-3">
                <strong>Provincia</strong>
                <div class="highlight-box">{{ employee.province }}</div>
              </div>
              <div class="mb-3">
                <strong>Cantón</strong>
                <div class="highlight-box">{{ employee.canton }}</div>
              </div>
              <div class="mb-3">
                <strong>Otras señas</strong>
                <div class="highlight-box">{{ employee.otherSigns ? employee.otherSigns : 'N/A' }}</div>
              </div>
              <div class="mb-3">
                <strong>Salario bruto</strong>
                <div class="highlight-box">{{ employee.salary }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue';
  import axios from 'axios';

  const isLoading = ref(true);
  const isEditingContractual = ref(false);
  const notFound = ref(false);
  const employee = ref(null);
  const userData = ref(null);

  async function fetchEmployeeData() {
    isLoading.value = true;
    notFound.value = false;

    try {
      const email = userData.value.email;
      const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/CommonHomepage/${email}`);
      const data = response.data;

      if (response) {
        employee.value = {
          firstName: data.firstName,
          lastName: data.lastName,
          role: data.role,
          jobPosition: data.jobPosition,
          contractType: data.contractType,
          phoneNumbers: data.phoneNumbers.join(', '),
          email: data.email,
          province: data.province,
          canton: data.canton,
          otherSigns: data.otherSigns,
          salary: `${data.grossSalary.toLocaleString()}₡`
        };
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

  function toggleEditContractual() {
    isEditingContractual.value = !isEditingContractual.value;
  }

  onMounted(async () => {
    try {
      const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/login/authenticate`, { withCredentials: true });
      userData.value = response.data;

      await fetchEmployeeData();
    } catch (error) {
      console.error('Authentication failed', error);
      isLoading.value = false;
    }
  });
</script>

<style scoped>
  .container {
    color: #003c63;
    max-width: 1200px;
    margin: 0 auto;
    overflow-x: hidden;
  }

  .row.custom-gap {
    display: flex;
    flex-wrap: wrap;
    gap: 2rem;
  }

  .custom-gap .col-md-12 {
    flex: 1 1 48%;
    padding-left: 1.5rem;
    padding-right: 1.5rem;
    min-width: 0;
  }

  @media (max-width: 768px) {
    .custom-gap .col-md-12 {
      flex: 1 1 100%;
      padding-left: 1rem;
      padding-right: 1rem;
    }
  }

  .highlight-box {
    background-color: #f2f2f2;
    border: 1px solid #f2f2f2;
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
</style>
