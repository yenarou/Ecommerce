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
