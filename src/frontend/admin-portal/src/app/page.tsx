export default function AdminDashboard() {
  const stats = [
    { label: "Total Merchants", value: "1,284", change: "+23 this week" },
    { label: "Daily Volume", value: "₹2.4Cr", change: "+8.3% vs yesterday" },
    { label: "Pending KYC", value: "47", change: "Needs review" },
    { label: "Fraud Alerts", value: "12", change: "3 critical" },
  ];

  return (
    <main className="min-h-screen p-8">
      <div className="max-w-7xl mx-auto">
        <div className="mb-8 flex items-center gap-3">
          <div className="h-10 w-10 rounded-xl bg-gradient-to-br from-rose-500 to-orange-500 flex items-center justify-center">
            <span className="text-white font-bold">A</span>
          </div>
          <div>
            <h1 className="text-2xl font-bold">Admin Portal</h1>
            <p className="text-sm text-gray-500">Platform monitoring and management</p>
          </div>
          <div className="ml-auto">
            <span className="text-xs bg-rose-100 text-rose-700 px-2 py-1 rounded-full font-medium">
              Admin Access
            </span>
          </div>
        </div>

        {/* Stats */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
          {stats.map((stat) => (
            <div key={stat.label} className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-6">
              <p className="text-sm text-gray-500">{stat.label}</p>
              <p className="text-2xl font-bold mt-1">{stat.value}</p>
              <p className="text-xs text-gray-400 mt-1">{stat.change}</p>
            </div>
          ))}
        </div>

        {/* Nav grid */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {[
            { href: "/merchants", label: "Merchant Management", desc: "Approve, suspend, manage merchants" },
            { href: "/transactions", label: "Transaction Monitor", desc: "Real-time transaction surveillance" },
            { href: "/fraud", label: "Fraud Monitoring", desc: "Review fraud alerts and rules" },
            { href: "/settlements", label: "Settlement Engine", desc: "Process and monitor settlements" },
            { href: "/compliance", label: "Compliance & KYC", desc: "AML, KYC reviews, and reports" },
            { href: "/audit", label: "Audit Logs", desc: "Complete platform audit trail" },
          ].map((item) => (
            <a
              key={item.href}
              href={item.href}
              className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-6 hover:border-brand-500 hover:shadow-sm transition group"
            >
              <h3 className="font-semibold group-hover:text-brand-600">{item.label}</h3>
              <p className="text-sm text-gray-500 mt-1">{item.desc}</p>
            </a>
          ))}
        </div>
      </div>
    </main>
  );
}
