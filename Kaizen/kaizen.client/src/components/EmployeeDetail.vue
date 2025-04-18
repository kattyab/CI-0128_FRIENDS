<template>
  <div class="empleado-detalle-container flex flex-col items-center bg-gray-100 p-6 w-full">

    <!-- Loading spinner displayed while data is being fetched -->
    <div v-if="isLoading" class="spinner-container">
      <div class="spinner"></div>
      <p class="loading-text">Cargando información del empleado...</p>
    </div>

    <!-- Special "Not Found" page if no employee data is found -->
    <div v-else-if="notFound" class="not-found-container">
      <h1 class="text-3xl font-bold text-red-600">Empleado no encontrado</h1>
      <p class="text-lg text-gray-700">Lo sentimos, no hemos podido encontrar los detalles del empleado.</p>
    </div>

    <!-- Employee details displayed once data is loaded -->
    <div v-else-if="empleado" class="empleado-details">
      <h1 class="text-2xl font-bold text-blue-900 mb-6 text-center">
        {{ empleado.nombre }} {{ empleado.apellido }}
      </h1>

      <div class="empleado-detalle flex flex-wrap justify-center items-start gap-8 bg-transparent w-full max-w-5xl">
        <!-- Left Column: Contractual Details -->
        <section class="flex-1 bg-white rounded-lg shadow p-6 space-y-4">
          <div class="section-header flex items-center gap-4">
            <h2 class="text-lg font-bold text-blue-900">Datos Contractuales</h2>
            <button class="edit-button" @click="toggleEditContractual">
              {{ isEditingContractual ? 'Guardar' : 'Editar' }}
            </button>
          </div>

          <div class="field-container">
            <p><strong>Salario Bruto:</strong></p>
            <template v-if="isEditingContractual">
              <input v-model="empleado.salario" class="field-content" />
            </template>
            <template v-else>
              <p class="field-content">{{ empleado.salario }}</p>
            </template>
          </div>

          <div class="field-container">
            <p><strong>Tipo de Contrato:</strong></p>
            <template v-if="isEditingContractual">
              <select v-model="empleado.tipoContrato" class="field-content">
                <option value="Tiempo Completo">Tiempo Completo</option>
                <option value="Medio Tiempo">Medio Tiempo</option>
                <option value="Servicios Profesionales">Servicios Profesionales</option>
                <option value="Por Horas">Por Horas</option>
              </select>
            </template>
            <template v-else>
              <p class="field-content">{{ empleado.tipoContrato }}</p>
            </template>
          </div>

          <div class="field-container">
            <p><strong>Estado:</strong></p>
            <template v-if="isEditingContractual">
              <select v-model="empleado.estado" class="field-content">
                <option value="Activo">Activo</option>
                <option value="Inactivo">Inactivo</option>
              </select>
            </template>
            <template v-else>
              <p class="field-content">{{ empleado.estado }}</p>
            </template>
          </div>

          <div class="field-container">
            <p><strong>Periodicidad:</strong></p>
            <template v-if="isEditingContractual">
              <input v-model="empleado.periodicidad" class="field-content" />
            </template>
            <template v-else>
              <p class="field-content">{{ empleado.periodicidad }}</p>
            </template>
          </div>

          <div class="field-container">
            <p><strong>Puesto:</strong></p>
            <template v-if="isEditingContractual">
              <input v-model="empleado.puesto" class="field-content" />
            </template>
            <template v-else>
              <p class="field-content">{{ empleado.puesto }}</p>
            </template>
          </div>

          <div class="field-container">
            <p><strong>Rol:</strong></p>
            <template v-if="isEditingContractual">
              <select v-model="empleado.rol" class="field-content">
                <option value="Empleado">Empleado</option>
                <option value="Supervisor">Supervisor</option>
                <option value="Administrador">Administrador</option>
              </select>
            </template>
            <template v-else>
              <p class="field-content">{{ empleado.rol }}</p>
            </template>
          </div>
        </section>

        <!-- Right Column: Personal Information -->
        <section class="flex-1 bg-white rounded-lg shadow p-6 space-y-4">
          <h2 class="text-lg font-bold text-blue-900">Información Personal</h2>

          <div class="field-container"><p><strong>Beneficios:</strong></p><p class="field-content">{{ empleado.beneficios }}</p></div>
          <div class="field-container"><p><strong>Fecha de Contratación:</strong></p><p class="field-content">{{ empleado.fechaContratacion }}</p></div>
          <div class="field-container"><p><strong>Teléfonos:</strong></p><p class="field-content">{{ empleado.telefono }}</p></div>
          <div class="field-container"><p><strong>Correo:</strong></p><p class="field-content">{{ empleado.correo }}</p></div>
          <div class="field-container"><p><strong>Provincia:</strong></p><p class="field-content">{{ empleado.provincia }}</p></div>
          <div class="field-container"><p><strong>Cantón:</strong></p><p class="field-content">{{ empleado.canton }}</p></div>
          <div class="field-container"><p><strong>Otras Señás:</strong></p><p class="field-content">{{ empleado.otrasSenas }}</p></div>
        </section>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';

//const email = "carlos.ramirez@example.com";
//const email = "luis.sibaja@example.com";
const email = "maria.fernandez@example.com";
const isLoading = ref(true);
const isEditingContractual = ref(false);
const notFound = ref(false);

// Empleado is now a ref holding an object
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
            fechaContratacion: new Date(data.fechaIngreso).toLocaleDateString(),
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
/* Same styling as you already had */
.spinner-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-top: 3rem;
}
.spinner {
  border: 8px solid #f3f3f3;
  border-top: 8px solid #1e3a8a;
  border-radius: 50%;
  width: 60px;
  height: 60px;
  animation: spin 1s linear infinite;
}
.loading-text {
  margin-top: 1rem;
  color: #1e3a8a;
  font-weight: bold;
  font-size: 1rem;
}
@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
.empleado-detalle-container {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
  text-align: center;
}
h1 {
  color: #1e3a8a;
  margin-bottom: 5rem;
  margin-top: 3rem;
}
.empleado-detalle {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  align-items: flex-start;
  gap: 8rem;
}
section {
  background: #ffffff;
  border-radius: 0.75rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.5);
  padding: 1.5rem;
  width: 100%;
  max-width: 500px;
  text-align: justify;
}
.field-container {
  background: #f4f7fb;
  border-radius: 0.5rem;
  padding: 0.5rem 1rem;
  margin-bottom: 1rem;
  text-align: justify;
}
.field-content {
  background: transparent;
  border: none;
  font-size: 0.875rem;
  color: #1e3a8a;
  width: 100%;
  text-align: justify;
}
.field-content:focus {
  outline: none;
  border: 1px solid #3b82f6;
}
.edit-button {
  background-color: #1e3a8a;
  color: white;
  border: none;
  border-radius: 0.5rem;
  padding: 0.5rem 1rem;
  cursor: pointer;
  font-size: 0.875rem;
}
.edit-button:hover {
  background-color: #3b82f6;
}
.section-header {
  display: flex;
  align-items: center;
  gap: 1rem;
}
html, body {
  margin: 0;
  padding: 0;
  height: 100%;
  width: 100%;
}
.app {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  width: 100vw;
  overflow: auto;
}
.main-content {
  display: flex;
  flex-grow: 1;
  width: 100%;
}
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  font-family: Arial, sans-serif;
}
</style>
