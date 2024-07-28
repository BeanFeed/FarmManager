<script setup>
import {ref} from "vue";
import {backendUrl} from "../../main.js";
import axios from "axios";
import {toast} from "vue3-toastify";
import {router} from "../../router.js";

let emits = defineEmits(['close']);

let locationName = ref("");

let showRequired = ref(false);
function submit() {
  if(locationName.value === "") {
    showRequired.value = true;
    return;
  }
  let req = axios.post(backendUrl + "/v1/ticket/addlocation?Name=" + locationName.value, {}, {withCredentials: true}).then(response => {
    close();
  }).catch(error => {
    toast(error.response.data.length < 30 ? error.response.data : error.body, {
      "type": "error",
      "closeOnClick": true,
      "autoClose": 2000,
      "pauseOnFocusLoss": false,
      "transition": "bounce"
    });
    if (error.response.data === "User not logged in" || error.response.data === "Session Invalid") {
      router.push('/login');
    }
    close();
  });
}

function close() {
  locationName.value = "";
  showRequired.value = false;
  emits('close')
}
</script>

<template>
  <div class="fixed top-0 left-0 fixHeight w-screen bg-black bg-opacity-25 z-30 flex items-center justify-center" @click.self="$emit('close')">
    <form v-on:submit.prevent="" class="bg-white p-6 rounded-3xl">
      <h1>Add Location</h1>
      <hr class="my-2">
      <p class="text-left">Name <span v-if="showRequired" class="text-red-500">Required</span></p>
      <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input placeholder="New Location" type="text" v-model="locationName" class="w-full text-green-500 focus:outline-none bg-gray-200">
          </li>
        </ul>
      </div>
      <hr class="my-2">
      <div class="flex justify-center w-full">
        <input class="w-full bg-green-500 hover:bg-green-600 cursor-pointer p-2 rounded-lg mr-1 text-white" type="submit" value="Submit">
        <input @click="close" class="w-full bg-gray-200 hover:bg-gray-300 cursor-pointer p-2 rounded-lg ml-1" type="button" value="Cancel">
      </div>
    </form>
  </div>
</template>

<style scoped>

</style>