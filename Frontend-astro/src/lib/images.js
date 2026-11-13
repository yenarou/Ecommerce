export function thumbUrl(url = '') {
  return url.endsWith('.webp') ? url.replace(/\.webp$/, '-400.webp') : url
}