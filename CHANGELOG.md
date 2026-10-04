# Historial de versiones

## v1.0.7 · Aplicador 1.0.7.0 · 4 de octubre de 2026

- Lleva al EXE los mensajes y ayudas revisados. Se conservan las funciones, las recetas, las voces y los componentes de pantalla de v1.0.6.
- 38 pruebas automáticas con el EXE completo, dos compilaciones idénticas y revisión de la interfaz en dos tamaños. [Archivo, pruebas y límites](docs/CANDIDATO_1.0.7.md). Publicada desde el commit limpio `512e3cf`, con CI correcto y los tres archivos descargados cotejados. EXE: 18.579.968 bytes, SHA256 `af1d7d01b82d7488fef2eec44355d82b4a9fbc4bfd7b2ef21c160d874205c1a4`.

## Revisión de textos · 4 de octubre de 2026

- README, guías y fichas técnicas con frases más directas y menos repeticiones. Se mantienen fechas, hashes, créditos, resultados históricos y pruebas pendientes. La guía de contribución enlaza ahora a la ficha y los comandos actuales.
- Mensajes del aplicador y comentarios del código más claros. Solo cambian textos: la lógica del parche, las recetas y los recursos se conservan.
- La compilación de desarrollo pasó las 38 pruebas automáticas existentes. La revisión inicial no incluyó nuevas partidas ni cambió el EXE publicado de v1.0.6. Los textos se distribuyeron después en v1.0.7, con las comprobaciones indicadas arriba.

## v1.0.6 · Aplicador 1.0.6.0 · 28 de septiembre de 2026

