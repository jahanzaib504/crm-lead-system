
const getCurrentDate = (): string => {
    // Get current date in user's local timezone and format it as YYYY-MM-DD
    const now = new Date();
    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, "0");
    const day = String(now.getDate()).padStart(2, "0");
    return `${year}-${month}-${day}`;
}
const getCurrentDateTime = (): string => {
    // Get current date and time in user's local timezone and format it as YYYY-MM-DDTHH:MM
    const now = new Date();
    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, "0");
    const day = String(now.getDate()).padStart(2, "0");
    const hours = String(now.getHours()).padStart(2, "0");
    const minutes = String(now.getMinutes()).padStart(2, "0");
    const v = `${year}-${month}-${day}T${hours}:${minutes}`;
    console.log("Current DateTime:", v);
    return v;
}
export { getCurrentDate, getCurrentDateTime };