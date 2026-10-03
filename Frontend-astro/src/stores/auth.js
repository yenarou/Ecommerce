import { create } from 'zustand'
import { persist } from 'zustand/middleware'

export const useAuth = create(
  persist(
    (set) => ({
      user: null, // { userId, username, token }
      setUser: (user) => set({ user }),
      logout: () => set({ user: null }),
    }),
    { name: 'auth' }
  )
)