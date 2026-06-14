import api from "./AuthService";

export const getAll = () => api.get("/transaction");
export const getByAccount = (accountId) => api.get(`/transaction/${accountId}`);
export const reverse = (id) => api.post(`/transaction/${id}/reverse`);