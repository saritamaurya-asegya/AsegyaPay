"use client";

import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { useState } from "react";
import { clsx } from "clsx";
import { Search, Filter, Download, Eye } from "lucide-react";

const mockPayments = [
  { id: "pay_abc123", orderId: "ORD_001", amount: 4500, currency: "INR", method: "UPI", status: "Captured", customer: "Rahul Sharma", createdAt: "2024-01-15T10:30:00Z", fraudScore: 45 },
  { id: "pay_def456", orderId: "ORD_002", amount: 12000, currency: "INR", method: "Card", status: "Captured", customer: "Priya Mehta", createdAt: "2024-01-15T09:15:00Z", fraudScore: 120 },
  { id: "pay_ghi789", orderId: "ORD_003", amount: 2899, currency: "INR", method: "Wallet", status: "Pending", customer: "Arjun Kumar", createdAt: "2024-01-15T08:45:00Z", fraudScore: 80 },
  { id: "pay_jkl012", orderId: "ORD_004", amount: 8500, currency: "INR", method: "Net Banking", status: "Failed", customer: "Sneha Reddy", createdAt: "2024-01-15T08:00:00Z", fraudScore: 200 },
  { id: "pay_mno345", orderId: "ORD_005", amount: 1299, currency: "INR", method: "UPI", status: "Refunded", customer: "Vikram Tiwari", createdAt: "2024-01-14T22:10:00Z", fraudScore: 30 },
];

const statusColors: Record<string, string> = {
  Captured: "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-400",
  Pending: "bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-400",
  Failed: "bg-rose-100 text-rose-700 dark:bg-rose-900/30 dark:text-rose-400",
  Refunded: "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400",
  Authorized: "bg-purple-100 text-purple-700 dark:bg-purple-900/30 dark:text-purple-400",
  Cancelled: "bg-gray-100 text-gray-700 dark:bg-gray-800 dark:text-gray-400",
};

const fraudRiskColor = (score: number) => {
  if (score <= 200) return "text-emerald-600";
  if (score <= 500) return "text-amber-600";
  return "text-rose-600";
};

export default function PaymentsPage() {
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("all");

  const filtered = mockPayments.filter((p) => {
    const matchesSearch =
      p.id.includes(search) ||
      p.orderId.toLowerCase().includes(search.toLowerCase()) ||
      p.customer.toLowerCase().includes(search.toLowerCase());
    const matchesStatus = statusFilter === "all" || p.status === statusFilter;
    return matchesSearch && matchesStatus;
  });

  return (
    <DashboardLayout>
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-bold">Payments</h1>
            <p className="text-sm text-gray-500 mt-1">
              {mockPayments.length} total transactions
            </p>
          </div>
          <button className="flex items-center gap-2 px-4 py-2 text-sm border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800 transition">
            <Download className="h-4 w-4" />
            Export
          </button>
        </div>

        {/* Filters */}
        <div className="flex flex-col sm:flex-row gap-3">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
            <input
              type="text"
              placeholder="Search by payment ID, order ID, or customer..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="w-full pl-9 pr-4 py-2 text-sm border border-gray-200 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-900 focus:outline-none focus:ring-2 focus:ring-brand-500"
            />
          </div>
          <select
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value)}
            className="px-3 py-2 text-sm border border-gray-200 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-900 focus:outline-none focus:ring-2 focus:ring-brand-500"
          >
            <option value="all">All Status</option>
            <option value="Captured">Captured</option>
            <option value="Pending">Pending</option>
            <option value="Failed">Failed</option>
            <option value="Refunded">Refunded</option>
            <option value="Authorized">Authorized</option>
          </select>
        </div>

        {/* Table */}
        <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 overflow-hidden">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-gray-100 dark:border-gray-800 bg-gray-50 dark:bg-gray-800/50">
                <th className="text-left px-6 py-3 font-medium text-gray-500 dark:text-gray-400">Payment ID</th>
                <th className="text-left px-6 py-3 font-medium text-gray-500 dark:text-gray-400">Customer</th>
                <th className="text-left px-6 py-3 font-medium text-gray-500 dark:text-gray-400">Method</th>
                <th className="text-right px-6 py-3 font-medium text-gray-500 dark:text-gray-400">Amount</th>
                <th className="text-center px-6 py-3 font-medium text-gray-500 dark:text-gray-400">Status</th>
                <th className="text-center px-6 py-3 font-medium text-gray-500 dark:text-gray-400">Risk Score</th>
                <th className="text-right px-6 py-3 font-medium text-gray-500 dark:text-gray-400">Date</th>
                <th className="px-6 py-3" />
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100 dark:divide-gray-800">
              {filtered.map((payment) => (
                <tr key={payment.id} className="hover:bg-gray-50 dark:hover:bg-gray-800/30 transition">
                  <td className="px-6 py-4">
                    <span className="font-mono text-xs text-brand-600 dark:text-brand-400">{payment.id}</span>
                  </td>
                  <td className="px-6 py-4">{payment.customer}</td>
                  <td className="px-6 py-4 text-gray-500">{payment.method}</td>
                  <td className="px-6 py-4 text-right font-semibold">
                    ₹{payment.amount.toLocaleString("en-IN")}
                  </td>
                  <td className="px-6 py-4 text-center">
                    <span className={clsx("text-xs px-2 py-0.5 rounded-full font-medium", statusColors[payment.status])}>
                      {payment.status}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-center">
                    <span className={clsx("font-semibold text-xs", fraudRiskColor(payment.fraudScore))}>
                      {payment.fraudScore}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-right text-gray-500 text-xs">
                    {new Date(payment.createdAt).toLocaleString("en-IN", {
                      dateStyle: "medium",
                      timeStyle: "short",
                    })}
                  </td>
                  <td className="px-6 py-4">
                    <button className="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200">
                      <Eye className="h-4 w-4" />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {filtered.length === 0 && (
            <div className="text-center py-12 text-gray-400">No payments found.</div>
          )}
        </div>
      </div>
    </DashboardLayout>
  );
}
