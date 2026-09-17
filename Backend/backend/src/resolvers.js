const { v4: uuidv4 } = require('uuid')
const db = require('./db')

function toProduct(row) {
  if (!row) return null
  return {
    id: row.id,
    name: row.name,
    description: row.description,
    price: row.price,
    stock: row.stock,
    imageUrl: row.image_url,
    imageUrlAlt: row.image_url_alt,
    daysToMake: row.days_to_make,
    createdAt: row.created_at,
    categoryId: row.category_id,
  }
}

function toCategory(row) {
  if (!row) return null
  return { id: row.id, slug: row.slug, name: row.name, description: row.description }
}

function toCustomization(row) {
  if (!row) return null
  return {
    id: row.id,
    description: row.description,
    imageUrl: row.image_url,
    additionalPrice: row.additional_price,
  }
}

const resolvers = {
  Query: {
    //categorias
    categories: () => db.prepare('SELECT * FROM categories ORDER BY name').all().map(toCategory),

    //slugs de categorias
    category: (_parent, { slug }) =>
      toCategory(db.prepare('SELECT * FROM categories WHERE slug = ?').get(slug)),

    //filtro de categorias
    products: (_parent, { page, pageSize, categorySlug }) => {
      const offset = (page - 1) * pageSize

      let where = ''
      const params = []
      if (categorySlug) {
        where = 'WHERE c.slug = ?'
        params.push(categorySlug)
      }

      const totalRow = db
        .prepare(`SELECT COUNT(*) as count FROM products p JOIN categories c ON c.id = p.category_id ${where}`)
        .get(...params)

      const rows = db
        .prepare(
          `SELECT p.* FROM products p JOIN categories c ON c.id = p.category_id
           ${where} ORDER BY p.created_at DESC LIMIT ? OFFSET ?`
        )
        .all(...params, pageSize, offset)

      return {
        items: rows.map(toProduct),
        total: totalRow.count,
        page,
        pageSize,
        totalPages: Math.max(1, Math.ceil(totalRow.count / pageSize)),
      }
    },

    product: (_parent, { id }) => toProduct(db.prepare('SELECT * FROM products WHERE id = ?').get(id)),

    orders: (_parent, { userId }) =>
      db
        .prepare('SELECT * FROM orders WHERE user_id = ? ORDER BY created_at DESC')
        .all(userId)
        .map((row) => ({
          id: row.id,
          status: row.status,
          createdAt: row.created_at,
          updatedAt: row.updated_at,
          userId: row.user_id,
        })),
  },

  Mutation: {
    createProduct: (_parent, { input }) => {
      const id = uuidv4()
      db.prepare(
        `INSERT INTO products (id, category_id, name, description, price, stock, image_url, image_url_alt, days_to_make)
         VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)`
      ).run(
        id,
        input.categoryId,
        input.name,
        input.description ?? null,
        input.price,
        input.stock,
        input.imageUrl ?? null,
        input.imageUrlAlt ?? null,
        input.daysToMake
      )
      return toProduct(db.prepare('SELECT * FROM products WHERE id = ?').get(id))
    },

    updateProduct: (_parent, { id, input }) => {
      const existing = db.prepare('SELECT * FROM products WHERE id = ?').get(id)
      if (!existing) throw new Error(`Producto ${id} no existe`)

      db.prepare(
        `UPDATE products SET category_id = ?, name = ?, description = ?, price = ?, stock = ?,
         image_url = ?, image_url_alt = ?, days_to_make = ? WHERE id = ?`
      ).run(
        input.categoryId,
        input.name,
        input.description ?? null,
        input.price,
        input.stock,
        input.imageUrl ?? null,
        input.imageUrlAlt ?? null,
        input.daysToMake,
        id
      )
      return toProduct(db.prepare('SELECT * FROM products WHERE id = ?').get(id))
    },

    deleteProduct: (_parent, { id }) => {
      const result = db.prepare('DELETE FROM products WHERE id = ?').run(id)
      return result.changes > 0
    },

    createOrder: (_parent, { input }) => {
      const orderId = uuidv4()

      const run = db.transaction(() => {
        db.prepare(`INSERT INTO orders (id, user_id, status) VALUES (?, ?, 'pending')`).run(
          orderId,
          input.userId
        )

        for (const item of input.items) {
          const product = db.prepare('SELECT * FROM products WHERE id = ?').get(item.productId)
          if (!product) throw new Error(`Producto ${item.productId} no existe`)
          if (product.stock < item.quantity) {
            throw new Error(`No hay stock suficiente de "${product.name}"`)
          }

          let unitPrice = product.price
          if (item.customizationId) {
            const custom = db
              .prepare('SELECT * FROM customization WHERE id = ?')
              .get(item.customizationId)
            if (!custom) throw new Error(`Personalización ${item.customizationId} no existe`)
            unitPrice += custom.additional_price
          }

          db.prepare(
            `INSERT INTO order_items (id, order_id, product_id, customization_id, quantity, unit_price)
             VALUES (?, ?, ?, ?, ?, ?)`
          ).run(uuidv4(), orderId, item.productId, item.customizationId ?? null, item.quantity, unitPrice)

          db.prepare('UPDATE products SET stock = stock - ? WHERE id = ?').run(
            item.quantity,
            item.productId
          )
        }
      })

      run()

      const row = db.prepare('SELECT * FROM orders WHERE id = ?').get(orderId)
      return { id: row.id, status: row.status, createdAt: row.created_at, updatedAt: row.updated_at, userId: row.user_id }
    },
  },


  Category: {
    products: (category) =>
      db.prepare('SELECT * FROM products WHERE category_id = ?').all(category.id).map(toProduct),
  },

  Product: {
    category: (product) =>
      toCategory(db.prepare('SELECT * FROM categories WHERE id = ?').get(product.categoryId)),
    customizationOptions: () => db.prepare('SELECT * FROM customization').all().map(toCustomization),
  },

  Order: {
    user: (order) => db.prepare('SELECT * FROM users WHERE id = ?').get(order.userId),
    total: (order) => {
      const row = db
        .prepare('SELECT SUM(unit_price * quantity) as total FROM order_items WHERE order_id = ?')
        .get(order.id)
      return row.total ?? 0
    },
    items: (order) =>
      db
        .prepare('SELECT * FROM order_items WHERE order_id = ?')
        .all(order.id)
        .map((row) => ({
          id: row.id,
          quantity: row.quantity,
          unitPrice: row.unit_price,
          subtotal: row.unit_price * row.quantity,
          productId: row.product_id,
          customizationId: row.customization_id,
        })),
  },

  OrderItem: {
    product: (item) => toProduct(db.prepare('SELECT * FROM products WHERE id = ?').get(item.productId)),
    customization: (item) =>
      item.customizationId
        ? toCustomization(db.prepare('SELECT * FROM customization WHERE id = ?').get(item.customizationId))
        : null,
  },

  User: {
    authProvider: (user) => user.auth_provider,
  },
}

module.exports = resolvers
