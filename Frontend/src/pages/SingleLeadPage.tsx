import { useQuery } from "@tanstack/react-query";
import { useParams, useSearchParams } from "react-router"
import { getLead } from "../services/leadService";
import { useEffect, useState } from "react";
import { useUser } from "../store/userStore";

// Allows the user to select a single user for a lead
const UsersModel = ({setOpen}) =>{
    return <div></div>
}

export const SingleLeadPage = () => {
    const {id} = useParams<{id: string}>();
    const leadId = Number(id);
    const { data: lead, isPending, isError } = useQuery({ queryFn: () => getLead(leadId), queryKey: ["lead", leadId],
     })
    const role = useUser((state)=> state.user?.role.name);
    const [open, setOpen] = useState(false);
    // If the role sales manager he can assign it to any user
    return (
        <div>
            {isPending && <div>Loading</div>}
            
            {isError && <div>Error while loading lead</div>}
            {/* {!lead && <div>Lead not found</div>} */}
            {lead &&
                <div
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
                    {open && <UsersModel setOpen={setOpen}/>}
                    {role == "Sales Manager" && <button className="bg-blue-500 p-2 rounded-xl w-fit text-white hover:bg-blue-600" onClick={()=>setOpen(true)}>Assign to Sales Representative</button>}
                </div>}
        </div>
    )

}