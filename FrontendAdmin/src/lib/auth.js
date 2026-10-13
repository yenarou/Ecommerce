const TOKEN_KEY = 'ecommerce_admin_token'
const USER_KEY = 'ecommerce_admin_user'

export function saveSession(data) {
  localStorage.setItem(TOKEN_KEY, data.token || data.Token)
  localStorage.setItem(USER_KEY, JSON.stringify(data))
}

export function getToken() {
  if (typeof window === 'undefined') return null
  return localStorage.getItem(TOKEN_KEY)
}

export function getSession() {
  if (typeof window === 'undefined') return null
  const raw = localStorage.getItem(USER_KEY)
  return raw ? JSON.parse(raw) : null
}

export function clearSession() {
  localStorage.removeItem(TOKEN_KEY)
  localStorage.removeItem(USER_KEY)
}

export function isAuthenticated() {
  return Boolean(getToken())
}
