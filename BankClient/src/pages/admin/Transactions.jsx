import { useEffect, useState } from "react";
import * as TransactionService from "../../services/TransactionService";

function Transactions() {
    const [transactions, setTransactions] = useState([]);
    const [filterAccountId, setFilterAccountId] = useState("");
    const [msg, setMsg] = useState("");
    const [error, setError] = useState("");

    useEffect(() => { loadAll(); }, []);

    const loadAll = async () => {
        try { const res = await TransactionService.getAll(); setTransactions(res.data); }
        catch { setError("Failed to load transactions."); }
    };

    const handleFilter = async (e) => {
        e.preventDefault(); setMsg(""); setError("");
        try { const res = await TransactionService.getByAccount(filterAccountId); setTransactions(res.data); }
        catch { setError("Failed to filter transactions."); }
    };

    const handleReverse = async (id) => {
        if (!window.confirm("Reverse this transaction?")) return;
        setMsg(""); setError("");
        try { await TransactionService.reverse(id); setMsg("Transaction reversed."); loadAll(); }
        catch { setError("Failed to reverse transaction."); }
    };

    return (
        <div>
            <h4>Transactions</h4>
            {msg && <div className="alert alert-success">{msg}</div>}
            {error && <div className="alert alert-danger">{error}</div>}

            <form onSubmit={handleFilter} className="d-flex gap-2 mb-3">
                <input
                    className="form-control" style={{ maxWidth: "200px" }}
                    placeholder="Filter by Account ID"
                    value={filterAccountId} onChange={(e) => setFilterAccountId(e.target.value)}
                />
                <button type="submit" className="btn btn-secondary">Filter</button>
                <button type="button" className="btn btn-outline-secondary" onClick={() => { setFilterAccountId(""); loadAll(); }}>Clear</button>
            </form>

            <table className="table table-bordered table-hover">
                <thead className="table-dark">
                    <tr>
                        <th>ID</th><th>Type</th><th>Amount</th><th>Description</th>
                        <th>From</th><th>To</th><th>Date</th><th>Reversed</th><th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {transactions.map((t) => (
                        <tr key={t.id}>
                            <td>{t.id}</td>
                            <td>{t.transactionType}</td>
                            <td>${t.amount.toFixed(2)}</td>
                            <td>{t.description}</td>
                            <td>{t.fromAccountId ?? "-"}</td>
                            <td>{t.toAccountId ?? "-"}</td>
                            <td>{new Date(t.date).toLocaleString()}</td>
                            <td>{t.isReversed ? "Yes" : "No"}</td>
                            <td>
                                {!t.isReversed && (
                                    <button className="btn btn-sm btn-danger" onClick={() => handleReverse(t.id)}>Reverse</button>
                                )}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default Transactions;