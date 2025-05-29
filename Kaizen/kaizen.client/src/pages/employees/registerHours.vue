<template>
  <div class="p-6 max-w-3xl mx-auto">
    <h1 class="text-2xl font-bold text-center mb-6 text-sky-900">Registro de horas</h1>

    <!-- Registros existentes -->
    <div v-if="registers.length > 0" class="space-y-4">
      <div v-for="(reg, index) in registers"
           :key="index"
           class="flex justify-between items-center border-b pb-4">
        <!-- Parte izquierda -->
        <div class="text-sky-900">
          <p class="text-sm font-medium">
            {{ reg.label || 'Semana' }}
          </p>
          <p>{{ formatDate(reg.start) }} - {{ formatDate(reg.end) }}</p>
        </div>

        <!-- Centro -->
        <div class="text-center text-sky-900">
          <p class="text-sm font-medium">Horas Trabajadas</p>
          <p>{{ reg.hours }}</p>
        </div>

        <!-- Derecha -->
        <div>
          <button v-if="reg.status === 'draft'"
                  @click="sendReview(index)"
                  class="bg-sky-900 text-white px-4 py-2 rounded hover:bg-sky-800 transition">
            Enviar a revisión
          </button>
          <div v-else
               class="bg-teal-500 text-white px-4 py-2 rounded text-center">
            En revisión
          </div>
        </div>
      </div>
    </div>

    <!-- Botón añadir -->
    <div class="mt-10 text-center">
      <button @click="showForm = !showForm"
              class="text-xl font-semibold text-gray-400 flex items-center justify-center space-x-2 hover:text-gray-600 transition">
        <span>Añadir registro de horas</span>
        <span class="text-2xl font-bold">+</span>
      </button>
    </div>

    <!-- Formulario -->
    <div v-if="showForm" class="mt-6 border-t pt-6">
      <div class="flex flex-col md:flex-row md:items-end md:space-x-4 space-y-4 md:space-y-0">
        <!-- Semana -->
        <div class="flex-1">
          <label class="block mb-2 text-sm font-medium text-gray-700">Semana trabajada</label>
          <Datepicker v-model="selectedDate"
                      :enable-time-picker="false"
                      :format="formatDate"
                      :auto-apply="true"
                      :week-picker="true"
                      :start-week-on="1"
                      class="w-full bg-white"
                      placeholder="Selecciona una semana" />
        </div>

        <!-- Horas -->
        <div class="w-full md:w-1/3">
          <label class="block mb-2 text-sm font-medium text-gray-700">Horas trabajadas</label>
          <input type="number"
                 v-model="newRegister.hours"
                 class="border rounded p-2 w-full"
                 min="0" />
        </div>

        <!-- Guardar -->
        <div>
          <button @click="addRegister"
                  class="bg-sky-800 text-white px-4 py-2 rounded hover:bg-sky-700 transition">
            Guardar
          </button>
        </div>
      </div>
    </div>
  </div>
</template>




<script setup>
  import { ref, watch } from 'vue'
  import Datepicker from '@vuepic/vue-datepicker'
  import '@vuepic/vue-datepicker/dist/main.css'

  const registers = ref([])
  const showForm = ref(false)
  const selectedDate = ref(null)

  const newRegister = ref({
    start: '',
    end: '',
    hours: '',
    status: 'draft',
  })

  // Cuando seleccionas una fecha, se calcula la semana completa (lunes a domingo)
  watch(selectedDate, (date) => {
    if (!date) return
    const selected = new Date(date)
    const day = selected.getDay()
    const diffToMonday = day === 0 ? -6 : 1 - day
    const monday = new Date(selected)
    monday.setDate(selected.getDate() + diffToMonday)

    const sunday = new Date(monday)
    sunday.setDate(monday.getDate() + 6)

    newRegister.value.start = monday
    newRegister.value.end = sunday
  })

  // Función para agregar registro
  const addRegister = () => {
    if (newRegister.value.start && newRegister.value.end && newRegister.value.hours) {
      registers.value.push({ ...newRegister.value })
      newRegister.value = { start: '', end: '', hours: '', status: 'draft' }
      selectedDate.value = null
      showForm.value = false
    }
  }

  const sendReview = (index) => {
    registers.value[index].status = 'reviewing'
  }

  // Mostrar fecha como dd/mm/yyyy
  const formatDate = (date) => {
    if (!(date instanceof Date)) return ''
    const d = new Date(date)
    const day = String(d.getDate()).padStart(2, '0')
    const month = String(d.getMonth() + 1).padStart(2, '0')
    const year = d.getFullYear()
    return `${day}/${month}/${year}`
  }
</script>

<style>
  :root {
    --dp-background-color: white;
    --dp-text-color: #0f172a;
    --dp-border-color: #e5e7eb;
    --dp-border-radius: 8px;
    --dp-hover-color: #e0f2fe;
    --dp-primary-color: #0369a1;
  }
</style>
