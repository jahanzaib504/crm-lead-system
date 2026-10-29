import { useState } from "react";

export const WithdrawModal = ({ onClose, onWithdraw }: { onClose: () => void; onWithdraw: () => void }) => {
    
    const [isWithdrawing, setIsWithdrawing] = useState(false);
    const handleWithdraw = async () => {
        setIsWithdrawing(true);
        await onWithdraw();
        setIsWithdrawing(false);
    }   
    return (

        <div className="fixed inset-0 flex items-center justify-center z-50 bg-black/65">
            <div className="bg-white rounded-lg p-6 w-96">
                <h2 className="text-lg font-semibold mb-4">Confirm Withdrawal</h2>
                <p className="mb-4">Are you sure you want to withdraw this lead? This action cannot be undone.</p>
                <div className="flex justify-end gap-2">
                    <button
                        onClick={onClose}
                        disabled={isWithdrawing}
                        className="px-4 py-2 bg-gray-300 hover:bg-gray-400 text-gray-800 rounded-lg"
                    >
                        Cancel
                    </button>
                    <button
                        onClick={handleWithdraw}
                        disabled={isWithdrawing}
                        className="px-4 py-2 bg-green-600 hover:bg-green-700 text-white rounded-lg"
                    >
                        {isWithdrawing ? "Withdrawing..." : "Withdraw"}
                    </button>
                </div>
            </div>
        </div>
    );

}