# Protocolo de mensajes UNO (cliente ↔ servidor)

Este documento describe cómo se comunican **UnoGame** (cliente) y **UnoServer** (servidor).
La fuente de verdad es `Protocolo.cs` (namespace `UnoGame.Red`), que el servidor agrega "como vínculo". Si este documento y el código se contradicen, **gana el código** y hay que corregir el documento.

## 1. Reglas generales

1. **Un mensaje = una línea de texto JSON terminada en `\n`.** TCP es un flujo de bytes sin separaciones, así que el salto de línea es el delimitador. El JSON nunca contiene saltos de línea reales.
2. **Codificación UTF-8.**
3. **Transporte:** socket TCP. Puerto por defecto: **11111** (`Protocolo.PuertoPorDefecto`).
4. **Un solo tipo de mensaje con campos opcionales** (clase `Mensaje`). El campo `tipo` indica cuáles campos se usan.
5. **Nombres de campos en camelCase** (`cartaSuperior`, `tuMano`), y al leer no importan las mayúsculas.
6. **Los campos `null` no se envían.** Si un campo no aparece, es que no aplica.
7. **El servidor decide si una acción es válida.** El cliente solo *pide* (`jugar`, `robar`, ...); si la acción no es válida, el servidor responde con `error` y el estado de la partida no cambia. El cliente nunca debe asumir que su jugada se aplicó hasta recibir el `evento` + `estado` correspondiente.
8. **Cada jugador recibe un `estado` personalizado:** ve su mano completa y, de los demás, solo cuántas cartas tienen.
9. Si el otro lado cierra la conexión, `Recibir()` devuelve `null`.
10. Los envíos están protegidos con un candado (`Enviar` puede llamarse desde varios hilos sin mezclar mensajes).

## 2. Estructura del mensaje (`Mensaje`)

| Campo | Tipo JSON | Se usa en |
|---|---|---|
| `tipo` | string | **todos** (valor de `Tipos`) |
| `nombre` | string | `unirse` |
| `indice` | int | `bienvenida`, `fin` |
| `carta` | `CartaDto` | `jugar` |
| `color` | string | `jugar`, `elegirColor` |
| `texto` | string | `evento`, `error`, `rechazo`, `fin`, `abortada`, `sala` |
| `nombres` | array de string | `sala` |
| `estado` | `EstadoJuego` | `estado` |

### `CartaDto`

| Campo | Tipo | Valores |
|---|---|---|
| `numero` | int | 0–9 (en cartas especiales no tiene significado) |
| `color` | string | `"Rojo"`, `"Azul"`, `"Verde"`, `"Amarillo"`, `"Ninguno"` |
| `tipo` | string | `"Numero"`, `"Salta"`, `"Reversa"`, `"Mas2"`, `"Comodin"`, `"Mas4"` |

Si `color` o `tipo` no corresponden a un valor real, el servidor lo trata como carta inválida (`TryACarta` devuelve `false`).

### `EstadoJuego`

| Campo | Tipo | Descripción |
|---|---|---|
| `nombres` | array de string | Nombres de todos los jugadores, por asiento |
| `tuIndice` | int | Tu asiento dentro de `nombres` |
| `turno` | int | Asiento al que le toca jugar |
| `sentido` | int | `1` o `-1` |
| `colorActivo` | string | Color que se debe seguir |
| `cartaSuperior` | `CartaDto` | Carta visible en la mesa |
| `cartasBurra` | int | Cartas que quedan para robar |
| `cartasDescarte` | int | Cartas en el descarte |
| `cartasPorJugador` | array de int | Cuántas cartas tiene cada jugador |
| `tuMano` | array de `CartaDto` | Tu mano completa |
| `cartaPendiente` | `CartaDto` | Comodín robado cuyo color debes elegir (se omite si no hay) |
| `puedesDecirUno` | bool | Si ahora puedes cantar UNO |
| `terminada` | bool | Si la partida ya terminó |

## 3. Mensajes del cliente → servidor (5)

| `tipo` | Campos | Cuándo se envía |
|---|---|---|
| `unirse` | `nombre` | Al conectarse: "quiero entrar con este nombre" |
| `jugar` | `carta`, `color` (solo si es comodín) | "Juego esta carta" |
| `robar` | — | "Robo una carta de la burra" |
| `elegirColor` | `color` | Elegir color del comodín que se robó (`cartaPendiente`) |
| `decirUno` | — | Cantar "UNO" |

**Ejemplos**

`unirse`
```json
{"tipo":"unirse","nombre":"Ana"}
```

`jugar` (carta normal)
```json
{"tipo":"jugar","carta":{"numero":5,"color":"Rojo","tipo":"Numero"}}
```

`jugar` (comodín: lleva el color elegido)
```json
{"tipo":"jugar","carta":{"numero":0,"color":"Ninguno","tipo":"Comodin"},"color":"Azul"}
```

