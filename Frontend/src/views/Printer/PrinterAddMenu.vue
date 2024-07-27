<script setup>
import {onMounted, ref, watch} from "vue";
import axios from "axios";
import {backendUrl} from "../../main.js";
import {toast} from "vue3-toastify";
import {router} from "../../router.js";
import {useRoute} from "vue-router";

let emits = defineEmits(['close']);
let route = useRoute();

let name = ref("");
let serialNumber = ref("");
let brand = ref("");
let model = ref("");
let locationName = ref("");
let purchaseDate = ref("");

let locationOptions = ref([])
let showRequired = ref(false);

onMounted(() => {
  let locationReq =  axios.get(backendUrl + "/v1/ticket/getlocations", {withCredentials: true}).then(response => {
    locationOptions.value = response.data;
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
  });
})

function submit() {
  
  if (name.value === "" || serialNumber.value === "" || brand.value === "" || model.value === "" || locationName.value === "" || purchaseDate.value === "") {
    showRequired.value = true;
    return;
  }
  
  let data = {
    name: name.value,
    serialNumber: serialNumber.value,
    brand: brand.value, 
    model: model.value,
    locationName: locationName.value,
    purchaseDate: purchaseDate.value
  }

  if (data.length === 1) {
    close();
    return;
  }

  let req = axios.post(backendUrl + "/v1/printer/addprinter", data, {withCredentials: true}).then(response => {
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
  });
}

function close() {
  name = ref("");
  serialNumber = ref("");
  brand = ref("");
  model = ref("");
  locationName = ref("");
  locationOptions = ref([]);
  purchaseDate = ref("");
  showRequired = ref(false);
  
  emits('close');
}
</script>

<template>
  <div class="fixed top-0 left-0 fixHeight w-screen bg-black bg-opacity-25 z-30 flex items-center justify-center" @click.self="$emit('close')">
    <div class="bg-white p-6 rounded-3xl z-40">
      <h1>Add Printer</h1>
      <hr class="my-2">
      <p class="text-left">Name <span v-if="showRequired" class="text-red-500">Required</span></p>
      <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input type="text" v-model="name" class="w-full text-green-500 focus:outline-none bg-gray-200">
          </li>
        </ul>
      </div>
      <hr class="my-2">
      <p class="text-left">Serial Number <span v-if="showRequired" class="text-red-500">Required</span></p>
      <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input type="text" v-model="serialNumber" class="w-full text-green-500 focus:outline-none bg-gray-200">
          </li>
        </ul>
      </div>
      <hr class="my-2">
      <p class="text-left">Brand <span v-if="showRequired" class="text-red-500">Required</span></p>
      <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input type="text" v-model="brand" class="w-full text-green-500 focus:outline-none bg-gray-200">
          </li>
        </ul>
      </div>
      <hr class="my-2">
      <p class="text-left">Model <span v-if="showRequired" class="text-red-500">Required</span></p>
      <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input type="text" v-model="model" class="w-full text-green-500 focus:outline-none bg-gray-200">
          </li>
        </ul>
      </div>
      <hr class="my-2">
      <p class="text-left">Location <span v-if="showRequired" class="text-red-500">Required</span></p>
      <select v-model="locationName" class="text-green-500 p-1 bg-gray-200 rounded-lg w-full">
        <option v-for="(location, index) in locationOptions" :value="location.name">{{location.name}}</option>
      </select>
      <hr class="my-2">
      <p class="text-left">Purchase Date <span v-if="showRequired" class="text-red-500">Required</span></p>
      <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input type="date" v-model="purchaseDate" class="w-full text-green-500 focus:outline-none bg-gray-200">
          </li>
        </ul>
      </div>
      <hr class="my-2">
      <div class="flex justify-center">
        <div @click="submit" class="w-full bg-green-500 hover:bg-green-600 cursor-pointer p-2 rounded-lg mr-1">
          <p class="text-white">Submit</p>
        </div>
        <div @click="close" class="w-full bg-gray-200 hover:bg-gray-300 cursor-pointer p-2 rounded-lg mx-1">
          <p>Cancel</p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>