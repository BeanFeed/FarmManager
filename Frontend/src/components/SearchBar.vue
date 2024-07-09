<script setup>
import {ref} from "vue";

const emit = defineEmits(['update'])

let value = ref();
function update() {
  emit('update', value);
}

function checkEmpty() {
  console.log(value.value.length);
  if (value.value.length == 0) {
    emit('update', value);
  }
}
</script>

<template>
<div class="max-w-lg w-full bg-white p-3 rounded-3xl">
  <ul class="flex items-center">
    <li class="mx-1">
      <i class="bi bi-search text-green-500"></i>
    </li>
    <li class="mx-1 w-full">
      <input v-model="value" @input="update" class="w-full bg-white text-green-500 focus:outline-none" type="text">
    </li>
  </ul>
  
</div>
</template>

<style scoped>

</style>