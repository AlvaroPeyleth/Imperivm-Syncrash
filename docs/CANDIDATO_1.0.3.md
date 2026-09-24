# Syncrash 1.0.3.0: candidato local y comprobaciones

**Estado a 24/09/2026: candidato local, todavía sin publicar.** La [entrega pública v1.0.0](ENTREGA_ACTUAL.md) es otro archivo, con su propio hash. Este documento es la fuente principal de las pruebas del candidato.

## Identidad y alcance

| Dato | Resultado |
| --- | --- |
| Versión de ensamblado, archivo y manifiesto | `1.0.3.0` |
| Plataforma | .NET Framework 4.8, WinForms, AnyCPU, `asInvoker` |
| Compilador | Roslyn de .NET SDK `8.0.400`, `/langversion:5 /deterministic+ /pathmap` |
| EXE comprobado tras la auditoría del 24/09/2026 | 2.403.840 bytes; SHA256 `251c92e0cc17dec527086349d7e065705b33f9373e9b1a907485f4716f07d50c` |
| Firma digital | Sin firma Authenticode |
| Receta incrustada | SHA256 `9388df692e9f0f0478f8060d625e643618cfc16dd28298c1cbe879aaa6bfc583`; sin cambios |
| Resultado exigido en `gbr.exe` | SHA256 `752c95a475e62b0d61d88ec9fb7fabc07758cb217ab152d78651385a5de3d2cc` |

El candidato conserva la receta, los hashes admitidos, el banner y las tres correcciones nativas de v1.0.0. Solo cambia el aplicador, así que el `gbr.exe` resultante tiene el mismo hash. El 24/09/2026 se recompiló el código definitivo de este cambio y se obtuvo el SHA256 de la tabla.

## Cambios respecto al aplicador público

- `--test <ruta>` ya no aplica el parche: devuelve uso inválido, código 2. Para escribir hace falta `--apply <ruta>` o pulsar «Aplicar parche» en la interfaz.
- `--check <ruta>` valida los archivos y reconstruye el resultado en memoria, sin escribir. `--detect-only` enumera las instalaciones encontradas.
- Si la V2 exacta ya está instalada, responde «ya instalado» sin reescribir ni crear un temporal.
- Un mutex por ruta impide que dos ejecuciones de Syncrash apliquen el parche a la vez sobre el mismo juego. Justo antes de sustituir `gbr.exe` se comprueban de nuevo el proceso y los hashes.
- Los errores indican el estado del archivo que se pudo verificar. El temporal propio se elimina al terminar.
- Ensamblado, archivo y manifiesto comparten versión. La compilación usa herramientas fijadas y se compara byte a byte.
- La línea de comandos escribe UTF-8 sin BOM, incluidos los errores. Si el EXE WinForms no tiene consola, usa los flujos de salida directamente; la ruta de apertura de la interfaz no cambia la codificación.
- La interfaz detecta accesos denegados internos, también en errores agrupados de aplicación y limpieza, y añade la ayuda de permisos conservando el estado del archivo que informa el motor.
- El preparador obtiene la versión de `AssemblyInfo.cs` y la utiliza también en las instrucciones y el nombre del ZIP. CI ejecuta `push` solo en `main` y mantiene `pull_request`.

