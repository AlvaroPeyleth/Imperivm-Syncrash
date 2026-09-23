# Qué hace Syncrash Steam v1

Syncrash es un proyecto experimental de estabilidad para Imperivm. Esta entrega aplica la guarda V2 de cierres; las correcciones de desync Steam siguen en investigación. El repositorio `https://github.com/AlvaroPeyleth/Imperivm-Syncrash` permanece privado hasta que su propietario decida abrirlo. Un visitante sin acceso puede recibir un 404 de GitHub.

## Acceso a archivos

Al abrir, muestra información y busca instalaciones de Steam mediante sus rutas del registro, bibliotecas y manifiestos. El jugador puede seleccionar otra ruta. **No modifica el juego hasta pulsar Aplicar parche.**

Al aplicar:

1. Lee y comprueba `gbr.exe` y `Packs/data.pak`, y exige que el juego esté cerrado.
2. Reconstruye en memoria el ejecutable parcheado usando las diferencias incrustadas en el EXE. Si ya hay V2 exacta, vuelve a escribir esa misma imagen verificada.
3. Escribe un temporal en la carpeta del juego, verifica el resultado y sustituye únicamente `gbr.exe`. El temporal se elimina. No se guarda respaldo.
4. Comprueba el hash final y muestra el resultado.

No modifica PAK, mapas, guardados, configuración ni archivos de audio. No instala un observador, servicios, tareas programadas, actualizadores ni controladores. No envía Logs ni otros datos por red. El único enlace externo abre GitHub en el navegador cuando el usuario lo pulsa.

## Archivos admitidos

| Archivo | SHA256 |
| --- | --- |
| Steam original `gbr.exe` | `72b09d1abd4f311efe4213a9a1110185519bde4db4ee57769d346b475c748473` |
| Resultado V2 `gbr.exe` | `752c95a475e62b0d61d88ec9fb7fabc07758cb217ab152d78651385a5de3d2cc` |
| `Packs/data.pak` vanilla | `6926c286b8e44dba9244723fbcd153a3ee8fc28633e3c22cc49c27d206c96d50` |

El original ocupa 4.456.448 bytes y el resultado 4.460.544 bytes. La receta contiene cambios en cabeceras del ejecutable, tres llamadas y la sección añadida con la guarda. Solo se distribuyen el aplicador y esas diferencias; el jugador aporta su instalación de Steam.

## Confianza y límites

La pantalla de bienvenida no demuestra que un binario sea seguro. El código revisable, las instrucciones de compilación y el hash del archivo distribuido permiten comprobar su procedencia. El candidato actual no lleva firma digital Authenticode. La compilación puede generar un hash distinto por los metadatos del compilador; el hash de `gbr.exe` resultante sí debe ser exactamente el indicado.

Se han probado instalación, reinstalación y rechazo de archivos no admitidos en copias aisladas. La sesión de unos 79 minutos con V2 terminó normalmente, sin excepciones capturadas. Falta más prueba con distintos jugadores. No se promete corregir todos los cierres o desyncs ni se atribuye la anomalía de niebla observada a una causa aún no demostrada.

Para quitar Syncrash, reinstala Imperivm o verifica sus archivos desde Steam. Si Steam restaura `gbr.exe`, será necesario volver a aplicar Syncrash para usarlo.

## Community Mod

Su opción está desactivada. La futura variante de Syncrash se aplicará **después de instalar Community Mod por sus propios canales**. No se incluye, descarga ni redistribuye el mod de su creador. Se admitirán únicamente versiones del mod identificadas y probadas.
