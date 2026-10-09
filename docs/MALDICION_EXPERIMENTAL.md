# Protección al retirar una maldición

**09/10/2026 · incluida por defecto en [v1.0.8 publicada](ENTREGA_ACTUAL.md).** El usuario decidió incluir esta protección provisional para evitar el cierre conocido mientras se investiga el origen y la recuperación de desincronizaciones. Ya no requiere una compilación especial ni una casilla adicional.

## Qué protege

Al retirar un efecto de maldición/bendición, el motor puede resolver una referencia cuyo puntero intermedio es nulo y leer `[ECX+4]` en `0x005D4854`. La protección comprueba ese puntero y, si es nulo, conduce a la salida existente `0x005D4876`. El estado y el handle del efecto ya se han limpiado en esa rutina. Si el puntero no es nulo, ejecuta las instrucciones originales y continúa en `0x005D485B`, conservando registros, pila y banderas de esa ruta.

El código adicional ocupa 28 bytes desde `0x00A95040` en `.ivfix`; conserva el helper anterior de 42 bytes y LAA. La sección pasa a declarar 92 bytes virtuales. Se mantiene exactamente el código nativo del piloto inicial; cambia su integración en el aplicador. No modifica scripts ni PAK y solo se distribuyen diferencias exactas del ejecutable.

**Evita el acceso nulo observado; no repara el origen de la referencia inconsistente.** No cubre punteros no nulos dañados ni garantiza continuidad sincronizada. La rutina sirve también a bendición; los incidentes históricos identificados correspondían a maldición. No se ha observado una desync causada por este cambio, pero tampoco se ha validado su continuidad en partida.

## Instalación normal y compatibilidad

En Steam vanilla, Aplicar parche y las órdenes normales de instalación incluyen las cuatro protecciones y LAA. Se llega al mismo resultado desde el original, la versión anterior sin LAA y la versión anterior con LAA. Cada ruta reconstruye el resultado completo en memoria y hace un único reemplazo verificado; no necesita instalar primero un parche intermedio. Repetir la aplicación conserva bytes y fecha del EXE. Pantalla y voces aceptan el nuevo hash.

| Archivo admitido | SHA256 |
| --- | --- |
| Original Steam, 4456448 bytes | `72b09d1abd4f311efe4213a9a1110185519bde4db4ee57769d346b475c748473` |
| Syncrash anterior sin LAA, 4460544 bytes | `752c95a475e62b0d61d88ec9fb7fabc07758cb217ab152d78651385a5de3d2cc` |
| Syncrash anterior con LAA, 4460544 bytes | `af59a5bbb4956f50dd671a4a52753376b7b359b2028c0331a2db5fde6f64c965` |
| Resultado con cuatro protecciones, 4460544 bytes | `cd35437004a441b7e70a8f0fd606003dea6c1e084667a7d813736c5d82a456cc` |
| data.pak Steam vanilla | `6926c286b8e44dba9244723fbcd153a3ee8fc28633e3c22cc49c27d206c96d50` |
| data.pak Community v12 definitivo revisado | `f946faf95e31211da3443fec6956a811a42b701ecaf7291a2794dedf04cec81e` |

**Community conserva un alcance limitado:** únicamente las órdenes individuales siguientes admiten su PAK exacto, y requieren el EXE anterior con LAA o el resultado protegido. No se habilitan la instalación general ni las opciones de pantalla/voces para el mod.

## Compilación, aplicación y retirada individual

Con los requisitos de [compilación del proyecto](../README.md):

```powershell
& ./src/Syncrash/build.ps1 -OutputPath ./work/mi-candidato/Syncrash.exe
& ./tests/run.ps1 -OutputDirectory ./work/pruebas-integracion
```

Añadir `-ScreenBundleDirectory <bundle revisado>` incluye los componentes existentes de pantalla. La versión del ensamblado y manifiesto es 1.0.8.0 y la informativa `1.0.8`. La compilación normal incluye la protección; el switch anterior `-ExperimentalCurseGuard` se ha eliminado.

Con el juego cerrado, en Steam vanilla basta el botón Aplicar parche o `--apply <gbr.exe>`. Para mantenimiento individual, también en el Community exacto admitido:

