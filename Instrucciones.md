

#### Requisitos 

* &#x20;MySQL 8 instalado y corriendo .
* &#x20;Python 3 con las dependencias:

  * &#x20;pip install fastapi uvicorn sqlalchemy pymysql
* &#x20;Base de datos creada. Ejecuta schema.sql en MySQL (o copiar el las lineas de código)
* &#x20;Contraseña coherente. El usuario uno\_api debe tener la misma clave que usa unoApi.py (ClaveSegura2026). 





#### Paso 1 Iniciar MySQL





#### Paso 2 Lanzar la API

Colocar la siguiente linea de comando en VisualStudioCode, directamente en el archivo de unoApi

uvicorn unoApi:app --host 0.0.0.0 --port 5000



#### Paso 3 Lanzar el servidor de juego

Primero compilar archivos en la carpeta UnoServer e iniciar en VisualStudio (UnoGame y UnoServer), después en UnoServer/bin/Debug aparecerá el archivo ejecutable de UnoServer.exe importante ejecutar el .exe en el administrador de archivos o terminal, en UnoGame/bin/Debug saldrá el .exe pero del juego como tal





\- Si se ejecuta en terminal ./UnoServer.exe bots 127.0.0.1 11111 0 3 agrega bots para probar el juego



Debe imprimir 

Servidor UNO escuchando en el puerto 11111

Los jugadores deben conectarse a: x.x.x.x

en la terminal

LA MAYORIA DE VECES SALDRAN VARIAS, USAR LA RED LOCAL 



#### Paso 4 Configurar Firewall(Solo host)

En PowerShell como administrador



New-NetFirewallRule -DisplayName "UNO Servidor" -Direction Inbound -Protocol TCP -LocalPort 11111 -Action Allow

New-NetFirewallRule -DisplayName "UNO API" -Direction Inbound -Protocol TCP -LocalPort 5000 -Action Allow



#### &#x20;Como descargar correctamente desde el repositorio de github

* Descargar el zip del repositorio
* Abrir la carpeta en VS
* Se descarga sin /bin ni .exe
* Buscar y clickear el archivo UnoGame.slnx (morado)
* Solución "UnoGame" >> Restaurar paquetes de NuGeT >> compilar Solución
* En PowerShell colocar el siguiente comando  en carpeta padre para desbloquear archivos de internet 

Get-ChildItem -Recurse | Unblock-File




Comprobar conectividad de PC Cliente a PC Servidor
* Test-NetConnection 192.168.x.x -Port 11111

Modificar el firewall de PC Servidor
* New-NetFirewallRule -DisplayName "UNO Servidor" -Direction Inbound -Protocol TCP -LocalPort 11111 -Action Allow -Profile Any
* Get-NetFirewallRule -Action Block -Direction Inbound | Where-Object DisplayName -like "*UnoServer*" | Remove-NetFirewallRule

Cambiar de Red Pública a Privada
* Set-NetConnectionProfile -NetworkCategory Private