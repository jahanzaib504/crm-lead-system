interface Role{
    id: number,
    name: string
}
export interface User{
    id: number,
    fullName: string,
    email: string,
    role: Role,
    roleId: number,
    managerId: number | null,
    isActive: boolean

    createdAt: string | null,
    createdBy: string | null,
    updatedAt: string | null,
    updatedBy: string | null,
    isDeleted: boolean | null
}