<template>
  <div>
    <h1 class="text-center my-4">Lista de Empleadores</h1>
    <div class="mx-4">
      <table class="table table-hover">
        <thead>
          <tr>
            <th scope="col">Nombre de la empresa</th>
            <th scope="col">Nombre del dueño</th>
            <th scope="col">Segundo nombre del dueño</th>
            <th scope="col">ID del dueño</th>
            <th scope="col">Acciones</th>
          </tr>
        </thead>
        <tbody class="table-group-divider">
          <tr v-for="(company, index) in filteredCompanies" :key="index">
            <td>{{ company.companyName || '---' }}</td>
            <td>{{ company.name }}</td>
            <td>{{ company.lastName }}</td>
            <td>{{ company.id }}</td>
            <td>
              <button class="btn btn-danger" @click="confirmDelete(company)">
                <span class="material-icons">delete</span>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modal de confirmación -->
    <div class="modal fade" id="deleteModal" tabindex="-1" aria-labelledby="deleteModalLabel" aria-hidden="true" ref="modal">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header bg-warning">
            <h5 class="modal-title" id="deleteModalLabel">Confirmar eliminación</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
          </div>
          <div class="modal-body">
            ¿Estás seguro de que deseas eliminar al empleador <strong>{{ selectedCompany?.name }} {{ selectedCompany?.lastName }}</strong>?
            <br />
            Esta acción no se puede deshacer.
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
            <button type="button" class="btn btn-danger" @click="handleDelete">Eliminar</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import axios from 'axios';
  import { Modal } from 'bootstrap';

  export default {
    name: 'EmpresaLista',
    data() {
      return {
        search: '',
        ascendingOrder: true,
        companies: [],
        selectedCompany: null,
        modalInstance: null,
      };
    },
    computed: {
      filteredCompanies() {
        return this.companies
          .filter(company =>
            (company.companyName || '').toLowerCase().includes(this.search.toLowerCase())
          )
          .sort((a, b) => {
            const nameA = (a.companyName || '').toLowerCase();
            const nameB = (b.companyName || '').toLowerCase();
            return this.ascendingOrder
              ? nameA.localeCompare(nameB)
              : nameB.localeCompare(nameA);
          });
      }
    },
    methods: {
      orderByName() {
        this.ascendingOrder = !this.ascendingOrder;
      },
      confirmDelete(company) {
        this.selectedCompany = company;
        this.modalInstance.show();
      },
      async handleDelete() {
        const company = this.selectedCompany;
        const ownerPK = company.ownerPK;
        const apiBase = `${import.meta.env.VITE_API_URL}/api/DeleteEmployer`;

        try {
          if (!company.companyPK) {
            // Hard delete
            await axios.delete(`${apiBase}/${ownerPK}`, { withCredentials: true });
            alert('Empleador eliminado permanentemente.');
          } else if (company.companyIsDeleted === true && company.paidBy) {
            // Soft delete
            await axios.put(`${apiBase}/${ownerPK}`, null, { withCredentials: true });
            alert('Empleador eliminado lógicamente.');
          } else {
            alert('No se puede eliminar este empleador. Asegúrate de que la compañía esté eliminada y tenga planillas.');
            return;
          }

          this.modalInstance.hide();
          this.selectedCompany = null;
          this.loadCompanies();
        } catch (error) {
          console.error('Error al eliminar empleador:', error);
          alert('Error al intentar eliminar el empleador.');
        }
      },
      async loadCompanies() {
        try {
          const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/DeleteEmployer`, {
            withCredentials: true
          });
          this.companies = response.data;
        } catch (error) {
          console.error('Error cargando empleadores:', error);
        }
      }
    },
    mounted() {
      this.loadCompanies();

      const modalElement = this.$refs.modal;
      this.modalInstance = new Modal(modalElement, { backdrop: 'static' });
    }
  };
</script>
