<template>
  <div class="p-4">
    <div v-if="visible" class="modal fade show d-block" tabindex="-1">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ title }}</h5>
          </div>
          <div class="modal-body">
            <div v-html="text"></div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="closeModal">Cerrar</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    title: String,
    text: String,
    visible: Boolean
  },
  methods: {
    closeModal() {
      this.$emit('close')
    }
  }
}
</script>

<style scoped>
  .modal {
    background-color: rgba(0,0,0,0.5);
  }

  .modal-content {
    border-radius: 20px;
  }

  .modal-header {
    text-align: center;
    justify-content: center;
    background-color: #003c63;
    color: white;
    border-bottom: none;
    border-top-left-radius: 20px;
    border-top-right-radius: 20px;
  }

  .modal-title {
    font-weight: bold;
  }

  .modal-body {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    text-align: center ;
    color: black;
  }

  .modal-footer {
    padding-top: 0;
    padding-bottom: 1rem;
    border-top: none;
    justify-content: center;
  }

  .btn-secondary {
    background-color: #003c63;
    border-color: #003c63;
    border-radius: 10px;
    font-weight: bold;
    color:white;
  }

  .btn-close {
    filter: invert(1);
  }
</style>
