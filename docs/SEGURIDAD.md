# Seguridad y verificaciones de Syncrash

**Desarrollamos Syncrash para aplicar nuestras correcciones a Imperivm de forma controlada y verificable.** Publicamos el código, los cambios exactos del parche y las instrucciones para compilarlo. Aquí explicamos qué hace el programa, cómo comprobamos el ejecutable y qué alertas puedes encontrar.

Hemos revisado las operaciones del aplicador. En la revisión histórica de v1.0.0 contrastamos su EXE con una recompilación del código publicado y no encontramos código malicioso. También relacionamos varias reglas del analizador con funciones legítimas del programa; estas comprobaciones respaldan nuestra hipótesis de falsos positivos para aquel hash.

**Estado a 24/09/2026:** la [entrega actual v1.0.3](ENTREGA_ACTUAL.md), aplicador 1.0.3.0, está publicada como experimental. Su EXE se recompiló de forma idéntica y se aplicó con éxito a una copia de archivos reales; faltan una partida y un análisis antivirus de su hash. Los análisis y la solicitud a Microsoft documentados aquí corresponden exclusivamente a v1.0.0 y no se transfieren a la nueva entrega.

## Revisa, modifica o compila tu propia versión

El [código del aplicador](../src/Syncrash), la receta del parche y el proceso de compilación son públicos. Puedes inspeccionarlos, modificarlos y generar tu propio ejecutable siguiendo las [instrucciones de compilación](../README.md#compilar-y-conocer-el-proyecto). La [licencia MIT](../LICENSE) permite reutilizar el código conservando el aviso de copyright y la licencia.

## Qué hace el aplicador

El [código publicado](../src/Syncrash/Program.cs) permite seguir estas operaciones:

1. **Identifica la instalación.** Lee las rutas de Steam y permite seleccionar `gbr.exe` manualmente.
2. **Comprueba los archivos admitidos.** Calcula SHA256 de `gbr.exe` y `Packs/data.pak`. Solo admite el original Steam identificado o el resultado exacto de esta versión.
3. **Exige que Imperivm esté cerrado.** Consulta si existe un proceso llamado `gbr` antes de aplicar y de sustituir el archivo.
4. **Reconstruye el resultado en memoria.** Valida la receta incrustada, los bytes de entrada, los rangos modificados y el hash del resultado.
5. **Verifica antes de sustituir.** Escribe un temporal junto al destino, comprueba su hash y vuelve a comprobar que los archivos de entrada no hayan cambiado antes de reemplazar únicamente `gbr.exe`.
6. **Confirma el resultado final.** Verifica de nuevo su hash y elimina el temporal si queda pendiente.

La entrega v1.0.0 se probó en copias aisladas: instalación, reinstalación y rechazo de un EXE o PAK alterado. Esa versión reescribe el resultado si ya está instalado. El aplicador 1.0.3.0 responde «ya instalado» sin reescribir, impide dos aplicaciones simultáneas y añade `--check`, que solo lee. Sus [pruebas](CANDIDATO_1.0.3.md) no incluyen todavía una partida real. La protección del juego cubre las tres rutas de cierre descritas en [Funcionamiento](FUNCIONAMIENTO.md).

**No hay copia de seguridad automática.** Para recuperar el original, verifica los archivos del juego desde Steam o reinstálalo. Los hashes esperados están en [Transparencia](TRANSPARENCIA.md#archivos-admitidos).

## Alcance de los accesos

Hemos diseñado el aplicador con este alcance:

- Los accesos a Steam sirven para localizar el juego y verificar su versión.
- No hay funciones de descarga automática, telemetría, lectura de credenciales, instalación al arrancar, inyección en procesos ni desactivación de antivirus.
- Los enlaces externos son direcciones fijas de GitHub que se abren cuando el jugador los pulsa.
- Los recursos incrustados son la receta de cambios, el banner, el icono, la marca gráfica y la licencia. Las imágenes se usan en la interfaz.
- Los mapas, guardados, PAK y archivos de audio no se modifican durante la aplicación del parche.

El cambio que aplicamos al juego está explicado en [Funcionamiento](FUNCIONAMIENTO.md), junto con su alcance y las pruebas multijugador pendientes.

## Cómo contrastamos el EXE con el código

El archivo histórico v1.0.0 analizado en este apartado tiene SHA256:

```text
986141c161fb4e024ced0be7a174663eebb01f18d17270bc4862c9bdbfa47ed3
```

Recompilamos el código del commit de entrega `26c6c46` en un directorio aislado, usando el compilador de .NET Framework de Microsoft. Comprobamos:

- Coincidencia de los 57 métodos inspeccionados, sus instrucciones IL, las seis referencias a bibliotecas .NET y los cinco recursos incrustados.
- Tras reproducir los saltos de línea del manifiesto, el mismo tamaño y únicamente 47 bytes distintos, correspondientes a la fecha del encabezado PE y los identificadores generados por el compilador. Todos los demás bytes coinciden.

La comprobación relaciona el binario **publicado v1.0.0** con ese código fuente. Se hizo en la misma máquina y no equivale a una auditoría externa de toda la cadena de compilación. Un hash identifica un archivo: por sí solo no demuestra que sea seguro. El aplicador 1.0.3.0 tiene [otra comparación y otro hash](CANDIDATO_1.0.3.md); los informes de este apartado no se transfieren a él.

## Qué significan las alertas observadas

Los informes de este apartado describen las alertas de v1.0.0. No atribuyen un veredicto al nuevo hash de v1.0.3. Hemos examinado las reglas detalladas de MetaDefender y localizado las funciones a las que se refieren. Esta tabla explica su uso en Syncrash; los veredictos individuales de los motores antivirus se recogen después.

| Regla de MetaDefender | Qué detectó | Uso comprobado en Syncrash |
| --- | --- | --- |
| Hashing (`DN023`) | `SHA256.Create` y `ComputeHash`. | Comprobar archivos y receta; rechazar entradas o resultados no admitidos. |
| Descubrimiento de procesos (`DN032`) | `Process.GetProcessesByName`. | Exigir que Imperivm esté cerrado. |
| Información del sistema (`DN021`) | Métodos de `DriveInfo`. | Buscar bibliotecas Steam en unidades fijas. Son unidades de almacenamiento, aunque el rótulo del servicio use la palabra «driver». |
| Entropía elevada (`H000`) | Datos con una distribución de bytes similar a contenido comprimido. | Hay imágenes comprimidas incrustadas. El banner ocupa 2.145.191 de los 2.400.256 bytes del EXE. Es una explicación plausible de esta señal. |
| Posible contenido empaquetado (`SIGG017`) | Regla estática asociada a datos de alta entropía. | La compilación revisada no usa un empaquetador u ofuscador; incrusta recursos gráficos y la receta. La regla por sí sola no prueba que haya una carga maliciosa. |

**No quitamos comprobaciones de integridad o de juego cerrado para reducir alertas.** Cumplen una función de protección. Tampoco se ha demostrado que modificar las imágenes elimine las detecciones de los motores antivirus.

## Resultados y límites de los informes

Resultados observados el **24/09/2026** para el mismo SHA256:

| Servicio | Resultado observado |
| --- | --- |
| [VirusTotal](https://www.virustotal.com/gui/file/986141c161fb4e024ced0be7a174663eebb01f18d17270bc4862c9bdbfa47ed3) | 7/71 motores detectaron el archivo, incluida Microsoft. |
| [MetaDefender](https://metadefender.com/results/file/bzI2MDkyMzNHZWtQYUEzd2VWQklzTDJCaDY3_mdaas/overview) | 2/21 motores: Aurora y Webroot SMD. El resumen muestra «Suspicious» y el detalle de Adaptive Sandbox muestra «Low Risk». |

El detalle de MetaDefender clasifica cinco indicadores como sospechosos y tres como otros, con cero indicadores en las categorías «Malicious» y «Likely Malicious». Esas categorías pertenecen a esa sección: no anulan las detecciones antivirus. El informe no contiene datos de emulación, por lo que no demuestra una ejecución completa del aplicador.

Los dominios extraídos de recursos y las IP resueltas por el analizador tampoco equivalen a conexiones hechas por el programa. Por ejemplo, las imágenes incluyen metadatos de procedencia con referencias a organismos y certificados. La [revisión técnica](REVISION_ANTIVIRUS.md) recoge lo observado y sus límites.

Malwarebytes describe [MachineLearning/Anomalous.100%](https://www.malwarebytes.com/blog/detections/machinelearning-anomalous-100) como una detección genérica de su módulo de aprendizaje automático. Hemos identificado el uso legítimo de las funciones señaladas por varias reglas estáticas de MetaDefender; la causa interna de cada veredicto antivirus todavía no se conoce.

### Estado de la revisión con los proveedores

**Actualizado: 24/09/2026.** Hemos iniciado los reportes para que los proveedores revisen las detecciones que nuestras comprobaciones señalan como posibles falsos positivos.

| Gestión | Estado comprobado |
| --- | --- |
| Microsoft Security Intelligence | Solicitud enviada con el EXE publicado, su SHA256, el código y la revisión técnica. La última consulta muestra «No malware detected» en Cloud y Client; la determinación final sigue «Pending». |
| Otros proveedores que detectan el archivo | [Borradores por proveedor preparados](RECLAMACIONES_ANTIVIRUS.md); no enviados. Los análisis de VirusTotal y MetaDefender ya realizados no equivalen a una reclamación ante cada motor. |

El resultado actual del expediente de Microsoft es posterior al análisis de VirusTotal y pertenece a otro servicio. Conservamos ambos resultados con su fecha; no presentamos la solicitud como una resolución final ni como la retirada de las demás alertas. Publicaremos las respuestas relevantes sin exponer datos privados del expediente.

## Próximas comprobaciones

1. Contrastar cada EXE con una compilación del código fuente y publicar su SHA256 junto con el alcance de las pruebas.
2. Probar el aplicador y el `gbr.exe` reconstruido en una instalación legítima aislada, con protecciones activas. Registrar por separado cualquier aviso sobre ambos archivos y comprobar una partida.
3. Analizar el **hash exacto** de cada candidato y continuar las reclamaciones justificadas a proveedores. Preparar una nueva entrega binaria solo tras verificar sus activos.

La evidencia administrativa y los datos privados se conservan fuera del repositorio.

## Cómo comprobar una descarga y comunicar un problema

Descarga desde la [release oficial](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/releases/latest) y contrasta el archivo con el SHA256 de la [ficha de entrega](ENTREGA_ACTUAL.md) y `SHA256SUMS.txt`. Quien quiera revisar o compilar el código dispone de las instrucciones del [README](../README.md#compilar-y-conocer-el-proyecto).

Syncrash no lleva firma digital Authenticode, por eso Windows puede mostrar «Editor desconocido». Una firma identificaría al editor, pero no sería un veredicto antivirus. El SHA256 es lo que confirma que tienes exactamente el archivo publicado.

Si aparece una alerta, conserva el motor, la versión de sus definiciones, el nombre de la detección y el hash. No desactives el antivirus ni añadas exclusiones para forzar la ejecución. Puedes comunicar esos datos a `xtalvarotx` en Discord o abrir una incidencia sin datos privados.

Publicaremos las respuestas de los proveedores y actualizaremos esta documentación si cambia el ejecutable. Cada informe corresponde al hash indicado; para otra versión publicaremos sus propias comprobaciones.
