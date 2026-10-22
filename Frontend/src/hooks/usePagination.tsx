import { useQuery } from "@tanstack/react-query";
import { useEffect, useState } from "react"
import type { PaginationResponse } from "../types/PaginationResponse";

interface PaginationInterface<T>{
    keys: any[], // Use string array or primitive key
    page?: number,
    pageSize?: number,
    fn: (page: number, pageSize: number) => Promise<PaginationResponse<T>>
}

export const usePagination = <T, >(initial: PaginationInterface<T>) =>{
    const [page, setPage] = useState<number>(initial.page ?? 1);
    const pageSize = initial.pageSize ?? 8;

    const { data, isPending, isError } = useQuery<PaginationResponse<T>>({
        queryKey: [...initial.keys, page, pageSize], 
        queryFn: () => initial.fn(page, pageSize)
    });
    
    const totalCount = data?.totalCount ?? 0;

    // Reset page to 1 if the key values change (use JSON.stringify or join for stable dependency)
    useEffect(() => {
        setPage(1);
    }, [JSON.stringify(initial.keys)]);

    const prevButton = <button disabled={isPending || isError || (page - 1 < 1)} onClick={() => page > 1 && setPage(page - 1)}>Prev</button>;
    const nextButton = <button disabled={isPending || isError || (page * pageSize >= totalCount)} onClick={() => page * pageSize < totalCount && setPage(page + 1)}>Next</button>;

    return { data, prevButton, nextButton, page, pageSize, isPending, isError, totalCount };
}