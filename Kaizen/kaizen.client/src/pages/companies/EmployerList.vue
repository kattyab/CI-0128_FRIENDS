<template>
  <div>
    <h1 class="text-center my-4">Lista de Empleadores</h1>
    <div class="mx-4">
      <table class="table table-hover">
        <thead>
          <tr>
            <th scope="col">Nombre</th>
            <th scope="col">Apellido</th>
            <th scope="col">ID</th>
            <th scope="col">Email</th>
            <th scope="col">En Planilla</th>
            <th scope="col">Acciones</th>
          </tr>
        </thead>


        <tbody v-if="filteredEmployers.length === 0">
          <tr>
            <td colspan="6" class="text-center text-muted py-4">
              No hay empleadores para mostrar.
            </td>
          </tr>
        </tbody>


        <tbody class="table-group-divider" v-else>
          <tr v-for="(employer, index) in filteredEmployers" :key="index">
            <td>{{ employer.name }}</td>
            <td>{{ employer.lastName }}</td>
            <td>{{ employer.id }}</td>
            <td>{{ employer.email || '---' }}</td>
            <td>{{ employer.inCharge ? 'Sí' : 'No' }}</td>
            <td>
              <button class="btn btn-danger" @click="confirmDelete(employer)">
                <span class="material-icons">delete</span>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>


    <div class="modal fade" id="deleteModal" tabindex="-1" aria-labelledby="deleteModalLabel" aria-hidden="true" ref="modal">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header bg-warning">
            <h5 class="modal-title" id="deleteModalLabel">Confirmar eliminación</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
          </div>
          <div class="modal-body">
            ¿Estás seguro de que deseas eliminar al empleador
            <strong>{{ selectedEmployer?.name }} {{ selectedEmployer?.lastName }}</strong>?
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
        employers: [],
        selectedEmployer: null,
        modalInstance: null,
      };
    },
    computed: {
      filteredEmployers() {
        return this.employers
          .filter(emp =>
            (emp.name || '').toLowerCase().includes(this.search.toLowerCase())
          )
          .sort((a, b) => {
            const nameA = (a.name || '').toLowerCase();
            const nameB = (b.name || '').toLowerCase();
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
      confirmDelete(employer) {
        this.selectedEmployer = employer;
        this.modalInstance.show();
      },
      async handleDelete() {
        const employer = this.selectedEmployer;
        const ownerPK = employer.ownerPK;
        const apiBase = `${import.meta.env.VITE_API_URL}/api/DeleteEmployer`;

        try {
          if (!employer.inCharge) {
            // Hard delete
            await axios.delete(`${apiBase}/${ownerPK}`, { withCredentials: true });
            alert('Empleador eliminado permanentemente.');
          } else {
            // Soft delete
            await axios.put(`${apiBase}/${ownerPK}`, null, { withCredentials: true });
            alert('Empleador eliminado lógicamente.');
          }

          this.modalInstance.hide();
          this.selectedEmployer = null;
          this.loadEmployers();
        } catch (error) {
          console.error('Error al eliminar empleador:', error);
          alert('Error al intentar eliminar el empleador.');
        }
      },
      async loadEmployers() {
        try {
          const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/DeleteEmployer`, {
            withCredentials: true
          });
          this.employers = response.data;
        } catch (error) {
          console.error('Error cargando empleadores:', error);
        }
      }
    },
    mounted() {
      this.loadEmployers();

      const modalElement = this.$refs.modal;
      this.modalInstance = new Modal(modalElement, { backdrop: 'static' });
    }
  };
</script>
