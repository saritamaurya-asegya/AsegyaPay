import { clsx } from "clsx";

const transactions = [
  { id: "pay_1Abc123", amount: "₹4,500", method: "UPI", status: "captured", customer: "Rahul S.", time: "2m ago" },
  { id: "pay_2Def456", amount: "₹12,000", method: "Card", status: "captured", customer: "Priya M.", time: "8m ago" },
  { id: "pay_3Ghi789", amount: "₹2,899", method: "Wallet", status: "pending", customer: "Arjun K.", time: "15m ago" },
  { id: "pay_4Jkl012", amount: "₹8,500", method: "Net Banking", status: "failed", customer: "Sneha R.", time: "24m ago" },
  { id: "pay_5Mno345", amount: "₹1,299", method: "UPI", status: "captured", customer: "Vikram T.", time: "31m ago" },
];

const statusColors: Record<string, string> = {
  captured: "bg-emerald-100 text-emerald-700 dark:bg-emerald-900 dark:text-emerald-300",
  pending: "bg-amber-100 text-amber-700 dark:bg-amber-900 dark:text-amber-300",
  failed: "bg-rose-100 text-rose-700 dark:bg-rose-900 dark:text-rose-300",
  refunded: "bg-blue-100 text-blue-700 dark:bg-blue-900 dark:text-blue-300",
};

export function RecentTransactions() {
  return (
    <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900">
      <div className="flex items-center justify-between p-6 border-b border-gray-100 dark:border-gray-800">
        <h3 className="font-semibold">Recent Transactions</h3>
        <a href="/payments" className="text-sm text-brand-600 dark:text-brand-400 hover:underline">
          View all →
        </a>
      </div>
      <div className="divide-y divide-gray-100 dark:divide-gray-800">
        {transactions.map((tx) => (
          <div key={tx.id} className="flex items-center gap-4 px-6 py-4 hover:bg-gray-50 dark:hover:bg-gray-800/50 transition">
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium truncate">{tx.customer}</p>
              <p className="text-xs text-gray-500 font-mono">{tx.id}</p>
            </div>
            <span className="text-xs text-gray-500">{tx.method}</span>
            <span
              className={clsx(
                "text-xs px-2 py-0.5 rounded-full font-medium",
                statusColors[tx.status]
              )}
            >
              {tx.status}
            </span>
            <div className="text-right">
              <p className="text-sm font-semibold">{tx.amount}</p>
              <p className="text-xs text-gray-400">{tx.time}</p>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
