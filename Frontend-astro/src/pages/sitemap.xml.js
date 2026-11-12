import { fetchCategories, fetchProducts } from '../api/catalog'

export async function GET({ request, site }) {
  const origin = (site ?? new URL(request.url)).origin

  const categories = await fetchCategories()

  const first = await fetchProducts({ page: 1, pageSize: 50 })
  const products = [...first.items]
  for (let page = 2; page <= first.totalPages; page += 1) {
    const next = await fetchProducts({ page, pageSize: 50 })
    products.push(...next.items)
  }

  const urls = [
    '/',
    ...categories.map((c) => `/categoria/${c.slug}`),
    ...products.map((p) => `/products/${p.id}`),
  ]

  const xml = `<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
${urls.map((u) => `  <url><loc>${origin}${u}</loc></url>`).join('\n')}
</urlset>`

  return new Response(xml, {
    headers: { 'Content-Type': 'application/xml; charset=utf-8' },
  })
}