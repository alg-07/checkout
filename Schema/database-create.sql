CREATE DATABASE checkout
    WITH
    OWNER = checkout_admin
    ENCODING = 'UTF8'
    CONNECTION LIMIT = -1;

\c checkout

CREATE TABLE IF NOT EXISTS public.basket
(
    id SERIAL PRIMARY KEY,
    customer VARCHAR(50) NOT NULL,
    pays_vat BOOLEAN NOT NULL,
    is_closed BOOLEAN NOT NULL,
    is_payed BOOLEAN NOT NULL
);

CREATE TABLE IF NOT EXISTS public.item
(
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    price NUMERIC(2) NOT NULL,
    basket_id INT NOT NULL,
    CONSTRAINT fk_basket FOREIGN KEY(basket_id) REFERENCES basket(id)
);
