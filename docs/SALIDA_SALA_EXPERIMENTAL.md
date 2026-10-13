# Protecciones experimentales de salida de sala y emparejamiento

**13/10/2026 · versión `1.0.9` (aplicador `1.0.9.0`) publicada como experimental.** Estado de entrega y hashes en la [ficha de entrega](ENTREGA_ACTUAL.md). Esta ficha reúne el alcance y las comprobaciones de las dos protecciones online añadidas a las cuatro de v1.0.8.

## Fallo y alcance

El registro analizado muestra una lectura inválida de `0x60` en `0x0072BAF5`: la rutina recibe un puntero de conexión nulo y lee un campo sin comprobarlo. La pila y el código de referencia sitúan el acceso en la salida de la sala de preparación de partida. La identificación como `peerLeaveRoomA/W` es una inferencia estática por comparación con el SDK GameSpy, sin símbolos del ejecutable del jugador.

La protección redirige la entrada `0x0072BAF0` a un helper de 18 bytes en `0x00A95080`: conserva el prólogo y comprueba el puntero. Si es cero, salta al epílogo existente; si es distinto de cero, vuelve a `0x0072BAF5` para continuar el código original. Evitar esta lectura no demuestra que el estado online quede limpio ni resuelve por sí mismo el origen del puntero nulo. El RC2 protege también `0x0072B160`, identificada como `peerStopAutoMatch`: redirige siete bytes a un helper de 20 bytes en `0x00A950A0`; si el peer es cero, retorna por la salida existente `0x0072B17B`; si no es cero, conserva la lectura original y continúa en `0x0072B167`. Así se protege la llamada que precede a salir de sala en la ruta `0x00712EF0`. El destructor utiliza otras funciones después: estas guardas no cubren toda la destrucción del contexto, punteros no nulos dañados ni todos los cierres de menús. El registro recibido no permite certificar el ejecutable completo ni atribuirlo a una versión concreta de Syncrash. Los registros y volcados de jugadores permanecen privados.

## Archivos y mantenimiento individual

El flujo normal de v1.0.9 integra seis protecciones y LAA para Steam vanilla. La compatibilidad general de Community sigue pendiente; el PAK Community v12 exacto admite únicamente las órdenes individuales de las protecciones.

| Estado de `gbr.exe` | SHA256 |
| --- | --- |
| v1.0.8, cuatro protecciones y LAA | `cd35437004a441b7e70a8f0fd606003dea6c1e084667a7d813736c5d82a456cc` |
| v1.0.9 / RC2, seis protecciones y LAA | `f34b4684bf65eab26fa7e6f65ec5ffd196846d7fb4f0c0501a620cf9f280db04` |
| v1.0.9 / RC2 con las dos protecciones online y sin protección de maldición | `ad10ac2ffbc6dfad3029585798b302dfeb11cd70a141f2924485f680125d42a1` |

Los tres estados ocupan 4.460.544 bytes. Las recetas identifican por hash tanto entrada como resultado; no admiten cualquier ejecutable con direcciones similares.

Con el juego cerrado, usando Syncrash v1.0.9:

```powershell
Syncrash.exe --check-menu-guard 'RUTA/gbr.exe'
Syncrash.exe --apply-menu-guard 'RUTA/gbr.exe'
Syncrash.exe --remove-menu-guard 'RUTA/gbr.exe'
```

Las órdenes `menu-guard` gestionan juntas las protecciones de salida de sala y emparejamiento. Actualizan también RC1 con o sin maldición. La retirada individual conserva el estado de la protección de maldición. Retirar maldición en v1.0.9 conserva a su vez el paquete online. El mantenimiento de maldición sobre un RC1 conserva su protección de sala sin actualizarla silenciosamente. Aplicar normalmente v1.0.9 en vanilla reinstala ambas. Para recuperar el original completo, retirar primero pantalla y voces desde sus casillas y verificar después los archivos en Steam.

## Validación de v1.0.9

