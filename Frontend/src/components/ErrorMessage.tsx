
const ErrorMessage = ({ message }: { message: string }) => {
    return (
        <div className="p-4 bg-red-50 border border-red-200 text-red-700 rounded-xl text-sm">
            {message}
        </div>
    )
}

export default ErrorMessage;