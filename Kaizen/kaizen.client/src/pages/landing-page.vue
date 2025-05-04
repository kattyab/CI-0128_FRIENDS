<script setup>
  import { ref, onMounted, defineAsyncComponent } from 'vue'
  import axios from 'axios'

  const roleComponent = ref(null);

  const roleComponentMap = {
    'Administrador': defineAsyncComponent(() => import('./role_pages/Admin.vue')),
    'Empleado': defineAsyncComponent(() => import('./role_pages/Employee.vue')),
    'Dueño': defineAsyncComponent(() => import('./role_pages/Owner.vue')),
    'Superadmin': defineAsyncComponent(() => import('./role_pages/Super.vue')),
    'Supervisor': defineAsyncComponent(() => import('./role_pages/Supervisor.vue'))
  };

  onMounted(async () => {
    try {
      const { data } = await axios.get('https://localhost:7153/api/Login/whoami', {
        withCredentials: true
      });

      console.log(data.email);
      console.log(data.role);

      roleComponent.value = roleComponentMap[data.role] || null;
    } catch (error) {
      console.error('Failed to get user role:', error);
    }
  });
</script>

<template>
  <div>
    <component :is="roleComponent" v-if="roleComponent" />
    <div v-else>
      <p>Rol no válido o no asignado. Contacte al administrador.</p>
    </div>
  </div>
</template>
