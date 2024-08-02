<script setup>

import Sidebar from "./components/Sidebar/Sidebar.vue";
import {useRoute} from "vue-router";

let route = useRoute();
</script>

<template>
  <div class="w-screen fixHeight">
    <Sidebar v-if="!route.fullPath.startsWith('/login')"/>
    <RouterView/>
  </div>
</template>

<style scoped>

</style>