`robar`
```json
{"tipo":"robar"}
```

`elegirColor`
```json
{"tipo":"elegirColor","color":"Verde"}
```

`decirUno`
```json
{"tipo":"decirUno"}
```

## 4. Mensajes del servidor → cliente (8)

| `tipo` | Campos | Significado |
|---|---|---|
| `bienvenida` | `indice` | Te aceptaron; este es tu asiento |
| `rechazo` | `texto` | No te aceptaron (sala llena, nombre repetido...) |
| `sala` | `nombres`, `texto` | Quiénes están esperando |
| `estado` | `estado` | Cómo está la mesa (personalizado para cada jugador) |
| `evento` | `texto` | Texto de lo que acaba de pasar |
| `error` | `texto` | Tu acción no fue válida |
| `fin` | `indice`, `texto` | La partida terminó (`indice` = asiento ganador) |
| `abortada` | `texto` | Alguien se fue y se canceló la partida |

**Ejemplos**

`bienvenida`
```json
{"tipo":"bienvenida","indice":0}
```

`rechazo`
```json
{"tipo":"rechazo","texto":"El nombre Ana ya está en uso"}
```

`sala`
```json
{"tipo":"sala","nombres":["Ana","Luis"],"texto":"Esperando jugadores..."}
```

`estado` (visto por Ana, asiento 0; se omite `cartaPendiente` porque es `null`)
```json
{"tipo":"estado","estado":{"nombres":["Ana","Luis"],"tuIndice":0,"turno":0,"sentido":1,"colorActivo":"Rojo","cartaSuperior":{"numero":5,"color":"Rojo","tipo":"Numero"},"cartasBurra":93,"cartasDescarte":1,"cartasPorJugador":[7,7],"tuMano":[{"numero":5,"color":"Rojo","tipo":"Numero"},{"numero":2,"color":"Azul","tipo":"Numero"},{"numero":0,"color":"Verde","tipo":"Numero"},{"numero":7,"color":"Amarillo","tipo":"Numero"},{"numero":0,"color":"Rojo","tipo":"Salta"},{"numero":0,"color":"Ninguno","tipo":"Comodin"},{"numero":3,"color":"Azul","tipo":"Numero"}],"puedesDecirUno":false,"terminada":false}}
```

`estado` con comodín robado pendiente (fragmento: aparece `cartaPendiente`)
```json
{"tipo":"estado","estado":{"nombres":["Ana","Luis"],"tuIndice":0,"turno":0,"sentido":1,"colorActivo":"Rojo","cartaSuperior":{"numero":5,"color":"Rojo","tipo":"Numero"},"cartasBurra":92,"cartasDescarte":1,"cartasPorJugador":[8,7],"tuMano":[{"numero":5,"color":"Rojo","tipo":"Numero"}],"cartaPendiente":{"numero":0,"color":"Ninguno","tipo":"Mas4"},"puedesDecirUno":false,"terminada":false}}
```

`evento`
```json
{"tipo":"evento","texto":"Ana jugó Rojo 5"}
```

`error`
```json
{"tipo":"error","texto":"No es tu turno"}
```

`fin`
```json
{"tipo":"fin","indice":0,"texto":"¡Ana ganó la partida!"}
```

`abortada`
```json
{"tipo":"abortada","texto":"Luis se desconectó; la partida se canceló"}
```

> Los textos de `rechazo`, `sala`, `evento`, `error`, `fin` y `abortada` son informativos para mostrar al jugador. **El cliente no debe depender de su contenido exacto**; la lógica se basa en `tipo` y en los demás campos.

## 5. Recorrido de una partida

```
Cliente                                   Servidor
   │ ── unirse {nombre} ───────────────────> │  valida nombre y cupo
   │ <── bienvenida {indice}  (o rechazo) ── │
   │ <── sala {nombres} ─────────────────── │  a todos, cada vez que cambia la sala
   │                                          │  ... la partida arranca ...
   │ <── evento {texto} ─────────────────── │  a todos
   │ <── estado {estado} ────────────────── │  personalizado para cada jugador
   │ ── jugar {carta[,color]} ─────────────> │  el servidor valida
   │ <── evento + estado ────────────────── │  (o error si no fue válida)
   │ ── robar ─────────────────────────────> │
   │ <── evento + estado ────────────────── │  (si salió comodín: estado con cartaPendiente)
   │ ── elegirColor {color} ───────────────> │
   │ ── decirUno ──────────────────────────> │
   │ <── evento + estado ────────────────── │
   │ <── fin {indice, texto} ────────────── │  fin normal
   │ <── abortada {texto} ───────────────── │  o alguien se fue
```

Resumen: **unirse → bienvenida → sala → evento + estado → jugar → evento + estado → fin.**