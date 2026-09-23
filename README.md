# Syncrash

**Syncrash** es un único proyecto para mejorar la estabilidad de *Imperivm RTC: HD Edition — Great Battles of Rome*. Empezamos por **Steam vanilla**. La protección frente a cierres y las futuras correcciones de desincronización se publicarán como versiones del mismo producto.

Syncrash no está afiliado al desarrollador ni al editor del juego.

## Candidato Steam v1

La entrega actual es **un solo `Syncrash.exe`**, con una pantalla compacta, ilustración e icono propios. No necesitas instalar un observador ni escribir comandos. La [ficha de entrega](docs/ENTREGA_ACTUAL.md) recoge su versión, tamaño y SHA256.

1. Cierra Imperivm y abre Syncrash. La pantalla explica el alcance, muestra el repositorio y localiza el juego; abrirla no modifica archivos.
2. Deja seleccionada **Steam vanilla**. Si la ruta no aparece o quieres otra instalación, pulsa **Elegir** y selecciona su `gbr.exe`.
3. Pulsa **Aplicar parche**. Al terminar puedes abrir el juego desde Steam. Si ya tenías nuestra V2, la vuelve a aplicar y muestra el mismo resultado.

Syncrash comprueba los hashes de `gbr.exe` y `Packs/data.pak`. Solo acepta el Steam original admitido o la V2 exacta. **El único archivo del juego que sustituye es `gbr.exe`.** La interfaz tiene versión 1.0.1; el parche aplicado sigue siendo la misma guarda V2 que se estaba probando.

**No guarda copia ni ofrece restauración.** Para recuperar el juego vanilla hay que volver a descargarlo o verificar sus archivos desde Steam. El ejecutable distribuido no contiene archivos completos del juego, PAK, logs ni volcados.

| Función | Estado |
| --- | --- |
| Protección de cierres | La guarda V2 cubre tres puntos de llamada identificados. Instalación y reinstalación se prueban en copias. Una sesión local de unos 79 minutos con V2 terminó normalmente y sin excepciones capturadas. Los Logs recibidos de ambos jugadores no muestran un desync explícito; esto no garantiza que se hayan eliminado todos los fallos. |
| Corrección de desync Steam | **Aún no existe una corrección causal validada.** Esta v1 no debe presentarse como solución de desincronizaciones. Las herramientas de diagnóstico siguen siendo de investigación local y no forman parte del ejecutable compartido. |

**Community Mod aparece desactivado** en la pantalla. Su futura compatibilidad será un parche sobre una instalación del mod obtenida por separado de su creador. Syncrash no descargará ni redistribuirá Community Mod. La hipótesis `tgtbool = false` pertenece a Community v11.3 y no se aplica a Steam vanilla.

Steam vanilla se trata como base fijada por hash. No se espera que el juego reciba cambios, pero Syncrash seguirá rechazando archivos desconocidos. **El repositorio sigue privado por ahora**; el enlace requerirá acceso autorizado hasta que su propietario decida hacerlo público. No hay aún una versión pública validada.

## Código y comprobaciones

El código del aplicador, la receta de diferencias y la ilustración están en [`src/Syncrash/`](src/Syncrash/). Se compila en Windows con .NET Framework, sin descargar dependencias:

```powershell
& ./src/Syncrash/build.ps1
```

El resultado es el mismo archivo único para compartir. El instalador no envía datos ni instala servicios; el enlace a GitHub solo abre el navegador al pulsarlo. Una interfaz y un enlace no garantizan por sí solos la seguridad de un ejecutable: consulta [qué modifica y cómo comprobarlo](docs/TRANSPARENCIA.md).

Para las pruebas con más jugadores basta seguir la [guía sin observador](docs/PRUEBAS_SIN_OBSERVADOR.md). Consulta también la [hoja de ruta](docs/ROADMAP.md) y el [plan técnico](docs/PLAN_DE_EJECUCION.md). Los Logs, dumps y archivos completos del juego se conservan fuera de Git.
