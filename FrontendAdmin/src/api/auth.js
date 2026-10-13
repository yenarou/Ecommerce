import { AUTH_URL } from '../lib/config'

export async function loginAdmin(email, password) {
  const response = await fetch(`${AUTH_URL}/api/v1/auth/admin/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password })
  });

  const data = await response.json().catch(() => ({}))
    if (!response.ok) throw new Error(data.detail || data.message || data.title || 'Correo o contraseña incorrectos.')
  return data
}
