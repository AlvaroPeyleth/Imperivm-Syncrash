# Qué hace Syncrash Steam v1.0.5

Syncrash aplica la protección V2 de cierres y LAA al ejecutable de Imperivm Steam vanilla. Opcionalmente añade pantalla adaptable con suavizado GPU, manteniendo la resolución del escritorio. El [código y las fuentes](SEGURIDAD.md) son revisables.

**v1 experimental: esperamos feedback.** La reparación opcional de voces añade WAV sueltos extraídos de los PAK locales y un registro para cambiar de idioma al reaplicar o retirarlos. La [ficha de voces](CANDIDATO_1.0.5.md) detalla archivos, retirada, pruebas y límites.

## Acceso a archivos

La interfaz busca la instalación Steam y permite elegir `gbr.exe`. No modifica el juego al abrirse. Al aplicar, comprueba EXE y PAK, exige juego cerrado, reconstruye y verifica el resultado antes de sustituir `gbr.exe`. Si el resultado exacto ya está instalado, no lo reescribe.

Con pantalla adaptable marcada, instala `winmm.dll`, `dxwnd.dll`, `dxwnd.dxw`, la licencia y el registro de propiedad junto al juego. Conserva las proporciones, añade márgenes negros y suaviza la ampliación mediante GPU. El juego sigue arrancando desde Steam. Con voces marcadas, se añaden WAV bajo `CurrentLang/voices/` y el registro `.syncrash-voices/manifest.json`; se conservan PAK, mapas y guardados; no se instala un servicio, observador o actualizador ni se envían Logs automáticamente.

## Archivos admitidos

| Archivo | SHA256 |
| --- | --- |
| Steam original `gbr.exe` | `72b09d1abd4f311efe4213a9a1110185519bde4db4ee57769d346b475c748473` |
| V2 anterior admitido para actualizar | `752c95a475e62b0d61d88ec9fb7fabc07758cb217ab152d78651385a5de3d2cc` |
| Resultado V2 + LAA `gbr.exe` | `af59a5bbb4956f50dd671a4a52753376b7b359b2028c0331a2db5fde6f64c965` |
| `Packs/data.pak` vanilla | `6926c286b8e44dba9244723fbcd153a3ee8fc28633e3c22cc49c27d206c96d50` |

El original ocupa 4.456.448 bytes y el resultado 4.460.544. La [ficha técnica](CANDIDATO_1.0.4.md) identifica también cada componente de pantalla. Solo se distribuyen el aplicador, las diferencias y componentes propios/de terceros; necesitas tu instalación del juego.

## Retirada y límites

Para retirar las voces, desmarca **Reparar voces de unidades** y aplica; Steam no elimina los WAV añadidos. Para adaptar las voces a otro idioma, cierra el juego y reaplica Syncrash. Desmarcar pantalla conserva la que ya esté instalada. Para retirarla o cambiar de variante, pulsa **Restaurar pantalla original**. Esa acción conserva LAA; para recuperar también `gbr.exe`, verifica después los archivos del juego desde Steam. No se crea una copia de seguridad del ejecutable.

La [ficha de entrega](ENTREGA_ACTUAL.md) separa pruebas de archivos y partidas: la sesión multijugador comunicada corresponde a 1.0.4; la comprobación con voces sigue pendiente. No se incorpora una corrección causal de desync, resolución interna superior a 1080p ni texturas nuevas. Community Mod permanece desactivado y se estudiará después de Steam vanilla.

## Licencias y privacidad

Código propio bajo [MIT](../LICENSE); pantalla con sus [licencias y procedencia](../src/Screen/README.md), exportables junto a las fuentes completas desde el EXE. Los derechos de Imperivm pertenecen a sus titulares.

Logs, dumps y datos de jugadores permanecen fuera del repositorio. Las incidencias públicas deben describir el problema sin adjuntar datos personales.
