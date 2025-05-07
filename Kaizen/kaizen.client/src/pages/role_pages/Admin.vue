<template>
  <footer class="footer mt-auto py-3 bg-body-tertiary">
    <div class="container">
      <div class="text-body-secondary text-center">Footer content</div>
    </div>
  </footer>
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
      const response = await axios.get(`/api/CommonHomepage/${email}`);
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
      const response = await axios.get('/api/login/authenticate', { withCredentials: true });
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

