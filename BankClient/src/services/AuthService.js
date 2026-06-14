import axios from "axios";

const api = axios.create({
    baseURL: "https://localhost:7267/api",
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem("token");
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
});

export const login = (data) => api.post("/auth/login", data);
export const logout = () => localStorage.removeItem("token");

export default api;