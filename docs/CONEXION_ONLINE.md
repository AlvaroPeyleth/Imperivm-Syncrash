# Propuesta: conexión online automática

**27/09/2026 · Diseño preparado; implementación y pruebas de red pendientes.** La base vigente es [Syncrash v1.0.6](ENTREGA_ACTUAL.md). Esta función todavía no está en el aplicador ni en la descarga. Este documento concentra alcance, decisiones y criterios del piloto; no asigna una nueva versión.

## Objetivo y límites

Activar una opción en Syncrash, abrir Imperivm (versión Steam) y crear o entrar en las salas habituales sin configurar puertos manualmente, en las redes compatibles. Se conserva el servicio de salas existente; no se añaden VPN, cuentas, STUN público, coordinadores, relays ni telemetría externa. El control del router local forma parte de la propuesta.

La apertura automática sí hace accesibles puertos del juego y tiene riesgo de exposición. No equivale a acceso automático a los archivos del PC, pero tampoco ofrece riesgo cero. CGNAT, doble NAT, políticas de firewall o falta de coordinación pueden impedir conectar. Mejorar el acceso a una partida no corrige por sí mismo cierres ni desincronizaciones. La cobertura se medirá; no hay porcentaje de éxito demostrado.

## Casilla y comportamiento previsto

- **Etiqueta:** «Conexión online automática (experimental)».
- **Ayuda:** «Intenta facilitar la entrada a las salas habituales. Puede abrir temporalmente los puertos del juego en routers compatibles. Algunas redes seguirán necesitando configuración. Desmarca y aplica para retirarlo».
- Independiente de pantalla y voces. Desmarcada por defecto en instalaciones nuevas durante el piloto; al reabrir el aplicador, reflejar el estado instalado y verificado, sin reactivaciones silenciosas.
- Marcar y pulsar **Aplicar parche**, con el juego cerrado, instala y configura el componente. Abrir el aplicador o marcar la casilla no actúa sobre la red.
- Después de aplicar, se cierra Syncrash y se abre Imperivm (versión Steam) como siempre, también directamente desde `gbr.exe`. No se requiere navegador, pestaña web, aplicador abierto, otro launcher ni un proceso o servicio auxiliar permanente. Solo se vuelve al aplicador para cambiar opciones, actualizar o retirar la función.
- El juego carga automáticamente el componente instalado en su propio proceso en cada arranque; el trabajo de red se activa al usar el multijugador. Al salir, termina el componente y se retiran sus mapeos; ante un cierre inesperado, la concesión finita debe limitar su duración. Este ciclo es un requisito del prototipo pendiente de validar.
- Desmarcar y aplicar desactiva y retira solo los componentes propios de red. Conserva la protección de cierres, LAA, voces y pantalla, incluidos los recursos de carga compartidos que aún necesiten otras opciones. Informar de cualquier limpieza de mapeos pendiente; no mostrar retirada completa si no se comprobó.
- Si una vía no está disponible, terminar el intento adicional con tiempo y recursos acotados, conservar el comportamiento nativo y ofrecer un diagnóstico local. No exigir una nueva interacción en cada partida para repetir la configuración elegida.

## Secuencia que se investigará

| Vía | Trabajo previsto | Condición para habilitarla |
| --- | --- | --- |
| Conexión nativa | Observar creación de sala, intento de entrada, negociación y tráfico del juego. Mantener los casos que ya conectan. | Identificar señales reales de fase y conexión, interfaz, sockets y extremos usados en la base exacta. |
| A: preparación del router | Preparar los canales necesarios al crear/entrar en sala mediante UPnP compatible. Valorar NAT-PMP/PCP como ampliaciones según soporte observado. | Probar el ciclo de concesión, renovación, conflicto y retirada; comprobar el extremo concedido y comunicación real. |
| B: negociación NAT existente | Investigar NatNeg y posible envío coordinado desde los sockets reales del juego, usando solo los servicios actuales. | Demostrar cómo se identifica la sala, cómo se intercambian extremos y cómo participa cada cliente. Si el canal actual no lo permite, esta vía queda fuera del piloto. |
| Sin ruta viable | Registrar el motivo y permitir el flujo normal del juego. | No añadir un servidor, abrir más puertos ni cambiar protecciones como alternativa automática. |

Las direcciones de ingeniería inversa, la función de cada puerto y el soporte NatNeg del servidor actual son pistas pendientes de contrastar con el ejecutable identificado por hash. UDP 40444–40447 es el rango de partida de la investigación; no se abrirá completo por suposición ni se añadirán protocolos sin evidencia. No se presupone que baste con preparar únicamente al anfitrión.

