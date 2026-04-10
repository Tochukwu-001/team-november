import axios from 'axios';

const client = axios.create({
  // Base URL from your Swagger documentation
  baseURL: 'https://team-november.onrender.com/api', 
});

// This "Interceptor" runs before every single request
client.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  
  if (token) {
    // This adds the "ID card" to your request headers
    config.headers.Authorization = `Bearer ${token}`;
  }
  
  return config;
}, (error) => {
  return Promise.reject(error);
});

export default client;