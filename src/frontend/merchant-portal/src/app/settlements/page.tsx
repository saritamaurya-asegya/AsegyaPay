"use client";

import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { clsx } from "clsx";
import { Download } from "lucide-react";

const settlements = [
  { id: "set_001", period: "Jan 14, 2024", amount: 142500, currency: "INR", transactionCount: 32, status: "Settled", utrNumber: "UTR240114001234", settledAt: "2024-01-15T09:00:00Z" },
  { id: "set_002", period: "Jan 13, 2024", amount: 98750, currency: "INR", transactionCount: 24, status: "Settled", utrNumber: "UTR240113005678", settledAt: "2024-01-14T09:00:00Z" },
  { id: "set_003", period: "Jan 12, 2024", amount: 210000, currency: "INR", transactionCount: 51, status: "Settled", utrNumber: "UTR240112009012", settledAt: "2024-01-13T09:00:00Z" },
  { id: "set_004", period: "Jan 15, 2024", amount: 186000, currency: "INR", transactionCount: 43, status: "Processing", utrNumber: null, settledAt: null },
];

const statusColors: Record<string, string> = {
  Settled: "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-400",
  Processing: "bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-400",
  Failed: "bg-rose-100 text-rose-700 dark:bg-rose-900/30 dark:text-rose-400",
};

export default function SettlementsPage() {
  return (
    <DashboardLayout>
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-bold">Settlements</h1>
            <p className="text-sm text-gray-500 mt-1">
              T+2 settlement cycle. Funds are settled to your bank account.
            </p>
          </div>
          <button className="flex items-center gap-2 px-4 py-2 text-sm border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800 transition">
            <Download className="h-4 w-4" />
            Download Statement
          </button>
        </div>

        {/* Summary cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-5">
            <p className="text-sm text-gray-500">Next Settlement</p>
            <p className="text-2xl font-bold mt-1">₹1,86,000</p>
            <p className="text-xs text-gray-400 mt-1">Expected Jan 17, 2024</p>
          </div>
          <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-5">
            <p className="text-sm text-gray-500">Total Settled (Jan)</p>
            <p className="text-2xl font-bold mt-1">₹4,51,250</p>
            <p className="text-xs text-emerald-500 mt-1">+18.2% vs Dec</p>
          </div>
          <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-5">
            <p className="text-sm text-gray-500">Settlement Cycle</p>
            <p className="text-2xl font-bold mt-1">T+2</p>
            <p className="text-xs text-gray-400 mt-1">Business days</p>
          </div>
        </div>

        {/* Table */}
        <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 overflow-hidden">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-gray-100 dark:border-gray-800 bg-gray-50 dark:bg-gray-800/50">
                <th className="text-left px-6 py-3 font-medium text-gray-500">Settlement ID</th>
                <th className="text-left px-6 py-3 font-medium text-gray-500">Period</th>
                <th className="text-right px-6 py-3 font-medium text-gray-500">Amount</th>
                <th className="text-center px-6 py-3 font-medium text-gray-500">Transactions</th>
                <th className="text-center px-6 py-3 font-medium text-gray-500">Status</th>
                <th className="text-left px-6 py-3 font-medium text-gray-500">UTR Number</th>
                <th className="text-right px-6 py-3 font-medium text-gray-500">Settled On</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100 dark:divide-gray-800">
              {settlements.map((s) => (
                <tr key={s.id} className="hover:bg-gray-50 dark:hover:bg-gray-800/30 transition">
                  <td className="px-6 py-4 font-mono text-xs text-brand-600">{s.id}</td>
                  <td className="px-6 py-4">{s.period}</td>
                  <td className="px-6 py-4 text-right font-semibold">
                    ₹{s.amount.toLocaleString("en-IN")}
                  </td>
                  <td className="px-6 py-4 text-center">{s.transactionCount}</td>
                  <td className="px-6 py-4 text-center">
                    <span className={clsx("text-xs px-2 py-0.5 rounded-full font-medium", statusColors[s.status])}>
                      {s.status}
                    </span>
                  </td>
                  <td className="px-6 py-4 font-mono text-xs text-gray-500">
                    {s.utrNumber ?? "—"}
                  </td>
                  <td className="px-6 py-4 text-right text-xs text-gray-500">
                    {s.settledAt
                      ? new Date(s.settledAt).toLocaleDateString("en-IN")
                      : "Pending"}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </DashboardLayout>
  );
}
