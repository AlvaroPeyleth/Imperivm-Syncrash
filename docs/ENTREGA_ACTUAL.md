# Syncrash v1 · Ficha de entrega

**Primera entrega pública experimental para Steam vanilla.** Fecha: 23/09/2026. Aplicador: **1.0.2.0**. La protección del juego conserva la guarda V2 de las pruebas privadas.

[Descargar Syncrash.exe](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/releases/latest)

| Dato | Valor |
| --- | --- |
| Archivo | `Syncrash.exe` |
| Tamaño | 2.400.256 bytes |
| SHA256 del aplicador | `986141c161fb4e024ced0be7a174663eebb01f18d17270bc4862c9bdbfa47ed3` |
| SHA256 de `gbr.exe` tras aplicar | `752c95a475e62b0d61d88ec9fb7fabc07758cb217ab152d78651385a5de3d2cc` |
| Firma Authenticode | Sin firma digital. |
| VirusTotal | Análisis pendiente. No se afirma que haya pasado un análisis antivirus. |

## Qué contiene

Un único ejecutable con el aplicador, la receta de diferencias, los recursos gráficos propios y la licencia MIT incrustada. No contiene el juego completo, PAK, logs, volcados ni observador. Añade ayuda sin conexión y enlaces al código y a las descargas.

Esta v1 **no incorpora una corrección de desync Steam**. Community Mod permanece desactivado.

## Comprobaciones de esta entrega

- Instalación y reinstalación del EXE final en una copia aislada: salida 0 y resultado V2 exacto. La reinstalación vuelve a escribir el archivo.
- Ejecutable alterado y PAK incompatible: rechazo con salida 1, sin modificar `gbr.exe`.
- Ningún respaldo ni temporal restante tras las pruebas.
- Revisión del render nativo de la pantalla y de la ayuda desplegada. No equivale a verificar todas las resoluciones, escalados o interacciones en otros equipos.
- Receta idéntica a la entrega privada: SHA256 `9388df692e9f0f0478f8060d625e643618cfc16dd28298c1cbe879aaa6bfc583`.

La sesión Steam de unos 79 minutos y los registros pareados documentados corresponden a esta misma protección del juego, antes del cambio de interfaz. No se presenta una nueva partida como realizada con este aplicador.

Los resultados detallados y las copias de ensayo se conservan en el archivo local de investigación, fuera del repositorio. Consulta el [plan de pruebas](PLAN_DE_EJECUCION.md) y los [límites](TRANSPARENCIA.md).

## Verificar la descarga

La release incluye `SHA256SUMS.txt`. El hash identifica exactamente el archivo publicado. Las compilaciones propias pueden diferir por metadatos del compilador; el resultado parcheado del juego debe conservar el hash documentado.

Cuando exista un informe VirusTotal, se enlazará al hash exacto de esta entrega. Un análisis es una comprobación adicional, no una certificación de seguridad. Si tu antivirus lo bloquea, conserva el nombre de la detección y comunícalo; no desactives la protección para forzar su ejecución.
