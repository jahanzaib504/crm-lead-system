import { usePagination } from "../hooks/usePagination"
import type { AuditLog } from "../types/AuditLog"
import { getAuditLogsPaged } from "../services/auditLogsService"

export const AuditLogsPage = () => {
    const { data, isPending, isError, page, pageSize, totalCount, nextButton, prevButton } = usePagination<AuditLog>({
        keys: ["audit-logs"],
        fn: (page:number, pageSize:number) => getAuditLogsPaged(page, pageSize)
    })
    const logs = data?.items;

    return (
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-10 space-y-8">
            {/* Header Section */}
            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 border-b border-zinc-200 pb-6">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight text-zinc-900">Audit Logs</h2>
                    <p className="text-sm text-zinc-500 mt-1">Review activity, actions, and system changes performed by users.</p>
                </div>
                {totalCount !== undefined && (
                    <div className="inline-flex items-center px-3 py-1.5 rounded-full text-xs font-medium bg-zinc-100 text-zinc-800 border border-zinc-200 w-fit">
                        Total Logs: <span className="font-semibold ml-1">{totalCount}</span>
                    </div>
                )}
            </div>

            {/* Content Section */}
            <div className="bg-white rounded-xl shadow-sm border border-zinc-200 overflow-hidden">
                {isPending && (
                    <div className="flex items-center justify-center py-16 text-zinc-500">
                        <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-zinc-900 mr-3"></div>
                        Loading audit logs...
                    </div>
                )}

                {isError && (
                    <div className="p-6 bg-red-50 text-red-700 text-sm rounded-lg flex items-center justify-center m-6">
                        Failed to load audit logs. Please try again later.
                    </div>
                )}

                {!isPending && !isError && (
                    <>
                        <div className="overflow-x-auto">
                            <table className="w-full text-left border-collapse">
                                <thead>
                                    <tr className="border-b border-zinc-200 bg-zinc-50/75 text-xs font-semibold text-zinc-500 uppercase tracking-wider">
                                        <th className="py-3.5 px-6">ID</th>
                                        <th className="py-3.5 px-6">Timestamp</th>
                                        <th className="py-3.5 px-6">User ID</th>
                                        <th className="py-3.5 px-6">Action / Entity</th>
                                        <th className="py-3.5 px-6">Changes</th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-zinc-200 text-sm text-zinc-700">
                                    {logs?.length === 0 ? (
                                        <tr>
                                            <td colSpan={5} className="py-12 text-center text-zinc-400">
                                                No audit logs found.
                                            </td>
                                        </tr>
                                    ) : (
                                        logs?.map((log) => (
                                            <tr key={log.id} className="hover:bg-zinc-50/50 transition-colors">
                                                <td className="py-4 px-6 font-mono text-xs text-zinc-500">{log.id}</td>
                                                <td className="py-4 px-6 whitespace-nowrap text-zinc-600">{log.timestamp}</td>
                                                <td className="py-4 px-6 font-mono text-xs text-zinc-600">{log.performedByUserId}</td>
                                                <td className="py-4 px-6">
                                                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-md text-xs font-medium bg-zinc-100 text-zinc-800">
                                                        {log.entityName}
                                                    </span>
                                                </td>
                                                <td className="py-4 px-6 font-mono text-xs text-zinc-500 max-w-xs truncate" title={log.changesJson}>
                                                    {log.changesJson}
                                                </td>
                                            </tr>
                                        ))
                                    )}
                                </tbody>
                            </table>
                        </div>

                        {/* Pagination Footer */}
                        <div className="flex items-center justify-between px-6 py-4 bg-zinc-50/50 border-t border-zinc-200">
                            <div className="text-sm text-zinc-500">
                                Showing page items
                            </div>
                            <div className="flex items-center gap-4">
                                <div className="border border-zinc-800 py-1 px-3 rounded-lg hover:bg-amber-50">{prevButton}</div>
                                <div className="border border-zinc-800 py-1 px-3 rounded-lg hover:bg-amber-50">{nextButton}</div>
                            </div>
                        </div>
                    </>
                )}
            </div>
        </div>
    )
}