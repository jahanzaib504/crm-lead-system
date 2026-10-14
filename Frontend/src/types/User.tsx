
export interface User{
    id: number,
    fullName: string,
    email: string,
    role: string,
    managerId: number | null,
    isActive: boolean

    createdAt: string | null,
    createdBy: string | null,
    updatedAt: string | null,
    updatedBy: string | null,
    isDeleted: boolean | null
}