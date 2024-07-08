<script setup>
import {onMounted, ref} from "vue";

let props = defineProps(['color','text-color', 'sensitive'])
const emit = defineEmits(['update'])

onMounted(() => {
  console.log(props.textColor)
})

let value = ref();
function update() {
  emit('update', value);
}
</script>

<template>
  <div class="max-w-4xl w-full bg-white p-3 rounded-3xl" :class="color">
    <ul class="flex items-center">
      <li class="mx-1 w-full">
        <input v-model="value" @change="update" class="w-full text-green-500 focus:outline-none" :class="props.color + ' ' + props.textColor !== undefined ? textColor : 'bg-white'" :type="sensitive === 'true' ? 'password' : 'text'">
      </li>
    </ul>
  </div>
</template>

<style scoped>

</style>