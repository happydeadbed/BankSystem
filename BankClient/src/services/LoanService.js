import api from "./AuthService";

export const getAll = () => api.get("/loan");
export const issue = (data) => api.post("/loan", data);
export const getPayments = (loanId) => api.get(`/loan/${loanId}/payments`);
export const markPaid = (paymentId) => api.post(`/loan/payment/${paymentId}/pay`);
export const checkMissed = () => api.post("/loan/check-missed");