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
      <h1 class="text-center mb-4 mt-5 pt-4" style="color: #003c63;">
        {{ empleado?.nombre }} {{ empleado?.apellido }}
      </h1>

      <!-- Two Columns with Custom Gap -->
      <div class="row custom-gap">
        <!-- Left Column: Datos Contractuales -->
        <div class="col-md-6">
          <div class="d-flex justify-content-between align-items-center mb-3">
            <h5 class="fw-bold">Datos Contractuales</h5>
            <button class="btn btn-primary" @click="toggleEditContractual">
              Editar
            </button>
          </div>
          <div class="p-3 border rounded shadow-sm custom-box">
            <p>
              <strong>Salario:</strong>
              <div class="highlight-box">{{ empleado.salario }}</div>
            </p>
            <p>
              <strong>Tipo de Contrato:</strong>
              <div class="highlight-box">{{ empleado.tipoContrato }}</div>
            </p>
            <p>
              <strong>Estado:</strong>
              <div class="highlight-box">{{ empleado.estado }}</div>
            </p>
            <p>
              <strong>Periodicidad:</strong>
              <div class="highlight-box">{{ empleado.periodicidad }}</div>
            </p>
            <p>
              <strong>Puesto:</strong>
              <div class="highlight-box">{{ empleado.puesto }}</div>
            </p>
            <p>
              <strong>Rol:</strong>
              <div class="highlight-box">{{ empleado.rol }}</div>
            </p>
          </div>
        </div>

        <!-- Right Column: Datos Personales -->
        <div class="col-md-6">
          <h5 class="text-justify mb-3 fw-bold">Datos Personales</h5>
          <div class="p-3 border rounded shadow-sm custom-box">
            <p>
              <strong>Beneficios:</strong>
              <div class="highlight-box">{{ empleado.beneficios }}</div>
            </p>
            <p>
              <strong>Fecha de Contratación:</strong>
              <div class="highlight-box">{{ empleado.fechaContratacion }}</div>
            </p>
            <p>
              <strong>Teléfonos:</strong>
              <div class="highlight-box">{{ empleado.telefono }}</div>
            </p>
            <p>
              <strong>Correo:</strong>
              <div class="highlight-box">{{ empleado.correo }}</div>
            </p>
            <p>
              <strong>Provincia:</strong>
              <div class="highlight-box">{{ empleado.provincia }}</div>
            </p>
            <p>
              <strong>Cantón:</strong>
              <div class="highlight-box">{{ empleado.canton }}</div>
            </p>
            <p>
              <strong>Otras señas:</strong>
              <div class="highlight-box">{{ empleado.otrasSenas }}</div>
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

  const email = "carlos.ramirez@example.com";
  //const email = "luis.sibaja@example.com";
  //const email = "maria.fernandez@example.com";
  const isLoading = ref(true);
  const isEditingContractual = ref(false);
  const notFound = ref(false);

  const empleado = ref(null);

  function fetchEmpleadoData() {
    isLoading.value = true;
    axios.get(`https://localhost:7058/api/Employee/${email}`)
      .then(response => {
        const data = response.data;
        if (data) {
          empleado.value = {
            cedula: data.cedula,
            nombre: data.nombre,
            apellido: data.apellido,
            salario: `${data.salarioBruto.toLocaleString()}₡`,
            tipoContrato: data.tipoContrato,
            estado: data.estado,
            periodicidad: data.periodicidad,
            puesto: data.puesto,
            rol: data.rol,
            beneficios: Array.isArray(data.beneficios) && data.beneficios.length > 0
              ? data.beneficios.join(', ')
              : 'N/A',
            fechaContratacion: new Date(data.fechaIngreso).toLocaleDateString('es-CR', {
              day: '2-digit',
              month: '2-digit',
              year: 'numeric'
            }),
            telefono: data.telefonos.join(', '),
            correo: data.correo,
            provincia: data.provincia,
            canton: data.canton,
            otrasSenas: data.otrasSenas
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
    fetchEmpleadoData();
  });
</script>

<style scoped>

  .custom-gap {
    margin-left: -2rem;
    margin-right: -2rem;
  }

    .custom-gap .col-md-6 {
      padding-left: 3.5rem;
      padding-right: 3.5rem;
    }

  .highlight-box {
    background-color: #ffffff;
    border: 1px solid #d1d5db;
    padding: 5px;
    border-radius: 4px;
  }

  .custom-box {
    background-color: #f2f2f2;
    border-color: #d1d5db;
  }

  .container {
    font-family: 'DM Sans', sans-serif;
    color: #003c63;
  }

  .custom-box h5,
  .custom-box p {
    color: #003c63;
  }
</style>
