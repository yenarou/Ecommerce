import { useEffect, useState } from 'react';
import { useTranslations } from '../i18n/ui.js';

export default function InstallButton({ locale = 'es' }) {
  const t = useTranslations(locale);
  const [disponible, setDisponible] = useState(false);

  useEffect(() => {
    if (window.matchMedia('(display-mode: standalone)').matches) return;
    if (window.__pwaPrompt) setDisponible(true);

    const mostrar = () => setDisponible(true);
    const ocultar = () => setDisponible(false);
    window.addEventListener('pwa-installable', mostrar);
    window.addEventListener('pwa-installed', ocultar);
    return () => {
      window.removeEventListener('pwa-installable', mostrar);
      window.removeEventListener('pwa-installed', ocultar);
    };
  }, []);

  if (!disponible) return null;

  const instalar = async () => {
    const evento = window.__pwaPrompt;
    if (!evento) return;
    evento.prompt();
    await evento.userChoice;
    window.__pwaPrompt = null;
    setDisponible(false);
  };

  return (
    <button type="button" className="install-btn" onClick={instalar}>
      {t('pwa.instalar')}
    </button>
  );
}