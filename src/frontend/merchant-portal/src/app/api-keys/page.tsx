"use client";

import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { useState } from "react";
import { Plus, Copy, Trash2, Eye, EyeOff } from "lucide-react";
import { clsx } from "clsx";
import { toast } from "sonner";

const mockApiKeys = [
  {
    id: "key_1",
    name: "Production Key",
    prefix: "rzp_live_aBcD",
    type: "live",
    isRevoked: false,
    createdAt: "2024-01-01T00:00:00Z",
    lastUsedAt: "2024-01-15T10:30:00Z",
  },
  {
    id: "key_2",
    name: "Test Key",
    prefix: "rzp_test_xYzW",
    type: "test",
    isRevoked: false,
    createdAt: "2024-01-01T00:00:00Z",
    lastUsedAt: "2024-01-14T18:00:00Z",
  },
  {
    id: "key_3",
    name: "Old Dev Key",
    prefix: "rzp_test_aBc1",
    type: "test",
    isRevoked: true,
    createdAt: "2023-12-01T00:00:00Z",
    lastUsedAt: "2023-12-31T00:00:00Z",
  },
];

export default function ApiKeysPage() {
  const [showCreateModal, setShowCreateModal] = useState(false);

  const copyToClipboard = (text: string) => {
    navigator.clipboard.writeText(text);
    toast.success("Copied to clipboard!");
  };

  return (
    <DashboardLayout>
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-bold">API Keys</h1>
            <p className="text-sm text-gray-500 mt-1">
              Manage your API keys for authentication.
            </p>
          </div>
          <button
            onClick={() => setShowCreateModal(true)}
            className="flex items-center gap-2 px-4 py-2 text-sm gradient-brand text-white rounded-lg hover:opacity-90 transition"
          >
            <Plus className="h-4 w-4" />
            Create Key
          </button>
        </div>

        {/* Warning */}
        <div className="rounded-lg border border-amber-200 bg-amber-50 dark:border-amber-800 dark:bg-amber-950/30 p-4 text-sm text-amber-800 dark:text-amber-300">
          <strong>⚠️ Keep your keys secure!</strong> Never share your secret key in public repositories, client-side code, or anywhere publicly accessible.
        </div>

        {/* Keys table */}
        <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 overflow-hidden">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-gray-100 dark:border-gray-800 bg-gray-50 dark:bg-gray-800/50">
                <th className="text-left px-6 py-3 font-medium text-gray-500">Name</th>
                <th className="text-left px-6 py-3 font-medium text-gray-500">Key</th>
                <th className="text-left px-6 py-3 font-medium text-gray-500">Type</th>
                <th className="text-left px-6 py-3 font-medium text-gray-500">Status</th>
                <th className="text-left px-6 py-3 font-medium text-gray-500">Last Used</th>
                <th className="px-6 py-3" />
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100 dark:divide-gray-800">
              {mockApiKeys.map((key) => (
                <tr key={key.id} className="hover:bg-gray-50 dark:hover:bg-gray-800/30">
                  <td className="px-6 py-4 font-medium">{key.name}</td>
                  <td className="px-6 py-4">
                    <div className="flex items-center gap-2">
                      <code className="font-mono text-xs bg-gray-100 dark:bg-gray-800 px-2 py-1 rounded">
                        {key.prefix}••••••••
                      </code>
                      <button
                        onClick={() => copyToClipboard(key.prefix)}
                        className="text-gray-400 hover:text-gray-600"
                      >
                        <Copy className="h-3.5 w-3.5" />
                      </button>
                    </div>
                  </td>
                  <td className="px-6 py-4">
                    <span className={clsx(
                      "text-xs px-2 py-0.5 rounded-full font-medium",
                      key.type === "live"
                        ? "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-400"
                        : "bg-gray-100 text-gray-600 dark:bg-gray-800 dark:text-gray-400"
                    )}>
                      {key.type}
                    </span>
                  </td>
                  <td className="px-6 py-4">
                    {key.isRevoked ? (
                      <span className="text-xs text-rose-600">Revoked</span>
                    ) : (
                      <span className="text-xs text-emerald-600">Active</span>
                    )}
                  </td>
                  <td className="px-6 py-4 text-gray-500 text-xs">
                    {key.lastUsedAt
                      ? new Date(key.lastUsedAt).toLocaleDateString("en-IN")
                      : "Never"}
                  </td>
                  <td className="px-6 py-4 text-right">
                    {!key.isRevoked && (
                      <button className="text-rose-400 hover:text-rose-600 text-xs">
                        Revoke
                      </button>
                    )}
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
