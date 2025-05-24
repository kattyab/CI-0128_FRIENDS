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
        <tr class="position-relative" v-for="(empresa, index) in empresasFiltradas" :key="index">
          <th scope="row">{{ empresa.companyName }}</th>
          <td>{{ empresa.ownerName }}</td>
          <td>{{ empresa.companyID }}</td>
          <td>{{ empresa.employeesCount }}</td>
          <td>
            <a :href="`/companieslist/${empresa.companyPK}`" class="btn btn-primary">
              <span class="material-icons">visibility</span>
            </a>
            <a :href="`/companieslist/${empresa.companyPK}/edit`" class="btn btn-danger ms-1">
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
        busqueda: '',
        ordenAscendente: true,
        empresas: [],
      };
    },
    computed: {
      empresasFiltradas() {
        let resultado = this.empresas.filter(empresa =>
          empresa.companyName.toLowerCase().includes(this.busqueda.toLowerCase())
        );

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
      ordenarPorNombre() {
        this.ordenAscendente = !this.ordenAscendente;
      },
      async cargarEmpresas() {
        try {
          const response = await axios.get('https://localhost:7153/api/CompaniesList', {
            withCredentials: true
          });
          this.empresas = response.data;
        } catch (error) {
          console.error('Error loading companies:', error);
        }
      },
    },
    mounted() {
      this.cargarEmpresas();
    }
  };
</script>

