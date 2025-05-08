<script setup>
  import { ref, onMounted, markRaw } from 'vue'
  import { useRouter } from 'vue-router'
  import axios from 'axios'
  import { defineAsyncComponent } from 'vue'

  const router = useRouter()
  const roleComponent = ref(null)

  const roleComponentMap = {
    'Administrador': markRaw(defineAsyncComponent(() => import('./role_pages/Admin.vue'))),
    'Empleado': markRaw(defineAsyncComponent(() => import('./role_pages/Employee.vue'))),
    'Dueño': markRaw(defineAsyncComponent(() => import('./role_pages/Owner.vue'))),
    'Superadmin': markRaw(defineAsyncComponent(() => import('./role_pages/Super.vue'))),
    'Supervisor': markRaw(defineAsyncComponent(() => import('./role_pages/Supervisor.vue')))
  }

  onMounted(async () => {
    try {
      const { data } = await axios.get('/api/login/authenticate', {
        withCredentials: true
      })
      roleComponent.value = roleComponentMap[data.role] || null
    } catch (error) {
    }
  })
</script>

<template>
  <div>
    <component :is="roleComponent" v-if="roleComponent" />
    <div v-else>
      <p>Rol no válido o no asignado. Contacte al administrador.</p>
    </div>
  </div>
</template>
