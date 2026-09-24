# Seguridad y verificaciones de Syncrash

**El propósito de abrir el código es que el comportamiento del parche pueda comprobarse.** Esta página explica las medidas de protección, la revisión del ejecutable y las alertas conocidas de Syncrash v1.

**La revisión del aplicador no ha encontrado código malicioso y ha contrastado el EXE distribuido con el código público.** Estas comprobaciones respaldan la hipótesis de falsos positivos. A continuación se documentan las operaciones del programa, la comparación del binario y las alertas examinadas.

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

Estas medidas hacen que la aplicación sea repetible y permiten rechazar archivos incompatibles. Se han probado instalación, reinstalación y rechazo de EXE/PAK alterados en copias aisladas. La protección del juego cubre las tres rutas de cierre descritas en [Funcionamiento](FUNCIONAMIENTO.md).

**No hay copia de seguridad automática.** Para recuperar el original, verifica los archivos del juego desde Steam o reinstálalo. Los hashes esperados están en [Transparencia](TRANSPARENCIA.md#archivos-admitidos).

## Alcance de los accesos

En el código revisado:

- Los accesos a Steam sirven para localizar el juego y verificar su versión.
- No hay funciones de descarga automática, telemetría, lectura de credenciales, instalación al arrancar, inyección en procesos ni desactivación de antivirus.
- Los enlaces externos son direcciones fijas de GitHub que se abren cuando el jugador los pulsa.
- Los recursos incrustados son la receta de cambios, el banner, el icono, la marca gráfica y la licencia. Las imágenes se usan en la interfaz.
- Los mapas, guardados, PAK y archivos de audio no se modifican durante la aplicación del parche.

Estas afirmaciones describen el aplicador revisado. El cambio aplicado al juego tiene su propio [alcance técnico y límites](FUNCIONAMIENTO.md), y su validación multijugador continúa.

## Cómo contrastamos el EXE con el código

El archivo analizado y publicado tiene SHA256:

```text
986141c161fb4e024ced0be7a174663eebb01f18d17270bc4862c9bdbfa47ed3
```

Se recompiló el código del commit de entrega `26c6c46` en un directorio aislado, usando el compilador de .NET Framework con firma válida de Microsoft. La comparación encontró:

- Coincidencia de los 57 métodos inspeccionados, sus instrucciones IL, las seis referencias a bibliotecas .NET y los cinco recursos incrustados.
- Tras reproducir los saltos de línea del manifiesto, el mismo tamaño y únicamente 47 bytes distintos, correspondientes a la fecha del encabezado PE y los identificadores generados por el compilador. Todos los demás bytes coinciden.

La comprobación relaciona el binario distribuido con ese código fuente. Se hizo en la misma máquina y no equivale a una auditoría externa de toda la cadena de compilación. Un hash identifica un archivo: por sí solo no demuestra que sea seguro. El EXE actual no está firmado con Authenticode.

## Qué significan las alertas observadas

Una herramienta de análisis puede señalar funciones o características que también tienen usos legítimos. Para interpretarlas hay que examinar dónde y para qué se usan.

| Regla de MetaDefender | Qué detectó | Uso comprobado en Syncrash |
| --- | --- | --- |
| Hashing (`DN023`) | `SHA256.Create` y `ComputeHash`. | Comprobar archivos y receta; rechazar entradas o resultados no admitidos. |
| Descubrimiento de procesos (`DN032`) | `Process.GetProcessesByName`. | Exigir que Imperivm esté cerrado. |
| Información del sistema (`DN021`) | Métodos de `DriveInfo`. | Buscar bibliotecas Steam en unidades fijas. Son unidades de almacenamiento, aunque el rótulo del servicio use la palabra «driver». |
| Entropía elevada (`H000`) | Datos con una distribución de bytes similar a contenido comprimido. | Hay imágenes comprimidas incrustadas. El banner ocupa 2.145.191 de los 2.400.256 bytes del EXE. Es una explicación plausible de esta señal. |
| Posible contenido empaquetado (`SIGG017`) | Ejecutable sin firma y datos de alta entropía. | La compilación revisada no usa un empaquetador u ofuscador; incrusta recursos gráficos y la receta. La regla por sí sola no prueba que haya una carga maliciosa. |

**No quitamos comprobaciones de integridad o de juego cerrado para reducir alertas.** Cumplen una función de protección. Tampoco se ha demostrado que modificar las imágenes o firmar el programa elimine las detecciones de los motores antivirus.

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

La hipótesis de falsos positivos es la valoración del proyecto basada en las comprobaciones descritas. Sigue pendiente solicitar y obtener una revisión de los proveedores que detectan el archivo. Mientras se aclaran esas detecciones, mantenemos la recomendación de posponer nuevas instalaciones.

## Cómo comprobar una descarga y comunicar un problema

Descarga desde la [release oficial](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/releases/latest) y contrasta el archivo con el SHA256 de la [ficha de entrega](ENTREGA_ACTUAL.md) y `SHA256SUMS.txt`. Quien quiera revisar o compilar el código dispone de las instrucciones del [README](../README.md#compilar-y-conocer-el-proyecto).

Si aparece una alerta, conserva el motor, la versión de firmas, el nombre de detección y el hash. No desactives el antivirus ni añadas exclusiones para forzar la ejecución. Puedes comunicar esos datos a `xtalvarotx` en Discord o abrir una incidencia sin datos privados.

La revisión de posibles falsos positivos con los proveedores sigue pendiente. Se actualizará la documentación con sus conclusiones y con cualquier cambio del ejecutable; un informe corresponde a un hash concreto y no debe reutilizarse como garantía para otra versión.
