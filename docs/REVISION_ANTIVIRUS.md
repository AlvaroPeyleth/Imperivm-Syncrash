# Historial de análisis antivirus

Resumen consolidado el 27/09/2026. Desde esa fecha los análisis periódicos y las gestiones con proveedores dejan de ser requisitos de entrega. La información para revisar el código está en [Código y transparencia](SEGURIDAD.md).

## Muestra analizada · 24/09/2026

Solo se analizaron estos bytes: v1.0.0, aplicador 1.0.2.0, **2.400.256 bytes**, SHA256 `986141c161fb4e024ced0be7a174663eebb01f18d17270bc4862c9bdbfa47ed3`.

| Comprobación | Resultado histórico |
| --- | --- |
| [VirusTotal](https://www.virustotal.com/gui/file/986141c161fb4e024ced0be7a174663eebb01f18d17270bc4862c9bdbfa47ed3) | 7/71 detecciones: Arctic Wolf, DeepInstinct, Malwarebytes, MaxSecure, Microsoft, SecureAge y Trapmine. |
| [MetaDefender](https://metadefender.com/results/file/bzI2MDkyMzNHZWtQYUEzd2VWQklzTDJCaDY3_mdaas/overview) | 2/21: Aurora y Webroot SMD. Resumen «Suspicious» y apartado Adaptive Sandbox «Low Risk», sin datos de emulación. |
| Contraste con el código | Coincidieron 57 métodos inspeccionados y los recursos. Tras igualar el manifiesto, quedaron 47 bytes distintos de metadatos generados por el compilador. |
| Microsoft | Solicitud enviada. Última consulta registrada: Cloud y Client «No malware detected»; determinación final «Pending». No se ha comprobado una resolución posterior. |
| Otros proveedores | Borradores preparados, no enviados. [Registro histórico](RECLAMACIONES_ANTIVIRUS.md). |

Varias reglas estáticas correspondían a comprobaciones de hashes, procesos, búsqueda de Steam y recursos comprimidos. Esto apoyó la hipótesis de falsos positivos, pero no identificó la causa interna de cada veredicto ni confirmó su retirada. La revisión se hizo en la misma máquina, sin auditoría independiente. No se obtuvo un análisis de Defender local ni de Filescan.io.

## Versiones posteriores

No se realizaron nuevos análisis para 1.0.3.0 ni 1.0.4.0. Sus compilaciones y pruebas funcionales constan en las fichas de [1.0.3](CANDIDATO_1.0.3.md) y [1.0.4](CANDIDATO_1.0.4.md).

El candidato local 1.0.4.0 tiene **18.362.368 bytes**, SHA256 `88814908bccdee383c8d1c6d9e374a31c7a2f450b3d970941944fbc1a101fa3c`; los resultados de la tabla no se le atribuyen. La evidencia extensa y las versiones anteriores del informe se conservan en Git y en el archivo privado `work/`.
