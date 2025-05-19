<template>
  <div class="p-4">
    <h1 class="text-2xl font-bold" style="color: #003C63;">Superadmin Page</h1>
    <p style="color: #003C63;">Welcome, Superadmin! You have full access.</p>

    <button @click="logout"
            class="mt-4 px-4 py-2 bg-transparent border border-[#003C63] rounded hover:bg-[#003C63] transition"
            style="color: #003C63;">
      Logout
    </button>
  </div>
</template>

<script setup>
  import { useLogout } from '@/composables/useLogout'

  const { logout } = useLogout()
</script>
