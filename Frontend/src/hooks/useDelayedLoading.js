import {useEffect, useState} from 'react'

export function useDelayedLoading(ms=5000, deps=[]){
    const [isLoading, setIsLoading] = useState(true)

    useEffect(()=>{
        setIsLoading(true)
        const timer = setTimeout(()=> setIsLoading(false), ms)
        return () => clearTimeout(timer)
    }, deps)
    return isLoading
}