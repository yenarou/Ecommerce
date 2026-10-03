export const prerender = false

const BACKEND = (process.env.API_INTERNAL_URL || import.meta.env.PUBLIC_API_URL || 'http://localhost:14001/graphql')
  .replace(/\/graphql\/?$/, '')

function reply(res, body) {
  return new Response(body, {
    status: res.status,
    headers: { 'Content-Type': res.headers.get('Content-Type') ?? 'application/json' },
  })
}

export async function POST({ params, request }) {
  const action = params.action
  if (action !== 'login' && action !== 'register') {
    return new Response('No encontrado', { status: 404 })
  }

  try {
    const res = await fetch(`${BACKEND}/api/v1/auth/${action}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: await request.text(),
    })
    return reply(res, await res.text())
  } catch {
    return new Response('Backend no disponible', { status: 502 })
  }
}

export async function GET({ params, request }) {
  if (params.action !== 'me') {
    return new Response('No encontrado', { status: 404 })
  }

  const headers = {}
  const auth = request.headers.get('Authorization')
  if (auth) headers.Authorization = auth

  try {
    const res = await fetch(`${BACKEND}/api/v1/auth/me`, { headers })
    return reply(res, await res.text())
  } catch {
    return new Response('Backend no disponible', { status: 502 })
  }
}