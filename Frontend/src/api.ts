import axios from "axios";
import { useUser } from "./store/userStore";

const api = axios.create({
  baseURL: "http://localhost:5220",
  headers: {
    "Content-Type": "application/json",
  },
});


api.interceptors.request.use(
  (config) => {
    // Retrieve token from storage (e.g., localStorage in browser or env variable)
    const token = typeof window !== "undefined" ? localStorage.getItem("token") : null;
    const expiresAt = typeof window !== "undefined" ? localStorage.getItem("expiresAt") : null;
    console.log(expiresAt)
    // if(expiresAt && Date.now() >= Date.parse(expiresAt) || !expiresAt){ // If current time is greater than expires at
    //   localStorage.removeItem("token");
    //   localStorage.removeItem("expiresAt");
    //   useUser.setState({user: null}); // Discard the user
    // }
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default api;