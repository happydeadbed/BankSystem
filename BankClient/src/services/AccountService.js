import api from "./AuthService";

export const getAll = () => api.get("/bankaccount");
export const create = (data) => api.post("/bankaccount", data);
export const update = (id, data) => api.put(`/bankaccount/${id}`, data);
export const remove = (id) => api.delete(`/bankaccount/${id}`);
export const deposit = (id, data) => api.post(`/bankaccount/${id}/deposit`, data);
export const withdraw = (id, data) => api.post(`/bankaccount/${id}/withdraw`, data);
export const transfer = (data) => api.post("/bankaccount/transfer", data);