La vía B no depende de una sala genérica en un INI ni identifica jugadores solo por su IP pública: debe distinguir varios PC que compartan salida y reiniciar su estado al cambiar de sala, sockets o red. Una respuesta del router o un datagrama cualquiera no son prueba de conexión al jugador esperado. IPv6 y nuevos servicios quedan fuera de este primer piloto.

## Integración y controles del piloto

El aplicador actual es C#/.NET Framework 4.8 y el juego es x86. Aplicar una vez instala el componente; no abre puertos de forma permanente. El componente integrado en el juego preparará y renovará los mapeos mientras se necesiten: una apertura única durante la instalación no cubre reinicios, cambios de IP ni caducidad. La instalación persiste entre sesiones; la actividad de red depende del uso del multijugador.

Hay que resolver un único punto de carga gestionado por Syncrash: la pantalla ya instala y valida por hash `winmm.dll`. No se puede reemplazar por otro proxy sin adaptar instalación, retirada, manifiestos y pruebas. La decisión entre ampliar el cargador propio o emplear otra vía verificada queda para el prototipo. Red debe funcionar con pantalla desactivada, y retirar pantalla debe conservar red si sigue seleccionada. La inicialización de red debe ocurrir fuera del bloqueo de carga de DLL y del hilo de simulación.

Condiciones técnicas para el candidato:

- Seleccionar el gateway y la dirección local coherentes con el socket/ruta efectiva; no elegir el primer adaptador privado ni deshabilitar WSL, VPN o interfaces del jugador. Registrar incertidumbre cuando no se pueda determinar la ruta.
- Solicitar solo los puertos demostrados, sin DMZ, cambios de DNS, redirección del servicio de salas, desactivación de firewall/antivirus ni reglas amplias de Windows. Un permiso local de Windows puede seguir siendo necesario.
- Preferir concesiones finitas, renovar durante su uso y verificar la retirada de recursos propios. La API de mapeos estáticos de Windows no ofrece un parámetro de caducidad en `Add`; liberar el objeto no elimina el mapeo. **Para el piloto desatendido, si no puede obtenerse una concesión finita verificable, omitir esa vía.** Los mapeos estáticos quedan para investigación controlada hasta resolver explícitamente el riesgo tras un cierre inesperado.
- Conservar mapeos ajenos, incluso si coinciden con un puerto necesario; verificar propiedad antes de renovar o eliminar. Si el gateway concede otro puerto, demostrar que el juego lo anuncia y utiliza, o rechazar esa vía.
- Mantener tiempos de espera, reintentos e hilos acotados. Si se necesitan hooks, cubrir recepción síncrona y asíncrona, cierre/reutilización de sockets y cambio de sesión; validar longitudes, extremos y transacciones sin consumir paquetes legítimos del juego.
- Preservar la protección de cierres y LAA y el comportamiento de simulación, temporización, pantalla y audio. Archivos propios con hashes y registro de propiedad; instalación repetible y recuperación de interrupciones. No sobrescribir archivos ajenos o modificados.
- Diagnóstico local limitado y exportable a petición, sin envío automático. IP, capturas y logs originales permanecen privados; los informes públicos solo incluyen resultados anonimizados.

## Trabajo preparado y puertas de avance

| Fase | Entregable | Criterio para avanzar |
| --- | --- | --- |
| 0. Reproducir y observar | Comparación sin módulo: sala visible, intento fallido y conexión correcta si se logra; hashes, opciones, sockets y trazas pareadas. | Localizar el fallo entre descubrimiento, anuncio de extremos, negociación y transporte. Determinar dónde activar/desactivar el componente sin alterar la simulación. |
| 1. Prototipo local de A | Componente independiente y pruebas con gateway simulado; decisión de carga y política de concesiones. | Probar errores, caducidad, renovación, mapeos ajenos, puertos alternativos y fallos de red; demostrar activación y retirada con pantalla/voces combinadas. |
| 2. Piloto en dos PC | Ensayos en redes de participantes autorizados, alternando anfitrión y opción apagada/encendida. | Entrar en la sala correcta, iniciar y terminar partida; repetir sesión, cambiar de sala y comprobar limpieza normal y tras cierre inesperado. |
| 3. Decisión sobre B | Evidencia del NatNeg efectivo y un diseño de coordinación con las salas actuales. | Solo implementar si existe una vía demostrada sin nuevos servicios; en caso contrario documentar el límite de A. |
| 4. Candidato de entrega | Informe con éxitos y fallos, fuentes/licencias, manifiestos y recuperación. | Revisión de regresiones, build reproducible y pruebas de integración. Actualizar versión/hashes únicamente al existir un candidato real; publicar requiere decisión posterior. |

