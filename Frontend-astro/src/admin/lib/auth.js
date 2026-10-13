import { useAuth } from '../../stores/auth'

export function getToken() {
  if (typeof window === 'undefined') return null
  return useAuth.getState().user?.token ?? null
}

export function getSession() {
  return useAuth.getState().user ?? null
}

export function clearSession() {
  useAuth.getState().logout()
}

export function isAuthenticated() {
  return Boolean(getToken())
}