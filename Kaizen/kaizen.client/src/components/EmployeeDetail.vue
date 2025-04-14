<template>
  <div class="empleado-detalle-container flex flex-col items-center bg-gray-100 p-6 w-full">
    <h1 class="text-2xl font-bold text-blue-900 mb-6 text-center">{{ empleado.nombre }} {{ empleado.apellido }}</h1>
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
        <div class="field-container">
          <p><strong>Beneficios:</strong></p>
          <p class="field-content">{{ empleado.beneficios }}</p>
        </div>
        <div class="field-container">
          <p><strong>Fecha de Contratación:</strong></p>
          <p class="field-content">{{ empleado.fechaContratacion }}</p>
        </div>
        <div class="field-container">
          <p><strong>Teléfonos:</strong></p>
          <p class="field-content">{{ empleado.telefono }}</p>
        </div>
        <div class="field-container">
          <p><strong>Correo:</strong></p>
          <p class="field-content">{{ empleado.correo }}</p>
        </div>
        <div class="field-container">
          <p><strong>Provincia:</strong></p>
          <p class="field-content">{{ empleado.provincia }}</p>
        </div>
        <div class="field-container">
          <p><strong>Cantón:</strong></p>
          <p class="field-content">{{ empleado.canton }}</p>
        </div>
        <div class="field-container">
          <p><strong>Otras Señás:</strong></p>
          <p class="field-content">{{ empleado.otrasSenas }}</p>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
  import { reactive, ref } from 'vue';

  let cedula = '987654321';

  // Make the empleado object reactive
  const empleado = reactive({
    cedula: '',
    nombre: '',
    apellido: '',
    salario: '',
    tipoContrato: '',
    estado: '',
    periodicidad: '',
    puesto: '',
    rol: '',
    beneficios: '',
    fechaContratacion: '',
    telefono: '',
    correo: '',
    provincia: '',
    canton: '',
    otrasSenas: ''
  });

  // State to track if the contractual details are being edited
  const isEditingContractual = ref(false);

  // Fetch employee data from the API based on the cedula
  async function fetchEmpleadoData() {
    try {
      const response = await fetch(`https://localhost:7058/api/Empleado/${cedula}`);
      const data = await response.json();

      // Check if data is returned and populate the empleado object
      if (data) {
        empleado.cedula = data.cedula;
        empleado.nombre = data.nombre;
        empleado.apellido = data.apellido;
        empleado.salario = `${data.salarioBruto.toLocaleString()}₡`; // Formatted salary
        empleado.tipoContrato = data.tipoContrato;
        empleado.estado = data.estado;
        empleado.periodicidad = data.periodicidad;
        empleado.puesto = data.puesto;
        empleado.rol = data.rol;
        empleado.beneficios = data.beneficios.join(', ');
        empleado.fechaContratacion = new Date(data.fechaIngreso).toLocaleDateString();
        empleado.telefono = data.telefonos.join(', ');
        empleado.correo = data.correo;
        empleado.provincia = data.provincia;
        empleado.canton = data.canton;
        empleado.otrasSenas = data.otrasSenas;
      } else {
        console.error('No data returned from API');
      }
    } catch (error) {
      console.error('Error fetching employee data:', error);
    }
  }

  // Call the function to fetch employee data on component load
  fetchEmpleadoData();

  // Function to toggle edit mode for contractual details
  function toggleEditContractual() {
    isEditingContractual.value = !isEditingContractual.value;
  }
</script>

<style scoped>
  /* Container styles */
  .empleado-detalle-container {
    width: 100%;
    max-width: 1200px;
    margin: 0 auto;
    text-align: center;
  }

  /* Title styles */
  h1 {
    color: #1e3a8a;
    margin-bottom: 5rem;
    margin-top: 3rem;
  }

  /* Employee details section styles */
  .empleado-detalle {
    display: flex;
    flex-wrap: wrap;
    justify-content: center;
    align-items: flex-start;
    gap: 8rem;
  }

  /* Section styles */
  section {
    background: #ffffff;
    border-radius: 0.75rem;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.5);
    padding: 1.5rem;
    width: 100%;
    max-width: 500px;
    text-align: justify; /* Justify text in the section */
  }

  /* Field container styles */
  .field-container {
    background: #f4f7fb;
    border-radius: 0.5rem;
    padding: 0.5rem 1rem;
    margin-bottom: 1rem;
    text-align: justify; /* Justify text in the field container */
  }

  /* Field content styles */
  .field-content {
    background: transparent;
    border: none;
    font-size: 0.875rem;
    color: #1e3a8a;
    width: 100%;
    text-align: justify; /* Justify text in the field content */
  }

    .field-content:focus {
      outline: none;
      border: 1px solid #3b82f6;
    }

  /* Edit button styles */
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

  /* Section header styles */
  .section-header {
    display: flex; /* Ensure horizontal alignment */
    align-items: center; /* Vertically align items */
    gap: 1rem; /* Add spacing between the heading and the button */
  }
</style>
