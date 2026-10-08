import { gql } from './client'
import { useAuth } from '../stores/auth'
import { fetchProducts } from './catalog'

export async function updateCart(items) {
  const firstPage = await fetchProducts({ page: 1, pageSize: 100 })
  const currentProducts = [...firstPage.items]

  for (let page = 2; page <= firstPage.totalPages; page += 1) {
    const nextPage = await fetchProducts({ page, pageSize: 100 })
    currentProducts.push(...nextPage.items)
  }

  const resolvedItems = items.map((item) => {
    if (currentProducts.some((product) => product.id === item.product_id)) {
      return item
    }

    const categoryName =
      item.product.category?.name ?? item.product.categoryName
    const matches = currentProducts.filter(
      (product) =>
        product.name === item.product.name &&
        product.category?.name === categoryName,
    )

    if (matches.length !== 1) {
      throw new Error(
        `Ya no se encuentra "${item.product.name}" en el catálogo. Quítalo del carrito y agrégalo de nuevo.`,
      )
    }

    return { ...item, product_id: matches[0].id }
  })

  const mutation = `
    mutation UpdateCart($request: CartRequestInput!) {
      updateCart(request: $request)
    }
  `

  const variables = {
    request: {
      items: resolvedItems.map(item => ({
        productId: item.product_id,
        quantity: item.quantity,
        customization: {
          personalizationDescription: item.customization_text || '',
          wrap: Boolean(item.wrap)
        }
      }))
    }
  }

  const token = useAuth.getState().user?.token
  if (!token) {
    throw new Error('Debes iniciar sesión para actualizar el carrito')
  }

  const headers = {
    Authorization: `Bearer ${token}`
  }

  return gql(mutation, variables, headers)
}
