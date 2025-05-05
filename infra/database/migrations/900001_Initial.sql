CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

CREATE SCHEMA todos;

CREATE TABLE todos.todos (
    id uuid NOT NULL,
    title text NOT NULL,
    completed boolean NOT NULL,
    CONSTRAINT pk_todos PRIMARY KEY (id)
);