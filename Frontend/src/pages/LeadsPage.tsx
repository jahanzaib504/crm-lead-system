import { useQuery } from "@tanstack/react-query";
import { useState } from "react";
import { getLeadsPaged } from "../services/leadService";

export const LeadsPage = () => {
  const [page, setPage] = useState(1);
  const pageSize = 2;

  const { data, isPending, isError } = useQuery({
    queryFn:  () =>  getLeadsPaged(page, pageSize),
    queryKey: ["leads", page],
  });

  const leads = data?.items;
  const totalCount = data?.totalCount ?? 0;
  const totalPages = Math.ceil(totalCount / pageSize) || 1;
  console.log(leads);
  return (
    <div className="max-w-4xl mx-auto p-6 space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center border-b pb-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Leads Overview</h1>
          <p className="text-sm text-gray-500">Manage and track your incoming leads</p>
        </div>
        <span className="text-sm font-medium text-gray-600 bg-gray-100 px-3 py-1 rounded-full">
          Total Leads: {totalCount}
        </span>
      </div>

      {/* Loading Skeleton */}
      {isPending && (
        <div className="space-y-4">
          {[...Array(pageSize)].map((_, i) => (
            <div key={i} className="p-5 border rounded-xl bg-gray-50 animate-pulse space-y-3">
              <div className="h-6 bg-gray-200 rounded w-1/3"></div>
              <div className="h-4 bg-gray-200 rounded w-3/4"></div>
              <div className="h-4 bg-gray-200 rounded w-1/2"></div>
            </div>
          ))}
        </div>
      )}

      {/* Error Banner */}
      {isError && (
        <div className="p-4 bg-red-50 border border-red-200 text-red-700 rounded-lg text-sm flex items-center justify-between">
          <span>Failed to load leads. Please try again later.</span>
        </div>
      )}


      {/* Filteration*/}
      <div className="p-3 border-2">
        <input type="text" placeholder="Search" className="text-lg w-3xs p-2 outline-0 border border-gray-300 rounded-xl"/>
        <div></div>
      </div>
      {/* Leads List */}
      {!isPending && !isError && (
        <div className="flex flex-col gap-4">
          {leads && leads.length > 0 ? (
            leads.map((lead) => (
              <div
                key={lead.id}
                className="bg-white border border-gray-200 rounded-xl p-5 shadow-sm hover:shadow-md transition-shadow duration-200 flex flex-col gap-3"
              >
                {/* Title & Stage */}
                <div className="flex justify-between items-start">
                  <h2 className="text-lg font-semibold text-gray-900">
                    {lead.title || "Untitled Lead"}
                  </h2>
                  {lead.leadStage?.name && (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-50 text-blue-700 border border-blue-200">
                      {lead.leadStage.name}
                    </span>
                  )}
                </div>

                {/* Query description */}
                {lead.query && (
                  <p className="text-sm text-gray-600 bg-gray-50 p-3 rounded-lg border border-gray-100">
                    {lead.query}
                  </p>
                )}

                {/* Details Footer */}
                <div className="grid grid-cols-2 sm:grid-cols-3 gap-4 pt-2 text-sm text-gray-500 border-t border-gray-100">
                  <div>
                    <span className="block text-xs text-gray-400 font-medium">Estimated Budget</span>
                    <span className="font-medium text-gray-800">
                      {lead.estimatedValue ? `$${lead.estimatedValue.toLocaleString()}` : "N/A"}
                    </span>
                  </div>

                  <div>
                    <span className="block text-xs text-gray-400 font-medium">Assigned User ID</span>
                    <span className="font-medium text-gray-800">
                      {lead.assignedToUserId ?? "Unassigned"}
                    </span>
                  </div>

                  <div>
                    <span className="block text-xs text-gray-400 font-medium">Score</span>
                    <span className="font-medium text-gray-800">
                      {lead.currentScore ?? 0}
                    </span>
                  </div>
                </div>
              </div>
            ))
          ) : (
            <div className="text-center py-12 text-gray-500 border border-dashed rounded-xl">
              No leads found.
            </div>
          )}
        </div>
      )}

      {/* Pagination Controls */}
      <div className="flex items-center justify-between border-t border-gray-200 pt-4">
        <span className="text-sm text-gray-600">
          Page <span className="font-medium text-gray-900">{page}</span> of{" "}
          <span className="font-medium text-gray-900">{totalPages}</span>
        </span>

        <div className="flex gap-2">
          <button
            onClick={() => setPage((prev) => (prev - 1 > 0 ? prev - 1 : prev))}
            disabled={page === 1 || isPending}
            className="px-4 py-2 text-sm font-medium border rounded-lg bg-white text-gray-700 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            Previous
          </button>
          <button
            onClick={() => setPage((prev) => prev + 1)}
            disabled={pageSize * page >= totalCount || isPending}
            className="px-4 py-2 text-sm font-medium border rounded-lg bg-white text-gray-700 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            Next
          </button>
        </div>
      </div>
    </div>
  );
};