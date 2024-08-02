<script setup>

import {onMounted, ref} from "vue";
import axios from "axios";
import {backendUrl} from "../../main.js";
import {toast} from "vue3-toastify";
import {router} from "../../router.js";

let emits = defineEmits(['close'])

let printerTypes = ref([]);
let brand = ref("");
let brandCustom = ref("");
let model = ref("");

let showRequired = ref(false);

onMounted(() => {
  let issueReq = axios.get(backendUrl + "/api/printer/getprintertypevariants", {withCredentials: true}).then(response => {
    printerTypes.value = response.data;
  }).catch(error => {
    console.log(error)
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

  });
});

function submit() {
  if(brand.value === "" || (brand.value === "custom" && brandCustom.value === "") || model.value === "") {
    showRequired.value = true;
    return;
  }
  let printerType = brand.value === 'custom' ? brandCustom.value : brand.value;
  let data = {
    brand: printerType,
    model: model.value
  };

  let req = axios.post(backendUrl + "/api/printer/addprintertype", data, {withCredentials:true}).then(response => {
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
  })
}

function close() {
  printerTypes = ref([]);
  brand = ref("");
  brandCustom = ref("");
  model = ref("");
  showRequired = ref(false);

  emits('close');
}
</script>

<template>
  <div class="fixed top-0 left-0 fixHeight w-screen bg-black bg-opacity-25 z-30 flex items-center justify-center" @click.self="$emit('close')">
    <form v-on:submit.prevent="submit" class="bg-white p-6 rounded-3xl">
      <h1>Add Printer Type</h1>
      <hr class="my-2">
      <p class="text-left">Brand <span v-if="showRequired" class="text-red-500">Required</span></p>
      <select v-model="brand" class="text-green-500 p-1 w-full bg-gray-200 rounded-lg">
        <option v-for="(printer, index) in printerTypes" :value="brand">{{printer}}</option>
        <option value="custom" class="font-bold">New</option>
      </select>
      <template v-if="brand==='custom'">
        <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
          <ul class="flex items-center">
            <li class="mx-1 w-full">
              <input placeholder="New Issue" type="text" v-model="brandCustom" class="w-full text-green-500 focus:outline-none bg-gray-200">
            </li>
          </ul>
        </div>
      </template>
      <hr class="my-2">
      <p class="text-left">Model <span v-if="showRequired" class="text-red-500">Required</span></p>
      <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input placeholder="New Issue" type="text" v-model="model" class="w-full text-green-500 focus:outline-none bg-gray-200">
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