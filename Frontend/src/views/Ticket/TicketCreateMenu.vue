<script setup>

let emits = defineEmits(['complete'])
let props = defineProps({'printerName':{
  default: null
  }})

import {onMounted, ref} from "vue";
import axios from "axios";
import {backendUrl} from "../../main.js";
import {toast} from "vue3-toastify";
import {TokenStore} from "../../store/TokenStore.js";

let tokenStore = TokenStore();

let printerName = ref("");
let issue = ref("");
let issueCustom = ref("");
let printerOptions = ref([]);
let issueTypes = ref([]);
let showRequired = ref(false);

onMounted(() => {
  if (props.printerName !== null) printerName.value = props.printerName;
  else {
    let printerReq = axios.get(backendUrl + "/v1/printer/getprinters", {headers: {
        Authorization: tokenStore.getAccess + "," + tokenStore.getRefresh
      }}).then(response => {
      //console.log(response.headers)
      tokenStore.setTokens(response.headers["authorization"].split(',')[0],response.headers["authorization"].split(',')[1]);
      printerOptions.value = response.data;
    }).catch(error => {
      toast(error.response.data.length < 30 ? error.response.data : error.body, {
        "type": "error",
        "closeOnClick": true,
        "autoClose": 2000,
        "pauseOnFocusLoss": false,
        "transition": "bounce"
      });
    });
  }
  
  let issueReq = axios.get(backendUrl + "/v1/ticket/getissuetypes", {headers: {
      Authorization: tokenStore.getAccess + "," + tokenStore.getRefresh
    }}).then(response => {
    //console.log(response.headers)
    tokenStore.setTokens(response.headers["authorization"].split(',')[0],response.headers["authorization"].split(',')[1]);
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
  let req = axios.post(backendUrl + "/v1/ticket/openticket", data, {headers: {
      Authorization: tokenStore.getAccess + "," + tokenStore.getRefresh
    }}).then(response => {
    tokenStore.setTokens(response.headers["authorization"].split(',')[0],response.headers["authorization"].split(',')[1]);

    printerName = ref("");
    issue = ref("");
    issueCustom = ref("");
    printerOptions = ref([]);
    issueTypes = ref([]);
    showRequired = ref(false);

    emits('complete');
  }).catch(error => {
    console.log(error)
    toast(error.response.data.length < 30 ? error.response.data : error.body, {
      "type": "error",
      "closeOnClick": true,
      "autoClose": 2000,
      "pauseOnFocusLoss": false,
      "transition": "bounce"
    });

  });
}

function cancel() {
  printerName = ref("");
  issue = ref("");
  issueCustom = ref("");
  printerOptions = ref([]);
  issueTypes = ref([]);
  showRequired = ref(false);

  emits('complete');
}

</script>

<template>
  <div class="fixed top-0 left-0 fixHeight w-screen bg-black bg-opacity-25 z-30 flex items-center justify-center">
    <div class="bg-white p-6 rounded-3xl">
      <h1>New Ticket</h1>
      <hr class="my-2">
      <div class="flex flex-col">
        <template v-if="props.printerName === null">
          <p class="text-left">Printer <span v-if="showRequired" class="text-red-500">Required</span></p>
          <select v-model="printerName" class="text-green-500 p-1 bg-gray-200 rounded-lg">
            <option v-for="(printer, index) in printerOptions" :value="printer.name">{{printer.name}}</option>
          </select>

          <hr class="my-2">
        </template>
        <p class="text-left">Issue <span v-if="showRequired" class="text-red-500">Required</span></p>
        <select v-model="issue" class="text-green-500 p-1 bg-gray-200 rounded-lg">
          <option v-for="(issue, index) in issueTypes" :value="issue.issue">{{issue.issue}}</option>
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
          <div @click="submitTicket" class="bg-green-500 hover:bg-green-600 cursor-pointer p-2 rounded-lg mr-1">
            <p class="text-white">Submit</p>
          </div>
          <div @click="cancel" class="bg-gray-200 hover:bg-gray-300 cursor-pointer p-2 rounded-lg ml-1">
            <p>Cancel</p>
          </div>
        </div>
        
      </div>

    </div>
  </div>
</template>

<style scoped>

</style>