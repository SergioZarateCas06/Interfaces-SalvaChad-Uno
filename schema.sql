CREATE DATABASE IF NOT EXISTS unogameproyecto;
USE unogameproyecto;

CREATE TABLE Partidas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fecha DATETIME NOT NULL
);

CREATE TABLE ParticipantesPartida (
    id INT AUTO_INCREMENT PRIMARY KEY,
    partida_id INT NOT NULL,
    nombre_jugador VARCHAR(100) NOT NULL,
    gano BOOLEAN NOT NULL DEFAULT FALSE,
    FOREIGN KEY (partida_id) REFERENCES Partidas(id)
);

CREATE TABLE Movimientos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    partida_id INT NOT NULL,
    numero_turno INT NOT NULL,
    nombre_jugador VARCHAR(100) NOT NULL,
    accion VARCHAR(50) NOT NULL,       -- "Jugar", "Robar", "ComerPenalizacion"
    descripcion_carta VARCHAR(50),     -- ej. "Rojo 7", "Comodin +4", null si fue robar
    fecha DATETIME NOT NULL
);

CREATE USER 'uno_api'@'localhost' IDENTIFIED BY 'hanasatominori'; -- Despues del IDENTIFIED BY se pone la contraseña del usuario
GRANT ALL PRIVILEGES ON unogameproyecto.* TO 'uno_api'@'localhost';
FLUSH PRIVILEGES;