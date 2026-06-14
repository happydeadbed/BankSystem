import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { login } from "../services/AuthService";

function Login() {
    const navigate = useNavigate();
    const [form, setForm] = useState({ name: "", password: "" });
    const [error, setError] = useState("");

    const handleChange = (e) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        try {
            const res = await login(form);
            localStorage.setItem("token", res.data.token);
            navigate("/admin");
        } catch {
            setError("Invalid username or password.");
        }
    };

    return (
        <div className="container mt-5" style={{ maxWidth: "400px" }}>
            <h3 className="mb-4">Admin Login</h3>
            {error && <div className="alert alert-danger">{error}</div>}
            <form onSubmit={handleSubmit}>
                <div className="mb-3">
                    <label className="form-label">Username</label>
                    <input
                        className="form-control"
                        name="name"
                        value={form.name}
                        onChange={handleChange}
                        required
                    />
                </div>
                <div className="mb-3">
                    <label className="form-label">Password</label>
                    <input
                        type="password"
                        className="form-control"
                        name="password"
                        value={form.password}
                        onChange={handleChange}
                        required
                    />
                </div>
                <button type="submit" className="btn btn-dark w-100">
                    Login
                </button>
            </form>
        </div>
    );
}

export default Login;