<template>
  <div>
    <h1 class="text-center">Lista de empresas</h1>
    <table class="table table-hover">
      <thead>
        <tr>
          <th scope="col">Nombre de la empresa</th>
          <th scope="col">Dueño</th>
          <th scope="col">Cédula Jurídica</th>
          <th scope="col">Cantidad de empleados</th>
          <th scope="col">Acciones</th>
        </tr>
      </thead>
      <tbody class="table-group-divider">
        <tr class="position-relative" v-for="(company, index) in filteredCompanies" :key="index">
          <th scope="row">{{ company.companyName }}</th>
          <td>{{ company.ownerName }}</td>
          <td>{{ company.companyID }}</td>
          <td>{{ company.employeesCount }}</td>
          <td>
            <a :href="`/companieslist/${company.companyPK}`" class="btn btn-primary">
              <span class="material-icons">visibility</span>
            </a>
            <a :href="`/companieslist/${company.companyPK}/edit`" class="btn btn-danger ms-1">
              <span class="material-icons">delete</span>
            </a>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
  import axios from 'axios';

  export default {
    name: 'EmpresaLista',
    data() {
      return {
        search: '',
        ascendingOrder: true,
        companies: [],
      };
    },
    computed: {
      filteredCompanies() {
        let result = this.companies.filter(company =>
          company.companyName.toLowerCase().includes(this.search.toLowerCase())
        );

        result.sort((a, b) => {
          const nameA = a.companyName.toLowerCase();
          const nameB = b.companyName.toLowerCase();
          return this.ascendingOrder
            ? nameA.localeCompare(nameB)
            : nameB.localeCompare(nameA);
        });

        return result;
      }
    },
    methods: {
      orderByName() {
        this.ascendingOrder = !this.ascendingOrder;
      },
      async loadCompanies() {
        try {
          const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/CompaniesList`, {
            withCredentials: true
          });
          this.companies = response.data;
        } catch (error) {
          console.error('Error loading companies:', error);
        }
      },
    },
    mounted() {
      this.loadCompanies();
    }
  };
</script>

