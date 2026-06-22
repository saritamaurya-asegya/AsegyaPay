import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { MetricCard } from "@/components/ui/MetricCard";
import { RevenueChart } from "@/components/charts/RevenueChart";
import { RecentTransactions } from "@/components/payments/RecentTransactions";
import { TrendingUp, CreditCard, RefreshCw, AlertTriangle } from "lucide-react";

export default function DashboardPage() {
  return (
    <DashboardLayout>
      <div className="space-y-6">
        {/* Header */}
        <div>
          <h1 className="text-2xl font-bold">Dashboard</h1>
          <p className="text-gray-500 dark:text-gray-400 text-sm mt-1">
            Welcome back! Here&apos;s what&apos;s happening with your payments today.
          </p>
        </div>

        {/* KPI Metrics */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <MetricCard
            title="Total Revenue"
            value="₹24,52,890"
            change="+12.5%"
            trend="up"
            icon={<TrendingUp className="h-5 w-5" />}
            description="vs last month"
          />
          <MetricCard
            title="Transactions"
            value="8,432"
            change="+8.2%"
            trend="up"
            icon={<CreditCard className="h-5 w-5" />}
            description="vs last month"
          />
          <MetricCard
            title="Refunds"
            value="₹1,24,500"
            change="-3.1%"
            trend="down"
            icon={<RefreshCw className="h-5 w-5" />}
            description="vs last month"
          />
          <MetricCard
            title="Failed Payments"
            value="124"
            change="-15.4%"
            trend="down"
            icon={<AlertTriangle className="h-5 w-5" />}
            description="vs last month"
            inverted
          />
        </div>

        {/* Charts */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <div className="lg:col-span-2">
            <RevenueChart />
          </div>
          <div>
            <PaymentMethodBreakdown />
          </div>
        </div>

        {/* Recent Transactions */}
        <RecentTransactions />
      </div>
    </DashboardLayout>
  );
}

function PaymentMethodBreakdown() {
  const methods = [
    { name: "UPI", percentage: 42, color: "bg-brand-500" },
    { name: "Cards", percentage: 28, color: "bg-purple-500" },
    { name: "Net Banking", percentage: 15, color: "bg-emerald-500" },
    { name: "Wallets", percentage: 10, color: "bg-amber-500" },
    { name: "BNPL", percentage: 5, color: "bg-rose-500" },
  ];

  return (
    <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-6">
      <h3 className="font-semibold mb-4">Payment Methods</h3>
      <div className="space-y-3">
        {methods.map((m) => (
          <div key={m.name}>
            <div className="flex justify-between text-sm mb-1">
              <span className="text-gray-600 dark:text-gray-400">{m.name}</span>
              <span className="font-medium">{m.percentage}%</span>
            </div>
            <div className="h-2 rounded-full bg-gray-100 dark:bg-gray-800">
              <div
                className={`h-2 rounded-full ${m.color}`}
                style={{ width: `${m.percentage}%` }}
              />
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
