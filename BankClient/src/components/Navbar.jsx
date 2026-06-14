import { useNavigate } from "react-router-dom";
import { logout } from "../services/AuthService";

function Navbar() {
    const navigate = useNavigate();
    const token = localStorage.getItem("token");

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    return (
        <nav className="navbar navbar-dark bg-dark px-4">
            <span className="navbar-brand">Banking Admin</span>
            {token && (
                <button className="btn btn-outline-light btn-sm" onClick={handleLogout}>
                    Logout
                </button>
            )}
        </nav>
    );
}

export default Navbar;