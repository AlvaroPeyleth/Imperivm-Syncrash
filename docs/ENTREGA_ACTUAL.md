# Syncrash · Ficha de entrega

## Entrega actual: v1.0.3 · 24/09/2026

**Entrega experimental para Steam vanilla y recopilación de feedback.** Aplicador **1.0.3.0**, compilado desde el commit `848ffbbad3fd1851334272af2a8314cf205bdf8c`. El cierre de esta versión no implica que se hayan completado las pruebas de partida ni el análisis antivirus del nuevo hash.

[Descargar Syncrash v1.0.3](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/releases/tag/v1.0.3)

| Dato | Valor |
| --- | --- |
| Archivo | `Syncrash.exe` |
| Tamaño | 2.403.840 bytes |
| SHA256 del aplicador | `251c92e0cc17dec527086349d7e065705b33f9373e9b1a907485f4716f07d50c` |
| SHA256 de `gbr.exe` tras aplicar | `752c95a475e62b0d61d88ec9fb7fabc07758cb217ab152d78651385a5de3d2cc` |
| Firma digital | Sin firma Authenticode |
| Antivirus del nuevo hash | Sin análisis nuevo; los veredictos históricos de abajo pertenecen a v1.0.0 |

Pasaron 14 pruebas con Windows PowerShell 5.1, dos builds fueron idénticos y el preparador recompiló desde el commit limpio conservando el hash. El ensayo sobre una copia real comprobó aplicación, acentos UTF-8, acceso denegado sin alterar el original y repetición sin escritura. El PAK quedó intacto. La [ficha técnica de 1.0.3.0](CANDIDATO_1.0.3.md) mantiene la evidencia y sus límites.

Se distribuyen el EXE, `SHA256SUMS.txt` y la licencia. No contienen archivos completos del juego ni registros privados. La receta y las tres protecciones de cierres se mantienen; no se añaden correcciones de desync ni soporte Community Mod.

Quedan pendientes la prueba manual de interfaz/partida en otra instalación y el análisis antivirus del EXE exacto. Para recoger resultados utiliza la [guía de pruebas](PRUEBAS_SIN_OBSERVADOR.md). Si aparece un bloqueo, conserva la detección y no desactives las protecciones para forzar la ejecución.

## Histórico: v1.0.0 · 23/09/2026

**Primera entrega pública experimental para Steam vanilla.** Fecha: 23/09/2026. Aplicador: **1.0.2.0**. La protección del juego conserva la guarda V2 de las pruebas privadas.

Los apartados siguientes conservan **exclusivamente la evidencia histórica de v1.0.0**. Sus archivos no se han sustituido y sus análisis no se transfieren a v1.0.3. El [estado vigente de las revisiones antivirus](SEGURIDAD.md) se actualiza por separado.

[Consultar la entrega histórica v1.0.0](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/releases/tag/v1.0.0)

