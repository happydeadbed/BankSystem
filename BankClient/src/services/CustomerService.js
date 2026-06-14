import api from "./AuthService";

export const getAll = () => api.get("/customer");
export const create = (data) => api.post("/customer", data);
export const update = (id, data) => api.put(`/customer/${id}`, data);
export const remove = (id) => api.delete(`/customer/${id}`);