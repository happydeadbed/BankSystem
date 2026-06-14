import { useEffect, useState } from "react";
import * as LoanService from "../../services/LoanService";

function Loans() {
    const [loans, setLoans] = useState([]);
    const [payments, setPayments] = useState([]);
    const [selectedLoanId, setSelectedLoanId] = useState(null);
    const [form, setForm] = useState({
        customerId: "", accountId: "", originalAmount: "",
        totalMonths: "", collateralDescription: "", collateralValue: ""
    });
    const [msg, setMsg] = useState("");
    const [error, setError] = useState("");

    useEffect(() => { loadLoans(); }, []);

    const loadLoans = async () => {
        try { const res = await LoanService.getAll(); setLoans(res.data); }
        catch { setError("Failed to load loans."); }
    };

    const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

    const clearForm = () => setForm({
        customerId: "", accountId: "", originalAmount: "",
        totalMonths: "", collateralDescription: "", collateralValue: ""
    });

    const handleSubmit = async (e) => {
        e.preventDefault(); setMsg(""); setError("");
        try {
            await LoanService.issue({
                ...form,
                customerId: parseInt(form.customerId),
                accountId: parseInt(form.accountId),
                originalAmount: parseFloat(form.originalAmount),
                totalMonths: parseInt(form.totalMonths),
                collateralValue: parseFloat(form.collateralValue),
            });
            setMsg("Loan issued successfully."); clearForm(); loadLoans();
        } catch { setError("Failed to issue loan."); }
    };

    const handleViewPayments = async (loanId) => {
        setMsg(""); setError("");
        try {
            const res = await LoanService.getPayments(loanId);
            setPayments(res.data); setSelectedLoanId(loanId);
        } catch { setError("Failed to load payments."); }
    };

    const handleMarkPaid = async (paymentId) => {
        setMsg(""); setError("");
        try {
            await LoanService.markPaid(paymentId);
            setMsg("Payment marked as paid.");
            handleViewPayments(selectedLoanId);
        } catch { setError("Failed to mark payment."); }
    };

    const handleCheckMissed = async () => {
        setMsg(""); setError("");
        try {
            await LoanService.checkMissed();
            setMsg("Missed payments and defaults updated.");
            loadLoans();
            if (selectedLoanId) handleViewPayments(selectedLoanId);
        } catch { setError("Failed to run missed check."); }
    };

    return (
        <div>
            <h4>Loans</h4>
            {msg && <div className="alert alert-success">{msg}</div>}
            {error && <div className="alert alert-danger">{error}</div>}

            <form onSubmit={handleSubmit} className="border p-3 rounded mb-4">
                <h5>Issue Loan</h5>
                <div className="row">
                    <div className="col-md-3 mb-2">
                        <label className="form-label">Customer ID</label>
                        <input className="form-control" name="customerId" value={form.customerId} onChange={handleChange} required />
                    </div>
                    <div className="col-md-3 mb-2">
                        <label className="form-label">Account ID</label>
                        <input className="form-control" name="accountId" value={form.accountId} onChange={handleChange} required />
                    </div>
                    <div className="col-md-3 mb-2">
                        <label className="form-label">Loan Amount</label>
                        <input className="form-control" name="originalAmount" value={form.originalAmount} onChange={handleChange} required />
                    </div>
                    <div className="col-md-3 mb-2">
                        <label className="form-label">Total Months</label>
                        <input className="form-control" name="totalMonths" value={form.totalMonths} onChange={handleChange} required />
                    </div>
                    <div className="col-md-6 mb-2">
                        <label className="form-label">Collateral Description</label>
                        <input className="form-control" name="collateralDescription" value={form.collateralDescription} onChange={handleChange} required />
                    </div>
                    <div className="col-md-3 mb-2">
                        <label className="form-label">Collateral Value</label>
                        <input className="form-control" name="collateralValue" value={form.collateralValue} onChange={handleChange} required />
                    </div>
                </div>
                <button type="submit" className="btn btn-primary me-2">Issue Loan</button>
                <button type="button" className="btn btn-outline-danger" onClick={handleCheckMissed}>Run Missed Check</button>
            </form>

            <table className="table table-bordered table-hover mb-4">
                <thead className="table-dark">
                    <tr>
                        <th>ID</th><th>Customer ID</th><th>Account ID</th>
                        <th>Amount</th><th>Monthly</th><th>Months</th>
                        <th>Status</th><th>Collateral</th><th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {loans.map((l) => (
                        <tr key={l.id}>
                            <td>{l.id}</td>
                            <td>{l.customerId}</td>
                            <td>{l.accountId}</td>
                            <td>${l.originalAmount.toFixed(2)}</td>
                            <td>${l.monthlyAmount.toFixed(2)}</td>
                            <td>{l.totalMonths}</td>
                            <td>
                                <span className={`badge ${l.status === "Active" ? "bg-success" : l.status === "Defaulted" ? "bg-danger" : "bg-secondary"}`}>
                                    {l.status}
                                </span>
                            </td>
                            <td>{l.collateralDescription}</td>
                            <td>
                                <button className="btn btn-sm btn-info text-white" onClick={() => handleViewPayments(l.id)}>Payments</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            {selectedLoanId && (
                <div className="border p-3 rounded">
                    <h5>Payment Schedule — Loan #{selectedLoanId}</h5>
                    <table className="table table-sm table-bordered">
                        <thead className="table-secondary">
                            <tr><th>ID</th><th>Due Date</th><th>Amount</th><th>Status</th><th>Actions</th></tr>
                        </thead>
                        <tbody>
                            {payments.map((p) => (
                                <tr key={p.id}>
                                    <td>{p.id}</td>
                                    <td>{new Date(p.dueDate).toLocaleDateString()}</td>
                                    <td>${p.amount.toFixed(2)}</td>
                                    <td>
                                        <span className={`badge ${p.status === "Paid" ? "bg-success" : p.status === "Missed" ? "bg-danger" : "bg-warning text-dark"}`}>
                                            {p.status}
                                        </span>
                                    </td>
                                    <td>
                                        {p.status !== "Paid" && (
                                            <button className="btn btn-sm btn-success" onClick={() => handleMarkPaid(p.id)}>Mark Paid</button>
                                        )}
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
}

export default Loans;