| Dato | Valor |
| --- | --- |
| Archivo | `Syncrash.exe` |
| Tamaño | 2.400.256 bytes |
| SHA256 del aplicador | `986141c161fb4e024ced0be7a174663eebb01f18d17270bc4862c9bdbfa47ed3` |
| SHA256 de `gbr.exe` tras aplicar | `752c95a475e62b0d61d88ec9fb7fabc07758cb217ab152d78651385a5de3d2cc` |
| Firma digital | Sin firma Authenticode; Windows puede mostrar «Editor desconocido». |
| VirusTotal | **7/71 motores detectan el archivo**, análisis del 24/09/2026. [Ver informe](https://www.virustotal.com/gui/file/986141c161fb4e024ced0be7a174663eebb01f18d17270bc4862c9bdbfa47ed3). Investigado; revisión de Microsoft enviada y resolución final pendiente. |

## Qué contiene

Un único ejecutable con el aplicador, la receta de diferencias, los recursos gráficos propios y la licencia MIT incrustada. No contiene el juego completo, PAK, logs, volcados ni observador. Añade ayuda sin conexión y enlaces al código y a las descargas.

Esta v1 **no incorpora una corrección de desync Steam**. Community Mod permanece desactivado.

## Comprobaciones de esta entrega

- Instalación y reinstalación del EXE final en una copia aislada: salida 0 y resultado V2 exacto. La reinstalación vuelve a escribir el archivo.
- Ejecutable alterado y PAK incompatible: rechazo con salida 1, sin modificar `gbr.exe`.
- Ninguna copia de seguridad ni temporal restante tras las pruebas.
- Revisión del render nativo de la pantalla y de la ayuda desplegada. No equivale a verificar todas las resoluciones, escalados o interacciones en otros equipos.
- Receta idéntica a la entrega privada: SHA256 `9388df692e9f0f0478f8060d625e643618cfc16dd28298c1cbe879aaa6bfc583`.

La sesión Steam de unos 79 minutos y los registros pareados documentados corresponden a esta misma protección del juego, antes del cambio de interfaz. No se presenta una nueva partida como realizada con este aplicador.

Los resultados detallados y las copias de ensayo se conservan en el archivo local de investigación, fuera del repositorio. Consulta el [plan de pruebas](PLAN_DE_EJECUCION.md) y los [límites](TRANSPARENCIA.md).

## Verificar la descarga

La release incluye `SHA256SUMS.txt`. El hash identifica exactamente el archivo publicado. Las compilaciones propias pueden diferir por metadatos del compilador; el resultado parcheado del juego debe conservar el hash documentado.

El informe VirusTotal enlazado corresponde al SHA256 exacto del EXE publicado. Un análisis es una comprobación adicional, no una certificación de seguridad. Si tu antivirus lo bloquea, conserva el nombre de la detección y comunícalo; no desactives la protección para forzar su ejecución.

## Resultado de VirusTotal · 24/09/2026

El análisis final muestra **7 detecciones entre 71 motores**, con estos nombres:

| Motor | Detección |
| --- | --- |
| Arctic Wolf | Unsafe |
| DeepInstinct | MALICIOUS |
| Malwarebytes | MachineLearning/Anomalous.100% |
| MaxSecure | Trojan.Malware.300983.susgen |
| Microsoft | Trojan:Win32/Wacatac.C!ml |
| SecureAge | Malicious |
| Trapmine | Suspicious.low.ml.score |

Hemos revisado nuestro aplicador y contrastado este EXE con el código publicado, sin encontrar código malicioso. Estas comprobaciones respaldan nuestra hipótesis de falsos positivos. Ya hemos enviado la solicitud a Microsoft; queda seguir su resolución y tramitar las solicitudes a los demás proveedores. Recomendamos posponer nuevas instalaciones hasta aclarar esas detecciones; no desactives el antivirus ni añadas exclusiones para ejecutar el archivo.

El ejecutable publicado no se ha sustituido: el informe analiza exactamente esa entrega. La comparación con el código publicado ya se ha realizado y se resume a continuación. El estado de las solicitudes se mantiene en [Seguridad](SEGURIDAD.md#estado-de-la-revisión-con-los-proveedores). El informe puede cambiar con nuevas definiciones o análisis; esta tabla refleja el resultado observado en la fecha indicada.

## Revisión del aplicador

En nuestra [revisión del 24/09/2026](REVISION_ANTIVIRUS.md) comprobamos la correspondencia entre el EXE publicado y el código de la entrega: coinciden las instrucciones de los métodos inspeccionados y los recursos; al igualar los saltos de línea del manifiesto solo cambian metadatos generados por el compilador. Publicamos el procedimiento para que puedas contrastarlo y compilar tu propia versión. La causa exacta de cada detección sigue pendiente de aclaración con los proveedores.

La [explicación de seguridad](SEGURIDAD.md) reúne las comprobaciones y los resultados de VirusTotal y MetaDefender con sus límites.
