# Syncrash 1.0.5 · v1 experimental con voces opcionales

**27/09/2026: publicada como v1.0.5. Syncrash v1 es experimental y esperamos feedback de los jugadores.** Esta ficha concentra uso, cambios, pruebas y límites. La autorización para lanzar no equivale a nuevas pruebas en partida; las partidas del prototipo no se atribuyen al nuevo ejecutable.

## Binario publicado

| Dato | Valor |
| --- | --- |
| Aplicador | `1.0.5.0` · `1.0.5` |
| Tamaño del EXE completo | 18.579.968 bytes |
| SHA256 | `187d62824b609385f2dc1371ffd873a25d0f2cb70e0a6292f7f72dd2c57a4118` |
| Estado de fuentes | Commit de compilación y tag: `6260bc848a0e1b70c6fbdd84c69c27e0dd19ac70`. |

El EXE incluye pantalla, fuentes/licencias y el mapa de voces. Las grabaciones se extraen de la instalación del jugador, no se distribuyen. El nombre de archivo de esta ficha se conserva para mantener sus enlaces.

## Qué añade

El aplicador incorpora **Reparar voces de unidades**, marcada por defecto y desactivable. Lee el idioma de `Settings.ini` y copia las grabaciones necesarias desde el PAK local a las rutas sueltas que solicita el juego. Conserva los PAK originales; no redistribuye grabaciones ni modifica los XML del juego.

| Idioma | Rutas instaladas | Comprobación de archivos |
| --- | ---: | --- |
| Español | 188 | Extracción, decodificación completa y aplicación/retirada sobre copia real. |
| Italiano | 188 | Extracción, decodificación completa y aplicación/retirada sobre copia real. |
| Inglés | 393 | 235 nombres conservados con prefijo de ruta corregido y 158 correspondencias de frases; extracción, decodificación y aplicación/retirada sobre copia real. |

Las correspondencias de frases se limitan a listas `UnitOrder` de una misma unidad con frecuencias iguales. Se emparejan nombres numéricos y frases ordenadas; **no se afirma recuperar la numeración histórica de cada frase**. En inglés se excluyen los WAV numéricos sobrantes al seleccionar las frases de `CHERO`. La variedad y el contenido escuchado deben validarse en partida.

Memoria, protección de cierres y componentes de pantalla mantienen el resultado de v1.0.4. Cambia la versión del aplicador a **1.0.5.0**, identificada como `1.0.5`; el `gbr.exe` producido conserva su SHA256 anterior.

## Uso y cambio de idioma

1. Cierra Imperivm y abre Syncrash. Selecciona su `gbr.exe` si no se detecta.
2. Deja marcada **Reparar voces de unidades** y pulsa **Aplicar parche**. Pantalla adaptable conserva su casilla independiente, también marcada por defecto.
3. Abre Imperivm (versión Steam), también puedes usar `gbr.exe` directamente. No necesitas dejar abierto Syncrash.
4. **Si cambias el idioma del juego, ciérralo y vuelve a aplicar Syncrash antes de jugar.** Comprueba que el resultado indica el idioma elegido. Los destinos son comunes: el aplicador sustituye las copias registradas del idioma anterior y elimina las que sobran.
5. Para retirar las voces, **desmarca Reparar voces de unidades y pulsa Aplicar parche**. No basta con cerrar la ventana ni con verificar archivos en Steam. Para quitar también pantalla y protección, restaura después la pantalla y verifica los archivos del juego en Steam.

La casilla de voces expresa el estado deseado al aplicar: desmarcarla retira la reparación. La de pantalla mantiene su comportamiento anterior: desmarcarla conserva la pantalla instalada; su botón **Restaurar pantalla original** la retira. Abrir la interfaz o cambiar una casilla por sí solo no escribe archivos.

## Propiedad, compatibilidad y recuperación

