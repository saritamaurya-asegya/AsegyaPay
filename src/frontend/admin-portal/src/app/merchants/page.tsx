export default function MerchantsPage() {
  const merchants = [
    { id: "mch_1", name: "Tech Corp India", email: "tech@corp.in", status: "Active", kyc: "Verified", volume: "₹12.4L", joinedAt: "2024-01-01" },
    { id: "mch_2", name: "Fashion Store", email: "admin@fashion.com", status: "Pending", kyc: "Submitted", volume: "—", joinedAt: "2024-01-14" },
    { id: "mch_3", name: "Food Delivery Co", email: "ops@foodco.in", status: "Active", kyc: "Verified", volume: "₹8.2L", joinedAt: "2023-12-20" },
    { id: "mch_4", name: "StartupX", email: "founder@startupx.io", status: "Suspended", kyc: "Rejected", volume: "₹0.8L", joinedAt: "2023-11-15" },
  ];

  const statusColor = (s: string) => {
    switch (s) {
      case "Active": return "bg-emerald-100 text-emerald-700";
      case "Pending": return "bg-amber-100 text-amber-700";
      case "Suspended": return "bg-rose-100 text-rose-700";
      default: return "bg-gray-100 text-gray-700";
    }
  };

  const kycColor = (k: string) => {
    switch (k) {
      case "Verified": return "text-emerald-600";
      case "Submitted": return "text-amber-600";
      case "Rejected": return "text-rose-600";
      default: return "text-gray-400";
    }
  };

  return (
    <main className="p-8">
      <div className="max-w-7xl mx-auto">
        <h1 className="text-2xl font-bold mb-6">Merchant Management</h1>
        <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 overflow-hidden">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-gray-100 dark:border-gray-800 bg-gray-50 dark:bg-gray-800/50">
                <th className="text-left px-6 py-3 font-medium text-gray-500">Merchant</th>
                <th className="text-left px-6 py-3 font-medium text-gray-500">Email</th>
                <th className="text-center px-6 py-3 font-medium text-gray-500">Status</th>
                <th className="text-center px-6 py-3 font-medium text-gray-500">KYC</th>
                <th className="text-right px-6 py-3 font-medium text-gray-500">MTD Volume</th>
                <th className="text-right px-6 py-3 font-medium text-gray-500">Joined</th>
                <th className="px-6 py-3" />
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100 dark:divide-gray-800">
              {merchants.map((m) => (
                <tr key={m.id} className="hover:bg-gray-50 dark:hover:bg-gray-800/30 transition">
                  <td className="px-6 py-4">
                    <div>
                      <p className="font-medium">{m.name}</p>
                      <p className="text-xs font-mono text-gray-400">{m.id}</p>
                    </div>
                  </td>
                  <td className="px-6 py-4 text-gray-500">{m.email}</td>
                  <td className="px-6 py-4 text-center">
                    <span className={`text-xs px-2 py-0.5 rounded-full font-medium ${statusColor(m.status)}`}>
                      {m.status}
                    </span>
                  </td>
                  <td className={`px-6 py-4 text-center text-sm font-medium ${kycColor(m.kyc)}`}>
                    {m.kyc}
                  </td>
                  <td className="px-6 py-4 text-right font-semibold">{m.volume}</td>
                  <td className="px-6 py-4 text-right text-xs text-gray-500">{m.joinedAt}</td>
                  <td className="px-6 py-4">
                    <button className="text-brand-600 text-xs hover:underline">View</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </main>
  );
}
