# Revisión de detecciones antivirus — 24/09/2026

## Resultado

**Hemos revisado el aplicador que desarrollamos y contrastado el EXE distribuido con nuestro código público.** No encontramos código malicioso. Documentamos a continuación las comprobaciones que respaldan nuestra hipótesis de falsos positivos. Sigue pendiente solicitar y obtener la revisión de los proveedores; todavía no conocemos la regla o característica concreta que activó cada motor.

## Evidencia

- Archivo publicado/analizado: SHA256 `986141c161fb4e024ced0be7a174663eebb01f18d17270bc4862c9bdbfa47ed3`, 2.400.256 bytes.
- Recompilación desde el commit de entrega `26c6c46`, en directorio aislado, con el compilador de .NET Framework cuya firma Microsoft Windows es válida.
- Coinciden los 57 métodos inspeccionados (IL), las seis referencias a bibliotecas .NET y los cinco recursos incrustados. Se usó carga de reflexión sin ejecución del programa inspeccionado.
- La primera recompilación difería además en los saltos de línea del manifiesto nativo. Al reproducir sus finales de línea LF, ambos EXE tienen el mismo tamaño y solo difieren en 47 bytes: timestamp PE (dentro de 0x88–0x8B), GUID del nombre generado por el compilador (0x22C44C–0x22C46F) y GUID de módulo (0x22E8C4–0x22E8D3). Todos los demás bytes coinciden.
- El código revisado solo lee rutas/recursos Steam, valida hashes y sustituye el gbr.exe admitido a petición del jugador. Los únicos Process.Start abren enlaces fijos de GitHub al pulsarlos. No contiene descarga de cargas, persistencia, robo de credenciales, inyección de procesos ni rutinas para desactivar antivirus.
- VirusTotal muestra 7/71 detecciones. El resumen de comportamiento disponible no mostraba comunicaciones de red, archivos depositados ni detecciones de sandbox; Zenbox seguía pendiente. Esto no cubre todas las rutas de uso: el sandbox puede no disponer del juego ni pulsar Aplicar parche.
- CAPA muestra etiquetas de referencias a cadenas anti-VM/analysis y «obfuscated». Son resultados automáticos de capacidades; en el código y build revisados no hay comprobaciones de VM ni un paso de ofuscación. No se ha localizado la cadena exacta que disparó esas etiquetas.
- No se obtuvo estado/escaneo de Defender local: la consulta CIM devolvió acceso no disponible. No se han cambiado protecciones ni añadido exclusiones.

## Interpretación

Malwarebytes documenta MachineLearning/Anomalous.100% como una detección genérica de su clasificador, no como una identificación manual de una familia concreta. Un aplicador que incorpora cambios binarios y sustituye otro EXE podría activar heurísticas; es una hipótesis, no una causa demostrada. La falta de firma tampoco prueba la causa ni garantiza que firmarlo resuelva las detecciones.

La revisión vincula el archivo distribuido con el código público. No equivale a una auditoría independiente ni a una confirmación del proveedor de antivirus. El compilador y sistema usados para recompilar son los de esta misma máquina.

## Siguiente paso

Solicitar revisión de posible falso positivo a Microsoft y Malwarebytes aportando el EXE exacto, el hash, el código y este contraste. No alterar el EXE para intentar eludir los clasificadores. Mantener visible el resultado del análisis mientras se resuelve.

Fuentes:
- https://www.malwarebytes.com/blog/detections/machinelearning-anomalous-100
- https://www.microsoft.com/security/portal/submit.aspx/
- https://help.malwarebytes.com/hc/en-us/articles/31589211404571-Report-a-false-positive-to-Malwarebytes-Support

## Segundo servicio: MetaDefender Cloud

Consulta del 24/09/2026 sobre el mismo SHA256 publicado, sin modificar el EXE:

https://metadefender.com/results/file/bzI2MDkyMzNHZWtQYUEzd2VWQklzTDJCaDY3_mdaas/overview

- Metascan: 2/21 motores señalan el archivo.
- Aurora: Malware_-10.
- Webroot SMD: Malware_52.2.
- Adaptive Sandbox: Suspicious; 60 indicadores de compromiso, 8 indicadores de amenaza, 2 reglas YARA, 16 archivos extraídos y 911 cadenas extraídas. Etiquetas visibles: packed, reconnaissance, peexe, dotnet_pe, html.
- Esos contadores no equivalen a comunicaciones o archivos maliciosos ejecutados: el detalle no está disponible en la vista anónima y no se ha comprobado qué elementos los originan.
- La vista indica que hace falta iniciar sesión para el informe detallado. No se contrató ningún servicio.
- El resultado no confirma falsos positivos ni explica por sí mismo las detecciones de VirusTotal. Los conjuntos de motores son distintos; 2/21 no puede interpretarse como que se han resuelto cinco detecciones del 7/71 anterior.

Se intentó la selección de archivos en Filescan.io, pero no abrió el selector con los controles disponibles; no se completó ni se atribuye un análisis de ese servicio.

## Detalle autenticado de MetaDefender · 24/09/2026

La sesión iniciada permitió consultar el informe existente del mismo hash, sin una segunda subida. El resumen general mantiene 2/21 motores y «Suspicious»; la sección Adaptive Sandbox Summary muestra «Low Risk». La tabla tiene 0 indicadores Malicious, 0 Likely Malicious, 5 Suspicious y 3 Other. Esto no cambia los veredictos de Aurora/Webroot ni los de VirusTotal.

Las cinco alertas sospechosas desplegadas se fundamentan en:

| Regla | Evidencia del servicio | Correspondencia en el proyecto |
| --- | --- | --- |
| DN023 | SHA256.Create y HashAlgorithm.ComputeHash | Comprobación de identidad e integridad de archivos y receta. |
| DN032 | Process.GetProcessesByName | Se consulta gbr para exigir que el juego esté cerrado. |
| DN021 | DriveInfo.GetDrives, DriveType, IsReady, RootDirectory | Búsqueda de bibliotecas Steam en unidades fijas. El rótulo inglés dice driver, pero las API mostradas corresponden a unidades de almacenamiento. |
| H000 | Entropía .text 7.98047971725 y .rsrc 7.95666837692 | El EXE contiene PNG comprimidos: el banner ocupa 2.145.191 bytes de los 2.400.256 del aplicador, además del icono. Es una explicación plausible de la entropía, no una demostración del motivo de los otros motores. |
| SIGG017 | Ejecutable sin firma con datos de alta entropía interpretados como posible payload empaquetado | No se utiliza un empaquetador en la compilación revisada; hay recursos gráficos y una receta incrustados. |

Los IOC visibles incluyen dominios extraídos de archivos como cert.ssl.com, creativecommons.org e iptc.org; las IP mostradas figuran como DOMAIN_RESOLVE/EXTRACTED_FILE. No son por sí solos conexiones realizadas por Syncrash. El informe señala Emulation Data 0 y «No emulation data available for this file», por lo que no permite concluir que haya ejecutado y observado todas sus rutas.

Estas evidencias explican las reglas estáticas concretas de este informe, no la causa interna de cada detección antivirus. No se recomienda quitar verificaciones de hash o de juego cerrado para reducir contadores de sospecha.
