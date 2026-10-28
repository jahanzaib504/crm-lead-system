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
    if (typeof window === "undefined") {
      return config;
    }

    const token = localStorage.getItem("token");
    const expiresAt = localStorage.getItem("expiresAt");

    if (expiresAt) {
      const expirationTime = Date.parse(expiresAt);

      // Check if expiration date is valid AND current time has passed it
      if (!isNaN(expirationTime) && Date.now() >= expirationTime) {
        // Clear storage
        console.log("Token expired");
        localStorage.removeItem("token");
        localStorage.removeItem("expiresAt");

        // Clear Zustand state
        useUser.setState({ user: null });
        
        // Throw error to prevent sending an unauthenticated request to backend
        return Promise.reject(new Error("Token expired. Please log in again."));
      }
    }

    // Attach token if valid
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