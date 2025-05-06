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
        {{ employee?.firstName }} {{ employee?.lastName }}
      </h1>

      <!-- Two Columns with Custom Gap -->
      <div class="row custom-gap">
        <!-- Left Column: Datos Contractuales -->
        <div class="col-md-6">
          <div class="d-flex justify-content-between align-items-center mb-3">
            <h5 class="fw-bold">Datos Contractuales</h5>
            <button class="btn custom-btn-blue" @click="toggleEditContractual">
              Editar
            </button>
          </div>
          <div class="p-3 border rounded shadow-sm custom-box">
            <p>
              <strong>Salario:</strong>
              <div class="highlight-box">{{ employee.salary }}</div>
            </p>
            <p>
              <strong>Tipo de Contrato:</strong>
              <div class="highlight-box">{{ employee.contractType }}</div>
            </p>
            <p>
              <strong>Estado:</strong>
              <div class="highlight-box">{{ employee.status }}</div>
            </p>
            <p>
              <strong>Periodicidad:</strong>
              <div class="highlight-box">{{ employee.payCycle }}</div>
            </p>
            <p>
              <strong>Puesto:</strong>
              <div class="highlight-box">{{ employee.jobPosition }}</div>
            </p>
            <p>
              <strong>Rol:</strong>
              <div class="highlight-box">{{ employee.role }}</div>
            </p>
          </div>
        </div>

        <!-- Right Column: Datos Personales -->
        <div class="col-md-6">
          <h5 class="text-justify mb-3 fw-bold">Datos Personales</h5>
          <div class="p-3 border rounded shadow-sm custom-box">
            <p>
              <strong>Beneficios:</strong>
              <div class="highlight-box">{{ employee.benefits }}</div>
            </p>
            <p>
              <strong>Fecha de Contratación:</strong>
              <div class="highlight-box">{{ employee.startDate }}</div>
            </p>
            <p>
              <strong>Teléfonos:</strong>
              <div class="highlight-box">{{ employee.phoneNumbers }}</div>
            </p>
            <p>
              <strong>Correo:</strong>
              <div class="highlight-box">{{ employee.email }}</div>
            </p>
            <p>
              <strong>Provincia:</strong>
              <div class="highlight-box">{{ employee.province }}</div>
            </p>
            <p>
              <strong>Cantón:</strong>
              <div class="highlight-box">{{ employee.canton }}</div>
            </p>
            <p>
              <strong>Otras señas:</strong>
              <div class="highlight-box">{{ employee.otherSigns }}</div>
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue';
  import axios from 'axios';

  const email = "juan.perez@example.com"; // Empleado
  //const email = "miguel.torres@example.com"; //Supervisor
  //const email = "ana.lopez@example.com"; // Administrador

  const isLoading = ref(true);
  const isEditingContractual = ref(false);
  const notFound = ref(false);

  const employee = ref(null);

  function fetchEmployeeData() {
    isLoading.value = true;
    axios.get(`https://localhost:7153/api/EmployeeDetails/${email}`)
      .then(response => {
        const data = response.data;
        if (data) {
          employee.value = {
            id: data.id,
            firstName: data.firstName,
            lastName: data.lastName,
            salary: `${data.grossSalary.toLocaleString()}₡`,
            contractType: data.contractType,
            status: data.status,
            payCycle: data.payCycle,
            jobPosition: data.jobPosition,
            role: data.role,
            benefits: Array.isArray(data.benefits) && data.benefits.length > 0
              ? data.benefits.join(', ')
              : 'N/A',
            startDate: new Date(data.startDate).toLocaleDateString('es-CR', {
              day: '2-digit',
              month: '2-digit',
              year: 'numeric'
            }),
            phoneNumbers: data.phoneNumbers.join(', '),
            email: data.email,
            province: data.province,
            canton: data.canton,
            otherSigns: data.otherSigns
          };
        } else {
          console.warn('No data received from API.');
        }
      })
      .catch(error => {
        console.error('Error fetching employee data:', error);
        notFound.value = true;
      })
      .finally(() => {
        isLoading.value = false;
      });
  }


  function toggleEditContractual() {
    isEditingContractual.value = !isEditingContractual.value;
  }

  onMounted(() => {
    fetchEmployeeData();
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

  .custom-gap .col-md-6 {
    flex: 1 1 48%;
    padding-left: 1.5rem;
    padding-right: 1.5rem;
    min-width: 0;
  }

  @media (max-width: 768px) {
    .custom-gap .col-md-6 {
      flex: 1 1 100%;
      padding-left: 1rem;
      padding-right: 1rem;
    }
  }

  .highlight-box {
    background-color: #f2f2f2;
    border: 1px solid #d1d5db;
    padding: 5px;
    border-radius: 4px;
    word-break: break-word;
  }

  .custom-btn-blue {
    background-color: #003c63;
    color: white;
    border: none;
  }

  .custom-box {
    border-color: #d1d5db;
  }

    .custom-box h5,
    .custom-box p {
      color: #003c63;
    }
</style>
