# Borradores de revisión antivirus de Syncrash

**Preparados el 24/09/2026. Salvo la solicitud a Microsoft, ya enviada, están pendientes de envío.** El [estado público vigente](SEGURIDAD.md#estado-de-la-revisión-con-los-proveedores) y los resultados técnicos fechados en [Revisión antivirus](REVISION_ANTIVIRUS.md) son las fuentes del proyecto. Los números de expediente y comunicaciones originales permanecen en `work/`, fuera de Git.

## Muestra histórica exacta

- Producto: Syncrash, parche experimental de estabilidad para Imperivm Steam vanilla. Aplicador C#/.NET Framework WinForms de código propio MIT.
- Release: v1.0.0, aplicador 1.0.2.0; EXE publicado `Syncrash.exe` de 2.400.256 bytes.
- SHA256: `986141c161fb4e024ced0be7a174663eebb01f18d17270bc4862c9bdbfa47ed3`.
- [Código](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/tree/26c6c46), [revisión técnica](REVISION_ANTIVIRUS.md), [ficha de entrega](ENTREGA_ACTUAL.md), [VirusTotal](https://www.virustotal.com/gui/file/986141c161fb4e024ced0be7a174663eebb01f18d17270bc4862c9bdbfa47ed3) y [MetaDefender](https://metadefender.com/results/file/bzI2MDkyMzNHZWtQYUEzd2VWQklzTDJCaDY3_mdaas/overview).
- Función: comprueba hashes de `gbr.exe` y `Packs/data.pak`, exige juego cerrado, reconstruye y verifica el resultado, y sustituye `gbr.exe` por acción del usuario. No descarga cargas, no tiene telemetría, servicios ni rutinas para desactivar antivirus. La entrega v1.0.0 también tenía un comando `--test` que aplicaba; el [candidato 1.0.3.0](CANDIDATO_1.0.3.md) lo retiró.
- Evidencia y límite: la comparación IL/recursos de v1.0.0 se hizo en la **misma máquina**; la ejecución en sandbox no cubrió necesariamente el recorrido de aplicación al juego. No se conoce la regla interna que originó cada veredicto. Se solicita una revisión, no una exclusión para toda la empresa.

**Texto común adaptable para el titular:**

> Somos los autores de Syncrash. Solicitamos revisar la clasificación de la muestra con SHA256 indicado arriba y confirmar el resultado sobre ese archivo exacto. Su función legítima, código, receta y controles se describen en los enlaces. Adjuntamos únicamente el aplicador cuando el canal autorizado lo permita; no adjuntamos el EXE/PAK original del juego ni datos de jugadores. La revisión técnica apoya una hipótesis de falso positivo, pero no pretende sustituir vuestro análisis. Por favor, indicad el veredicto, motor/producto y, si es posible, qué evidencia adicional necesitáis.

Antes de enviarlo, el titular debe comprobar el veredicto **actual** y la política de cada canal. Si se usa un EXE futuro, sustituir **todos** los datos de muestra por su hash, versión, fecha y resultados propios; no copiar `7/71` o `2/21` como si fueran su análisis. Registrar `observado` → `borrador preparado` → `enviado` → `respuesta recibida` → `comprobado después`, cada transición con fecha y evidencia real.

## Adaptación por proveedor

| Proveedor | Observación histórica del 24/09/2026 y solicitud específica | Canal contrastado el 24/09/2026 | Estado |
| --- | --- | --- | --- |
| Microsoft | VirusTotal: `Trojan:Win32/Wacatac.C!ml`. Aportar evidencia al **expediente existente**, preguntando por el veredicto de Defender sobre el hash. Registrar por separado SmartScreen (reputación) y Smart App Control (modo y bloqueo observados); no tratarlos como un solo resultado. Cloud y Client mostraban «No malware detected», determinación final `Pending` en la última consulta comunicada. | [Microsoft Security Intelligence](https://www.microsoft.com/en-us/wdsi/filesubmission) | Solicitud histórica enviada; seguimiento pendiente. Sin nuevo envío. |
| Malwarebytes | VirusTotal: `MachineLearning/Anomalous.100%`. Pedir revisión de la clasificación del archivo exacto y distinguirla de una conclusión manual sobre una familia. | [Soporte oficial de falsos positivos](https://help.malwarebytes.com/hc/en-us/articles/31589211404571-Report-a-false-positive-to-Malwarebytes-Support) | Borrador preparado; no enviado. |
| Webroot | **Webroot SMD en MetaDefender**: `Malware_52.2`. Mencionar expresamente MetaDefender, no atribuir esta observación al informe de VirusTotal. Pedir revisión de esa muestra en su motor. | [Formulario para proveedores](https://www.webroot.com/us/en/business/support/vendor-dispute-contact-us) | Borrador preparado; no enviado. |
| Deep Instinct | VirusTotal: `MALICIOUS`. Pedir veredicto de su motor para el hash y la clase de evidencia que permita revisar la detección. | [Directorio de contactos de VirusTotal](https://docs.virustotal.com/docs/false-positive-contacts), entrada Deep Instinct; confirmar el contacto al enviar. | Borrador preparado; no enviado. |
| Arctic Wolf | VirusTotal: `Unsafe`. Pedir confirmar qué producto o motor emitió el veredicto y su canal de revisión para archivos. | **Por confirmar**: no se identificó un canal específico de falso positivo de archivos en las fuentes oficiales consultadas. | Borrador preparado; canal pendiente. |
| SecureAge | VirusTotal: `Malicious`. Pedir revisión del archivo exacto y respuesta sobre el motor. | [Directorio de VirusTotal](https://docs.virustotal.com/docs/false-positive-contacts), entrada SecureAge que remite a su contacto oficial. | Borrador preparado; no enviado. |
| MaxSecure | VirusTotal: `Trojan.Malware.300983.susgen`. Preguntar si la firma genérica sigue aplicando al hash y aportar la relación código–binario con sus límites. | [Directorio de VirusTotal](https://docs.virustotal.com/docs/false-positive-contacts), entrada MaxSecure; confirmar el contacto al enviar. | Borrador preparado; no enviado. |
| Trapmine | VirusTotal: `Suspicious.low.ml.score`. Pedir revisión de la clasificación ML de la muestra, sin interpretarla como probabilidad. | [Directorio de VirusTotal](https://docs.virustotal.com/docs/false-positive-contacts), entrada Trapmine; confirmar el contacto al enviar. | Borrador preparado; no enviado. |
| OPSWAT / MetaDefender | MetaDefender: **Aurora** `Malware_-10`, además de Webroot SMD. Preguntar qué motor concreto es Aurora, qué evidencia fundamenta el veredicto y si se ejecutó el flujo de aplicación al juego. No equiparar Aurora con Arctic Wolf por el nombre. | [Portal oficial de reporte de detección incorrecta](https://www.opswat.com/docs/my/support-services/report-false-detection) | Consulta redactada; no enviada. |

VirusTotal aclara que [agrega resultados y remite las reclamaciones al proveedor](https://docs.virustotal.com/docs/false-positive-contacts). Ninguno de estos borradores equivale a una respuesta o aprobación antivirus. Si un proveedor solicita el juego original, acordar un canal y una base legítima por separado; nunca incorporarlo al repositorio o paquete público.
