import { useState } from "react";
import Customers from "./Customers";
import Accounts from "./Accounts";
import Transactions from "./Transactions";
import Loans from "./Loans";
import Marketplace from "./Marketplace";

function AdminDashboard() {
    const [activeTab, setActiveTab] = useState("customers");

    const tabs = [
        { key: "customers", label: "Customers" },
        { key: "accounts", label: "Accounts" },
        { key: "transactions", label: "Transactions" },
        { key: "loans", label: "Loans" },
        { key: "marketplace", label: "Marketplace" },
    ];

    return (
        <div className="container mt-4">
            <h2 className="mb-3">Admin Dashboard</h2>
            <ul className="nav nav-tabs mb-4">
                {tabs.map((tab) => (
                    <li className="nav-item" key={tab.key}>
                        <button
                            className={`nav-link ${activeTab === tab.key ? "active" : ""}`}
                            onClick={() => setActiveTab(tab.key)}
                        >
                            {tab.label}
                        </button>
                    </li>
                ))}
            </ul>
            {activeTab === "customers" && <Customers />}
            {activeTab === "accounts" && <Accounts />}
            {activeTab === "transactions" && <Transactions />}
            {activeTab === "loans" && <Loans />}
            {activeTab === "marketplace" && <Marketplace />}
        </div>
    );
}

export default AdminDashboard;