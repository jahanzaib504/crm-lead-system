import { useState } from "react";

export const DeleteModal = ({ isOpen, onClose, onDelete, label }: { isOpen: boolean; onClose: () => void; onDelete: () => void; label: string }) => {
    if (!isOpen) return null;
    const [isDeleting, setIsDeleting] = useState(false);
    const handleDelete = async () => {
        setIsDeleting(true);
        await onDelete();
        setIsDeleting(false);
    }   
    return (

        <div className="fixed inset-0 flex items-center justify-center z-50 bg-black/65">
            <div className="bg-white rounded-lg p-6 w-96">
                <h2 className="text-lg font-semibold mb-4">Confirm Deletion</h2>
                <p className="mb-4">Are you sure you want to delete this {label}? This action cannot be undone.</p>
                <div className="flex justify-end gap-2">
                    <button
                        onClick={onClose}
                        disabled={isDeleting}
                        className="px-4 py-2 bg-gray-300 hover:bg-gray-400 text-gray-800 rounded-lg"
                    >
                        Cancel
                    </button>
                    <button
                        onClick={handleDelete}
                        disabled={isDeleting}
                        className="px-4 py-2 bg-red-600 hover:bg-red-700 text-white rounded-lg"
                    >
                        {isDeleting ? "Deleting..." : "Delete"}
                    </button>
                </div>
            </div>
        </div>
    );

}