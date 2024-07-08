import {defineStore} from "pinia";

export const TokenStore = defineStore('token',{
    state: () => {
        return {
            access: null,
            ref: null
        }
    },
    getters: {
        getAccess(state) {
            return state.access !== null ? state.access : null;
        },
        getRefresh(state) {
            return state.ref !== null ? state.ref : null;
        }
    },
    actions: {
        setTokens(access, ref) {
            this.access = access;
            this.ref = ref;
        },
        clearTokens() {
            this.access = null;
            this.ref = null;
        }
    },
    persist: true
});