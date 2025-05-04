<template>
  <div class="rol-updater">
    <h4 class="text-center fw-bold mb-4">Actualización de roles</h4>

    <div class="row mb-3">
      <div class="col-md-5">
        <label for="userSelect" class="form-label" style="color: #003C63;">Usuario</label>
        <div class="dropdown-container" ref="userDropdown">
          <input type="text"
                 class="form-control"
                 placeholder="Buscar usuario..."
                 v-model="userSearch"
                 @focus="showUserDropdown = true" />
          <div v-if="showUserDropdown" class="dropdown-list">
            <div v-for="user in cleanUserList"
                 :key="user"
                 class="dropdown-item"
                 @click="selectUser(user)">
              {{ user }}
            </div>
          </div>
        </div>
      </div>

      <div class="col-md-5">
        <label for="roleSelect" class="form-label" style="color: #003C63;">Nuevo rol</label>
        <div class="dropdown-container" ref="roleDropdown">
          <input type="text"
                 class="form-control"
                 placeholder="Seleccionar rol..."
                 v-model="roleSearch"
                 @focus="showRoleDropdown = true" />
          <div v-if="showRoleDropdown" class="dropdown-list">
            <div v-for="role in filteredRoles"
                 :key="role"
                 class="dropdown-item"
                 @click="selectRole(role)">
              {{ role }}
            </div>
          </div>
        </div>
      </div>

      <div class="col-md-2 d-flex align-items-end">
        <button class="btn btn-primary w-100"
                @click="confirmChange"
                style="background-color: #003C63;">
          Confirmar
        </button>
      </div>
    </div>

    <div v-if="success" class="mb-3" style="color: #00C3B6;">
      ✔ Rol asignado correctamente
    </div>
    <div v-else-if="error" class="mb-3" style="color: red;">
      ✖ El nuevo rol no puede ser igual al actual
    </div>
    <div v-else-if="warning" class="mb-3" style="color: #FFC107;">
      ⚠ Estás por degradar a un Administrador a Empleado. Hacé clic en Confirmar de nuevo para continuar.
    </div>

    <h6 class="fw-bold" style="color: #003C63;">Historial de modificaciones</h6>
    <div class="table-responsive historial-scroll">
      <table class="table">
        <thead class="table-light">
          <tr>
            <th>Responsable</th>
            <th>Usuario</th>
            <th>Fecha y hora</th>
            <th>Rol anterior</th>
            <th>Rol nuevo</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(entry, index) in history" :key="index">
            <td>{{ entry.responsable }}</td>
            <td>{{ entry.usuario }}</td>
            <td>{{ entry.fecha }}</td>
            <td>{{ entry.rolAnterior }}</td>
            <td>{{ entry.rolNuevo }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      users: [], // se llena desde backend
      roles: ['Empleado', 'Supervisor', 'Administrador', 'SuperAdmin'],
      currentRoles: {},

      sessionUser: 'jcastillo@email.com',
      userSearch: '',
      roleSearch: '',
      selectedUser: '',
      selectedRole: '',
      showUserDropdown: false,
      showRoleDropdown: false,
      error: false,
      success: false,
      warning: false,
      warningConfirmed: false,
      history: []
    };
  },
  computed: {
    filteredUsers() {
      return this.users.filter(u =>
        u.toLowerCase().includes(this.userSearch.toLowerCase())
      );
    },
    filteredRoles() {
      return this.roles.filter(r =>
        r.toLowerCase().includes(this.roleSearch.toLowerCase())
      );
    },
    cleanUserList() {
      return this.filteredUsers.map(email => email.split('@')[0]);
    }
  },
  methods: {
    async fetchUsers() {
      try {
        console.log("Enviando solicitud GET a /api/RolCambio...");
        const response = await axios.get('https://localhost:7153/api/RolCambio'); // asegúrate de usar tu ruta real
        const data = response.data;

        console.log("Respuesta recibida:", data);

        this.users = data.map(u => {
          console.log("Usuario encontrado en map:", u);
          return u.email;
        });

        data.forEach(u => {
          console.log(`Asignando rol: ${u.email} -> ${u.nuevoRol}`);
          this.currentRoles[u.email] = u.nuevoRol;
        });

        console.log("Lista de usuarios:", this.users);
        console.log("Roles actuales:", this.currentRoles);
      } catch (err) {
        console.error('Error al obtener usuarios FE:', err);
      }
    },

    selectUser(user) {
      this.selectedUser = user;
      this.userSearch = user;
      this.showUserDropdown = false;
    },
    selectRole(role) {
      this.selectedRole = role;
      this.roleSearch = role;
      this.showRoleDropdown = false;
    },

    async confirmChange() {
      if (this.selectedUser && this.selectedRole) {
        console.log("Usuario seleccionado:", this.selectedUser);
        console.log("Rol seleccionado:", this.selectedRole);
        const prev = this.currentRoles[this.selectedUser+'@email.com'];
        console.log("Rol actual guardado (prev):", prev);

        if (this.selectedRole === prev) {
          this.success = false;
          this.error = true;
          this.warning = false;
          this.warningConfirmed = false;
          setTimeout(() => (this.error = false), 3000);
        } else if (prev === 'Administrador' && this.selectedRole === 'Empleado' && !this.warningConfirmed) {
          this.success = false;
          this.error = false;
          this.warning = true;
          this.warningConfirmed = true;
          setTimeout(() => {
            this.warning = false;
            this.warningConfirmed = false;
          }, 5000);
        } else {
          try {
            console.log("Enviando solicitud PUT a cambiar-rol...");
            console.log("Email seleccionado:", this.selectedUser);
            console.log("Rol seleccionado:", this.selectedRole);
            await axios.put('https://localhost:7153/api/RolCambio/cambiar-rol', {
              email: this.selectedUser+'@email.com',
              nuevoRol: this.selectedRole
            });

            await this.fetchUsers();

            this.history.unshift({
              responsable: this.sessionUser,
              usuario: this.selectedUser,
              fecha: new Date().toLocaleString('en-US', {
                month: 'short',
                day: 'numeric',
                year: 'numeric',
                hour: 'numeric',
                minute: '2-digit',
                hour12: true
              }),
              rolAnterior: prev,
              rolNuevo: this.selectedRole
            });

            this.currentRoles[this.selectedUser] = this.selectedRole;
            this.success = true;
            this.error = false;
            this.warning = false;
            this.warningConfirmed = false;

            setTimeout(() => (this.success = false), 3000);

            this.userSearch = '';
            this.roleSearch = '';
            this.selectedUser = '';
            this.selectedRole = '';
          } catch (err) {
            console.error('Error al cambiar el rol:', err);
            this.error = true;
          }
        }
      }
    },

    handleClickOutside(event) {
      const userEl = this.$refs.userDropdown;
      const roleEl = this.$refs.roleDropdown;

      if (userEl && !userEl.contains(event.target)) {
        this.showUserDropdown = false;
      }

      if (roleEl && !roleEl.contains(event.target)) {
        this.showRoleDropdown = false;
      }
    }
  },
  async mounted() {
    document.addEventListener('click', this.handleClickOutside);
    await this.fetchUsers();
  },
  beforeUnmount() {
    document.removeEventListener('click', this.handleClickOutside);
  }
};
</script>


<style scoped lang="scss">

  th {
    color: #003C63;
  }

  h4 {
    color: #043C62;
  }
  .rol-updater {
    max-width: 900px;
    margin: auto;
    padding: 20px;

    .dropdown-container {
      position: relative;

      .dropdown-list {
        position: absolute;
        top: 100%;
        left: 0;
        right: 0;
        max-height: 150px;
        overflow-y: auto;
        border: 1px solid #ccc;
        background: white;
        z-index: 10;

        .dropdown-item {
          padding: 8px 12px;
          cursor: pointer;

          &:hover {
            background-color: #f0f0f0;
          }
        }
      }
    }

    .historial-scroll {
      max-height: 200px;
      overflow-y: auto;
      border: 1px solid #eee;
      margin-top: 10px;
    }

    table {
      margin-bottom: 0;
    }
  }
</style>
