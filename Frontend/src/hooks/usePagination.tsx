import { useQuery } from "@tanstack/react-query";
import { useEffect, useState } from "react"
import type { PaginationResponse } from "../types/PaginationResponse";

interface PaginationInterface<T>{
    keys: any[],
    page?: number,
    pageSize?: number,
    fn: ()=> Promise<PaginationResponse<T>>
}

export const usePagination = <T, >(initial: PaginationInterface<T>) =>{
    const [page, setPage] = useState<number>(initial.page ?? 1);
    const {data, isPending, isError} = useQuery<PaginationResponse<T>>({
        queryKey: initial.keys.concat([page]), 
        queryFn: initial.fn});
    const pageSize = initial.pageSize ?? 8;
    const totalCount = data?.totalCount ?? 0;
    // Reset page to 1 if the keys change
    useEffect(()=>{
        setPage(1)
    }, [initial.keys])
    const prevButton = <button disabled={isPending || isError || (page-1 < 1)} onClick={()=> page > 1 && setPage(page-1)}>Prev</button>;
    const nextButton = <button disabled={isPending || isError || (page*pageSize >= totalCount)} onClick={()=> page*pageSize < totalCount && setPage(page+1)}>Next</button>;

    return {data, prevButton, nextButton, page, pageSize, isPending, isError, totalCount};
}