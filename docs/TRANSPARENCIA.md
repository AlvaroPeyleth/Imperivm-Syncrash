# Qué hace Syncrash Steam v1.0.8

Syncrash aplica la protección de cierres y LAA al ejecutable de Imperivm Steam vanilla. Opcionalmente añade pantalla adaptable con suavizado GPU, manteniendo la resolución del escritorio. Puedes revisar el [código y las fuentes](SEGURIDAD.md).

**Esta versión es experimental.** La reparación opcional de voces añade WAV sueltos extraídos de los PAK locales y un registro para cambiar de idioma al reaplicar o retirarlos. La [ficha de la versión actual](ENTREGA_ACTUAL.md) detalla archivos, retirada, pruebas y límites.

## Acceso a archivos

La interfaz busca la instalación Steam y permite elegir `gbr.exe`. No modifica el juego al abrirse. Al aplicar, comprueba EXE y PAK, exige juego cerrado, reconstruye y verifica el resultado antes de sustituir `gbr.exe`. Si el resultado exacto ya está instalado, no lo reescribe.

Con pantalla adaptable marcada, instala `winmm.dll`, `dxwnd.dll`, `dxwnd.dxw`, la licencia y el registro de propiedad junto al juego. Conserva las proporciones, añade márgenes negros y suaviza la ampliación mediante GPU. El juego se abre normalmente, también desde `gbr.exe`. Con voces marcadas, se añaden WAV bajo `CurrentLang/voices/` y el registro `.syncrash-voices/manifest.json`; se conservan PAK, mapas y guardados; no se instala un servicio, observador o actualizador ni se envían Logs automáticamente.

## Archivos admitidos

Los hashes corresponden a v1.0.8. La [protección de maldición](MALDICION_EXPERIMENTAL.md) se incluye por defecto y tiene retirada individual. Community v12 exacto admite únicamente esa vía por CLI, sin soporte general de pantalla/voces.

| Archivo | SHA256 |
| --- | --- |
| Steam original `gbr.exe` | `72b09d1abd4f311efe4213a9a1110185519bde4db4ee57769d346b475c748473` |
| Ejecutable parcheado anterior admitido para actualizar | `752c95a475e62b0d61d88ec9fb7fabc07758cb217ab152d78651385a5de3d2cc` |
| Ejecutable anterior con LAA admitido para actualizar | `af59a5bbb4956f50dd671a4a52753376b7b359b2028c0331a2db5fde6f64c965` |
| Resultado con cuatro protecciones y LAA | `cd35437004a441b7e70a8f0fd606003dea6c1e084667a7d813736c5d82a456cc` |
| `Packs/data.pak` vanilla | `6926c286b8e44dba9244723fbcd153a3ee8fc28633e3c22cc49c27d206c96d50` |

El original ocupa 4.456.448 bytes y el resultado 4.460.544. La [ficha técnica](CANDIDATO_1.0.4.md) identifica también cada componente de pantalla. Solo se distribuyen el aplicador, las diferencias y componentes propios/de terceros; necesitas tu instalación del juego.

## Retirada y límites

Para retirar pantalla o voces, desmarca su casilla y aplica. Para cambiar una variante de pantalla, retírala así y después marca/aplica la nueva. Se conserva LAA. Para adaptar las voces a otro idioma, cierra el juego y reaplica Syncrash. Steam no elimina los WAV añadidos; para recuperar también `gbr.exe`, verifica los archivos del juego desde Steam tras retirar las opciones. No se crea una copia de seguridad del ejecutable.

Consulta las pruebas en la [ficha de entrega](ENTREGA_ACTUAL.md): la sesión multijugador comunicada corresponde a 1.0.4; la comprobación con voces y la nueva protección de maldición sigue pendiente. Todavía no corrige las desincronizaciones ni añade resolución interna superior a 1080p o texturas nuevas. La interfaz de Community permanece desactivada; la compatibilidad limitada por CLI se detalla en la ficha de maldición.

## Licencias y privacidad

Código propio bajo [MIT](../LICENSE); pantalla con sus [licencias y procedencia](../src/Screen/README.md), exportables junto a las fuentes completas desde el EXE. Los derechos de Imperivm pertenecen a sus titulares.

Logs, dumps y datos de jugadores permanecen fuera del repositorio. Las incidencias públicas deben describir el problema sin adjuntar datos personales.