El aplicador final pasó **40 pruebas automáticas y 86 operaciones CLI sobre copias reales**, incluyendo las migraciones, mantenimiento individual y opciones de pantalla/voces de la matriz RC2. Dos compilaciones completas son idénticas: **18.608.128 bytes**, SHA256 `145414c5019f888a89164dba85d5ee2c83e406e8e2e15a97ac372c2029f54999`. El delta nativo se conserva respecto a RC2; los ensayos emulados de los candidatos se mantienen abajo con su versión, sin atribuirlos a una nueva ejecución. No se ha iniciado el juego ni validado continuidad real, reconexión o multijugador. Publicación, CI y descarga comprobados: [fuente de compilación y entrega](ENTREGA_ACTUAL.md).

## Histórico RC2 · 13/10/2026

El RC2 pasó **40 pruebas automáticas y 86 operaciones CLI sobre copias reales**: migraciones desde versiones anteriores y RC1 con/sin maldición, retirada y reinstalación individuales, Steam/Community limitado, pantalla y voces en tres idiomas. Cuatro escenarios emulados sobre la imagen RC2 exacta contrastan detener emparejamiento con puntero nulo y no nulo: el nulo original falla y el candidato retorna; los recorridos válidos conservan los registros y banderas comparados hasta el retorno o la llamada final a la rutina de emparejamiento. Esa llamada final no se ejecuta en el emulador; no se simula el backend online. El aplicador completo ocupa **18.608.128 bytes**, SHA256 `974a8e1e1367b3639224d8c8934c95c153c118eaab6f0f80c6aa3aa21f870019`; dos compilaciones completas son idénticas. No se ha iniciado el juego ni validado continuidad real, reconexión o multijugador. No se ha publicado el candidato ni ejecutado CI remoto para él.

## Histórico RC1 · 13/10/2026

El RC1 final tenía solo la protección de salida de sala: aplicador de **18.597.376 bytes**, SHA256 `98095509591f41a23d6f903f13bbd9596d79f1975a8d8ccc63994878d0985e57`; juego con cinco protecciones y LAA `0c3d71821ac06a55f937e6d9ab38550e6ce14aa6f2acde213a13d2bb880a1c79`, sin maldición `c62ac2f66e687413b4568095d9f5a87e15d7ff6a0ffcc188090dd587a11cbc77`. Pasó 40 pruebas automáticas, 68 operaciones sobre copias y cuatro escenarios emulados, con dos compilaciones idénticas. La ruta activa emulada se detenía antes de ejecutar la limpieza de red completa. Esos resultados corresponden a RC1; las evidencias originales y el primer build previo a corregir los mensajes CLI se conservan en el archivo privado de trabajo. Las pruebas de [v1.0.8](MALDICION_EXPERIMENTAL.md) conservan también su fecha y alcance.

## Investigación y siguiente paso

La investigación estática localiza una ruta concreta: el objeto se construye con el campo de conexión a cero; la inicialización puede devolver cero y el juego guarda ese resultado. El menú comprueba que existe el objeto, pero no su conexión interna. Los retornos nulos examinados incluyen un estado global incompatible, un fallo de reserva o un fallo de inicialización interna. Esto refuerza la hipótesis de inicialización fallida; no demuestra cuál ocurrió en el incidente. No se ha localizado evidencia suficiente para atribuir el cero a una desconexión normal. Falta observar la secuencia real con el ejecutable exacto y un volcado.

La prueba de aceptación debe cubrir salir de una sala creada y de una sala unida, volver al menú, entrar de nuevo en otra sala y repetir el ciclo, incluyendo conexión fallida y desconexión previa. Registrar versión y hash, secuencia exacta y resultado; conservar el volcado del fallo de forma privada. Hasta superar esas pruebas, esta protección sigue siendo experimental.

## Defensas comunes evaluadas

No se ha añadido un filtro global de handles: el registro comparte objetos y buffers, algunos llamadores ignoran los errores y las búsquedas revisadas acceden directamente a la tabla. Devolver un valor genérico o filtrar por tipo podría cambiar comportamientos válidos sin resolver el origen. La ampliación aceptada se limita a la API de emparejamiento con salida local existente. Otras defensas requieren demostrar el contrato y el manejo del error en sus llamadores.
