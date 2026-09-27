# Syncrash: estado y próximos pasos

**27/09/2026.** El alcance actual es **Steam vanilla**. Community Mod y audio se estudiarán después.

## Candidato 1.0.4.0 preparado en local

Un único EXE con LAA y pantalla adaptable opcional, marcada por defecto e incluyendo suavizado GPU. El juego se abre desde Steam; mantiene la resolución del escritorio, centra la imagen y añade márgenes negros. La [ficha del candidato](CANDIDATO_1.0.4.md) concentra hashes, fuentes, pruebas y recuperación.

- 24 pruebas superadas, dos builds idénticos y 22 operaciones sobre copias reales.
- Preparador ejecutado con Windows PowerShell 5.1 desde el commit limpio `ce2ff9d`, reproduciendo el EXE validado.
- El usuario confirma más de una hora de multijugador con dos equipos Steam vanilla y la misma versión, sin incidencias. Es feedback comunicado; los componentes gráficos no cambian al unificar la casilla del aplicador.
- Monitor, imagen, clics, márgenes y Alt+Tab recibieron confirmación favorable. No se incorpora la resolución interna superior a 1080p del ensayo retirado.

La entrega pública sigue siendo [v1.0.3](ENTREGA_ACTUAL.md). No se ha autorizado subir este candidato. No se exigen nuevos análisis antivirus: [código y transparencia](SEGURIDAD.md). No hay una corrección causal de desync Steam incluida.

## Siguiente paso

Revisar el diff documental y publicar código/EXE solo tras autorización. Después, recopilar feedback de uso normal con versión, duración y configuración. Guardar/recargar con el filtro final, otros GPU/DPI y cambios de monitor en caliente permanecen como cobertura adicional, sin repetir las pruebas ya completadas.

Ante un incidente, conservar los Logs de ambos antes de volver a abrir el juego y seguir la [guía sencilla](PRUEBAS_SIN_OBSERVADOR.md). El observador es opcional; no se publican datos personales. La investigación extensa queda en `work/`.

## Investigación posterior

1. Localizar una causa de desync Steam a partir de registros pareados y validar cualquier corrección con ambos clientes iguales.
2. Ampliar compatibilidad de pantalla a partir de incidencias concretas, conservando el modo 1080p que ya funciona.
3. Estudiar audio y, posteriormente, Community Mod con sus propios archivos identificados y ensayos.

Las pruebas anteriores de V2 (incluida la sesión registrada de unos 79 minutos) conservan su fecha y alcance en el historial. No se trasladan a un EXE distinto como si se hubieran repetido.
