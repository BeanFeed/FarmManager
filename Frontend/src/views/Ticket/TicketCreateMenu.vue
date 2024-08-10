<script setup>

import {router} from "../../router.js";

let emits = defineEmits(['complete'])
let props = defineProps({'printerName':{
  default: null
  }})

import {onMounted, ref} from "vue";
import axios from "axios";
import {backendUrl} from "../../main.js";
import {toast} from "vue3-toastify";
import {useRoute} from "vue-router";

let route = useRoute();

let printerName = ref("");
let issue = ref("");
let issueCustom = ref("");
let printerOptions = ref([]);
let issueTypes = ref([]);
let showRequired = ref(false);

let loading = ref(false);

onMounted(() => {
  if (props.printerName !== null) printerName.value = props.printerName;
  else {
    axios.get(backendUrl + "/api/printer/getprinters", {withCredentials: true}).then(response => {
      printerOptions.value = response.data;
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
  
  axios.get(backendUrl + "/api/ticket/getissuevariants", {withCredentials: true}).then(response => {
    issueTypes.value = response.data;
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
})

function submitTicket() {
  if (issue.value === "" || (issue.value === 'custom' && issueCustom === "") || printerName.value === "") {
    showRequired.value = true;
    return;
  }
  let data = {
    issue: issue.value === 'custom' ? issueCustom.value : issue.value,
    printerName: printerName.value
  }
  issue = ref("");
  issueCustom = ref("");
  loading.value = true;
  axios.post(backendUrl + "/api/ticket/openticket", data, {withCredentials: true}).then(() => {
    printerName = ref("");
    issue = ref("");
    issueCustom = ref("");
    printerOptions = ref([]);
    issueTypes = ref([]);
    showRequired = ref(false);
    loading.value = false;
    
    emits('complete');
  }).catch(error => {
    console.log(error)
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

function cancel() {
  printerName = ref("");
  issue = ref("");
  issueCustom = ref("");
  printerOptions = ref([]);
  issueTypes = ref([]);
  showRequired = ref(false);
  loading.value = false;
  
  emits('complete');
}

</script>

<template>
  <div class="fixed top-0 left-0 fixHeight w-screen bg-black bg-opacity-25 z-30 flex items-center justify-center" @click.self="$emit('complete')">
    <form v-on:submit.prevent="submitTicket" class="bg-white p-6 rounded-3xl">
      <h1>New Ticket</h1>
      <hr class="my-2">
      <div class="flex flex-col">
        <template v-if="props.printerName === null">
          <p class="text-left">Printer <span v-if="showRequired" class="text-red-500">Required</span></p>
          <select v-model="printerName" class="text-green-500 p-1 bg-gray-200 rounded-lg">
            <option v-for="(printer) in printerOptions" :value="printer.name">{{printer.name}}</option>
          </select>

          <hr class="my-2">
        </template>
        <p class="text-left">Issue <span v-if="showRequired" class="text-red-500">Required</span></p>
        <select v-model="issue" class="text-green-500 p-1 bg-gray-200 rounded-lg">
          <option v-for="(issue) in issueTypes" :value="issue">{{issue}}</option>
          <option value="custom">Custom</option>
        </select>
        <template v-if="issue==='custom'">
          <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
            <ul class="flex items-center">
              <li class="mx-1 w-full">
                <input placeholder="New Issue" type="text" v-model="issueCustom" class="w-full text-green-500 focus:outline-none bg-gray-200">
              </li>
            </ul>
          </div>
        </template>
        <hr class="my-2">
        <div class="flex justify-center w-full">
          <button type="submit" class="w-full bg-green-500 hover:bg-green-600 disabled:bg-green-700 enabled:cursor-pointer p-2 rounded-lg mr-1 text-white" :disabled="loading">
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
          <input @click="cancel" class="w-full bg-gray-200 hover:bg-gray-300 cursor-pointer p-2 rounded-lg ml-1" type="button" value="Cancel">
        </div>
        
      </div>

    </form>
  </div>
</template>

<style scoped>

</style>