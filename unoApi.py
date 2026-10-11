from fastapi import FastAPI
from sqlalchemy import create_engine, text
from pydantic import BaseModel
from typing import List, Optional
from datetime import datetime

app = FastAPI()

connStr = "mysql+pymysql://uno_api:hanasatominori@localhost:3306/unogameproyecto" # Despues del uno_api: se pone la contraseña del usuario
engine = create_engine(connStr)


class NuevaPartida(BaseModel):
    jugadores: List[str]

class NuevoMovimiento(BaseModel):
    numero_turno: int
    nombre_jugador: str
    accion: str
    descripcion_carta: Optional[str] = None

class Ganador(BaseModel):
    nombre_jugador: str


@app.get("/")
async def root():
    return {"message": "API de UnoGame activa"}


@app.post("/partidas")
def crear_partida(datos: NuevaPartida):
    with engine.begin() as conn:
        res = conn.execute(
            text("INSERT INTO partidas (fecha) VALUES (:fecha)"),
            {"fecha": datetime.now()}
        )
        partida_id = res.lastrowid

        for nombre in datos.jugadores:
            conn.execute(
                text("""INSERT INTO participantespartida (partida_id, nombre_jugador, gano)
                        VALUES (:partida_id, :nombre, FALSE)"""),
                {"partida_id": partida_id, "nombre": nombre}
            )

        return {"partida_id": partida_id}


@app.post("/partidas/{partida_id}/movimientos")
def registrar_movimiento(partida_id: int, datos: NuevoMovimiento):
    with engine.begin() as conn:
        conn.execute(
            text("""INSERT INTO movimientos
                     (partida_id, numero_turno, nombre_jugador, accion, descripcion_carta, fecha)
                     VALUES (:partida_id, :numero_turno, :nombre_jugador, :accion, :descripcion_carta, :fecha)"""),
            {
                "partida_id": partida_id,
                "numero_turno": datos.numero_turno,
                "nombre_jugador": datos.nombre_jugador,
                "accion": datos.accion,
                "descripcion_carta": datos.descripcion_carta,
                "fecha": datetime.now()
            }
        )
        return {"ok": True}


@app.put("/partidas/{partida_id}/ganador")
def registrar_ganador(partida_id: int, datos: Ganador):
    with engine.begin() as conn:
        conn.execute(
            text("""UPDATE participantespartida SET gano = TRUE
                     WHERE partida_id = :partida_id AND nombre_jugador = :nombre"""),
            {"partida_id": partida_id, "nombre": datos.nombre_jugador}
        )
        return {"ok": True}


@app.get("/historial")
def historial():
    with engine.connect() as conn:
        res = conn.execute(text("""
            SELECT p.id AS partida_id, p.fecha, pp.nombre_jugador, pp.gano
            FROM partidas p
            JOIN participantespartida pp ON pp.partida_id = p.id
            ORDER BY p.fecha DESC
        """))
        filas = [dict(fila) for fila in res.mappings()]
        return filas


@app.get("/estadisticas")
def estadisticas():
    with engine.connect() as conn:
        res = conn.execute(text("""
            SELECT nombre_jugador,
                   SUM(CASE WHEN gano THEN 1 ELSE 0 END) AS victorias,
                   SUM(CASE WHEN NOT gano THEN 1 ELSE 0 END) AS derrotas
            FROM participantespartida
            GROUP BY nombre_jugador
            ORDER BY victorias DESC
        """))
        return [dict(fila) for fila in res.mappings()]


@app.get("/partidas/{partida_id}/movimientos")
def movimientos_de_partida(partida_id: int):
    with engine.connect() as conn:
        res = conn.execute(
            text("SELECT * FROM movimientos WHERE partida_id = :id ORDER BY numero_turno"),
            {"id": partida_id}
        )
        filas = [dict(fila) for fila in res.mappings()]
        return filas