import api from "./AuthService";

export const getAll = () => api.get("/marketplace");
export const sell = (id, buyerName) => api.post(`/marketplace/${id}/sell`, { buyerName });