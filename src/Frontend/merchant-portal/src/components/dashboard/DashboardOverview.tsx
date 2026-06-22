'use client';

import React from 'react';

interface MetricCardProps {
  title: string;
  value: string;
  change: string;
  changeType: 'positive' | 'negative' | 'neutral';
}

function MetricCard({ title, value, change, changeType }: MetricCardProps) {
  const changeColor = {
    positive: 'text-green-500',
    negative: 'text-red-500',
    neutral: 'text-muted-foreground',
  }[changeType];

  return (
    <div className="rounded-xl border border-border bg-card p-6">
      <p className="text-sm text-muted-foreground">{title}</p>
      <p className="mt-2 text-3xl font-bold">{value}</p>
      <p className={`mt-1 text-sm ${changeColor}`}>{change}</p>
    </div>
  );
}

export function DashboardOverview() {
  return (
    <div className="space-y-6">
      {/* Metrics Grid */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <MetricCard
          title="Total Revenue"
          value="₹12,45,678"
          change="+12.5% from last month"
          changeType="positive"
        />
        <MetricCard
          title="Transactions"
          value="3,842"
          change="+8.2% from last month"
          changeType="positive"
        />
        <MetricCard
          title="Success Rate"
          value="97.8%"
          change="+0.3% from last month"
          changeType="positive"
        />
        <MetricCard
          title="Avg. Transaction"
          value="₹3,240"
          change="-2.1% from last month"
          changeType="negative"
        />
      </div>

      {/* Recent Transactions */}
      <div className="rounded-xl border border-border bg-card">
        <div className="flex items-center justify-between border-b border-border p-6">
          <h2 className="text-lg font-semibold">Recent Transactions</h2>
          <a href="/payments" className="text-sm text-primary hover:underline">
            View all
          </a>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-border text-left text-sm text-muted-foreground">
                <th className="px-6 py-3 font-medium">Payment ID</th>
                <th className="px-6 py-3 font-medium">Amount</th>
                <th className="px-6 py-3 font-medium">Method</th>
                <th className="px-6 py-3 font-medium">Status</th>
                <th className="px-6 py-3 font-medium">Date</th>
              </tr>
            </thead>
            <tbody>
              <TransactionRow
                id="pay_8f7d6e5c4b3a2190"
                amount="₹5,000"
                method="UPI"
                status="captured"
                date="22 Jun 2026, 10:30 AM"
              />
              <TransactionRow
                id="pay_1a2b3c4d5e6f7890"
                amount="₹12,500"
                method="Card"
                status="captured"
                date="22 Jun 2026, 09:15 AM"
              />
              <TransactionRow
                id="pay_9h8g7f6e5d4c3b2a"
                amount="₹890"
                method="Wallet"
                status="failed"
                date="22 Jun 2026, 08:45 AM"
              />
              <TransactionRow
                id="pay_0i1j2k3l4m5n6o7p"
                amount="₹25,000"
                method="Net Banking"
                status="authorized"
                date="21 Jun 2026, 11:20 PM"
              />
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

function TransactionRow({
  id,
  amount,
  method,
  status,
  date,
}: {
  id: string;
  amount: string;
  method: string;
  status: string;
  date: string;
}) {
  const statusStyles: Record<string, string> = {
    captured: 'bg-green-500/10 text-green-500',
    authorized: 'bg-blue-500/10 text-blue-500',
    failed: 'bg-red-500/10 text-red-500',
    refunded: 'bg-yellow-500/10 text-yellow-500',
  };

  return (
    <tr className="border-b border-border last:border-0">
      <td className="px-6 py-4 text-sm font-mono">{id}</td>
      <td className="px-6 py-4 text-sm font-semibold">{amount}</td>
      <td className="px-6 py-4 text-sm">{method}</td>
      <td className="px-6 py-4">
        <span className={`inline-flex rounded-full px-2.5 py-0.5 text-xs font-medium ${statusStyles[status] || ''}`}>
          {status}
        </span>
      </td>
      <td className="px-6 py-4 text-sm text-muted-foreground">{date}</td>
    </tr>
  );
}
