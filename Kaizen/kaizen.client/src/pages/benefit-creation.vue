<template>

  <div class="page container-fluid mt-4">
    <div>
      <h1>Crear Beneficios</h1>
    </div>
    <div>
      <div class="row">
        <div class="col-1"></div>
        <div class="col-10">
          <form @submit.prevent="submitForm">
            <div class="mb-3">
              <label for="benefitName" class="form-label">Nombre beneficio</label>
              <input type="text"
                     class="form-control input-type"
                     id="benefitName"
                     v-model="formData.benefitName"
                     :class="{ 'is-invalid': validationErrors.benefitName }"
                     placeholder="Ingrese el nombre del beneficio"
                     required>
              <div class="invalid-feedback" v-if="validationErrors.benefitName">
                {{ validationErrors.benefitName }}
              </div>
            </div>

            <div class="mb-3">
              <label for="minimumTime" class="form-label">Tiempo mínimo (meses)</label>
              <input type="number"
                     class="form-control input-type"
                     id="minimumTime"
                     v-model.number="formData.minimumTime"
                     :class="{ 'is-invalid': validationErrors.minimumTime }"
                     placeholder="Ingrese el tiempo mínimo para ser elegible"
                     min="0"
                     required>
              <div class="invalid-feedback" v-if="validationErrors.minimumTime">
                {{ validationErrors.minimumTime }}
              </div>
            </div>

            <div class="mb-3 col-10">
              <label class="form-label">Elegibles</label>
              <div class="form-check">
                <input class="form-check-input"
                       type="checkbox"
                       id="empleado"
                       v-model="formData.elegibles.empleado">
                <label class="form-check-label" for="empleado">Empleado</label>
              </div>
              <div class="form-check">
                <input class="form-check-input"
                       type="checkbox"
                       id="admin"
                       v-model="formData.elegibles.admin">
                <label class="form-check-label" for="admin">Administrador</label>
              </div>
              <div class="form-check">
                <input class="form-check-input"
                       type="checkbox"
                       id="supervisor"
                       v-model="formData.elegibles.supervisor">
                <label class="form-check-label" for="supervisor">Supervisor</label>
              </div>
              <div class="text-danger small" v-if="validationErrors.elegibles">
                {{ validationErrors.elegibles }}
              </div>
            </div>

            <div class="mb-3">
              <label for="benefitType" class="form-label">Tipo de beneficio</label>
              <select class="form-select input-type"
                      id="benefitType"
                      v-model="formData.benefitType"
                      :class="{ 'is-invalid': validationErrors.benefitType }"
                      required>
                <option value="" disabled>Seleccione un tipo</option>
                <option value="fixedAmmount">Monto fijo</option>
                <option value="percentage">Porcentaje</option>
                <option value="api">API</option>
              </select>
              <div class="invalid-feedback" v-if="validationErrors.benefitType">
                {{ validationErrors.benefitType }}
              </div>
            </div>

            <div v-if="formData.benefitType === 'fixedAmmount'" class="mb-3">
              <label for="fixedAmmount" class="form-label">Monto fijo</label>
              <div class="input-group">
                <span class="input-group-text input-type">₡</span>
                <input type="number"
                       class="form-control input-type"
                       id="fixedAmmount"
                       v-model.number="formData.fixedAmmount"
                       :class="{ 'is-invalid': validationErrors.fixedAmmount }"
                       min="0"
                       placeholder="XXXXXX"
                       required>
                <div class="invalid-feedback" v-if="validationErrors.fixedAmmount">
                  {{ validationErrors.fixedAmmount }}
                </div>
              </div>
            </div>

            <div v-if="formData.benefitType === 'percentage'" class="mb-3">
              <label for="percentage" class="form-label">Porcentaje</label>
              <div class="input-group">
                <input type="number"
                       class="form-control input-type"
                       id="percentage"
                       v-model.number="formData.percentage"
                       :class="{ 'is-invalid': validationErrors.percentage }"
                       step="0.01"
                       min="0"
                       max="100"
                       placeholder="XXX"
                       required>
                <span class="input-group-text input-type">%</span>
                <div class="invalid-feedback" v-if="validationErrors.percentage">
                  {{ validationErrors.percentage }}
                </div>
              </div>
            </div>

            <div v-if="formData.benefitType === 'api'">
              <label for="apiUrl" class="form-label">URL de API</label>
              <input type="url"
                     class="form-control input-type mb-3"
                     id="apiUrl"
                     v-model="formData.apiUrl"
                     :class="{ 'is-invalid': validationErrors.apiUrl }"
                     placeholder="Ingrese un enlace válido"
                     required>
              <div class="invalid-feedback" v-if="validationErrors.apiUrl">
                {{ validationErrors.apiUrl }}
              </div>

              <div>
                <label for="parameterQuantity" class="form-label mt-3">Cantidad de parámetros</label>
                <select class="form-select input-type mb-3"
                        id="parameterQuantity"
                        v-model.number="formData.parameterQuantity"
                        :class="{ 'is-invalid': validationErrors.parameterQuantity }"
                        required>
                  <option value="1">1</option>
                  <option value="2">2</option>
                  <option value="3">3</option>
                </select>
                <div class="invalid-feedback" v-if="validationErrors.parameterQuantity">
                  {{ validationErrors.parameterQuantity }}
                </div>
              </div>
            </div>

            <div class="d-flex justify-content-center pt-3 pb-3">
              <button type="button" class="btn btn-secondary btn-lg btn-block me-2" @click="resetForm">Cancelar</button>
              <button type="submit" class="btn btn-primary btn-lg btn-block">Guardar</button>
            </div>
          </form>
        </div>
        <div class="col-1"></div>
      </div>
    </div>


    <!--<div class="mt-4" v-if="showPreview">
      <div class="card">
        <div class="card-header bg-info text-white">
          <h5 class="mb-0">Vista previa de datos</h5>
        </div>
        <div class="card-body">
          <pre>{{ formData }}</pre>
        </div>
      </div>
    </div>-->
  </div>

