<template>
  <div class="empresa-lista">
    <h4 class="text-center fw-bold mb-4">Lista de empresas</h4>

    <div class="mb-3">
      <input v-model="busqueda"
             type="text"
             class="form-control"
             placeholder="Buscar por nombre de empresa" />
    </div>

    <div class="table-wrapper">
      <table class="table">
        <thead class="table-light sticky-header">
          <tr>
            <th @click="ordenarPorNombre" style="cursor: pointer;">
              Nombre de empresa
              <span>{{ ordenAscendente ? '▲' : '▼' }}</span>
            </th>
            <th>Dueño</th>
            <th>Cédula jurídica</th>
            <th>Cantidad de empleados</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(empresa, index) in empresasFiltradas" :key="index">
            <td>{{ empresa.companyName }}</td>
            <td>{{ empresa.ownerName }}</td>
            <td>{{ empresa.companyID }}</td>
            <td>{{ empresa.employeesCount }}</td>
            <td>
              <button class="btn btn-sm btn-outline-primary">Acciones</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
  import axios from 'axios';

  export default {
    name: 'EmpresaLista',
    data() {
      return {
        // User search input
        busqueda: '',

        // Toggle for sorting order (ascending/descending)
        ordenAscendente: true,

        // List of companies retrieved from the API
        empresas: []
      };
    },
    computed: {
      // Returns a filtered and sorted list of companies
      empresasFiltradas() {
        // Filter companies based on the search input
        let resultado = this.empresas.filter(empresa =>
          empresa.companyName.toLowerCase().includes(this.busqueda.toLowerCase())
        );

        // Sort the result alphabetically (asc or desc)
        resultado.sort((a, b) => {
          const nombreA = a.companyName.toLowerCase();
          const nombreB = b.companyName.toLowerCase();
          return this.ordenAscendente
            ? nombreA.localeCompare(nombreB)
            : nombreB.localeCompare(nombreA);
        });

        return resultado;
      }
    },
    methods: {
      // Toggle the sorting order
      ordenarPorNombre() {
        this.ordenAscendente = !this.ordenAscendente;
      },

      // Fetch companies from the API
      async cargarEmpresas() {
        try {
          const response = await axios.get('https://localhost:7153/api/CompaniesList');
          this.empresas = response.data;
        } catch (error) {
          console.error('Error loading companies:', error);
        }
      }
    },

    // Called when the component is mounted to fetch the data
    mounted() {
      this.cargarEmpresas();
    }
  };
</script>




<style scoped lang="scss">
.empresa-lista {
  max-width: 900px;
  margin: auto;
  padding: 20px;

  h4 {
    color: #043c62;
  }

  th {
    color: #003c63;
    user-select: none;
  }

  .table-wrapper {
    max-height: 300px; /* Puedes ajustar esta altura */
    overflow-y: auto;
    border: 1px solid #dee2e6;
  }

  .table {
    margin-bottom: 0;
  }

  .table th,
  .table td {
    vertical-align: middle;
    white-space: nowrap;
  }

  .sticky-header {
    position: sticky;
    top: 0;
    z-index: 2;
    background-color: #f8f9fa;
  }

  .btn-outline-primary {
    color: #003c63;
    border-color: #003c63;
  }

  .btn-outline-primary:hover {
    background-color: #003c63;
    color: white;
  }
}
</style>
