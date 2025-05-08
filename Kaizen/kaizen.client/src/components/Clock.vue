<template>
  <div id="clock-container" ref="sketchContainer"></div>
</template>

<script setup>
  import { ref, onMounted, onBeforeUnmount } from 'vue'
  import clockSketch from '../composables/clock.js'

  const sketchContainer = ref(null)
  let p5Instance = null

  onMounted(() => {
    p5Instance = new p5(clockSketch, sketchContainer.value)
  })

  onBeforeUnmount(() => {
    if (p5Instance) p5Instance.remove()
  })
</script>

<style scoped>
  #clock-container {
    width: 400px;
    height: 400px;
    background: transparent;
  }
</style>
