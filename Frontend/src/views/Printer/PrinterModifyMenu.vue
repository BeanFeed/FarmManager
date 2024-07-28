<script setup>
import {onMounted, ref, watch} from "vue";
import axios from "axios";
import {backendUrl} from "../../main.js";
import {toast} from "vue3-toastify";
import {router} from "../../router.js";
import {useRoute} from "vue-router";

let props = defineProps(['printer']);
let emits = defineEmits(['close']);
let route = useRoute();

let serialNumber = ref("");
let brand = ref("");
let model = ref("");
let locationName = ref("");
let purchaseDate = ref("");

let locationOptions = ref([])


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
  let data = {
    name: props.printer.name
  }
  
  if (serialNumber.value !== "") data.serialNumber = serialNumber.value;
  if (brand.value !== "") data.brand = brand.value;
  if (model.value !== "") data.model = model.value;
  if (locationName.value !== "") data.locationName = locationName.value;
  if (purchaseDate.value !== "") data.purchaseDate = purchaseDate.value;
  
  if (data.length === 1) {
    close();
    return;
  }
  
  let req = axios.post(backendUrl + "/v1/printer/modifyprinter", data, {withCredentials: true}).then(response => {
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
  serialNumber = ref("");
  brand = ref("");
  model = ref("");
  locationName = ref("");
  purchaseDate = ref("");
  
  emits('close');
}

function deletePrinter() {
  if (!confirm("Are you sure you want to delete " + props.printer.name + "?")) {
    close();
    return;
  }
  
  let req = axios.delete(backendUrl + "/v1/printer/removeprinter?name=" + props.printer.name, {withCredentials: true}).then(response => {
    close();
  }).catch(error => {
    toast(error.response.data.length < 30 ? error.response.data : error.message, {
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
</script>

<template>
  <div class="fixed top-0 left-0 fixHeight w-screen bg-black bg-opacity-25 z-30 flex items-center justify-center" @click.self="$emit('close')">
    <form v-on:submit.prevent="submit" class="bg-white p-6 rounded-3xl">
      <h1>Modify Printer</h1>
      <hr class="my-2">
      <p class="text-left">Printer: {{printer.name}}</p>
      <hr class="my-2">
      <p class="text-left">Serial Number</p>
      <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input type="text" v-model="serialNumber" class="w-full text-green-500 focus:outline-none bg-gray-200">
          </li>
        </ul>
      </div>
      <hr class="my-2">
      <p class="text-left">Brand</p>
      <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input type="text" v-model="brand" class="w-full text-green-500 focus:outline-none bg-gray-200">
          </li>
        </ul>
      </div>
      <hr class="my-2">
      <p class="text-left">Model</p>
      <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input type="text" v-model="model" class="w-full text-green-500 focus:outline-none bg-gray-200">
          </li>
        </ul>
      </div>
      <hr class="my-2">
      <p class="text-left">Location</p>
      <select v-model="locationName" class="text-green-500 p-1 bg-gray-200 rounded-lg w-full">
        <option value=""></option>
        <option v-for="(location, index) in locationOptions" :value="location.name">{{location.name}}</option>
      </select>
      <hr class="my-2">
      <p class="text-left">Purchase Date</p>
      <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input type="date" v-model="purchaseDate" class="w-full text-green-500 focus:outline-none bg-gray-200">
          </li>
        </ul>
      </div>
      <hr class="my-2">
      <div class="flex justify-center">
        <input class="w-full bg-green-500 hover:bg-green-600 cursor-pointer p-2 rounded-lg mr-1 text-white" type="submit" value="Submit">
        <input @click="close" class="w-full bg-gray-200 hover:bg-gray-300 cursor-pointer p-2 rounded-lg mx-1" type="button" value="Cancel">
        <input @click="deletePrinter" class="w-full bg-red-500 hover:bg-red-600 cursor-pointer p-2 rounded-lg ml-1 text-white" type="button" value="Delete">
      </div>
    </form>
  </div>
</template>

<style scoped>

</style>