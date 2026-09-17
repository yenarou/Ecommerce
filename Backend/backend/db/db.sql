PRAGMA foreign_keys = ON;

CREATE TABLE users (
  id            CHAR(36) PRIMARY KEY,
  username      VARCHAR(50)  NOT NULL,
  email         VARCHAR(50)  NOT NULL UNIQUE,
  password_hash VARCHAR(255),
  google_id     VARCHAR(255),
  auth_provider VARCHAR(10)  NOT NULL DEFAULT 'local' CHECK (auth_provider IN ('local','google'))
);

CREATE TABLE categories (
  id          CHAR(36) PRIMARY KEY,
  slug        VARCHAR(50)  NOT NULL UNIQUE,
  name        VARCHAR(100) NOT NULL,
  description VARCHAR(255)
);

CREATE TABLE products (
  id            CHAR(36) PRIMARY KEY,
  category_id   CHAR(36) NOT NULL REFERENCES categories(id),
  name          VARCHAR(255) NOT NULL,
  description   VARCHAR(255),
  price         DECIMAL(10,2) NOT NULL,
  stock         INT NOT NULL DEFAULT 0,
  image_url     VARCHAR(255),
  image_url_alt VARCHAR(255),
  days_to_make  INT NOT NULL DEFAULT 1,
  created_at    DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE customization (
  id               CHAR(36) PRIMARY KEY,
  description      VARCHAR(255) NOT NULL,
  image_url        VARCHAR(255),
  additional_price DECIMAL(10,2) NOT NULL DEFAULT 0
);

CREATE TABLE carts (
  id         CHAR(36) PRIMARY KEY,
  user_id    CHAR(36) NOT NULL REFERENCES users(id),
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  status     VARCHAR(10) NOT NULL DEFAULT 'active' CHECK (status IN ('active','converted'))
);

CREATE TABLE cart_items (
  id                CHAR(36) PRIMARY KEY,
  cart_id           CHAR(36) NOT NULL REFERENCES carts(id) ON DELETE CASCADE,
  product_id        CHAR(36) NOT NULL REFERENCES products(id),
  customization_id  CHAR(36) REFERENCES customization(id),
  quantity          INT NOT NULL DEFAULT 1
);

CREATE TABLE orders (
  id         CHAR(36) PRIMARY KEY,
  user_id    CHAR(36) NOT NULL REFERENCES users(id),
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  status     VARCHAR(15) NOT NULL DEFAULT 'pending'
             CHECK (status IN ('pending','paid','processing','shipped','delivered','canceled'))
);

-- Renglones de pedido (la 2a. tabla "cart_items" del diagrama, ver nota arriba)
CREATE TABLE order_items (
  id                CHAR(36) PRIMARY KEY,
  order_id          CHAR(36) NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
  product_id        CHAR(36) NOT NULL REFERENCES products(id),
  customization_id  CHAR(36) REFERENCES customization(id),
  quantity          INT NOT NULL DEFAULT 1,
  unit_price         DECIMAL(10,2) NOT NULL -- precio congelado al momento del pedido
);

CREATE INDEX idx_products_category ON products(category_id);
CREATE INDEX idx_cart_items_cart ON cart_items(cart_id);
CREATE INDEX idx_order_items_order ON order_items(order_id);
CREATE INDEX idx_orders_user ON orders(user_id);
CREATE INDEX idx_carts_user ON carts(user_id);

INSERT INTO users (id, username, email, password_hash, auth_provider) VALUES
  ('11111111-1111-1111-1111-111111111111', 'demo', 'demo@example.com', NULL, 'local');

INSERT INTO categories (id, slug, name, description) VALUES
  ('c1000000-0000-0000-0000-000000000001', 'figuras', 'Figuras', 'Figuras de peluche tejidas, ideales para regalo o colección.'),
  ('c1000000-0000-0000-0000-000000000002', 'llaveros', 'Llaveros', 'Piezas pequeñas para mochila, bolsa o llaves.'),
  ('c1000000-0000-0000-0000-000000000003', 'decoracion', 'Decoración', 'Piezas para repisa, escritorio o pared.');

INSERT INTO products (id, category_id, name, description, price, stock, image_url, image_url_alt, days_to_make) VALUES
  ('p1000000-0000-0000-0000-000000000001', 'c1000000-0000-0000-0000-000000000001', 'Shy-Guy', 'Es un personaje vergon tejido a mano de 15cm de alto.', 320.00, 5, '/images/shyguy.png', '/images/shyguy.png', 5),
  ('p1000000-0000-0000-0000-000000000002', 'c1000000-0000-0000-0000-000000000001', 'Mario', 'Fontanero pendejo, le roban a la princesa cada 2 dias, mete buenos vergazos.', 310.00, 3, '/images/marioycapi.png', '/images/mario.png', 4),
  ('p1000000-0000-0000-0000-000000000003', 'c1000000-0000-0000-0000-000000000002', 'Snoop Dogg', 'Perro blanco y negro con los ojos cerrados porque ojos que no ven, corazon que no siente. Rapea durisimo.', 95.00, 20, '/images/snoopy.png', '/images/snoop.png', 1),
  ('p1000000-0000-0000-0000-000000000004', 'c1000000-0000-0000-0000-000000000002', 'Flor del sol', 'Flor pedorra que se parece al sol.', 110.00, 0, '/images/sunflower.png', '/images/snoop.png', 1),
  ('p1000000-0000-0000-0000-000000000005', 'c1000000-0000-0000-0000-000000000003', 'Ramo de flores', 'Flores de crochet.', 560.00, 2, '/images/flores.png', '/images/flores.png', 6),
  ('p1000000-0000-0000-0000-000000000006', 'c1000000-0000-0000-0000-000000000003', 'Mandalas colgantes', 'Decoracion de mandalas para colgar en la pared o ventana no se.', 310.00, 9, '/images/mandalas.png', '/images/snoop.png', 3);

INSERT INTO customization (id, description, image_url, additional_price) VALUES
  ('cz000000-0000-0000-0000-000000000001', 'Cambiar color base', 'https://images.unsplash.com/photo-1520903920243-7ce9d02d3a08?auto=format&fit=crop&w=200&q=60', 0),
  ('cz000000-0000-0000-0000-000000000002', 'Bordar iniciales', 'https://images.unsplash.com/photo-1519241047957-be31d7379a5d?auto=format&fit=crop&w=200&q=60', 60),
  ('cz000000-0000-0000-0000-000000000003', 'Agregar moño o accesorio', 'https://images.unsplash.com/photo-1517705008128-361805f42e07?auto=format&fit=crop&w=200&q=60', 45),
  ('cz000000-0000-0000-0000-000000000004', 'Empaque de regalo', 'https://images.unsplash.com/photo-1549465220-1a8b9238cd48?auto=format&fit=crop&w=200&q=60', 35);
