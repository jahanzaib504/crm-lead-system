import LoaderAnimation from "../components/LoaderAnimation";
import ErrorMessage from "../components/ErrorMessage";
import { getCompanies } from "../services/companyService";
import { useQuery } from "@tanstack/react-query";
import type { Company } from "../types/Company";
import { Layers } from "lucide-react";
const CompaniesPage = () => {
    const { data: companies, isPending, error } = useQuery<Company[]>({ queryKey: ["companies"], queryFn: getCompanies });

    if (isPending) return <LoaderAnimation />;
    if (error) return <ErrorMessage message="Failed to load companies." />;

    return (
        <div>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-5">
                <div className="bg-white p-5 rounded-xl border border-gray-200 shadow-xs">
                    <div className="flex items-center justify-between text-gray-500 mb-2">
                        <span className="text-xs font-semibold uppercase tracking-wider">Total Companies</span>
                        <div className="w-8 h-8 rounded-lg bg-blue-50 text-blue-600 flex items-center justify-center">
                            <Layers className="w-4 h-4" />
                        </div>
                    </div>
                    <div className="text-2xl font-bold text-gray-900">{companies?.length ?? 0}</div>
                    
                </div>
            </div>
            <div className="mt-5 bg-white p-5 rounded-xl border border-gray-200 shadow-xs">
                <h2 className="text-lg font-semibold text-gray-900 mb-4">Companies</h2>
                {companies && companies.length > 0 ? (
                    <div className="overflow-x-auto">
                        <table className="min-w-full divide-y divide-gray-200">
                            <thead className="bg-gray-50">
                                <tr>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Name</th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Industry</th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Employees</th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Annual Revenue</th>
                                </tr>
                            </thead>
                            <tbody className="bg-white divide-y divide-gray-200">
                                {companies.map((company) => (
                                    <tr key={company.id}>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{company.name}</td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{company.industry}</td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{company.employeeCount}</td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">Rs. {company.annualRevenue}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                ) : (
                    <p className="text-gray-500">No companies found.</p>
                )}
            </div>
        </div>
    );
}

export default CompaniesPage;