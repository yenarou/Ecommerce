export function GET({ request, site }) {
  const origin = (site ?? new URL(request.url)).origin
  const body = `User-agent: *
Allow: /
Disallow: /admin
Disallow: /carrito
Disallow: /checkout
Disallow: /ordenes
Disallow: /pago/
Disallow: /login
Disallow: /registro
Disallow: /api/

Sitemap: ${origin}/sitemap.xml
`
  return new Response(body, {
    headers: { 'Content-Type': 'text/plain; charset=utf-8' },
  })
}