**Siguiente trabajo concreto:** fase 0. La investigación documental no acredita que el juego o los servicios actuales negocien NAT correctamente. Ninguna fase de prototipo o ensayo se ha ejecutado para esta propuesta.

## Matriz mínima de pruebas

| Escenario | Qué comprobar |
| --- | --- |
| Conexión nativa que ya funciona | Sin regresión al activar/desactivar; tiempos comparables de entrada y partida. |
| Aplicación única y arranques posteriores del juego | Con Syncrash y el navegador cerrados, abrir directamente `gbr.exe`, comprobar la carga del componente dentro del juego y repetir tras reiniciar el PC sin reaplicar; al salir, no dejar procesos auxiliares y comprobar retirada o caducidad de mapeos. |
| Router con control compatible | Canales realmente necesarios, puerto exterior concedido, renovación y retirada. |
| Control deshabilitado, mapeo ocupado o concesión parcial | Fallo acotado, sin modificar recursos ajenos ni anunciar conexión conseguida. |
| Doble NAT, CGNAT y acceso móvil | Qué barrera sigue presente; éxitos y fallos reales sin atribuir el resultado solo a la etiqueta de red. |
| Varios adaptadores y dos jugadores bajo una IP pública | Ruta correcta, identidad de cada jugador y puertos sin colisiones. |
| Solo anfitrión, solo invitado y ambos con la opción | Cobertura real de instalación mixta; no prometer compatibilidad basándose en ensayos privados anteriores de la protección de cierres. |
| Segunda partida, cambio de sala/red, cierre normal e inesperado | Estado renovado, reconexión, caducidad y limpieza verificadas desde el gateway. |
| Ocho combinaciones de pantalla/voces/red | Aplicar, repetir y retirar cada función desmarcando/aplicando, sin eliminar componentes aún necesarios. Seguir el [criterio unificado de casillas](FUNCIONAMIENTO.md#casillas-unificadas). |
| Apagado de red y desinstalación | Ausencia de actividad adicional del componente; regreso al comportamiento nativo y registro de cualquier residuo. |

Empezar con dos PC y ampliar a 2–3 parejas con redes distintas antes de distribuir un piloto más amplio. Esto no demuestra estadísticamente que funcione para la mayoría. Cada intento tendrá fecha, hashes de ambos ejecutables/componentes, opciones, roles, tipo de red observado, mapeos antes/después, fase de fallo, tiempos de entrada, duración de partida y resultado de limpieza. Contabilizar todos los intentos, incluidos fallos y casos sin diagnóstico; no usar solo el contador interno «conectado» como evidencia. Conservar resultados originales en `work/`.

## Referencias y reconocimiento

Gracias a **Upercat** por su investigación comunitaria sobre conectividad de Imperivm, utilizada como referencia para esta propuesta. No se incorpora código de terceros en esta preparación documental. Una futura reutilización requiere revisar permiso/licencia y conservar la atribución correspondiente; los detalles de la revisión de material privado quedan en el archivo interno. [Créditos](CREDITOS.md).

- Base y componentes: [entrega vigente](ENTREGA_ACTUAL.md), [control de pantalla](../src/Syncrash/ScreenCompatibility.cs), [fuentes de pantalla](../src/Screen/README.md).
- UPnP de Windows: [crear mapeo](https://learn.microsoft.com/en-us/windows/win32/api/natupnp/nf-natupnp-istaticportmappingcollection-add) y [eliminar mapeo](https://learn.microsoft.com/en-us/windows/win32/api/natupnp/nf-natupnp-istaticportmappingcollection-remove).
- Concesiones y límites: [NAT-PMP, RFC 6886](https://www.rfc-editor.org/rfc/rfc6886.html), [PCP, RFC 6887](https://www.rfc-editor.org/rfc/rfc6887.html), [comportamiento NAT, RFC 4787](https://www.rfc-editor.org/rfc/rfc4787.html). Los estándares no demuestran soporte del router del jugador.

[Hoja de ruta](ROADMAP.md) · [Plan de ejecución](PLAN_DE_EJECUCION.md)
