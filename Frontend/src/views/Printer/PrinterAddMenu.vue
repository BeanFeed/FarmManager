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
let loading = ref(false);

let locationOptions = ref([]);
let printerBrands = ref([]);
let modelOptions = ref([]);
let showRequired = ref(false);

onMounted(() => {
  let locationReq =  axios.get(backendUrl + "/api/ticket/getlocations", {withCredentials: true}).then(response => {
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
  
  let printerBrandReq = axios.get(backendUrl + "/api/printer/getprintertypevariants", {withCredentials: true}).then(response => {
    printerBrands.value = response.data;
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

watch(brand, () => {
  if (brand.value === "" || brand.value === "custom") return;
  
  let printerReq = axios.get(backendUrl + "/api/printer/getprintertypevariants?modelByBrand=" + brand.value, {withCredentials: true}).then(response => {
    modelOptions.value = response.data;
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
    loading.value = false;
    return;
  }
  loading.value = true;
  
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

  let req = axios.post(backendUrl + "/api/printer/addprinter", data, {withCredentials: true}).then(response => {
    close();
  }).catch(error => {
    loading.value = false;
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
  loading.value = false;
  
  emits('close');
}
</script>

<template>
  <div class="fixed top-0 left-0 fixHeight w-screen bg-black bg-opacity-25 z-30 flex items-center justify-center" @click.self="$emit('close')">
    <form v-on:submit.prevent="submit" class="bg-white p-6 rounded-3xl z-40">
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
      <select v-model="brand" class="text-green-500 p-1 bg-gray-200 rounded-lg w-full">
        <option v-for="(brand, index) in printerBrands" :value="brand">{{brand}}</option>
      </select>
      <hr class="my-2">
      <p class="text-left">Model <span v-if="showRequired" class="text-red-500">Required</span></p>
      <select v-model="model" class="text-green-500 p-1 bg-gray-200 rounded-lg w-full">
        <option v-for="(model, index) in modelOptions" :value="model">{{model}}</option>
      </select>
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
        <button class="w-full bg-green-500 text-white p-2 rounded-lg enabled:cursor-pointer hover:bg-green-600 disabled:bg-green-700" type="submit" :disabled="loading">
          <template v-if="!loading">
            Submit
          </template>
          <template v-else>
            <div class="text-center">
              <div role="status">
                <svg aria-hidden="true" class="inline w-5 h-5 text-green-500 animate-spin fill-white" viewBox="0 0 100 101" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <path d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z" fill="currentColor"/>
                  <path d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z" fill="currentFill"/>
                </svg>
                <span class="sr-only">Loading...</span>
              </div>
            </div>
          </template>
        </button>
        <input @click="close" class="w-full bg-gray-200 hover:bg-gray-300 cursor-pointer p-2 rounded-lg mx-1" type="button" value="Cancel">
      </div>
    </form>
  </div>
</template>

<style scoped>

</style>