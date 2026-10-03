const AUTH_URL = import.meta.env.PUBLIC_AUTH_URL || 'http://localhost:5095'

// Cuando Genaro confirme que ya funciona: poner USE_MOCK en false.
const USE_MOCK = true

function mockAuth(username) {
  return {
    token: 'token-de-prueba',
    userId: '11111111-1111-1111-1111-111111111111',
    username,
  }
}

async function post(path, body) {
  const res = await fetch(`${AUTH_URL}/api/v1/auth/${path}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  })
  if (!res.ok) {
    let message = 'Error de autenticación'
    try {
      const data = await res.json()
      message = data.message || data.title || message
    } catch {}
    throw new Error(message)
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
  return typeof data === 'string' ? data : data.url
}