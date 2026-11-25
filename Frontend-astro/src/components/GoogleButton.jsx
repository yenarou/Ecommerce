import { useState } from 'react'
import { getGoogleUrl } from '../api/auth'
import { useTranslations } from '../i18n/ui.js'

export default function GoogleButton({ locale = 'es' }) {
  const t = useTranslations(locale)
  const [error, setError] = useState('')

  async function handleClick() {
    setError('')
    try {
      const url = await getGoogleUrl()
      if (!url) {
        setError(t('au.googleNo'))
        return
      }
      window.location.href = url
    } catch (err) {
      setError(err.message)
    }
  }

  return (
    <>
      <button type="button" className="btn auth-google" onClick={handleClick}>
        {t('au.google')}
      </button>
      {error && <p className="auth-form__error" role="alert">{error}</p>}
    </>
  )
}