- Los WAV se escriben bajo `CurrentLang/voices/`; `.syncrash-voices/manifest.json` registra la instalación o una operación pendiente. Se conserva un archivo de bloqueo y pueden quedar carpetas vacías al retirar.
- Un archivo sin registro se considera ajeno incluso si sus bytes coinciden. Un archivo registrado pero modificado bloquea la operación antes de tocar las voces; se conserva y se muestra la ruta. No se adopta ni sobrescribe contenido de otro mod.
- Se validan todos los destinos antes de aplicar el parche base. Se rechazan enlaces/junctions y rutas fuera de la instalación. Se exige el juego cerrado y se excluyen dos operaciones de voces simultáneas, incluido el instalador privado anterior.
- El registro se publica antes que los WAV y conserva los idiomas implicados durante una transición. Si hay una interrupción, mantén el juego cerrado y reaplica, o desmarca voces y aplica para retirarlas. Se comprueban los hashes antes de borrar o sustituir. Un cierre abrupto puede dejar temporales privados; no se promete una transacción atómica del conjunto de WAV.
- Se admite migrar el ensayo privado completo de 188 rutas españolas o italianas, verificando su mapa y hashes. No se reescriben WAV que ya coinciden. Los scripts privados anteriores no gestionan el nuevo registro: usa este aplicador después de migrar.
- Se comprueban los PAK exactos identificados abajo. **Conservarlos intactos evita sobrescribir cambios de otros mods, pero no garantiza compatibilidad con ellos:** un PAK modificado se rechaza para instalar voces. La retirada independiente puede usarse aunque los PAK hayan cambiado.
- No hay detección automática de idioma al arrancar el juego, servicios ni seguimiento permanente. Una operación conjunta puede completar memoria/pantalla y fallar después en voces; el mensaje distingue ese estado y permite recuperar las voces.

## Archivos identificados

| Recurso | SHA256 |
| --- | --- |
| Mapa de correspondencias incrustado | `d5d347d4d7184fce57b002c7ea696dcac42d6deaed4a3510e111bcfd7b3e96d2` |
| `Packs/data.pak` | `6926c286b8e44dba9244723fbcd153a3ee8fc28633e3c22cc49c27d206c96d50` |
| `local/spanish.pak` | `22fcebb4b0846420bac911812bd66f42c43c54fc6f5575a4319c6aaf993c76e2` |
| `local/italian.pak` | `a5f11c3013ec5beef25ae22768eeb3ce96145f95bfd1906bb26f280ea5c971de` |
| `local/english.pak` | `a2b6ce5281a84824a25f42aed592974785b538830e6d38ce0e3bcca8d0bd17f3` |
| Resultado `gbr.exe` con protección de cierres y LAA | `af59a5bbb4956f50dd671a4a52753376b7b359b2028c0331a2db5fde6f64c965` |

El [mapa público](../src/Syncrash/voice-map.json) contiene solo rutas, tamaños y hashes. [Su generador](../scripts/build-voice-map.py) lo reconstruye desde los PAK locales identificados; opcionalmente verifica la decodificación mediante FFmpeg. El aplicador usa únicamente .NET Framework 4.8: Python y FFmpeg son herramientas de preparación, no requisitos para el jugador.

## Verificación y pendientes

- **36 pruebas sintéticas/Windows** sobre el ejecutable publicado: incluye extracción PAK, tres idiomas, reaplicación, retirada, migración, archivos ajenos/modificados, interrupciones simuladas, concurrencia, rutas inseguras y regresiones de memoria/pantalla.
- **31 operaciones CLI sobre copias reales**, cotejando todos los WAV instalados y la integridad de los PAK y EXE correspondientes; incluye cambios de idioma y conservación de pantalla.
- **Dos compilaciones completas idénticas** con PowerShell 7.6.5 (.NET 10.0.11) y .NET SDK 8.0.400; el EXE de las pruebas coincide con ese mismo SHA256.
- **769 entradas de audio decodificadas** durante la preparación del mapa. No equivale a escucharlas en partida.
- Manejador real de **Aplicar** probado en una copia con la casilla activada y desactivada; retirada y reactivación de controles comprobadas. Ventana renderizada y revisada a 780×666 y 710×520. Bloqueo entre el prototipo Python y el nuevo aplicador comprobado entre procesos.

