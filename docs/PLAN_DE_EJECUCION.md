# Syncrash: estado y próximos pasos

**28/09/2026.** El alcance actual es **Steam vanilla**. v1.0.6 está preparada para publicación; la descarga vigente sigue siendo v1.0.5. **Es una v1 experimental y esperamos feedback de los jugadores.** Community Mod queda para después.

## Versión 1.0.6.0 preparada

Un único EXE con protección de cierres y LAA. Pantalla adaptable con suavizado y reparación de voces son opciones independientes, marcadas por defecto y desactivables. El juego se abre normalmente, también desde `gbr.exe`. La [ficha técnica](CANDIDATO_1.0.6.md) concentra hashes, fuentes, pruebas, recuperación y límites.

Voces cubre español, italiano e inglés con WAV sueltos extraídos de los PAK identificados; el cambio de idioma se realiza al reaplicar. Las pruebas históricas y la sesión multijugador de 1.0.4 siguen en su [ficha](CANDIDATO_1.0.4.md): no se atribuyen al nuevo aplicador ni a las voces. No se exigen nuevos análisis antivirus; se mantienen comprobaciones de integridad y funcionamiento.

## Siguiente paso

Publicar el [candidato v1.0.6](CANDIDATO_1.0.6.md), tras preparación desde commit limpio y CI. Casillas unificadas, enlaces al pie y correcciones documentales probados; no incorpora conectividad.

Recopilar feedback de uso normal con versión, idioma, opciones activadas, duración y resultado, también cuando todo funciona. Guardar/recargar con el filtro final, otros GPU/DPI y cambios de monitor en caliente permanecen como cobertura adicional, sin repetir las pruebas ya completadas.

Ante un incidente, conservar los Logs de ambos antes de volver a abrir el juego y seguir la [guía sencilla](PRUEBAS_SIN_OBSERVADOR.md). El observador es opcional; no se publican datos personales. La investigación extensa queda en `work/`.

## Preparación de conectividad

El [plan de conexión online automática](CONEXION_ONLINE.md) está preparado para una futura casilla opcional, independiente de pantalla y voces. Se conservarán las salas existentes sin nuevos servidores externos. **No hay implementación ni pruebas de red de esta función.** El próximo trabajo es reproducir el fallo de entrada en dos PC e identificar sockets, interfaz y negociación antes de preparar el prototipo de mapeos temporales. El documento enlazado define fases, retirada y matriz de aceptación; no cambia la entrega 1.0.6.

## Investigación posterior

1. Localizar una causa de desync Steam a partir de registros pareados y validar cualquier corrección con ambos clientes iguales.
2. Ampliar compatibilidad de pantalla a partir de incidencias concretas, conservando el modo 1080p que ya funciona.
3. Recoger feedback de voces en los tres idiomas, cambios de idioma, retirada desde la casilla y multijugador. Mapeo, integración, decodificación y pruebas sobre copias están completados; la escucha completa y las partidas siguen pendientes. La publicación experimental está autorizada; no presentar esa autorización como validación auditiva. La investigación permanece en `work/` y el estado de entrega en la [ficha pública](ENTREGA_ACTUAL.md).

Las pruebas privadas anteriores de la protección de cierres (incluida la sesión registrada de unos 79 minutos) conservan su fecha y alcance en el historial. No se trasladan a un EXE distinto como si se hubieran repetido.
