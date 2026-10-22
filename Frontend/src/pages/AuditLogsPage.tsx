
export const AuditLogsPage = () => {
    
    return (
        <div>
            <div className="flex flex-col gap-2">
                <h2 className="text-2xl font-semibold">Audit Logs</h2>
                <p className="text-lg text-zinc-700">Review Activity by users</p>
            </div>
            <div>
                <table>
                    <thead>
                        <tr>
                            <th>DateTime</th>
                            <th>Username</th>
                            <th>UserId</th>
                            <th>Actions</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        
                    </tbody>
                </table>
            </div>
        </div>
        )
}