import { messages } from './messages.js'

export const locales = Object.keys(messages)

export function localePrefix(locale) {
  return locale === 'es' ? '' : `/${locale}`
}

export function localizedPath(locale, path = '/') {
  const normalizedPath = path.startsWith('/') ? path : `/${path}`
  const prefix = localePrefix(locale)
  return normalizedPath === '/' ? `${prefix}/` || '/' : `${prefix}${normalizedPath}`
}

export function stripLocalePrefix(pathname) {
  const locale = locales.find((supportedLocale) => (
    pathname === `/${supportedLocale}` || pathname.startsWith(`/${supportedLocale}/`)
  ))

  if (!locale) return pathname || '/'

  return pathname.slice(locale.length + 1) || '/'
}

export function localizeProduct(product, locale) {
  if (!product) return product

  const translation = messages[locale]?.catalog?.products?.[product.name]
  if (!translation) return product

  return {
    ...product,
    name: translation.name,
    description: translation.description,
  }
}

export function t(locale, id, values = {}) {
  const language = Object.hasOwn(messages, locale) ? locale : 'es'
  const message = id.split('.').reduce((value, part) => value?.[part], messages[language])

  if (typeof message !== 'string') {
    throw new Error(`Missing "${id}" translation for locale "${language}".`)
  }

  return message.replace(/\{(\w+)\}/g, (placeholder, key) => {
    if (!Object.hasOwn(values, key)) {
      throw new Error(`Missing "${key}" interpolation for translation "${id}".`)
    }
    return String(values[key])
  })
}

export function localizeCategory(category, locale) {
  if (!category) return category

  const categoryTranslations = messages[locale]?.catalog?.categories
  if (!categoryTranslations) return category

  const bySlug = categoryTranslations[category.slug]
  if (bySlug) {
    return { ...category, name: bySlug.name }
  }

  const spanishCategories = messages.es?.catalog?.categories || {}
  const matchingSlug = Object.entries(spanishCategories).find(([, value]) => (
    value.name === category.name
  ))?.[0]
  const translation = matchingSlug ? categoryTranslations[matchingSlug] : null

  return translation
    ? { ...category, name: translation.name }
    : category
}

export function categorySlugFromName(categoryName) {
  if (!categoryName) return null

  const spanishCategories = messages.es?.catalog?.categories || {}
  const matchingSlug = Object.entries(spanishCategories).find(([, value]) => (
    value.name === categoryName
  ))?.[0]

  return matchingSlug || null
}
