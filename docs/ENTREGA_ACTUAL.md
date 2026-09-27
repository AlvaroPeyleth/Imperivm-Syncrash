# Syncrash · Ficha de entrega

## Entrega actual: v1.0.3 · 24/09/2026

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

## Siguiente entrega local

El [candidato 1.0.4.0](CANDIDATO_1.0.4.md) está preparado en local con LAA, pantalla adaptable y suavizado GPU. Su EXE de 18.362.368 bytes tiene SHA256 `88814908bccdee383c8d1c6d9e374a31c7a2f450b3d970941944fbc1a101fa3c`. El preparador lo reprodujo desde el commit limpio `ce2ff9d`. **Aún no está publicado ni sustituye la descarga anterior.**

## Histórico: v1.0.0 · 23/09/2026

[Primera entrega experimental](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/releases/tag/v1.0.0): aplicador 1.0.2.0, 2.400.256 bytes, SHA256 `986141c161fb4e024ced0be7a174663eebb01f18d17270bc4862c9bdbfa47ed3`. Producía el mismo `gbr.exe` V2 que v1.0.3 y reescribía al repetir. Se comprobaron aplicación, reaplicación y rechazo de archivos desconocidos en copias aisladas, sin PAK modificado ni temporales restantes.

La sesión registrada de unos 79 minutos corresponde a esa protección del juego, antes del cambio de interfaz. Los [análisis antivirus de 24/09](REVISION_ANTIVIRUS.md) se conservan en un único historial, sin atribuirlos a versiones posteriores. El EXE histórico no se ha sustituido.
