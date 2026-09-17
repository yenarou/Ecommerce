import './Footer.css'

export default function Footer() {
  return (
    <footer className="footer">
      <div className="container footer__inner">
        <div>
          <h2 className="footer__brand">Hilo &amp; Alma</h2>
          <p className="footer__text">Taller de crochet | Zona Metropolitana de Guadalajara</p>
        </div>
        <div className="footer__cols">
          <div>
            <h3 className="footer__col-title">Contactos</h3>
            <p className="footer__text">genCasBet@hiloyalma.mx</p>
            <p className="footer__text">diegBenGon@hiloyalma.mx</p>
            <p className="footer__text">andSanSot@hiloyalma.mx</p>
          </div>
          <div>
            <h3 className="footer__col-title">Envíos</h3>
            <p className="footer__text">Entrega local y envío nacional</p>
            <p className="footer__text">Piezas hechas por encargo</p>
          </div>
        </div>
      </div>
      <p className="footer__legal">© {new Date().getFullYear()} Hilo y Alma</p>
    </footer>
  )
}
