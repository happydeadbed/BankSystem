import { useEffect, useState } from "react";
import * as CustomerService from "../../services/CustomerService";

function Customers() {
    const [customers, setCustomers] = useState([]);
    const [form, setForm] = useState({ fullName: "", username: "", password: "" });
    const [editId, setEditId] = useState(null);
    const [isEdit, setIsEdit] = useState(false);
    const [msg, setMsg] = useState("");
    const [error, setError] = useState("");

    useEffect(() => { loadCustomers(); }, []);

    const loadCustomers = async () => {
        try {
            const res = await CustomerService.getAll();
            setCustomers(res.data);
        } catch { setError("Failed to load customers."); }
    };

    const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

    const clearForm = () => {
        setForm({ fullName: "", username: "", password: "" });
        setEditId(null);
        setIsEdit(false);
    };

    const handleCreate = async (e) => {
        e.preventDefault(); setMsg(""); setError("");
        try {
            await CustomerService.create(form);
            setMsg("Customer created successfully.");
            clearForm(); loadCustomers();
        } catch { setError("Failed to create customer."); }
    };

    const handleEdit = (customer) => {
        setForm({ fullName: customer.fullName, username: customer.username, password: customer.password });
        setEditId(customer.id);
        setIsEdit(true);
        setMsg(""); setError("");
    };

    const handleUpdate = async (e) => {
        e.preventDefault(); setMsg(""); setError("");
        try {
            await CustomerService.update(editId, form);
            setMsg("Customer updated successfully.");
            clearForm(); loadCustomers();
        } catch { setError("Failed to update customer."); }
    };

    const handleDelete = async (id) => {
        if (!window.confirm("Are you sure you want to delete this customer?")) return;
        try {
            await CustomerService.remove(id);
            setMsg("Customer deleted.");
            loadCustomers();
        } catch { setError("Failed to delete customer."); }
    };

    return (
        <div>
            <h4>Customers</h4>
            {msg && <div className="alert alert-success">{msg}</div>}
            {error && <div className="alert alert-danger">{error}</div>}

            {!isEdit && (
                <form onSubmit={handleCreate} className="border p-3 rounded mb-4">
                    <h5>Create Customer</h5>
                    <div className="row">
                        <div className="col-md-4 mb-2">
                            <label className="form-label">Full Name</label>
                            <input className="form-control" name="fullName" value={form.fullName} onChange={handleChange} required />
                        </div>
                        <div className="col-md-4 mb-2">
                            <label className="form-label">Username</label>
                            <input className="form-control" name="username" value={form.username} onChange={handleChange} required />
                        </div>
                        <div className="col-md-4 mb-2">
                            <label className="form-label">Password</label>
                            <input className="form-control" name="password" value={form.password} onChange={handleChange} required />
                        </div>
                    </div>
                    <button type="submit" className="btn btn-primary">Create</button>
                </form>
            )}

            {isEdit && (
                <form onSubmit={handleUpdate} className="border p-3 rounded mb-4">
                    <h5>Edit Customer</h5>
                    <div className="row">
                        <div className="col-md-4 mb-2">
                            <label className="form-label">Full Name</label>
                            <input className="form-control" name="fullName" value={form.fullName} onChange={handleChange} required />
                        </div>
                        <div className="col-md-4 mb-2">
                            <label className="form-label">Username</label>
                            <input className="form-control" name="username" value={form.username} onChange={handleChange} required />
                        </div>
                        <div className="col-md-4 mb-2">
                            <label className="form-label">Password</label>
                            <input className="form-control" name="password" value={form.password} onChange={handleChange} required />
                        </div>
                    </div>
                    <button type="submit" className="btn btn-primary me-2">Save</button>
                    <button type="button" className="btn btn-secondary" onClick={clearForm}>Cancel</button>
                </form>
            )}

            <table className="table table-bordered table-hover">
                <thead className="table-dark">
                    <tr>
                        <th>ID</th><th>Customer #</th><th>Full Name</th>
                        <th>Username</th><th>Created At</th><th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {customers.map((c) => (
                        <tr key={c.id}>
                            <td>{c.id}</td>
                            <td>{c.customerNumber}</td>
                            <td>{c.fullName}</td>
                            <td>{c.username}</td>
                            <td>{new Date(c.createdAt).toLocaleDateString()}</td>
                            <td>
                                <button className="btn btn-sm btn-warning me-2" onClick={() => handleEdit(c)}>Edit</button>
                                <button className="btn btn-sm btn-danger" onClick={() => handleDelete(c.id)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default Customers;