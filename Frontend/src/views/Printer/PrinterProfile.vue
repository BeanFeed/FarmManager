<script setup>

import {useRoute} from "vue-router";
import {onMounted, ref} from "vue";
import axios from "axios";
import {backendUrl} from "../../main.js";
import {toast} from "vue3-toastify";
import {UserStore} from "../../store/UserStore.js";
import {router} from "../../router.js";
import {useBrowserLocation} from "@vueuse/core";
import QrcodeVue from "qrcode.vue";
import {hasPerm} from "../../utils.js";
import TicketCreateMenu from "../Ticket/TicketCreateMenu.vue";

let route = useRoute();
let location = useBrowserLocation();
let printerName = route.params.printerName;
let userStore = UserStore();
let printer = ref({
  name: "",
  serialNumber: "",
  brand: "",
  model: "",
  location: {
    name: ""
  },
  purchaseDate: "0"
});

let newTicketOpen = ref(false);

let tickets = ref([]);
onMounted(() => {
  let loadingToast = toast.loading("Loading printer...", {
    transition: "bounce",
    closeOnClick: false,
    pauseOnHover: false
  });
  axios.get(backendUrl + "/api/printer/getprinter?name=" + printerName, {withCredentials: true}).then(response => {
    toast.update(loadingToast, {"closeOnClick": true, render: "Loaded printers", type: "success", isLoading: false, autoClose: 2000});
    printer.value = response.data;
  }).catch(error => {
    toast.update(loadingToast, {"closeOnClick": true, render: error.response.data.length < 30 ? error.response.data : "Error loading printers. Try refreshing.", type: "error", isLoading: false, autoClose: 2000});
    if (error.response.data === "User not logged in" || error.response.data === "Session Invalid") {
      router.push('/login');
    }
  })
  if (hasPerm(userStore, "Technician")) {
    let ticketLoadingToast = toast.loading("Loading tickets...", {
      transition: "bounce",
      closeOnClick: true,
      autoClose: 2000,
      pauseOnHover: false
    });
    axios.get(backendUrl + "/api/ticket/gettickets?printerName=" + printerName, {withCredentials: true}).then(response => {
      
      toast.update(ticketLoadingToast, {"closeOnClick": true, render: "Loaded tickets", type: "success", isLoading: false, autoClose: 2000});
      tickets.value = response.data;
    }).catch(error => {
      toast.update(ticketLoadingToast, {"closeOnClick": true, render: error.response.data.length < 30 ? error.response.data : "Error loading tickets. Try refreshing.", type: "error", isLoading: false, autoClose: 2000});
      if (error.response.data === "User not logged in" || error.response.data === "Session Invalid") {
        router.push('/login');
      }
    });
  }
  
})

const qrcode = ref(null)
const downloadQrCode = () => {
  let canvasImage = qrcode.value.getElementsByTagName('canvas')[0].toDataURL('image/png');
  let xhr = new XMLHttpRequest();
  xhr.responseType = 'blob';
  xhr.onload = function () {
    let a = document.createElement('a');
    a.href = window.URL.createObjectURL(xhr.response);
    a.download = 'qrcode.png';
    a.style.display = 'none';
    document.body.appendChild(a);
    a.click();
    a.remove();
  };
  xhr.open('GET', canvasImage);
  xhr.send();
}

function refreshTickets() {
  if (!hasPerm(userStore,'Technician')) return;
  axios.get(backendUrl + "/api/ticket/gettickets?printerName=" + printerName, {withCredentials: true}).then(response => {
    
    tickets.value = response.data;
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
  });
}
</script>

<template>
<div class="flex items-center justify-center fixHeight p-2">
  <div class="bg-white rounded-3xl p-6 overflow-auto ml:w-[50vw] h-[60vh]">
    <div class="w-full h-full overflow-auto">
      <h1>{{printer.name}}</h1>
      <div class="overflow-auto">
        <table
            class="min-w-full text-left text-surface ">
          <thead
              class="border-b border-gray-200 ">
          <tr>
            <th scope="col" class="px-6 py-4">Serial Number</th>
            <th scope="col" class="px-6 py-4">Brand</th>
            <th scope="col" class="px-6 py-4">Model</th>
            <th scope="col" class="px-6 py-4">Location</th>
            <th scope="col" class="px-6 py-4">Purchase Date</th>
          </tr>
          </thead>
          <tbody>
          <tr class="border-b border-gray-200">
            <td class="whitespace-nowrap px-6 py-4">{{ printer.serialNumber }}</td>
            <td class="whitespace-nowrap px-6 py-4">{{ printer.brand }}</td>
            <td class="whitespace-nowrap px-6 py-4">{{ printer.model }}</td>
            <td class="whitespace-nowrap px-6 py-4">{{ printer.location.name }}</td>
            <td class="whitespace-nowrap px-6 py-4">{{ new Date(printer.purchaseDate).toLocaleDateString()}}</td>
          </tr>

          </tbody>
        </table>
      </div>

      <div class="flex items-center justify-center mt-8">
        <div class="m-10">
          <div @click="newTicketOpen = true" class="bg-green-500 p-4 rounded-3xl text-xl text-white hover:bg-green-600 cursor-pointer">Create ticket</div>
        </div>
        <div class="m-10">
          <p class="text-lg">Profile Code</p>
          <div @click="downloadQrCode" ref="qrcode" class="cursor-pointer" title="Download">
            <QrcodeVue :value="location.href" level="L" render-as="canvas" />
          </div>

        </div>
      </div>
      <template v-if="hasPerm(userStore,'Technician')">
        <h3 class="text-lg">Ticket History</h3>
        <div class="overflow-x-auto">
          <table
              class="min-w-full text-left text-surface ">
            <thead
                class="border-b border-gray-200 ">
            <tr>
              <th scope="col" class="px-6 py-4">Date Opened</th>
              <th scope="col" class="px-6 py-4">Status</th>
              <th scope="col" class="px-6 py-4">Issue</th>
              <th scope="col" class="px-6 py-4">Repair</th>
              <th scope="col" class="px-6 py-4">Opened By</th>
              <th scope="col" class="px-6 py-4">Technician</th>
            </tr>
            </thead>
            <tbody>
            <tr v-for="(ticket, index) in tickets" class="border-b border-gray-200">
              <td class="whitespace-nowrap px-6 py-4">{{ new Date(ticket.dateOpened).toLocaleString() }}</td>
              <td :title="new Date(ticket.dateClosed).toLocaleString()" class="whitespace-nowrap px-6 py-4">{{ ticket.dateClosed === null ? "Open" : "Closed" }}</td>
              <td class="whitespace-nowrap px-6 py-4">{{ ticket.issue }}</td>
              <td class="whitespace-nowrap px-6 py-4">{{ ticket.repair === null ? "Pending" : ticket.repair }}</td>
              <td class="whitespace-nowrap px-6 py-4">{{ ticket.openedBy }}</td>
              <td class="whitespace-nowrap px-6 py-4">{{ ticket.technician === null ? "Pending" : ticket.technician }}</td>
            </tr>

            </tbody>
          </table>
        </div>
      </template>
    </div>
    
  </div>
  
  <TicketCreateMenu :printerName="printer.name" v-if="newTicketOpen" @complete="newTicketOpen = false; refreshTickets();" />
</div>
</template>

<style scoped>

</style>