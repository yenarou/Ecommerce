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
    if (path === 'register' && res.status === 409) {
      throw new Error('Ya existe una cuenta con ese correo. Inicia sesión o usa otro correo.')
    }
    if (res.status === 429) {
      throw new Error('Demasiados intentos. Espera un momento e inténtalo de nuevo.')
    }
    throw new Error(
      path === 'login'
        ? 'Correo o contraseña incorrectos.'
        : 'No se pudo crear la cuenta. Revisa tus datos e inténtalo de nuevo.'
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