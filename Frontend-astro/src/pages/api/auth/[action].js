export const prerender = false

const BACKEND = (process.env.API_INTERNAL_URL || import.meta.env.PUBLIC_API_URL || 'http://localhost:14001/graphql')
  .replace(/\/graphql\/?$/, '')

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
    return new Response(await res.text(), {
      status: res.status,
      headers: { 'Content-Type': res.headers.get('Content-Type') ?? 'application/json' },
    })
  } catch {
    return new Response('Backend no disponible', { status: 502 })
  }
}