"use client";

import {
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from "recharts";

const data = [
  { date: "Jan 1", revenue: 180000, transactions: 412 },
  { date: "Jan 5", revenue: 240000, transactions: 580 },
  { date: "Jan 10", revenue: 210000, transactions: 490 },
  { date: "Jan 15", revenue: 320000, transactions: 720 },
  { date: "Jan 20", revenue: 280000, transactions: 640 },
  { date: "Jan 25", revenue: 450000, transactions: 980 },
  { date: "Jan 30", revenue: 380000, transactions: 850 },
];

const formatCurrency = (value: number) =>
  `₹${(value / 1000).toFixed(0)}K`;

export function RevenueChart() {
  return (
    <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-6">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h3 className="font-semibold">Revenue Overview</h3>
          <p className="text-sm text-gray-500 dark:text-gray-400">Last 30 days</p>
        </div>
        <select className="text-sm border border-gray-200 dark:border-gray-700 rounded-lg px-3 py-1.5 bg-transparent focus:outline-none focus:ring-2 focus:ring-brand-500">
          <option>Last 30 days</option>
          <option>Last 7 days</option>
          <option>Last 90 days</option>
        </select>
      </div>
      <ResponsiveContainer width="100%" height={240}>
        <AreaChart data={data} margin={{ top: 0, right: 0, left: 0, bottom: 0 }}>
          <defs>
            <linearGradient id="colorRevenue" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#0589e9" stopOpacity={0.15} />
              <stop offset="95%" stopColor="#0589e9" stopOpacity={0} />
            </linearGradient>
          </defs>
          <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" vertical={false} />
          <XAxis
            dataKey="date"
            tick={{ fontSize: 12, fill: "#9ca3af" }}
            axisLine={false}
            tickLine={false}
          />
          <YAxis
            tickFormatter={formatCurrency}
            tick={{ fontSize: 12, fill: "#9ca3af" }}
            axisLine={false}
            tickLine={false}
            width={55}
          />
          <Tooltip
            formatter={(value: number) => [formatCurrency(value), "Revenue"]}
            contentStyle={{
              background: "white",
              border: "1px solid #e5e7eb",
              borderRadius: "8px",
              fontSize: "12px",
            }}
          />
          <Area
            type="monotone"
            dataKey="revenue"
            stroke="#0589e9"
            strokeWidth={2}
            fill="url(#colorRevenue)"
          />
        </AreaChart>
      </ResponsiveContainer>
    </div>
  );
}
