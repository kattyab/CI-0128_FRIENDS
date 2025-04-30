<script setup>
import { ref, onMounted, defineAsyncComponent } from 'vue'

const role = ref(localStorage.getItem('userRole') || '');

const roleComponentMap = {
  'Administrador': defineAsyncComponent(() => import('./role_pages/Admin.vue')),
  'Empleado': defineAsyncComponent(() => import('./role_pages/Employee.vue')),
  'Dueño': defineAsyncComponent(() => import('./role_pages/Owner.vue')),
  'Superadmin': defineAsyncComponent(() => import('./role_pages/Super.vue')),
  'Supervisor': defineAsyncComponent(() => import('./role_pages/Supervisor.vue'))
};

const RoleComponent = ref(null);

onMounted(() => {
  RoleComponent.value = roleComponentMap[role.value] || null;
});
</script>

<template>
  <div>
    <component :is="RoleComponent" v-if="RoleComponent" />
    <div v-else>
      <p>Rol no válido o no asignado. Contacte al administrador.</p>
    </div>
  </div>
</template>
