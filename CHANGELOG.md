# Historial de versiones

## v1.0.3 · Aplicador 1.0.3.0 · 24 de septiembre de 2026

- `--test` ya no aplica el parche. `--check` solo lee y `--apply` es la única orden de aplicación por línea de comandos; cada resultado tiene su propio código de salida.
- Si el parche exacto ya está instalado, no se reescribe `gbr.exe`. Syncrash impide dos aplicaciones simultáneas sobre el mismo juego y vuelve a comprobar los archivos justo antes de sustituirlos.
- Mensajes de error más claros y eliminación del temporal propio. Ante un acceso denegado ya no se recomienda abrir el programa como administrador.
- Compilación determinista con .NET SDK 8.0.400 y 14 pruebas sintéticas y de línea de comandos superadas en Windows PowerShell 5.1; preparador local con validación de versión y compilación.
- Salidas de línea de comandos en UTF-8, incluso sin consola, y ayuda de permisos para excepciones internas sin perder el estado del archivo.
- El preparador lee la versión de `AssemblyInfo.cs`, también para las instrucciones y el ZIP. CI limita `push` a `main` y actualiza a `actions/checkout@v7` y `actions/setup-dotnet@v6` (últimas releases comprobadas: v7.0.1 y v6.0.0, con Node.js 24).
- Misma versión en ensamblado, archivo y manifiesto; los metadatos indican la empresa editora. La receta y los hashes del juego no cambian.

**Publicado como entrega experimental v1.0.3.** EXE final tras la auditoría: **2.403.840 bytes**, SHA256 `251c92e0cc17dec527086349d7e065705b33f9373e9b1a907485f4716f07d50c`. El [informe del candidato](docs/CANDIDATO_1.0.3.md) recoge las pruebas y lo que falta. El 24/09/2026 se repitió el ensayo con este EXE en una copia aislada de archivos reales admitidos: produjo el hash V2 esperado, no tocó `data.pak`, no volvió a escribir al repetir y conservó el original ante un reemplazo denegado. La verificación anterior del hash `1407eddc…910d812` se conserva como histórica. Código confirmado en main, empaquetado desde árbol limpio y CI remoto correctos. Se publicaron el EXE, sus sumas y la licencia para recoger feedback; siguen pendientes las comprobaciones de partida y antivirus del hash exacto. Estos cambios no alteran la entrega v1.0.0 ni añaden correcciones de desync.

## Documentación y distribución · 24 de septiembre de 2026

- Captura de la interfaz actual publicada en el README.
- Solicitud de revisión antivirus enviada a Microsoft; la resolución final seguía pendiente en la última consulta.
- Se preparó la firma digital Authenticode con Azure Artifact Signing y se descartó el mismo día, sin firmar ningún EXE. Syncrash se distribuye sin firma y cada ficha de entrega publica el SHA256 del archivo.
- Reglas de mantenimiento documental y separación entre documentación pública y archivo interno en `AGENTS.md`.

Estos avances no cambian el binario, la protección ni la versión de la entrega v1.

## Syncrash v1 · 23 de septiembre de 2026

Primera entrega pública experimental para Steam vanilla. Versión del aplicador: **1.0.2.0**.

- Un solo `Syncrash.exe`, con búsqueda de la instalación y aplicación mediante botón.
- Protección de tres llamadas mediante comprobación del tipo de objeto; misma guarda V2 de las pruebas privadas.
- Validación de `gbr.exe`, `Packs/data.pak` y resultado por SHA256.
- Reaplicación de la versión exacta ya instalada, sin crear copia de seguridad.
- Ayuda integrada sin conexión, enlace al código y a la última descarga.
- Icono e ilustración propios, licencia MIT y créditos.

**No incluye correcciones de desync Steam. Community Mod sigue desactivado.** Para recuperar vanilla, verifica los archivos desde Steam o reinstala el juego.

Las denominaciones V1/V2 de investigación anteriores identifican versiones internas de la guarda; no son paquetes públicos adicionales.
