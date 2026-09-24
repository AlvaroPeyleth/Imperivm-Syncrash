# Qué hace Syncrash Steam v1

Syncrash es un proyecto experimental de estabilidad para Imperivm. Esta entrega aplica la guarda V2 de cierres; las correcciones de desync Steam siguen en investigación. El código y las versiones se publican en [GitHub](https://github.com/AlvaroPeyleth/Imperivm-Syncrash).

## Acceso a archivos

Al abrir, muestra información y busca instalaciones de Steam mediante sus rutas del registro, bibliotecas y manifiestos. El jugador puede seleccionar otra ruta. **No modifica el juego hasta pulsar Aplicar parche.**

Al aplicar:

1. Lee y comprueba `gbr.exe` y `Packs/data.pak`, y exige que el juego esté cerrado.
2. Reconstruye en memoria el ejecutable parcheado usando las diferencias incrustadas en el EXE. El candidato 1.0.3.0 reconoce V2 exacta y termina sin reescritura; la entrega v1.0.0 la reescribía.
3. Escribe un temporal en la carpeta del juego, verifica el resultado y sustituye únicamente `gbr.exe`. El temporal se elimina. No se guarda copia de seguridad.
4. Comprueba el hash final y muestra el resultado.

No modifica PAK, mapas, guardados, configuración ni archivos de audio. No instala un observador, servicios, tareas programadas, actualizadores ni controladores. No envía Logs ni otros datos por red. Los enlaces de código y descargas abren GitHub en el navegador cuando el usuario los pulsa. La ayuda integrada se puede leer sin conexión.

## Archivos admitidos

| Archivo | SHA256 |
| --- | --- |
| Steam original `gbr.exe` | `72b09d1abd4f311efe4213a9a1110185519bde4db4ee57769d346b475c748473` |
| Resultado V2 `gbr.exe` | `752c95a475e62b0d61d88ec9fb7fabc07758cb217ab152d78651385a5de3d2cc` |
| `Packs/data.pak` vanilla | `6926c286b8e44dba9244723fbcd153a3ee8fc28633e3c22cc49c27d206c96d50` |

El original ocupa 4.456.448 bytes y el resultado 4.460.544 bytes. La receta contiene cambios en cabeceras del ejecutable, tres llamadas y la sección añadida con la guarda. Solo se distribuyen el aplicador y esas diferencias; el jugador aporta su instalación de Steam.

## Confianza y límites

Consulta [Seguridad y verificaciones](SEGURIDAD.md) para conocer las medidas del aplicador, la comparación entre código y EXE y la explicación de las alertas antivirus conocidas.

La pantalla de bienvenida no demuestra que un binario sea seguro. El código revisable, las instrucciones de compilación y el hash del archivo distribuido permiten comprobar su procedencia. La recompilación histórica de la [entrega v1.0.0](ENTREGA_ACTUAL.md) podía diferir en metadatos; dos builds del [aplicador 1.0.3.0](CANDIDATO_1.0.3.md) fueron idénticos en esta máquina. El hash de `gbr.exe` resultante debe ser exactamente el indicado.

Se han probado instalación, reinstalación y rechazo de archivos no admitidos en copias aisladas. La sesión de unos 79 minutos con V2 terminó normalmente, sin excepciones capturadas. Falta más prueba con distintos jugadores. No se promete corregir todos los cierres o desyncs ni se atribuye la anomalía de niebla observada a una causa aún no demostrada.

Para quitar Syncrash, reinstala Imperivm o verifica sus archivos desde Steam. Si Steam restaura `gbr.exe`, será necesario volver a aplicar Syncrash para usarlo.

## Community Mod

Su opción está desactivada. La futura variante de Syncrash se aplicará **después de instalar Community Mod por sus propios canales**. No se incluye, descarga ni redistribuye el mod de su creador. Se admitirán únicamente versiones del mod identificadas y probadas.

## Licencia, privacidad y análisis

El código propio se ofrece bajo [MIT](../LICENSE), con autoría de AlvaroPeyleth (Discord: xtalvarotx). Consulta los [créditos](CREDITOS.md) y el [funcionamiento técnico](FUNCIONAMIENTO.md). No se conceden derechos sobre el código original de Imperivm.

La [ficha de entrega](ENTREGA_ACTUAL.md) identifica el binario y el estado de VirusTotal. Un hash permite identificar el archivo; un análisis antivirus no certifica que sea infalible o inocuo. No aconsejamos desactivar el antivirus para ejecutarlo.

Los logs y dumps de jugadores permanecen fuera del repositorio, incluido su historial revisado. Las incidencias públicas deben describir el problema sin adjuntar datos personales. El historial conserva la identidad y el correo profesional del autor por decisión expresa suya.