El 24/09/2026 se comprobaron las últimas releases oficiales: [checkout v7.0.1](https://github.com/actions/checkout/releases/tag/v7.0.1) y [setup-dotnet v6.0.0](https://github.com/actions/setup-dotnet/releases/tag/v6.0.0). El workflow usa sus mayores `@v7` y `@v6`; ambos declaran Node.js 24 en sus manifiestos. La ejecución remota de este workflow queda pendiente del push autorizado.

Códigos de salida de la línea de comandos:

| Código | Significado |
| --- | --- |
| `0` | Comprobación o aplicación correcta |
| `1` | Error de validación, lectura o escritura |
| `2` | Uso inválido |
| `3` | Ya instalado; no se escribe nada |
| `4` | Otra ejecución de Syncrash está usando el mismo destino |

El mutex no protege frente a Steam, otros programas, enlaces a la misma ruta ni otras sesiones de Windows. Queda una pequeña ventana frente a cambios externos. No se crea copia de seguridad; Steam permite restaurar los archivos del juego.

## Verificación final del 24/09/2026

Con **Windows PowerShell 5.1.26100.9444** se ejecutaron `tests/run.ps1` y `scripts/compare-builds.ps1` sobre los cambios locales posteriores a `2dd79aa`, todavía sin commit. Pasaron las **14 pruebas sintéticas y de línea de comandos**: reconstrucción, entradas y receta inválidas, salida incorrecta, juego abierto o consulta de procesos fallida, comprobación de solo lectura, repetición sin escritura, fallos antes y después de sustituir, reemplazo denegado, concurrencia, mensajes de permisos con causas internas y salida redirigida sin consola. Dos compilaciones del mismo código fueron idénticas byte a byte, con el SHA256 de la tabla, también idéntico al EXE usado en las pruebas. Son resultados de una sola máquina, no una reproducción independiente.

Después se copiaron **archivos reales ya admitidos** a una carpeta local, fuera de Git. Los hashes iniciales eran `72b09d1abd4f311efe4213a9a1110185519bde4db4ee57769d346b475c748473` para `gbr.exe` y `6926c286b8e44dba9244723fbcd153a3ee8fc28633e3c22cc49c27d206c96d50` para `Packs/data.pak`. Con el EXE de la tabla:

| Operación sobre la copia | Salida | Resultado observado |
| --- | --- | --- |
| `--check` original | 0 | Hash y fecha de `gbr.exe` intactos; ningún temporal |
| `--apply` con la copia marcada como solo lectura | 1 | Reemplazo denegado; original intacto y sin temporal; se conserva el estado en el error |
| `--apply` original | 0 | `gbr.exe` pasó exactamente al hash V2 esperado |
| `--check` ya instalado | 3 | Sin cambios de hash, fecha ni temporales |
| `--apply` de nuevo | 3 | Sin cambios de hash, fecha ni temporales |

El hash de `data.pak` no cambió en ningún paso, ni los archivos fuente usados para crear la copia. Se capturaron los bytes de stdout y stderr y se decodificaron como UTF-8 estricto; los acentos de «Instalación» y «ya está instalado» se conservaron. La receta y los hashes admitidos siguen sin cambios. Las salidas, los binarios comparados y `real-copy-verification.json` se conservan en `work/audit-final-20260924/`, fuera de Git. Esta prueba cubre el aplicador sobre archivos reales en una copia. **No cubre arrancar el juego con el candidato, jugar una partida ni usar la interfaz en otro equipo.**

### Evidencia histórica anterior de la misma versión

El EXE de `2dd79aa`, SHA256 `1407eddc96743a90a669257acb21146bc2ee99f28ba155d08d853f2a7910d812`, tenía 2.403.328 bytes. Sus 12 pruebas y la aplicación sobre una copia real del 24/09/2026 pertenecen exclusivamente a ese hash; se conservan en `work/codex-verification-20260924/`. La verificación final anterior es una ejecución nueva con el hash de la tabla, no una reasignación de aquella evidencia.

Durante esta auditoría, una compilación intermedia (`c0cc4075ecb453f34757278da0d968660c14ef2cad312c0db6a951feee094e44`) falló al fijar `Console.OutputEncoding` sin consola y mostró un aviso de excepción. Se corrigió con la alternativa de flujos UTF-8 y se repitieron las pruebas completas con éxito. El registro fallido también se conserva en el archivo local; ese EXE intermedio no es el candidato final.

## Compilar y preparar una copia

En Windows, con .NET SDK `8.0.400` y referencias de .NET Framework 4.8. Los scripts funcionan con Windows PowerShell 5.1:

```powershell
& ./src/Syncrash/build.ps1 -OutputPath ./work/mi-candidato/Syncrash.exe
& ./tests/run.ps1 -OutputDirectory ./work/mi-prueba
& ./scripts/compare-builds.ps1 -OutputDirectory ./work/mi-comparacion
```

La compilación no sobrescribe un EXE existente. El preparador exige un commit sin cambios pendientes, valida la versión contra `AssemblyInfo.cs`, recompila el código y comprueba que el EXE indicado coincide con esa compilación. Después crea un ZIP local con el aplicador, la licencia, instrucciones, hashes y un manifiesto:

```powershell
& ./scripts/prepare-release.ps1 -ExePath './work/mi-candidato/Syncrash.exe' -OutputDirectory './work/mi-paquete'
```

El ZIP local no es una release ni sustituye a v1.0.0. Registra su hash al crearlo: el hash de un ZIP anterior no identifica uno nuevo.

Se verificaron la sintaxis y el BOM de los scripts en PowerShell 5.1, la lectura de la versión actual, el rechazo cuando `AssemblyInfo.cs` declara otra versión o carece de ella, y el rechazo del árbol con cambios sin crear paquete. El recorrido completo del preparador con este candidato queda pendiente del commit revisado y del árbol limpio. No se ha creado un paquete final ni se ha hecho commit o push durante esta auditoría.

## Pendiente antes de distribuir el binario

Hay que probar el EXE exacto y el juego reconstruido en una instalación legítima aislada, con las protecciones activas:

1. Abrir la interfaz y aplicar el parche como usuario normal.
2. Arrancar el juego desde Steam, jugar una partida, guardar, cargar y salir.
3. Repetir la aplicación para confirmar que no reescribe, y comprobar que Steam restaura el original.
4. Registrar por separado cualquier alerta sobre el aplicador y sobre el juego, con versiones y hashes.

Las pruebas de este documento no demuestran la compatibilidad en partida ni el resultado en otros equipos.

Publicar el código y publicar un nuevo binario son decisiones distintas. Antes de cada una se revisan el diff y los archivos exactos. Los juegos completos, los registros y los datos de jugadores siguen siendo privados.
