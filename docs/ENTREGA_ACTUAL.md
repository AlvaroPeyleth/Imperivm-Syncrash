# Syncrash · Ficha de entrega

## Entrega actual: v1.0.6 · 28/09/2026

La [revisión de textos del 04/10/2026](../CHANGELOG.md#revisión-de-textos--4-de-octubre-de-2026) cambia la documentación y los mensajes en el código fuente. El EXE descargable sigue siendo el publicado el 28/09; su versión y sus hashes se mantienen.

**Publicada como v1 experimental.** [Descargar Syncrash v1.0.6](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/releases/tag/v1.0.6). Pantalla y voces se instalan o retiran desde sus casillas. Los enlaces están al pie de la ventana. Incluye correcciones de documentación; la conexión online automática sigue pendiente. [Ficha técnica, uso y pruebas](CANDIDATO_1.0.6.md).

| Dato | Valor |
| --- | --- |
| Aplicador | 1.0.6.0 · `Syncrash.exe` |
| Tamaño | 18.579.968 bytes |
| SHA256 del EXE | `c3be15d959263a69e32b4abd0623b9cc29e5d162695e4c65fa51977965d8eeb2` |
| SHA256 de `gbr.exe` con protección de cierres y LAA | `af59a5bbb4956f50dd671a4a52753376b7b359b2028c0331a2db5fde6f64c965` |

38 pruebas automáticas y siete operaciones del manejador real de la interfaz en copia aislada. Dos builds completos idénticos en PowerShell 7.6.5. Reproducida desde commit limpio `c73edca69b05559a42234138f7b7d5ab12e50963` (tag `v1.0.6`), con [CI correcto](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/actions/runs/36354762010). EXE, sumas y licencia descargados de GitHub y cotejados. Las entregas anteriores se conservan.

## Histórico: v1.0.5 · 27/09/2026

**Publicada como experimental.** [Descargar Syncrash v1.0.5](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/releases/tag/v1.0.5). Incorpora reparación de voces opcional en español, italiano e inglés, marcada por defecto y desactivable. Se conservan las funciones de memoria, cierres y pantalla de 1.0.4.

| Dato | Valor |
| --- | --- |
| Aplicador | 1.0.5.0 · `Syncrash.exe` |
| Tamaño | 18.579.968 bytes |
| SHA256 del EXE | `187d62824b609385f2dc1371ffd873a25d0f2cb70e0a6292f7f72dd2c57a4118` |
| SHA256 de `gbr.exe` con protección de cierres y LAA | `af59a5bbb4956f50dd671a4a52753376b7b359b2028c0331a2db5fde6f64c965` |

**Fuente y comprobaciones:** tag y commit de compilación `6260bc848a0e1b70c6fbdd84c69c27e0dd19ac70`; 36 pruebas locales, 31 operaciones sobre copias reales, acciones de la interfaz y dos builds completos idénticos. El preparador recompiló desde el commit limpio con PowerShell 7.6.5. [CI del commit](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/actions/runs/36350599727) superó pruebas y comparación de la base; el EXE completo se comprobó localmente. Se descargaron y cotejaron los tres archivos publicados: EXE, `SHA256SUMS.txt` y licencia.

La [ficha técnica](CANDIDATO_1.0.5.md) explica el uso, las pruebas y cómo retirar o recuperar el parche. La escucha completa en italiano/inglés y el multijugador con voces siguen pendientes. [Cómo dar feedback](../README.md#ayuda-a-mejorar-syncrash).

## Histórico: v1.0.4 · 27/09/2026

**Publicada como experimental para Steam vanilla.** [Descargar Syncrash v1.0.4](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/releases/tag/v1.0.4).

| Dato | Valor |
| --- | --- |
| Aplicador | 1.0.4.0 · `Syncrash.exe` |
| Tamaño | 18.362.368 bytes |
| SHA256 del EXE | `88814908bccdee383c8d1c6d9e374a31c7a2f450b3d970941944fbc1a101fa3c` |
| SHA256 de `gbr.exe` con protección de cierres y LAA | `af59a5bbb4956f50dd671a4a52753376b7b359b2028c0331a2db5fde6f64c965` |
| Commit de compilación y tag | `30aecbce6e35d7a691b94b5f57f814c07ebdfbe5` |

La release incluye el EXE, `SHA256SUMS.txt` y la licencia del aplicador. Pantalla adaptable y suavizado GPU forman una opción marcada por defecto; sus componentes, fuentes completas y licencias están incrustados. No hace falta un launcher ni una carpeta externa. El juego se abre normalmente, también desde `gbr.exe`.

**Comprobaciones:** 24 pruebas locales, dos EXE completos idénticos, 22 operaciones sobre copias reales y recompilación desde commit limpio con Windows PowerShell 5.1. [CI del commit publicado](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/actions/runs/36282247853) superó pruebas y comparación de builds de la base sin componentes; el EXE completo se validó localmente. Se descargaron de GitHub los tres archivos publicados y sus SHA256 coinciden con las copias locales. El usuario comunica más de una hora sin incidencias en dos equipos Steam vanilla con los mismos componentes gráficos.

Para retirar pantalla, usa **Restaurar pantalla original**. Para recuperar también el ejecutable del juego, verifica después los archivos en Steam. Desmarcar la casilla no retira una pantalla instalada. Consulta las fuentes, el historial y los límites en la [ficha técnica](CANDIDATO_1.0.4.md). Esta versión no admite Community Mod ni corrige las desincronizaciones.
## Histórico: v1.0.3 · 24/09/2026

**Entrega experimental para Steam vanilla y recopilación de feedback.** Aplicador **1.0.3.0**, compilado desde el commit `848ffbbad3fd1851334272af2a8314cf205bdf8c`.

[Descargar Syncrash v1.0.3](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/releases/tag/v1.0.3)

| Dato | Valor |
| --- | --- |
| Archivo | `Syncrash.exe` |
| Tamaño | 2.403.840 bytes |
| SHA256 del aplicador | `251c92e0cc17dec527086349d7e065705b33f9373e9b1a907485f4716f07d50c` |
| SHA256 de `gbr.exe` tras aplicar | `752c95a475e62b0d61d88ec9fb7fabc07758cb217ab152d78651385a5de3d2cc` |
| Firma digital | Sin firma Authenticode |

Pasaron 14 pruebas con Windows PowerShell 5.1, dos builds fueron idénticos y el preparador recompiló desde el commit limpio conservando el hash. El ensayo sobre una copia real comprobó aplicación, acentos UTF-8, acceso denegado sin alterar el original y repetición sin escritura. El PAK quedó intacto. La [ficha técnica de 1.0.3.0](CANDIDATO_1.0.3.md) mantiene la evidencia y sus límites.

Se distribuyen el EXE, `SHA256SUMS.txt` y la licencia. No contienen archivos completos del juego ni registros privados. La receta y las tres protecciones de cierres se mantienen; no se añaden correcciones de desync ni soporte Community Mod.

Esta ficha conserva los resultados de v1.0.3; no atribuye a ese EXE las partidas realizadas con candidatos posteriores. Para comunicar resultados utiliza la [guía de pruebas](PRUEBAS_SIN_OBSERVADOR.md).

## Histórico: v1.0.0 · 23/09/2026

[Primera entrega experimental](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/releases/tag/v1.0.0): aplicador 1.0.2.0, 2.400.256 bytes, SHA256 `986141c161fb4e024ced0be7a174663eebb01f18d17270bc4862c9bdbfa47ed3`. Producía el mismo `gbr.exe` parcheado que v1.0.3 y reescribía al repetir. Se comprobaron aplicación, reaplicación y rechazo de archivos desconocidos en copias aisladas, sin PAK modificado ni temporales restantes.

La sesión registrada de unos 79 minutos corresponde a esa protección del juego, antes del cambio de interfaz. Los [análisis antivirus de 24/09](REVISION_ANTIVIRUS.md) se conservan en un único historial, sin atribuirlos a versiones posteriores. El EXE histórico no se ha sustituido.