</template>

<script>
import { ref, reactive, computed } from 'vue';

export default {
  name: 'CrearBeneficios',
  setup() {
    const formData = reactive({
      benefitName: '',
      minimumTime: 0,
      elegibles: {
        empleado: false,
        admin: false,
        supervisor: false
      },
      benefitType: '',
      fixedAmmount: null,
      percentage: null,
      apiUrl: '',
      parameterQuantity: 1
    });

    const validationErrors = reactive({});
    const showPreview = ref(true);
    const validateForm = () => {
      const errors = {};

      if (!formData.benefitName.trim()) {
        errors.benefitName = 'El nombre del beneficio es requerido';
      }

      if (formData.minimumTime === null || formData.minimumTime < 0) {
        errors.minimumTime = 'El tiempo mínimo debe ser un número positivo';
      }

      if (!formData.elegibles.empleado && !formData.elegibles.admin && !formData.elegibles.supervisor) {
        errors.elegibles = 'Debe seleccionar al menos una opción';
      }

      if (!formData.benefitType) {
        errors.benefitType = 'Debe seleccionar un tipo de beneficio';
      }

      if (formData.benefitType === 'fixedAmmount') {
        if (formData.fixedAmmount === null || formData.fixedAmmount < 0) {
          errors.fixedAmmount = 'El monto debe ser un número positivo';
        }
      } else if (formData.benefitType === 'percentage') {
        if (formData.percentage === null || formData.percentage < 0 || formData.percentage > 100) {
          errors.percentage = 'El porcentaje debe estar entre 0 y 100';
        }
      } else if (formData.benefitType === 'api') {
        const urlPattern = /^(https?:\/\/)?([\da-z.-]+)\.([a-z.]{2,6})([/\w .-]*)*\/?$/;
        if (!formData.apiUrl || !urlPattern.test(formData.apiUrl)) {
          errors.apiUrl = 'Debe ingresar una URL válida';
        }

        if (![1, 2, 3].includes(formData.parameterQuantity)) {
          errors.parameterQuantity = 'Debe seleccionar una cantidad válida de parámetros';
        }
      }

      Object.keys(validationErrors).forEach(key => {
        delete validationErrors[key];
      });

      Object.keys(errors).forEach(key => {
        validationErrors[key] = errors[key];
      });

      return Object.keys(errors).length === 0;
    };


    const submitForm = () => {
      if (validateForm()) {

        const submitData = {
          benefitName: formData.benefitName,
          minimumTime: formData.minimumTime,
          elegibles: Object.keys(formData.elegibles).filter(key => formData.elegibles[key]),
          benefitType: formData.benefitType,
        };

        if (formData.benefitType === 'fixedAmmount') {
          submitData.valor = formData.fixedAmmount;
        } else if (formData.benefitType === 'percentage') {
          submitData.valor = formData.percentage;
        } else if (formData.benefitType === 'api') {
          submitData.apiUrl = formData.apiUrl;
          submitData.parameterQuantity = formData.parameterQuantity;
        }

        // Backend logic
        console.log('Successfully registered benefit');
        alert('Beneficio guardado correctamente'); // This is not an alert
        resetForm();
      } else {
        console.log('Invalid form');
      }
    };

    const resetForm = () => {
      formData.benefitName = '';
      formData.minimumTime = 0;
      formData.elegibles.empleado = false;
      formData.elegibles.admin = false;
      formData.elegibles.supervisor = false;
      formData.benefitType = '';
      formData.fixedAmmount = null;
      formData.percentage = null;
      formData.apiUrl = '';
      formData.parameterQuantity = 1;

      Object.keys(validationErrors).forEach(key => {
        delete validationErrors[key];
      });
    };

    return {
      formData,
      validationErrors,
      showPreview,
      submitForm,
      resetForm
    };
  }
};
</script>

<style scoped>
  .page {

    margin-top: 1rem;
    margin-bottom: 1rem;
    background: white; /*eliminate when done*/
  }

  h1 {
    text-align: center;
    color: #003c63;
    font-weight: bold;
  }

  .mb-3 {
      padding-bottom: 1.25rem;
  }

  .input-type {
    border-radius: 10px;
    padding: 0.75rem;
    background-color: #f2f2f2;
    border: 1px solid #f2f2f2;
    transition: box-shadow 0.2s ease;
    
  }
  ::placeholder {
    color: #6c757d !important;
  }

  input, select, textarea {
    color: #000 !important;
  }

    .input-type:focus {
      outline: none;
      border-color: #aaa;
      box-shadow: 0 0 0 2px rgba(0, 60, 99, 0.15);
    }

  .form-check {
    margin-bottom: 0.25rem;
  }

  .form-check-input:focus {
    outline: none;
    border-color: #aaa;
    box-shadow: 0 0 0 2px rgba(0, 60, 99, 0.15);
  }

  .btn-primary {
    background-color: #003c63;
    border-color: #003c63;
    font-weight: bold;
  }

  .btn-secondary {
    background-color: #6c757d;
    border-color: #6c757d;
    font-weight: bold;
  }
</style>
