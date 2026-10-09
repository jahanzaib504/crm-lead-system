const { default: axios } = require("axios");

const api = axios.create({
  baseURL: "https://localhost:5544",
  headers: {
    "Content-Type": "application/json",
  },
});


api.interceptors.request.use(
  (config) => {
    // Retrieve token from storage (e.g., localStorage in browser or env variable)
    const token = typeof window !== "undefined" ? localStorage.getItem("token") : null;

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

module.exports = api;