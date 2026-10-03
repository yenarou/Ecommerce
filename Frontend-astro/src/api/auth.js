const API_BASE = (import.meta.env.PUBLIC_API_URL || 'http://localhost:14001/graphql').replace(/\/graphql\/?$/, '')
const AUTH_URL = import.meta.env.PUBLIC_AUTH_URL || API_BASE

const USE_MOCK = false

function mockAuth(username) {
  return {
    token: 'token-de-prueba',
    userId: '11111111-1111-1111-1111-111111111111', 
    username,
  }
}

async function post(path, body) {
  let res
  try {
    res = await fetch(`/api/auth/${path}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body),
    })
  } catch (err) {
    console.error('Error de red o CORS:', err)
    throw new Error('No se pudo conectar con el servidor. Intenta de nuevo.')
  }

    if (!res.ok) {
    let message = null
    try {
      message = (await res.json()).message
    } catch {}
    throw new Error(
      message ||
        (path === 'login'
          ? 'No se pudo iniciar sesión. Revisa tu correo y contraseña.'
          : 'No se pudo crear la cuenta. Revisa tus datos o prueba con otro correo.')
    )
  }
  return res.json()
}

export async function login(email, password) {
  if (USE_MOCK) return mockAuth(email.split('@')[0])
  return post('login', { email, password })
}

export async function register(username, email, password) {
  if (USE_MOCK) return mockAuth(username)
  return post('register', { username, email, password })
}

export async function getGoogleUrl() {
  if (USE_MOCK) return null
  const res = await fetch(`${AUTH_URL}/api/v1/auth/google/url`)
  if (!res.ok) throw new Error('No se pudo obtener el enlace de Google')
  const data = await res.json()
  return data.url
}

export async function getMe(token) {
  if (USE_MOCK) return { username: 'mock' }
  const res = await fetch('/api/auth/me', {
    headers: { Authorization: `Bearer ${token}` },
  })
  if (res.status === 401) return null
  if (!res.ok) throw new Error('No se pudo validar la sesión')
  return res.json()
}