import {defineStore} from "pinia";

export const UserStore = defineStore('user',{
    state: () => {
        return {
            id: null,
            name: null,
            role: null
        }
    },
    getters: {
        getUser() {
            return this.id !== null ? this : null;
        },
        isLoggedIn() {return this.id !== null}
    },
    actions: {
        setUser(data) {
            this.id = data.id;
            this.name = data.name;
            this.role = data.role;
        },
        clearUser() {
            this.id = null;
            this.name = null;
            this.role = null;
        }
    },
    persist: true
});