- Pantalla y voces comparten el criterio de marcar/desmarcar y aplicar. Se retira el botón separado de restauración de pantalla, se atenúan las notas de retirada y los enlaces pasan al pie fijo.
- Compilación local, 38 pruebas automáticas y siete acciones de Aplicar en copia aislada, con las cuatro combinaciones de casillas. [Funcionamiento, comprobaciones y límites](docs/FUNCIONAMIENTO.md#casillas-unificadas). Publicada desde commit limpio con CI correcto y descarga cotejada. SHA256: `c3be15d959263a69e32b4abd0623b9cc29e5d162695e4c65fa51977965d8eeb2`; 18.579.968 bytes. No implementa conectividad.

## Documentación · Ajustes de texto y propuesta online · 27 de septiembre de 2026

- Corregidas las instrucciones de inicio: «versión Steam» identifica la edición compatible; se puede abrir Imperivm desde `gbr.exe` o el acceso directo habitual. Ajustado también el mensaje de instalación de pantalla en el código fuente; la corrección se distribuye con v1.0.6. La prueba prevista de conexión online incluye el arranque directo del ejecutable.
- Documentación pública unificada con nombres de funciones y versiones publicadas; se conservan fechas, hashes y resultados históricos. La propuesta online exige aplicar una vez y abrir Imperivm (versión Steam) sin navegador ni aplicador abiertos, con una prueba específica pendiente para ese flujo.
- Preparada la [propuesta de conexión online automática](docs/CONEXION_ONLINE.md): futura casilla independiente, salas actuales, mapeos temporales y evaluación condicionada de la negociación NAT existente, sin nuevos servidores. Incluye fases, compatibilidad con pantalla/voces, retirada y matriz de pruebas.
- Investigación de Upercat reconocida como referencia. La propuesta permanece sin implementar ni probar; no se incorpora código externo de conectividad.

## v1.0.5 · Aplicador 1.0.5.0 · 27 de septiembre de 2026

**v1 experimental: esperamos feedback de los usuarios. Publicada el 27/09/2026.**

- Nueva opción **Reparar voces de unidades**, marcada por defecto y desactivable. Recupera 188 rutas españolas, 188 italianas o 393 inglesas desde los PAK locales, sin modificarlos ni distribuir audios.
- Reaplicar después de cambiar de idioma sustituye las voces propias; desmarcar y aplicar las retira. Migración del ensayo anterior, protección de archivos ajenos/modificados y recuperación de operaciones interrumpidas.
- Mantiene memoria, protección y pantalla de v1.0.4. Escucha completa en italiano/inglés y multijugador con voces pendientes; las pruebas de archivos no se presentan como partidas. [Ficha, hashes y límites](docs/CANDIDATO_1.0.5.md).
- Directorio de [proyectos de la comunidad](docs/PROYECTOS_COMUNIDAD.md) con fichas breves de autor, enlace, utilidad y enfoque. Primera referencia: ImperivmVoicesPatch de Upercat. El detalle de revisión queda en la investigación interna.
- Se conserva la investigación inicial y la corrección del recuento inglés: sus WAV estaban fuera del prefijo examinado.
- EXE publicado: 18.579.968 bytes, SHA256 `187d62824b609385f2dc1371ffd873a25d0f2cb70e0a6292f7f72dd2c57a4118`; recompilado desde commit limpio, CI correcto y descarga cotejada. [Entrega](docs/ENTREGA_ACTUAL.md).

## v1.0.4 · Aplicador 1.0.4.0 · 27 de septiembre de 2026

- LAA para el juego x86 en Windows de 64 bits, conservando la protección de cierres. No se atribuyen mejoras de FPS ni correcciones de desync a este cambio.
- EXE único con componentes de pantalla y fuentes/licencias incrustados; ya no requiere una carpeta externa. Pantalla adaptable incluye suavizado GPU en una sola opción, marcada por defecto y desactivable. El juego se abre normalmente, también desde `gbr.exe`.
- Menú más legible con el filtro GPU: feedback favorable de imagen y fluidez en un equipo. El ensayo HALFTONE por CPU se descartó porque ralentizaba la partida. No se modifican texturas ni se ofrece resolución interna superior a 1080p.
- Sesión de más de una hora sin incidencias comunicada por dos jugadores en Steam vanilla, con la misma versión del paquete anterior. Mismos componentes gráficos en el nuevo aplicador; no se recogió el hash remoto.
- Interfaz simplificada con información desplegable y ayudas al pasar el ratón. Restauración de pantalla y exportación de fuentes accesibles desde el aplicador.
- Aplicador publicado: **18.362.368 bytes**, SHA256 `88814908bccdee383c8d1c6d9e374a31c7a2f450b3d970941944fbc1a101fa3c`. Windows PowerShell 5.1: 24 pruebas, dos compilaciones completas idénticas y 22 operaciones CLI sobre copias reales. Históricos anteriores conservados en la ficha.
- Instalación/retirada por hashes, incluida retirada de la pantalla histórica. Para cambiar una configuración existente, restaurar pantalla y volver a aplicar. Desmarcar conserva lo instalado. Márgenes negros, monitor principal y modo del escritorio conservados.
- Ensayo nativo 3440×1440 retirado tras cierre en partida; PAK original conservado. Otras configuraciones de hardware quedan para ampliar feedback.

- Entrega reproducida desde el commit limpio `ce2ff9d` con Windows PowerShell 5.1, conservando el EXE validado. Documentación de código y fuentes simplificada; antivirus histórico archivado y sin análisis periódicos como requisito.

**Publicada como experimental.** Código en main, release v1.0.4 desde `30aecbc`, CI correcto y EXE idéntico al validado. La [ficha de entrega](docs/ENTREGA_ACTUAL.md) identifica los archivos; la [ficha técnica](docs/CANDIDATO_1.0.4.md) conserva ensayos y límites.

## v1.0.3 · Aplicador 1.0.3.0 · 24 de septiembre de 2026

- `--test` ya no aplica el parche. `--check` solo lee y `--apply` es la única orden de aplicación por línea de comandos; cada resultado tiene su propio código de salida.
- Si el parche exacto ya está instalado, no se reescribe `gbr.exe`. Syncrash impide dos aplicaciones simultáneas sobre el mismo juego y vuelve a comprobar los archivos justo antes de sustituirlos.
- Mensajes de error más claros y eliminación del temporal propio. Ante un acceso denegado ya no se recomienda abrir el programa como administrador.
- Compilación determinista con .NET SDK 8.0.400 y 14 pruebas sintéticas y de línea de comandos superadas en Windows PowerShell 5.1; preparador local con validación de versión y compilación.
- Salidas de línea de comandos en UTF-8, incluso sin consola, y ayuda de permisos para excepciones internas sin perder el estado del archivo.
- El preparador lee la versión de `AssemblyInfo.cs`, también para las instrucciones y el ZIP. CI limita `push` a `main` y actualiza a `actions/checkout@v7` y `actions/setup-dotnet@v6` (últimas releases comprobadas: v7.0.1 y v6.0.0, con Node.js 24).
- Misma versión en ensamblado, archivo y manifiesto; los metadatos indican la empresa editora. La receta y los hashes del juego no cambian.

**Publicado como entrega experimental v1.0.3.** EXE final tras la auditoría: **2.403.840 bytes**, SHA256 `251c92e0cc17dec527086349d7e065705b33f9373e9b1a907485f4716f07d50c`. El [informe del candidato](docs/CANDIDATO_1.0.3.md) recoge las pruebas y lo que falta. El 24/09/2026 se repitió el ensayo con este EXE en una copia aislada de archivos reales admitidos: produjo el hash esperado del juego parcheado, no tocó `data.pak`, no volvió a escribir al repetir y conservó el original ante un reemplazo denegado. La verificación anterior del hash `1407eddc…910d812` se conserva como histórica. Código confirmado en main, empaquetado desde árbol limpio y CI remoto correctos. Se publicaron el EXE, sus sumas y la licencia para recoger feedback; en aquel cierre quedaron sin realizar las comprobaciones de partida y antivirus del hash exacto. Estos cambios no alteran la entrega v1.0.0 ni añaden correcciones de desync.

## Documentación y distribución · 24 de septiembre de 2026

- Captura de la interfaz actual publicada en el README.
- Solicitud de revisión antivirus enviada a Microsoft; la resolución final seguía pendiente en la última consulta.
- Se preparó la firma digital Authenticode con Azure Artifact Signing y se descartó el mismo día, sin firmar ningún EXE. Syncrash se distribuye sin firma y cada ficha de entrega publica el SHA256 del archivo.
- Reglas de mantenimiento documental y separación entre documentación pública y archivo interno en `AGENTS.md`.

Estos avances no cambian el binario, la protección ni la versión de la entrega v1.

## Syncrash v1 · 23 de septiembre de 2026

Primera entrega pública experimental para Steam vanilla. Versión del aplicador: **1.0.2.0**.

- Un solo `Syncrash.exe`, con búsqueda de la instalación y aplicación mediante botón.
- Protección de tres llamadas mediante comprobación del tipo de objeto; la misma protección validada en las pruebas privadas.
- Validación de `gbr.exe`, `Packs/data.pak` y resultado por SHA256.
- Reaplicación de la versión exacta ya instalada, sin crear copia de seguridad.
- Ayuda integrada sin conexión, enlace al código y a la última descarga.
- Icono e ilustración propios, licencia MIT y créditos.

**No incluye correcciones de desync Steam. Community Mod sigue desactivado.** Para recuperar vanilla, verifica los archivos desde Steam o reinstala el juego.

Los ensayos privados previos se conservan en el archivo de investigación.
