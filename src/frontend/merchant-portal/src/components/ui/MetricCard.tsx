import { clsx } from "clsx";
import { TrendingUp, TrendingDown } from "lucide-react";

interface MetricCardProps {
  title: string;
  value: string;
  change: string;
  trend: "up" | "down";
  icon: React.ReactNode;
  description?: string;
  inverted?: boolean;
}

export function MetricCard({
  title,
  value,
  change,
  trend,
  icon,
  description,
  inverted = false,
}: MetricCardProps) {
  const isPositive = inverted ? trend === "down" : trend === "up";

  return (
    <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-6">
      <div className="flex items-center justify-between mb-3">
        <p className="text-sm font-medium text-gray-500 dark:text-gray-400">{title}</p>
        <div className="h-9 w-9 rounded-lg bg-brand-50 dark:bg-brand-950 flex items-center justify-center text-brand-600 dark:text-brand-400">
          {icon}
        </div>
      </div>
      <p className="text-2xl font-bold">{value}</p>
      <div className="flex items-center gap-1 mt-2">
        {isPositive ? (
          <TrendingUp className="h-3.5 w-3.5 text-emerald-500" />
        ) : (
          <TrendingDown className="h-3.5 w-3.5 text-rose-500" />
        )}
        <span
          className={clsx(
            "text-xs font-medium",
            isPositive ? "text-emerald-600 dark:text-emerald-400" : "text-rose-600 dark:text-rose-400"
          )}
        >
          {change}
        </span>
        {description && (
          <span className="text-xs text-gray-400 ml-1">{description}</span>
        )}
      </div>
    </div>
  );
}
