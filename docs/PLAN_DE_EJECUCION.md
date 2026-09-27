# Syncrash: estado y próximos pasos

**27/09/2026.** El alcance actual es **Steam vanilla**. La publicación de v1.0.5 está autorizada y en preparación. **Es una v1 experimental y esperamos feedback de los jugadores.** Community Mod queda para después.

## Versión 1.0.5.0

Un único EXE con protección de cierres y LAA. Pantalla adaptable con suavizado y reparación de voces son opciones independientes, marcadas por defecto y desactivables. El juego se abre desde Steam. La [ficha técnica](CANDIDATO_1.0.5.md) concentra hashes, fuentes, pruebas, recuperación y límites.

Voces cubre español, italiano e inglés con WAV sueltos extraídos de los PAK identificados; el cambio de idioma se realiza al reaplicar. Las pruebas históricas y la sesión multijugador de 1.0.4 siguen en su [ficha](CANDIDATO_1.0.4.md): no se atribuyen al nuevo aplicador ni a las voces. No se exigen nuevos análisis antivirus; se mantienen comprobaciones de integridad y funcionamiento.

## Siguiente paso

Recopilar feedback de uso normal con versión, idioma, opciones activadas, duración y resultado, también cuando todo funciona. Guardar/recargar con el filtro final, otros GPU/DPI y cambios de monitor en caliente permanecen como cobertura adicional, sin repetir las pruebas ya completadas.

Ante un incidente, conservar los Logs de ambos antes de volver a abrir el juego y seguir la [guía sencilla](PRUEBAS_SIN_OBSERVADOR.md). El observador es opcional; no se publican datos personales. La investigación extensa queda en `work/`.

## Investigación posterior

1. Localizar una causa de desync Steam a partir de registros pareados y validar cualquier corrección con ambos clientes iguales.
2. Ampliar compatibilidad de pantalla a partir de incidencias concretas, conservando el modo 1080p que ya funciona.
3. Recoger feedback de voces en los tres idiomas, cambios de idioma, retirada desde la casilla y multijugador. Mapeo, integración, decodificación y pruebas sobre copias están completados; la escucha completa y las partidas siguen pendientes. La publicación experimental está autorizada; no presentar esa autorización como validación auditiva. La investigación permanece en `work/` y el estado de entrega en la [ficha pública](ENTREGA_ACTUAL.md).

Las pruebas anteriores de V2 (incluida la sesión registrada de unos 79 minutos) conservan su fecha y alcance en el historial. No se trasladan a un EXE distinto como si se hubieran repetido.