La evidencia privada del lanzamiento se conserva en `work/release-1.0.5-20260927/`; la investigación y el candidato anterior permanecen en `work/audio-voices-20260927/README.md`, con sus fechas, informes y hashes. Los resultados anteriores se mantienen separados de los del ejecutable publicado.

La validación auditiva anterior del usuario corresponde al prototipo español; la captura del 27/09 observó 20 rutas reparadas de seis unidades abiertas correctamente, sin errores DirectMusic en el log final. Una sesión anterior tuvo dos errores sin recurso identificado y un retraso inicial no explicado. No se han escuchado individualmente todas las rutas ni validado partidas en italiano/inglés con el candidato.

**Feedback y pruebas pendientes:** validar la versión desde la interfaz en español, italiano e inglés, volver a español y comprobar la retirada con la casilla. Probar voces de héroe, arquero y sacerdote/sacerdotisa, además de las unidades afectadas disponibles de otras facciones; confirmar que el idioma y la variedad son correctos. Completar la comprobación multijugador. Si reaparece el retraso inicial, conservar el log de esa sesión e investigarlo sin atribuirle una causa aún no demostrada.

La [release v1.0.5](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/releases/tag/v1.0.5) se publicó el 27/09 tras reproducir el EXE desde el commit limpio `6260bc8`, con [CI correcto](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/actions/runs/36350599727). Los tres archivos descargados de GitHub coinciden con las copias locales. CI prueba y compara la base sin componentes gráficos; el EXE completo se verificó localmente. No se atribuyen a esta versión análisis antivirus de otros ejecutables. Para dar feedback, sigue la [guía del README](../README.md#ayuda-a-mejorar-syncrash): idioma, opciones, unidades probadas y resultado, también cuando todo funcione.

## Historial del candidato local

El candidato `1.0.5-local` del 27/09 medía 18.579.968 bytes y tenía SHA256 `25f9ad2c107e660e9610d553d96ff5d7cc92c83b9c3a8bdc99b0422ad47eea67`. Superó 36 pruebas y 31 operaciones en copias; sus acciones de interfaz y capturas se conservan. La entrega cambia la identificación y el título/subtítulo a «v1 experimental»; mapa y lógica de voces se mantienen. Este hash histórico no identifica la descarga de la release.

## CLI y compilación

| Orden con la ruta de `gbr.exe` | Acción |
| --- | --- |
| `--check-voices` | Solo lectura; comprueba el idioma actual, fuentes y archivos instalados. |
| `--apply-voices` | Solo voces; exige una instalación base reconocida. |
| `--remove-voices` | Solo retirada de WAV registrados; no exige PAK originales. |
| `--apply-with-voices` | Memoria, cierres y voces; conserva la pantalla existente. |
| `--apply-all` | Memoria, cierres, pantalla con suavizado y voces. |

Las órdenes anteriores `--apply`, `--apply-with-screen` y `--apply-with-smoothing` conservan su alcance y no gestionan voces. Código de salida `0`: cambio realizado o comprobación admisible; `3`: ya aplicado o nada que retirar; `1`: error; `4`: otra operación en curso. No ejecutes dos aplicadores a la vez.

Las instrucciones de compilación del [README](../README.md#compilar-y-conocer-el-proyecto) y `tests/run.ps1` incorporan el módulo y mapa de voces. El mapa ya está incluido: regenerarlo no es necesario para compilar. Para reproducir el hash del EXE completo de esta entrega se usa PowerShell 7.6.5 (.NET 10.0.11). Windows PowerShell 5.1 empaqueta el ZIP de fuentes incrustado con otro método: conserva los mismos archivos, pero produce un EXE de hash y tamaño diferentes. La comprobación de entrega rechaza esa diferencia; no se sustituye silenciosamente el binario validado. Para reproducir su preparación, usa una ruta de salida nueva:

```powershell
python -B ./scripts/build-voice-map.py --game-root '<carpeta del juego>' --output '<mapa nuevo.json>' --ffmpeg ffmpeg
```

Conserva fuera de Git las copias de PAK, WAV, ensayos y ejecutables. El generador no guarda grabaciones; la decodificación opcional se hace por tuberías.
