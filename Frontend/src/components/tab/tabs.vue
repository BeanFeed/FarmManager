<script setup>

import {onMounted, ref} from "vue";
import {useRoute} from "vue-router";
import {router} from "../../router.js";
import {UserStore} from "../../store/UserStore.js";
import {hasPerm} from "../../utils.js";

let route = useRoute();
let userStore = UserStore();

onMounted(() => {
})
</script>

<template>
  <div class="flex items-center justify-center">
    <div class="bg-white rounded-3xl p-2 w-full">
      <div class="bg-gray-200 py-1 rounded-2xl">
        <div class="flex items-center justify-center">
          <div @click="router.push('account')" class="w-full px-2 rounded-3xl cursor-pointer transition-color ease-in-out duration-200 bg-white" :class="(route.name === 'Account' ? 'bg-opacity-100 ' : 'bg-opacity-0') + ' ' + (hasPerm(userStore,'Head Technician') ? 'ml-1' : 'mx-1')">
            Account
          </div>
          <div v-if="hasPerm(userStore,'Owner')" @click="router.push('users')" class="w-full mx-3 px-2 rounded-3xl cursor-pointer transition-color ease-in-out duration-200 bg-white" :class="route.name === 'Users' ? 'bg-opacity-100 ' : 'bg-opacity-0'">
            Users
          </div>
          <div v-if="hasPerm(userStore,'Head Technician')" @click="router.push('options')" class="w-full mr-1 px-2 rounded-3xl cursor-pointer transition-color ease-in-out duration-200 bg-white" :class="route.name === 'Options' ? 'bg-opacity-100 ' : 'bg-opacity-0'">
            Options
          </div>
          <div></div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>