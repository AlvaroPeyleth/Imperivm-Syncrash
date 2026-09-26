# Candidato local 1.0.4.0: memoria y pruebas de pantalla

Estado al 26/09/2026: **en pruebas, sin publicar**. Se conserva en Git el candidato LAA y la documentación de los ensayos. La entrega pública continúa siendo [v1.0.3](ENTREGA_ACTUAL.md). Este candidato incorpora LAA; todavía no incorpora una corrección de pantalla. Las modificaciones siguientes requieren pruebas de juego antes de plantear su distribución.

## Memoria

El ejecutable del juego sigue siendo x86. Se activa `IMAGE_FILE_LARGE_ADDRESS_AWARE`: en Windows de 64 bits permite hasta 4 GB de espacio de direcciones de usuario. No reserva 4 GB de RAM, no convierte el juego a 64 bits y no demuestra por sí solo mejoras de FPS o estabilidad. Hay que validar partidas, guardado/carga y el uso de direcciones superiores a 2 GB. [Referencia de Microsoft](https://learn.microsoft.com/en-us/windows/win32/memory/memory-limits-for-windows-releases).

Se conserva la guarda V2. Respecto a su resultado anterior solo cambian dos bytes: el indicador LAA en el desplazamiento 326 (`0f` a `2f`) y el checksum PE en el 392 (`69` a `89`). El checksum se calculó y contrastó con `MapFileAndCheckSumW`. No cambia el tamaño del juego ni se tocan sus instrucciones de simulación.

El aplicador admite el original Steam o el resultado V2 exacto de las entregas anteriores. Ambas rutas producen el mismo resultado. Conserva validación de PAK, reconstrucción por hashes, exclusión entre instancias, comprobación de juego cerrado y sustitución mediante archivo temporal. La comprobación distingue una actualización de una instalación original; los errores conservan el estado de la versión anterior.

| Archivo o estado | Bytes | SHA256 |
| --- | ---: | --- |
| Aplicador local 1.0.4.0 revisado | 2.405.888 | `9545acebf3d2136aa7da2d84c9794e0460cf52f5b36f0e9e936a749836b79df7` |
| gbr.exe original admitido | 4.456.448 | `72b09d1abd4f311efe4213a9a1110185519bde4db4ee57769d346b475c748473` |
| gbr.exe V2 anterior admitido para actualizar | 4.460.544 | `752c95a475e62b0d61d88ec9fb7fabc07758cb217ab152d78651385a5de3d2cc` |
| gbr.exe V2 + LAA | 4.460.544 | `af59a5bbb4956f50dd671a4a52753376b7b359b2028c0331a2db5fde6f64c965` |
| Packs/data.pak admitido por el aplicador | 2.518.522 | `6926c286b8e44dba9244723fbcd153a3ee8fc28633e3c22cc49c27d206c96d50` |

## Comprobaciones del 26/09/2026

- `tests/run.ps1`, con Windows PowerShell 5.1: **16 pruebas superadas**. Incluyen actualización, idempotencia, errores de permisos, entrada cambiada durante la operación y rechazo de archivos desconocidos.
- `scripts/compare-builds.ps1`, también con PowerShell 5.1: dos compilaciones locales idénticas al hash de la tabla. Árbol con cambios sobre `960fe3c`; no se afirma reproducibilidad entre máquinas ni empaquetado desde un commit limpio.
- Una compilación intermedia (`9a958bb6…6456279`, antes de normalizar el formato de las recetas) superó diez operaciones sobre copias de archivos reales: comprobación y aplicación desde original/V2, rechazo por permisos, idempotencia y rechazo de ejecutable alterado. Es evidencia histórica de ese hash.
- El aplicador revisado `9545aceb…6b79df7` se utilizó después sobre una instalación real, con autorización del usuario: actualización desde V2 correcta, resultado `af59a5bb…f64c965` y PAK original intacto. Se preservaron el EXE y la configuración anteriores en el archivo privado.
- Prueba de partida con LAA y PAK original a 1920×1080: el usuario confirma entrada correcta y responde favorablemente a la comprobación de guardado, recarga, Alt+Tab y cierre. La incidencia comunicada es que el escritorio sigue viéndose grande al tabular. Es feedback de una sesión, sin prueba de estrés, direcciones superiores a 2 GB ni multijugador. Antivirus del aplicador revisado, Windows 10 y segundo equipo: pendientes.

Los logs, binarios de ensayo, archivos originales y mediciones permanecen en `work/`, fuera del repositorio público. Las pruebas de v1.0.3 siguen vinculadas a sus hashes y no se atribuyen a esta versión.

## Pantalla: ensayo nativo retirado; escalado pendiente

En una instalación con V2 se observó que el monitor principal bajaba de 3440×1440 a 1920×1080 durante el juego y recuperaba su modo al cerrarlo. El segundo monitor no cambió. El menú ofrecía como máximo 1920×1080. Esto explica el aumento aparente del tamaño de otras aplicaciones en esa sesión; no acredita un fallo de DPI en todos los equipos.

La lista procede de la sección `[Resolutions]` de `DATA/CONST.INI`, dentro del PAK admitido. Se ha preparado una prueba local que sustituye exclusivamente `Res7_x=1280` / `Res7_y=1024` por 3440 / 1440. Conserva 1920×1080 y el índice y tamaño del PAK. El PAK de ensayo tiene SHA256 `067ce32095a3a5478925d1a4d0b8b9794f34dd8654a3c3350847b5558d83511f`.

El usuario pudo seleccionar 3440×1440 y las mediciones registraron el escritorio en su resolución nativa. Sin embargo, comunicó que la interfaz quedaba demasiado pequeña y que el juego se cerró al entrar a jugar. El registro conservado identifica una excepción C++ `std::bad_alloc`; esto acredita un fallo de asignación, pero no demuestra falta de RAM física ni establece por sí solo la causa. El ensayo se considera fallido.

Se conservaron los registros antes de otro arranque y se restauraron el PAK original, comprobado por hash, y `Resolution=0` (1920×1080), manteniendo LAA. El usuario confirmó después que podía entrar en una partida normal. No consta que ambos intentos usasen un mapa y configuración idénticos; falta una comparación controlada para atribuir la causa del cierre.

El PAK de ensayo queda retirado y nunca se integró en el aplicador, que lo rechaza. El objetivo de los ensayos posteriores es conservar una resolución interna compatible y escalar su imagen manteniendo el escritorio nativo, las proporciones y el cursor correcto. El aplicador todavía no incorpora esta función; la prueba local favorable más reciente se detalla debajo.

En ensayos locales del 26/09/2026 con DxWnd 2.06.15 se logró mostrar el menú completo y el usuario confirmó clics correctos en Opciones y Salir. Una primera modalidad produjo menor nitidez del menú y bloques móviles en partida. Al cambiar a un contexto de dibujo GDI emulado, el usuario comunicó un resultado favorable y aportó capturas del menú y de una partida; queda como observación cierta pixelación al ampliar. Este perfil conserva la corrección del ratón y tiene SHA256 de preparación `26a10b8435cf76c349f9ca1559a343a65e69f4d731eb339d118202e7eba3ba5f`. Es una prueba básica favorable en un equipo: faltan guardado/recarga, Alt+Tab, reinicio y pruebas prolongadas con este perfil antes de integrar el escalado en Syncrash. Estos resultados no alteran la evidencia de la prueba LAA sin DxWnd.

## Criterios pendientes antes de publicar

Una prueba posterior de carga automática permitió al usuario arrancar desde Steam sin abrir DxWnd, con resultado favorable en partida. La barra de tareas tapaba parte del borde inferior y seguía pendiente la calidad de ampliación. El siguiente ensayo de suavizado y selección de área del monitor produjo corrupción progresiva al mover el ratón y apertura en el monitor secundario: se retiró. Se recuperaron la geometría anterior y la presentación sin suavizado, conservando únicamente el aviso de pantalla completa a Windows para comprobar la barra de tareas por separado.

El usuario confirmó después monitor correcto, imagen estable, barra inferior completa, clics correctos y Alt+Tab sin problemas. Resultado básico favorable del perfil automático SHA256 `8f290c151ce6ebd7c50a7a9449825fd6997d23676921a821f0b0f7dac7055007`, con proxy winmm SHA256 `7669547b9a4b4ec57870202146a1200691ccdbed7b2ea9e2d29da6d2a6342cb1` y DxWnd DLL SHA256 `d46109c8cfa9111e33cb4136f605407beef4bbd809047b3a06792ce0e8adabc3`; archivos instalados comprobados por hash. Es feedback del equipo de prueba, no una nueva medición del modo físico ni validación de otros monitores. Faltan guardado/recarga con este perfil, duración y arranques repetidos; su geometría aún es específica del equipo. No forma parte del aplicador. La integración prevista será opcional al aplicar el parche, sin convertir Syncrash en launcher; requiere instalación/retirada, adaptación al monitor y revisión de distribución de dependencias.

La instalación de ensayo vuelve a utilizar el PAK original y LAA. La prueba de resolución nativa fallida se conserva como evidencia histórica, no como función disponible.

1. Ampliar la prueba LAA a partidas exigentes y multijugador, incluyendo guardado y recarga. La sesión básica confirmada por el usuario no demuestra compatibilidad con todas las direcciones de memoria ni todas las partidas.
2. Investigar una presentación escalada que mantenga el escritorio nativo sin exigir al motor renderizar a 3440×1440. Verificar menú, cursor, partida, Alt+Tab, reinicio y recuperación de ajustes antes de integrarla.
3. Diseñar la integración de pantalla según el resultado, con estados admitidos y recuperación explícitos; repetir pruebas del aplicador y del juego para los hashes definitivos.
4. Revisar documentación, diff, compilaciones, análisis antivirus y paquete de entrega antes de publicar. El audio queda para una investigación posterior.

## Siguiente investigación: calidad de ampliación y distintos monitores

Dirección acordada el 26/09/2026: conservar la configuración favorable y estudiar mejor presentación sin exigir resoluciones internas superiores a 1920×1080. Separar resolución interna del juego y tamaño de salida: una pantalla 3440×1440 puede mostrar la imagen 1920×1080 ampliada a 2560×1440, centrada y sin deformación. No requiere que el motor dibuje el mapa a 3440×1440. El filtrado puede suavizar bordes, pero no recupera detalle original inexistente.

Propuesta pendiente de implementación: identificar el monitor principal de Windows por `MONITORINFOF_PRIMARY`, trabajar con coordenadas físicas coherentes con DPI y calcular el mayor rectángulo que respete la proporción interna. No reutilizar el índice cero ni las coordenadas fijas del ensayo como detección general. [Documentación de Microsoft](https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-monitorinfo).

Ejemplos de geometría prevista para entrada 1920×1080; son cálculos, no pruebas de compatibilidad:

| Monitor | Imagen presentada | Márgenes |
| --- | --- | --- |
| 1920×1080 | 1920×1080, sin ampliación | Ninguno |
| 1920×1200 | 1920×1080 | 60 píxeles arriba y abajo |
| 2560×1080 | 1920×1080 | 320 píxeles a cada lado |
| 3440×1440 | 2560×1440 | 440 píxeles a cada lado |

Para pantallas menores habrá que comprobar reducción y legibilidad, o elegir un modo interno ya admitido; no se prometen modos nuevos del motor. No se amplía ahora el alcance a renderizado interno 1440p/4K ni a texturas con IA.

La lectura del código oficial DxWnd 2.06.15 muestra que `AcquireEmulatedDC` copia la ventana presentada al contexto virtual y `ReleaseEmulatedDC` la vuelve a ampliar. Esto confirma un recorrido de ida y vuelta, compatible con degradación acumulativa al filtrar, aunque no demuestra por sí solo toda la causa del fallo observado. La siguiente prueba debe conservar una superficie original independiente y filtrar únicamente la salida, contemplando repintados parciales, cambios de tamaño y recuperación tras Alt+Tab. Activar `DXWND_SAVELASTDC` sin más no elimina la copia de retorno en el código examinado.

Se compararán presentación actual, tamaño original y un filtro suave opcional antes de considerar enfoque adicional. El filtro HALFTONE de Windows exige también establecer el origen del pincel, según [Microsoft](https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-setstretchbltmode); esa llamada no aparece en la rama examinada, pero añadirla por sí sola no garantiza corregir el recorrido acumulativo. Suavizado, coste por fotograma y nitidez del texto requieren una prueba nueva; no se ha instalado otro ensayo sobre la configuración favorable.
