# Candidato local 1.0.4.0: memoria, pantalla y suavizado opcionales

Estado al 27/09/2026: **entrega preparada en local, sin publicar**. La entrega pública sigue siendo [v1.0.3](ENTREGA_ACTUAL.md). El candidato se distribuye como **un único Syncrash.exe**, con componentes y fuentes de pantalla incrustados. «Añadir pantalla adaptable» es opcional y está marcada por defecto, con suavizado GPU incluido. Al desmarcarla se aplica solo memoria y cierres. No incluye resolución interna superior a 1080p ni cambios de texturas/audio.

## Memoria

El ejecutable del juego sigue siendo x86. Se activa `IMAGE_FILE_LARGE_ADDRESS_AWARE`: en Windows de 64 bits permite hasta 4 GB de espacio de direcciones de usuario. No reserva 4 GB de RAM, no convierte el juego a 64 bits y no demuestra por sí solo mejoras de FPS o estabilidad. La sesión comunicada se recoge debajo; no se ha medido el uso de direcciones superiores a 2 GB. [Referencia de Microsoft](https://learn.microsoft.com/en-us/windows/win32/memory/memory-limits-for-windows-releases).

Se conserva la guarda V2. Respecto a su resultado anterior solo cambian dos bytes: el indicador LAA en el desplazamiento 326 (`0f` a `2f`) y el checksum PE en el 392 (`69` a `89`). El checksum se calculó y contrastó con `MapFileAndCheckSumW`. No cambia el tamaño del juego ni se tocan sus instrucciones de simulación.

El aplicador admite el original Steam o el resultado V2 exacto de las entregas anteriores. Ambas rutas producen el mismo resultado. Conserva validación de PAK, reconstrucción por hashes, exclusión entre instancias, comprobación de juego cerrado y sustitución mediante archivo temporal. La comprobación distingue una actualización de una instalación original; los errores conservan el estado de la versión anterior.

| Archivo o estado | Bytes | SHA256 |
| --- | ---: | --- |
| Aplicador local 1.0.4.0, EXE único | 18.362.368 | `88814908bccdee383c8d1c6d9e374a31c7a2f450b3d970941944fbc1a101fa3c` |
| gbr.exe original admitido | 4.456.448 | `72b09d1abd4f311efe4213a9a1110185519bde4db4ee57769d346b475c748473` |
| gbr.exe V2 anterior admitido para actualizar | 4.460.544 | `752c95a475e62b0d61d88ec9fb7fabc07758cb217ab152d78651385a5de3d2cc` |
| gbr.exe V2 + LAA | 4.460.544 | `af59a5bbb4956f50dd671a4a52753376b7b359b2028c0331a2db5fde6f64c965` |
| Packs/data.pak admitido por el aplicador | 2.518.522 | `6926c286b8e44dba9244723fbcd153a3ee8fc28633e3c22cc49c27d206c96d50` |

## Comprobaciones del EXE actual · 27/09/2026

- Windows PowerShell 5.1: `tests/run.ps1`, **24 pruebas superadas**, con el EXE completo. La compilación base sin componentes superó esas pruebas con el candidato anterior del 26/09. Incluye protección de archivos, recuperación, concurrencia, CLI y exportación de fuentes sin sobrescribir un destino existente.
- `scripts/compare-builds.ps1`: dos EXE completos idénticos byte a byte, SHA256 de la tabla. Fuentes locales sobre `2f8f86f`, con cambios sin confirmar. Esta repetibilidad corresponde al aplicador C# con entradas nativas fijadas por hash; no demuestra builds nativos idénticos entre toolchains.
- **22 operaciones CLI sobre copias reales** del original y de V2: EXE aislado sin carpeta `screen`, parche base, ambas variantes de pantalla, idempotencia, retirada, rechazo de cambios de configuración sin retirada previa, retirada de la variante histórica, rechazo de archivos alterados/ajenos y exportación de las cinco entradas de fuentes verificadas. Ambas rutas producen `gbr.exe` LAA exacto y conservan el PAK. El juego no se ejecutó en estas copias.
- Interfaz simplificada, renderizada a 780×566 y 710×520: ruta, funciones incluidas, una casilla marcada con suavizado incluido, retirada visible y acción principal fija. Información mediante ayuda al pasar el ratón y panel desplegable accesible por clic/teclado. No equivale a validar todos los DPI ni lectores de pantalla.
- Evidencia nativa del 26/09, con los mismos hashes actuales: las fuentes nativas completas se reconstruyeron en un directorio nuevo desde upstream más el archivo de cambios incluido. Compilación x86 correcta. Pruebas de superficie: 400 presentaciones, fuente intacta, repintados parciales, cambio de modo y limpieza GDI. Pruebas GPU: colores, alineación 1:1, interpolación, bordes, fuente intacta, dimensiones inválidas, recreaciones y conservación del control FPU.
- Prueba manual inicial del filtro GPU: buena imagen y fluidez en menú y partida. La traza confirmó GPU activa, entrada 1920×1080 y salida 2560×1440. La sesión multijugador posterior se registra debajo; no se midieron FPS.
- El EXE anterior del 26/09 instaló y comprobó en la copia de uso las mismas DLL/perfil GPU ya ensayados, ahora con registro de propiedad para retirada. EXE del juego y PAK conservan los hashes de la tabla. No se atribuye otro ensayo manual a esta operación administrativa.

Evidencia actual en `work/screen-default-20260927/`; evidencia anterior en `work/embedded-screen-20260926/`, `work/menu-sharpness-20260926/`, `work/menu-filter-20260926/` y `work/menu-gpu-20260926/`. No se publican datos de jugadores ni archivos del juego.

## Nitidez y rendimiento: resultado vigente

Se conserva una superficie original independiente a resolución interna, sin volver a copiar la ventana ampliada sobre ella. El filtro opcional sube esa superficie a una textura dinámica y la presenta con interpolación bilineal por D3D9 en ventana. No solicita un cambio de modo del monitor, no cambia texturas, y usa `D3DCREATE_FPU_PRESERVE`. Si la presentación acelerada falla, recurre a la salida sin filtro. Recuperación tras pérdida real de dispositivo, GPU distintas y monitores en caliente siguen pendientes.

El primer filtro CPU HALFTONE mejoró las letras, pero produjo lag en partida. Al desactivar solo ese filtro, el usuario confirmó recuperación de fluidez; **se descartó**. Benchmark aislado GDI a 2560×1440: mediana 3,143 ms sin filtro y 57,711 ms con HALFTONE. La prueba GPU obtuvo 1,142 ms de mediana para subida/dibujo/Present desde una ventana oculta. Son métodos distintos, no una medición comparable de FPS ni del tiempo GPU completo. La validación práctica del nuevo filtro es la prueba manual favorable, con sus límites.

Los botones ya llegan dibujados en el bitmap del juego observado. No se han extraído ni sustituido fuentes o recursos del menú. Preparar assets a varias resoluciones requiere estudiar su formato y las dimensiones que admite el motor; no se presenta como una función implementada.

| Componente incrustado | SHA256 |
| --- | --- |
| dxwnd.dll modificado, 1.816.064 bytes | `829edcdfbc72e2c9b06d76f1ca64eed0279f0d01ab6bb54bde80cc53d66f7e89` |
| dxwnd.dxw sin suavizado | `94bfff55becb095ed0730e2e65e5598cecdb02c0704a2c7165dcbd458eebb8b5` |
| Perfil opcional GPU | `17992547c419d5a92f6aa445eb06cdc0be5bbbdbf439132fc93c9859744bc92a` |
| winmm.dll, 145.408 bytes | `34a17195617d5a3a5471807053b6d566a591278dfc8c531cc89d84ce077cbcd6` |
| Licencia de pantalla | `3972dc9744f6499f0f9b2dbf76696f2ae7ad8af9b23dde66d6af86c9dfb36986` |

## Uso, recuperación y fuentes

Sin opciones: memoria y protección de cierres; conserva una pantalla ya instalada. Pantalla adaptable: amplía el juego hasta encajar en el monitor principal, con proporciones y márgenes negros, manteniendo el escritorio. La casilla está marcada por defecto e incluye el filtro GPU sobre esa imagen. Se abre siempre desde Steam, sin launcher adicional.

La instalación escribe cuatro archivos de pantalla y su registro junto al juego; el usuario solo necesita descargar el EXE. No sobrescribe archivos ajenos ni adopta un ensayo sin registro. Para cambiar una variante ya instalada, pulsa **Restaurar pantalla original** y vuelve a aplicar las opciones deseadas. La retirada valida todos los archivos, quita primero el proxy y conserva LAA; admite también el paquete histórico de márgenes negros. Para retirar todo, restaura pantalla y después verifica el juego en Steam.

CLI: `--apply` aplica la base; `--apply-with-screen` añade pantalla sin suavizado; `--apply-with-smoothing` añade ambas. `--check` comprueba base y `--check-screen` el conjunto instalado, sin escribir. Código 0 si se aplica/admite aplicación, 3 si ya está completo, 1 ante error, 2 para uso inválido y 4 ante otra operación. `--remove-screen` retira pantalla. `--export-screen-sources <nuevo.zip>` guarda fuentes/licencias sin sobrescribir archivos.

«Licencias y fuentes de pantalla» exporta el archivo upstream intacto, fuentes del proxy, cambios completos de DxWnd con comandos/tests, procedencia y GPL. El aplicador propio conserva MIT; los componentes de terceros tienen su licencia independiente. No se incrustan archivos del juego. Las [fuentes nativas y perfiles revisados](../src/Screen/README.md) se conservan también en Git, con licencia y procedencia, sin DLL ni archivos del juego. El ZIP de cambios nativos tiene SHA256 `a513116c3d82dc6a4d857b097f8ee23b10df3b341674c8cdd815c1e9620b6a26`.

Para reproducir el EXE completo, añade `-ScreenBundleDirectory <bundle revisado>` a `build.ps1`, `tests/run.ps1` y `compare-builds.ps1`. Cada entrada se fija por hash y el archivo de fuentes se crea con orden y fechas constantes. Sin ese parámetro se compila la base de desarrollo y las opciones de pantalla quedan deshabilitadas. El preparador de entrega exige el bundle para reconstruir y comparar; no lo copia como carpeta externa.

**Preparación local completada:** el 27/09 el preparador recompiló desde el commit limpio `ce2ff9d786aea84664d61da2cbb7bce06ddab936` con Windows PowerShell 5.1 y reprodujo el EXE de la tabla. Se generó el paquete y su manifiesto; evidencia en `work/release-ready-20260927/clean-commit/`. No se han hecho nuevos análisis antivirus y no son un requisito de entrega. Falta autorizar la publicación. Guardado/recarga con el filtro final, otros GPU/DPI y cambios de monitor en caliente quedan como cobertura adicional no comprobada, no como nuevas pruebas obligatorias para este cierre.

## Prueba comunicada y cambio de opciones · 27/09/2026

El usuario confirma una sesión de **más de una hora con su hermano, ambos en Steam vanilla y con la misma versión del parche, sin incidencias**. Se registra como validación multijugador comunicada del paquete anterior facilitado (`169914bf167de1eba0fc2eb23811bc3b61d0e47cd15f6e55630f91f8b2e691af`, 18.362.880 bytes). Los componentes gráficos son los mismos del candidato actual; no se recogieron hash remoto ni logs de esta sesión. No añade una corrección causal de desync ni acredita por separado guardar/cerrar/recargar.

A petición del usuario se fusiona el suavizado dentro de Pantalla adaptable y se marca esa única opción por defecto. Se mantiene opcional, la retirada explícita y las órdenes CLI existentes para ensayos avanzados. No cambian los componentes gráficos, perfiles, receta del juego ni la instalación real; solo cambia el aplicador y su ayuda. Las comprobaciones automáticas de arriba se repitieron con el nuevo hash; la partida sigue siendo evidencia del paquete anterior, cuyos componentes gráficos son idénticos.

El EXE anterior superó 24 pruebas, dos builds idénticos y 22 operaciones en copias reales, conservadas en `work/embedded-screen-20260926/`.

## Historial resumido · 26/09/2026

Estos resultados pertenecen a candidatos anteriores; no sustituyen las comprobaciones del EXE actual. El registro completo, incluidos perfiles intermedios, hashes, logs y decisiones, se conserva en `work/project-status/COMPATIBILIDAD_WINDOWS.md`. La ficha anterior a esta consolidación está en `work/screen-default-20260927/candidate-before-summary.md`. No se publican archivos del juego ni datos de jugadores.

| Aplicador anterior (SHA256) | Bytes | Evidencia histórica |
| --- | ---: | --- |
| `9545acebf3d2136aa7da2d84c9794e0460cf52f5b36f0e9e936a749836b79df7` | 2.405.888 | LAA: 16 pruebas y dos builds idénticos. Actualización real desde V2 conservando el PAK; feedback favorable de partida, guardado/recarga y Alt+Tab, con el escritorio todavía ampliado. |
| `d704394587cbca8070d6626e74676157b8fa37c268b2336f798ba02aac055417` | 2.414.592 | Instalación y retirada de pantalla en copia real; evidencias anteriores a los márgenes negros. |
| `9c63f1217837f78d6239dadf1367d26e7c54f8672c20287f72e0184ab2b36832` | 2.414.592 | Siete operaciones en copia real. Instalación autorizada a las 20:31 UTC y respuesta favorable a las 20:33 sobre márgenes negros, barra inferior, clics, Alt+Tab y cierre. |
| `b643aba1ef23254f9c30d91434de5b958503edb144d2f66c4f32e7a09692643c` | 2.416.640 | Pantalla opcional con bundle externo: 23 pruebas, dos builds idénticos y 18 operaciones CLI en copias reales desde original/V2. |

**Resolución nativa descartada.** El juego cambiaba el monitor principal de 3440×1440 a 1920×1080 y restauraba el modo al cerrar. Se ensayó sustituir una entrada de `DATA/CONST.INI` por 3440×1440 (PAK `067ce32095a3a5478925d1a4d0b8b9794f34dd8654a3c3350847b5558d83511f`): interfaz demasiado pequeña y cierre al entrar en partida, con `std::bad_alloc`. No demuestra falta de RAM física ni una causa única. Tras restaurar PAK y 1920×1080, conservando LAA, una partida normal funcionó; no consta igualdad de mapa/configuración entre intentos. Ese PAK nunca se integró y el aplicador lo rechaza.

**Adaptación y márgenes.** Los primeros perfiles tuvieron bloques móviles, clics desplazados, barra inferior tapada o monitor incorrecto. Los ajustes posteriores recibieron feedback favorable en el equipo de prueba. El proxy actual detecta el monitor principal en cada arranque, conserva la resolución del escritorio y centra la imagen; las franjas negras aparecen con el juego activo y desaparecen al tabular/cerrar. Las pruebas automáticas cubrieron nueve casos de geometría y nueve de márgenes, además de entradas inválidas. La detección no se actualiza en caliente. El detalle de cada perfil retirado queda en el registro histórico; solo los componentes de la tabla vigente forman parte del candidato.

Ejemplos calculados para entrada 1920×1080; no son pruebas en esos dispositivos:

| Monitor | Imagen presentada | Márgenes |
| --- | --- | --- |
| 1920×1080 | 1920×1080 | Ninguno |
| 1920×1200 | 1920×1080 | 60 píxeles arriba y abajo |
| 2560×1080 | 1920×1080 | 320 píxeles a cada lado |
| 3440×1440 | 2560×1440 | 440 píxeles a cada lado |

**Nitidez.** Las capturas comparadas tenían distintos encuadres y tamaños (1466×868 y 1899×1197), por lo que no permiten medir la pérdida píxel a píxel. El código original de DxWnd copiaba la ventana presentada de vuelta al contexto virtual: un recorrido susceptible de degradación, sin demostrar que fuese la única causa. Se sustituyó por una superficie original independiente. El suavizado CPU se retiró por lag; el resultado GPU y sus límites están en «Nitidez y rendimiento», arriba. Las preferencias históricas de casillas separadas o desmarcadas quedan sustituidas por la opción conjunta marcada por defecto del 27/09.
