<script setup>

import {onMounted, ref} from "vue";
import axios from "axios";
import {backendUrl} from "../../main.js";
import {toast} from "vue3-toastify";
import {router} from "../../router.js";

let emits = defineEmits(['close'])

let issueTypes = ref([]);
let issue = ref("");
let issueCustom = ref("");
let repair = ref("");

let showRequired = ref(false);

onMounted(() => {
  let issueReq = axios.get(backendUrl + "/v1/ticket/getissuevariants", {withCredentials: true}).then(response => {
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
});

function submit() {
  if(issue.value === "" || (issue.value === "custom" && issueCustom.value === "") || repair.value === "") {
    showRequired.value = true;
    return;
  }
  let issueType = issue.value === 'custom' ? issueCustom.value : issue.value;
  let data = {
    issue: issueType,
    repair: repair.value
  };

  let req = axios.post(backendUrl + "/v1/ticket/addissuetype", data, {withCredentials:true}).then(response => {
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
  issueTypes = ref([]);
  issue = ref("");
  issueCustom = ref("");
  repair = ref("");
  showRequired = ref(false);

  emits('close');
}
</script>

<template>
  <div class="fixed top-0 left-0 fixHeight w-screen bg-black bg-opacity-25 z-30 flex items-center justify-center">
    <div class="bg-white p-6 rounded-3xl">
      <h1>Add Issue Type</h1>
      <hr class="my-2">
      <p class="text-left">Issue <span v-if="showRequired" class="text-red-500">Required</span></p>
      <select v-model="issue" class="text-green-500 p-1 w-full bg-gray-200 rounded-lg">
        <option v-for="(issue, index) in issueTypes" :value="issue">{{issue}}</option>
        <option value="custom" class="font-bold">New</option>
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
      <p class="text-left">Repair <span v-if="showRequired" class="text-red-500">Required</span></p>
      <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input placeholder="New Issue" type="text" v-model="repair" class="w-full text-green-500 focus:outline-none bg-gray-200">
          </li>
        </ul>
      </div>
      <hr class="my-2">
      <div class="flex justify-center w-full">
        <div @click="submit" class="w-full bg-green-500 hover:bg-green-600 cursor-pointer p-2 rounded-lg mr-1">
          <p class="text-white">Submit</p>
        </div>
        <div @click="close" class="w-full bg-gray-200 hover:bg-gray-300 cursor-pointer p-2 rounded-lg ml-1">
          <p>Cancel</p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>