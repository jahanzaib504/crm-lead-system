import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import type { Lead } from "../types/Lead";
import { getLeadsPaged } from "../services/leadService";

export const ShowLeads = () => {
  const [page, setPage] = useState<number>(1);

  const {
    data: leads,
    isPending,
    isError,
  } = useQuery<Lead[]>({
    queryKey: ["leads", page],
    queryFn: () => getLeadsPaged(page),
  });

  return (
    <div className="max-w-6xl mx-auto p-6 space-y-6">
      <div className="flex justify-between items-center border-b pb-4">
        <h1 className="text-2xl font-bold text-gray-800">Sales Dashboard</h1>
        <span className="text-sm text-gray-500">Page {page}</span>
      </div>

      {isPending && (
        <div className="flex justify-center items-center p-12 text-gray-500">
          Loading leads...
        </div>
      )}

      {isError && (
        <div className="p-4 bg-red-50 text-red-600 rounded-md border border-red-200">
          Failed to load leads. Please try again.
        </div>
      )}

      {leads && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {leads.map((lead) => (
            <div
              key={lead.id ?? Math.random()}
              className="bg-white rounded-lg border border-gray-200 p-5 shadow-sm hover:shadow-md transition-shadow flex flex-col justify-between space-y-4"
            >
              <div className="space-y-2">
                <div className="flex justify-between items-start gap-2">
                  <h2 className="font-semibold text-lg text-gray-900 leading-snug">
                    {lead.title || "Untitled Lead"}
                  </h2>
                  <span className="bg-blue-50 text-blue-700 text-xs px-2.5 py-1 rounded-full font-medium whitespace-nowrap">
                    {lead.leadStage?.name || "Unassigned"}
                  </span>
                </div>

                {lead.query && (
                  <p className="text-sm text-gray-600 line-clamp-2">
                    {lead.query}
                  </p>
                )}
              </div>

              <div className="pt-3 border-t border-gray-100 text-xs text-gray-500 space-y-1">
                <div className="flex justify-between">
                  <span>Source:</span>
                  <span className="font-medium text-gray-700">
                    {lead.source || "N/A"}
                  </span>
                </div>
                <div className="flex justify-between">
                  <span>Est. Value:</span>
                  <span className="font-semibold text-green-600">
                    ${lead.estimatedValue ?? 0}
                  </span>
                </div>
                <div className="flex justify-between">
                  <span>Score:</span>
                  <span className="font-medium text-gray-700">
                    {lead.currentScore}
                  </span>
                </div>
                {false && lead.User && (
                  <div className="flex justify-between">
                    <span>Assigned To:</span>
                    <span className="font-medium text-gray-700">
                      {lead?.User?.fullName || lead?.User?.email || "User"}
                    </span>
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Pagination Controls */}
      <div className="flex justify-between items-center pt-4">
        <button
          onClick={() => setPage((prev) => Math.max(prev - 1, 1))}
          disabled={page === 1 || isPending}
          className="px-4 py-2 border rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Previous
        </button>
        <button
          onClick={() => setPage((prev) => prev + 1)}
          disabled={isPending || (leads && leads.length === 0)}
          className="px-4 py-2 border rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Next
        </button>
      </div>
    </div>
  );
};