<script setup>

import Tabs from "../../components/tab/tabs.vue";
import {onMounted} from "vue";
import {router} from "../../router.js";
import {UserStore} from "../../store/UserStore.js";
import {useRoute} from "vue-router";
import {hasPerm} from "../../utils.js";

let userStore = UserStore();
let route = useRoute();
onMounted(() => {
  if (userStore.id === null) {
    router.push('/login');
  }
})
</script>

<template>
<div class="fixHeight flex items-center justify-center">
  <div class="w-[75vw]">
    <Tabs v-if="hasPerm(userStore, 'Head Technician')"/>
    <div class="bg-white p-6 min-h-[50vh] rounded-3xl mt-2 overflow-y-auto">
    <RouterView class="h-full"/>
    </div>
  </div>
  
</div>
</template>

<style scoped>

</style>