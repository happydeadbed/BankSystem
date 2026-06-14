import { useEffect, useState } from "react";
import * as AccountService from "../../services/AccountService";

function Accounts() {
    const [accounts, setAccounts] = useState([]);
    const [form, setForm] = useState({ customerId: "", accountType: "Savings" });
    const [editForm, setEditForm] = useState({ accountType: "Savings", status: "Active" });
    const [editId, setEditId] = useState(null);
    const [isEdit, setIsEdit] = useState(false);
    const [depositForm, setDepositForm] = useState({ amount: "", description: "" });
    const [withdrawForm, setWithdrawForm] = useState({ amount: "", description: "" });
    const [transferForm, setTransferForm] = useState({ fromAccountId: "", toAccountId: "", amount: "", description: "" });
    const [selectedDepositId, setSelectedDepositId] = useState("");
    const [selectedWithdrawId, setSelectedWithdrawId] = useState("");
    const [msg, setMsg] = useState("");
    const [error, setError] = useState("");

    useEffect(() => { loadAccounts(); }, []);

    const loadAccounts = async () => {
        try { const res = await AccountService.getAll(); setAccounts(res.data); }
        catch { setError("Failed to load accounts."); }
    };

    const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });
    const handleEditChange = (e) => setEditForm({ ...editForm, [e.target.name]: e.target.value });
    const handleDepositChange = (e) => setDepositForm({ ...depositForm, [e.target.name]: e.target.value });
    const handleWithdrawChange = (e) => setWithdrawForm({ ...withdrawForm, [e.target.name]: e.target.value });
    const handleTransferChange = (e) => setTransferForm({ ...transferForm, [e.target.name]: e.target.value });

    const clearForm = () => {
        setForm({ customerId: "", accountType: "Savings" });
        setEditId(null); setIsEdit(false);
    };

    const handleCreate = async (e) => {
        e.preventDefault(); setMsg(""); setError("");
        try {
            await AccountService.create(form);
            setMsg("Account created."); clearForm(); loadAccounts();
        } catch { setError("Failed to create account."); }
    };

    const handleEditClick = (account) => {
        setEditForm({ accountType: account.accountType, status: account.status });
        setEditId(account.id); setIsEdit(true); setMsg(""); setError("");
    };

    const handleUpdate = async (e) => {
        e.preventDefault(); setMsg(""); setError("");
        try {
            await AccountService.update(editId, editForm);
            setMsg("Account updated."); clearForm(); loadAccounts();
        } catch { setError("Failed to update account."); }
    };

    const handleDelete = async (id) => {
        if (!window.confirm("Delete this account?")) return;
        try { await AccountService.remove(id); setMsg("Account deleted."); loadAccounts(); }
        catch (err) { setError(err.response?.data || "Cannot delete account with non-zero balance."); }
    };

    const handleDeposit = async (e) => {
        e.preventDefault(); setMsg(""); setError("");
        try {
            await AccountService.deposit(selectedDepositId, depositForm);
            setMsg("Deposit successful.");
            setDepositForm({ amount: "", description: "" }); loadAccounts();
        } catch { setError("Deposit failed."); }
    };

    const handleWithdraw = async (e) => {
        e.preventDefault(); setMsg(""); setError("");
        try {
            await AccountService.withdraw(selectedWithdrawId, withdrawForm);
            setMsg("Withdrawal successful.");
            setWithdrawForm({ amount: "", description: "" }); loadAccounts();
        } catch { setError("Withdrawal failed. Check balance."); }
    };

    const handleTransfer = async (e) => {
        e.preventDefault(); setMsg(""); setError("");
        try {
            await AccountService.transfer(transferForm);
            setMsg("Transfer successful.");
            setTransferForm({ fromAccountId: "", toAccountId: "", amount: "", description: "" }); loadAccounts();
        } catch { setError("Transfer failed."); }
    };

    return (
        <div>
            <h4>Accounts</h4>
            {msg && <div className="alert alert-success">{msg}</div>}
            {error && <div className="alert alert-danger">{error}</div>}

            {!isEdit && (
                <form onSubmit={handleCreate} className="border p-3 rounded mb-4">
                    <h5>Create Account</h5>
                    <div className="row">
                        <div className="col-md-4 mb-2">
                            <label className="form-label">Customer ID</label>
                            <input className="form-control" name="customerId" value={form.customerId} onChange={handleChange} required />
                        </div>
                        <div className="col-md-4 mb-2">
                            <label className="form-label">Account Type</label>
                            <select className="form-select" name="accountType" value={form.accountType} onChange={handleChange}>
                                <option>Savings</option>
                                <option>Checking</option>
                            </select>
                        </div>
                    </div>
                    <button type="submit" className="btn btn-primary">Create</button>
                </form>
            )}

            {isEdit && (
                <form onSubmit={handleUpdate} className="border p-3 rounded mb-4">
                    <h5>Edit Account</h5>
                    <div className="row">
                        <div className="col-md-4 mb-2">
                            <label className="form-label">Account Type</label>
                            <select className="form-select" name="accountType" value={editForm.accountType} onChange={handleEditChange}>
                                <option>Savings</option>
                                <option>Checking</option>
                            </select>
                        </div>
                        <div className="col-md-4 mb-2">
                            <label className="form-label">Status</label>
                            <select className="form-select" name="status" value={editForm.status} onChange={handleEditChange}>
                                <option>Active</option>
                                <option>Inactive</option>
                                <option>Frozen</option>
                            </select>
                        </div>
                    </div>
                    <button type="submit" className="btn btn-primary me-2">Save</button>
                    <button type="button" className="btn btn-secondary" onClick={clearForm}>Cancel</button>
                </form>
            )}

            <form onSubmit={handleDeposit} className="border p-3 rounded mb-4">
                <h5>Deposit</h5>
                <div className="row">
                    <div className="col-md-3 mb-2">
                        <label className="form-label">Account ID</label>
                        <input className="form-control" value={selectedDepositId} onChange={(e) => setSelectedDepositId(e.target.value)} required />
                    </div>
                    <div className="col-md-3 mb-2">
                        <label className="form-label">Amount</label>
                        <input className="form-control" name="amount" value={depositForm.amount} onChange={handleDepositChange} required />
                    </div>
                    <div className="col-md-4 mb-2">
                        <label className="form-label">Description</label>
                        <input className="form-control" name="description" value={depositForm.description} onChange={handleDepositChange} />
                    </div>
                </div>
                <button type="submit" className="btn btn-success">Deposit</button>
            </form>

            <form onSubmit={handleWithdraw} className="border p-3 rounded mb-4">
                <h5>Withdraw</h5>
                <div className="row">
                    <div className="col-md-3 mb-2">
                        <label className="form-label">Account ID</label>
                        <input className="form-control" value={selectedWithdrawId} onChange={(e) => setSelectedWithdrawId(e.target.value)} required />
                    </div>
                    <div className="col-md-3 mb-2">
                        <label className="form-label">Amount</label>
                        <input className="form-control" name="amount" value={withdrawForm.amount} onChange={handleWithdrawChange} required />
                    </div>
                    <div className="col-md-4 mb-2">
                        <label className="form-label">Description</label>
                        <input className="form-control" name="description" value={withdrawForm.description} onChange={handleWithdrawChange} />
                    </div>
                </div>
                <button type="submit" className="btn btn-warning">Withdraw</button>
            </form>

            <form onSubmit={handleTransfer} className="border p-3 rounded mb-4">
                <h5>Transfer</h5>
                <div className="row">
                    <div className="col-md-3 mb-2">
                        <label className="form-label">From Account ID</label>
                        <input className="form-control" name="fromAccountId" value={transferForm.fromAccountId} onChange={handleTransferChange} required />
                    </div>
                    <div className="col-md-3 mb-2">
                        <label className="form-label">To Account ID</label>
                        <input className="form-control" name="toAccountId" value={transferForm.toAccountId} onChange={handleTransferChange} required />
                    </div>
                    <div className="col-md-2 mb-2">
                        <label className="form-label">Amount</label>
                        <input className="form-control" name="amount" value={transferForm.amount} onChange={handleTransferChange} required />
                    </div>
                    <div className="col-md-3 mb-2">
                        <label className="form-label">Description</label>
                        <input className="form-control" name="description" value={transferForm.description} onChange={handleTransferChange} />
                    </div>
                </div>
                <button type="submit" className="btn btn-info text-white">Transfer</button>
            </form>

            <table className="table table-bordered table-hover">
                <thead className="table-dark">
                    <tr>
                        <th>ID</th><th>Account #</th><th>Type</th>
                        <th>Balance</th><th>Status</th><th>Customer ID</th><th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {accounts.map((a) => (
                        <tr key={a.id}>
                            <td>{a.id}</td>
                            <td>{a.accountNumber}</td>
                            <td>{a.accountType}</td>
                            <td>${a.balance.toFixed(2)}</td>
                            <td>{a.status}</td>
                            <td>{a.customerId}</td>
                            <td>
                                <button className="btn btn-sm btn-warning me-2" onClick={() => handleEditClick(a)}>Edit</button>
                                <button className="btn btn-sm btn-danger" onClick={() => handleDelete(a.id)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default Accounts;