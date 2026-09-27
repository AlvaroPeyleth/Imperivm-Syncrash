# Syncrash · Ficha de entrega

## Entrega actual: v1.0.4 · 27/09/2026

**Publicada como experimental para Steam vanilla.** [Descargar Syncrash v1.0.4](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/releases/tag/v1.0.4).

| Dato | Valor |
| --- | --- |
| Aplicador | 1.0.4.0 · `Syncrash.exe` |
| Tamaño | 18.362.368 bytes |
| SHA256 del EXE | `88814908bccdee383c8d1c6d9e374a31c7a2f450b3d970941944fbc1a101fa3c` |
| SHA256 de `gbr.exe` V2 + LAA | `af59a5bbb4956f50dd671a4a52753376b7b359b2028c0331a2db5fde6f64c965` |
| Commit de compilación y tag | `30aecbce6e35d7a691b94b5f57f814c07ebdfbe5` |

La release incluye el EXE, `SHA256SUMS.txt` y la licencia del aplicador. Pantalla adaptable y suavizado GPU forman una opción marcada por defecto; sus componentes, fuentes completas y licencias están incrustados. No hace falta un launcher ni una carpeta externa. El juego se abre desde Steam.

**Comprobaciones:** 24 pruebas locales, dos EXE completos idénticos, 22 operaciones sobre copias reales y recompilación desde commit limpio con Windows PowerShell 5.1. [CI del commit publicado](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/actions/runs/36282247853) superó pruebas y comparación de builds de la base sin componentes; el EXE completo se validó localmente. Se descargaron de GitHub los tres archivos publicados y sus SHA256 coinciden con las copias locales. El usuario comunica más de una hora sin incidencias en dos equipos Steam vanilla con los mismos componentes gráficos.

Para retirar pantalla, usa **Restaurar pantalla original**. Para recuperar también el ejecutable del juego, verifica después los archivos en Steam. Desmarcar la casilla no retira una pantalla instalada. La [ficha técnica](CANDIDATO_1.0.4.md) concentra fuentes, historial y límites. No incluye Community Mod ni una corrección causal de desync.
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

[Primera entrega experimental](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/releases/tag/v1.0.0): aplicador 1.0.2.0, 2.400.256 bytes, SHA256 `986141c161fb4e024ced0be7a174663eebb01f18d17270bc4862c9bdbfa47ed3`. Producía el mismo `gbr.exe` V2 que v1.0.3 y reescribía al repetir. Se comprobaron aplicación, reaplicación y rechazo de archivos desconocidos en copias aisladas, sin PAK modificado ni temporales restantes.

La sesión registrada de unos 79 minutos corresponde a esa protección del juego, antes del cambio de interfaz. Los [análisis antivirus de 24/09](REVISION_ANTIVIRUS.md) se conservan en un único historial, sin atribuirlos a versiones posteriores. El EXE histórico no se ha sustituido.