```powershell
& ./work/mi-candidato/Syncrash.exe --check-curse-guard 'RUTA/gbr.exe'
& ./work/mi-candidato/Syncrash.exe --apply-curse-guard 'RUTA/gbr.exe'
& ./work/mi-candidato/Syncrash.exe --remove-curse-guard 'RUTA/gbr.exe'
```

La comprobación no escribe. La retirada recupera exactamente el EXE anterior con LAA y las tres protecciones originales; repetirla no reescribe. **Volver a aplicar normalmente Syncrash en vanilla reinstala la cuarta protección.** Ya no es necesario retirarla para configurar pantalla o voces. La retirada exige también uno de los PAK admitidos. Salidas: 0 correcto/aplicable, 3 estado solicitado ya presente, 1 error, 2 argumentos incorrectos y 4 aplicación concurrente.

## Evidencia y entrega 1.0.8

Aplicador publicado y descargado para verificarlo: **18586112 bytes**, SHA256 `6835b488180ac785111abd83d772c0df0b7f7816f05f78c493c3c443e5f5d155`. Versión 1.0.8.0 / informativa 1.0.8. Conserva el parche nativo del RC1; únicamente se retira el sufijo RC del título y metadatos.

El candidato de integración `1.0.8-rc.1` pesaba **18586112 bytes**, SHA256 `0aad810e8db60296929505d36377719b1ceac62f1a6506f0132de750224b6564`. Se conserva como evidencia previa a la entrega 1.0.8.

- Entrega final 1.0.8: **39 pruebas del aplicador y 55 operaciones sobre copias reales**, con el hash final indicado arriba. Dos compilaciones completas coinciden byte a byte. Se comprobaron actualización, idempotencia, retirada/reinstalación, pantalla y voces en los tres idiomas y rechazo de archivos desconocidos. No se inició el juego.
- Integración RC1: 39 pruebas del aplicador y 55 operaciones sobre copias reales, con dos compilaciones idénticas. Incluyen actualización desde las tres bases, retirada individual y reinstalación, pantalla y voces en tres idiomas, alcance limitado de Community y rechazo de archivos desconocidos.
- Código nativo idéntico al piloto del 09/10: se conserva su evidencia de 25 casos de emulación. Cuatro reproducen la lectura nula anterior y retornan con la protección; 21 rutas restantes mantienen registros, banderas y estado comprobado. El callback de tipo usa un objeto sintético y la emulación se detiene antes de ejecutar la destrucción final.
- El piloto inicial `1.0.8-curse-preview.1` pasó 39 pruebas y 18 operaciones sobre copias. Su aplicador completo pesaba 18585600 bytes, SHA256 `84843a5e5126b40c835c2d1dd47a6a38262eacb2ed50be75a7b62d304092f071`. Es evidencia histórica; ese piloto rechazaba el flujo normal tras aplicar la protección.

| Receta vigente | SHA256 |
| --- | --- |
| Original → resultado | `bc437073ae1134502b510d898468cac20b74633f5407d17c79fe6c84b71d03aa` |
| Anterior sin LAA → resultado | `fc9f6343c5c7dbe5a9205a4141435eba7b5f988254bb4b965b9369f7b8ba00b6` |
| Anterior con LAA → resultado | `2c57f63766cae3ed4936f1fd16954e0a1b9ec606b3261bfe0c577f2a0a447d42` |
| Retirada individual | `5267caa51427dc679d9cc4e5be0a3351bc702dab98d1a1949b980bb276ed3c32` |

Fuentes: [motor de aplicación](../src/Syncrash/PatchEngine.cs), [mantenimiento individual](../src/Syncrash/CurseGuard.cs), [receta completa](../src/Syncrash/recipe.json), [actualización sin LAA](../src/Syncrash/upgrade-v2.json), [protección individual](../src/Syncrash/curse-guard.json) y [retirada](../src/Syncrash/curse-guard-remove.json). Evidencia extensa, logs y archivos del juego permanecen excluidos de Git en `work/curse-guard-20261009/`.

**Seguimiento pendiente:** caducidad real de maldición/bendición, muerte o retirada del efecto, guardado/carga y continuidad multijugador con dos clientes iguales; observar efectos residuales y desync. La integración está hecha por decisión del usuario, sin presentar esas pruebas como realizadas. La instalación local autorizada quedó protegida el 09/10: hash de salida verificado, PAK y configuración conservados. No se inició el juego. La descarga pública está verificada; la instalación no equivale a una prueba de partida.
