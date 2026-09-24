# Historial de versiones

## Documentación y distribución — 24 de septiembre de 2026

- Captura de la interfaz actual publicada en el README.
- Solicitud de revisión antivirus enviada a Microsoft; resolución final pendiente en la última consulta.
- Verificación empresarial de Azure Artifact Signing iniciada para firmar futuras entregas. El EXE publicado continúa sin firma.
- Reglas de mantenimiento documental y separación entre documentación pública y archivo interno en `AGENTS.md`.

Estos avances no cambian el binario, la protección ni la versión de la entrega v1.

## Syncrash v1 — 23 de septiembre de 2026

Primera entrega pública experimental para Steam vanilla. Versión del aplicador: **1.0.2.0**.

- Un solo `Syncrash.exe`, con búsqueda de la instalación y aplicación mediante botón.
- Protección de tres llamadas mediante comprobación del tipo de objeto; misma guarda V2 de las pruebas privadas.
- Validación de `gbr.exe`, `Packs/data.pak` y resultado por SHA256.
- Reaplicación de la versión exacta ya instalada, sin crear respaldo.
- Ayuda integrada sin conexión, enlace al código y a la última descarga.
- Icono e ilustración propios, licencia MIT y créditos.

**No incluye correcciones de desync Steam. Community Mod sigue desactivado.** Para recuperar vanilla, verifica los archivos desde Steam o reinstala el juego.

Las denominaciones V1/V2 de investigación anteriores identifican versiones internas de la guarda; no son paquetes públicos adicionales.
