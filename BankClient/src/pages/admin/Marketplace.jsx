import { useEffect, useState } from "react";
import * as MarketplaceService from "../../services/MarketplaceService";

function Marketplace() {
    const [listings, setListings] = useState([]);
    const [buyerName, setBuyerName] = useState("");
    const [selectedId, setSelectedId] = useState(null);
    const [msg, setMsg] = useState("");
    const [error, setError] = useState("");

    useEffect(() => { loadListings(); }, []);

    const loadListings = async () => {
        try { const res = await MarketplaceService.getAll(); setListings(res.data); }
        catch { setError("Failed to load listings."); }
    };

    const handleSellClick = (id) => {
        setSelectedId(id); setBuyerName(""); setMsg(""); setError("");
    };

    const handleSell = async (e) => {
        e.preventDefault(); setMsg(""); setError("");
        try {
            await MarketplaceService.sell(selectedId, buyerName);
            setMsg("Property sold successfully.");
            setSelectedId(null); setBuyerName(""); loadListings();
        } catch { setError("Failed to sell listing."); }
    };

    return (
        <div>
            <h4>Marketplace</h4>
            {msg && <div className="alert alert-success">{msg}</div>}
            {error && <div className="alert alert-danger">{error}</div>}

            {selectedId && (
                <form onSubmit={handleSell} className="border p-3 rounded mb-4">
                    <h5>Sell Listing #{selectedId}</h5>
                    <div className="mb-2">
                        <label className="form-label">Buyer Name</label>
                        <input
                            className="form-control" style={{ maxWidth: "300px" }}
                            value={buyerName} onChange={(e) => setBuyerName(e.target.value)} required
                        />
                    </div>
                    <button type="submit" className="btn btn-success me-2">Confirm Sale</button>
                    <button type="button" className="btn btn-secondary" onClick={() => setSelectedId(null)}>Cancel</button>
                </form>
            )}

            <table className="table table-bordered table-hover">
                <thead className="table-dark">
                    <tr>
                        <th>ID</th><th>Loan ID</th><th>Collateral</th><th>Coll. Value</th>
                        <th>Original Loan</th><th>Remaining</th><th>Listed Price</th>
                        <th>Status</th><th>Buyer</th><th>Sold At</th><th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {listings.map((l) => (
                        <tr key={l.id}>
                            <td>{l.id}</td>
                            <td>{l.loanId}</td>
                            <td>{l.collateralDescription}</td>
                            <td>${l.collateralValue.toFixed(2)}</td>
                            <td>${l.originalLoanAmount.toFixed(2)}</td>
                            <td>${l.remainingBalance.toFixed(2)}</td>
                            <td>${l.listedPrice.toFixed(2)}</td>
                            <td>
                                <span className={`badge ${l.status === "Listed" ? "bg-warning text-dark" : "bg-success"}`}>
                                    {l.status}
                                </span>
                            </td>
                            <td>{l.buyerName || "-"}</td>
                            <td>{l.soldAt ? new Date(l.soldAt).toLocaleDateString() : "-"}</td>
                            <td>
                                {l.status === "Listed" && (
                                    <button className="btn btn-sm btn-primary" onClick={() => handleSellClick(l.id)}>Sell</button>
                                )}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default Marketplace;