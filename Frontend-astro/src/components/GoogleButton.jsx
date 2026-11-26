import { useState } from 'react'
import { getGoogleUrl } from '../api/auth'
import { t } from '../i18n'

export default function GoogleButton({ locale = 'es' }) {
  const [error, setError] = useState('')

  async function handleClick() {
    setError('')
    try {
      const url = await getGoogleUrl()
      if (!url) {
        setError(t(locale, 'auth.googleUnavailable'))
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
        {t(locale, 'auth.continueGoogle')}
      </button>
      {error && <p className="auth-form__error" role="alert">{error}</p>}
    </>
  )
}