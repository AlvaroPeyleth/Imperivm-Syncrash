# Cómo funciona el parche

## La protección de cierres

La protección nació al investigar un fallo de tipo de objeto: una ruta obtenía un puntero a partir de un identificador y lo utilizaba como objeto de mapa sin comprobar que lo fuera. En los casos reconstruidos, esto podía acabar en una llamada incompatible y un desequilibrio de pila.

Syncrash redirige tres puntos de llamada a un helper de **42 bytes** que utiliza la comprobación nativa del juego para `CVXMapObj`. Para un objeto compatible, conserva la llamada al predicado con el puntero ajustado. Para un tipo incompatible, devuelve el resultado de rechazo para que continúe la rama existente del llamador.

| Referencia en la imagen admitida | Función |
| --- | --- |
| `0x57177C` y `0x5717F6` | Dos puntos protegidos en la primera guarda de investigación. |
| `0x55F500` | Punto añadido en `ObjList::ClearDead`. |
| `0xA95000` | Helper compartido por las tres llamadas. |

Son direcciones de la imagen admitida, no una búsqueda universal para cualquier edición. Las diferencias exactas están en [`recipe.json`](../src/Syncrash/recipe.json). La receta de 1.0.8 incluye cabeceras PE, las tres llamadas, la cuarta protección de maldición y una sección añadida; el aplicador verifica todos los bytes de entrada y el hash del resultado.

## Límites conocidos

El [parche 1.0.8 del 09/10](MALDICION_EXPERIMENTAL.md) añade por defecto una cuarta protección: comprueba el puntero nulo al retirar maldición/bendición antes de leerlo. Usa otros 28 bytes en la misma sección y conserva íntegro el helper anterior. La entrega histórica 1.0.7 conserva las tres protecciones anteriores.

El cast no detecta todos los identificadores obsoletos reutilizados para otro objeto de mapa válido, ni repara cualquier puntero corrupto, guardado antiguo o ruta de cierre ajena a estas llamadas. No demuestra por sí solo que dos simulaciones multijugador permanezcan sincronizadas.

Las reconstrucciones y ensayos locales sirven de evidencia para este mecanismo. Los registros originales y volcados se mantienen privados; no están incluidos en el repositorio. La validación con más jugadores continúa.

## Aplicación al archivo

El aplicador verifica la base Steam y los recursos, reconstruye el resultado en memoria y prepara un temporal junto a `gbr.exe`. Comprueba de nuevo los archivos y que el juego esté cerrado antes de sustituirlo. No crea copia de seguridad. En el [aplicador 1.0.3.0](CANDIDATO_1.0.3.md), si el resultado exacto ya está instalado, lo comunica sin crear temporal ni reescribir. La entrega pública v1.0.0 sí reescribía el resultado al repetir la aplicación.

Se distribuyen cambios parciales, no una copia completa de Imperivm. Los bytes de referencia del juego se incluyen para identificar y aplicar el cambio; la licencia MIT del trabajo propio no altera los derechos de sus titulares.

## Memoria y pantalla en 1.0.4

LAA habilita hasta 4 GB de espacio de direcciones de usuario para el juego x86 en Windows de 64 bits; no reserva RAM ni convierte el juego a 64 bits. Se conserva la protección de cierres y se admite actualizar el resultado de las entregas anteriores.

La pantalla adaptable opcional instala una capa de presentación junto al juego. Conserva una superficie original, amplía su imagen con suavizado GPU y añade márgenes negros sin cambiar el modo del escritorio. El juego se abre normalmente; la pantalla adaptable se retira desde Syncrash. Los componentes, fuentes y pruebas están en la [ficha técnica](CANDIDATO_1.0.4.md).
## Voces opcionales en 1.0.5

La nueva opción extrae grabaciones de los PAK identificados y crea las rutas sueltas que solicita el juego, sin escribir en esos paquetes. El aplicador registra sus archivos para cambiar de idioma al reaplicar o retirarlos al desmarcar la casilla. La [ficha de voces](CANDIDATO_1.0.5.md) recoge las pruebas de archivos y lo que falta escuchar en partida. La ficha de entrega identifica el EXE publicado de esta v1 experimental.

## Desincronizaciones y otros mods

Esta v1 todavía no corrige las desincronizaciones de Steam. Las correcciones de cierres y desincronizaciones que identifiquemos y validemos en otros mods podrán incorporarse a futuras versiones de Syncrash para ampliar su alcance más allá de Steam vanilla, con perfiles de compatibilidad comprobados.

## Casillas unificadas

Desde v1.0.6, las dos casillas funcionan igual: **marcar y aplicar instala; desmarcar y aplicar retira** la pantalla adaptable o la reparación de voces. Memoria y protección de cierres se conservan. Abrir Syncrash o cambiar una casilla no modifica archivos. Las dos siguen marcadas por defecto; el jugador elige qué conservar antes de aplicar.

Ya no hay un botón separado para restaurar la pantalla: se retira desde su casilla. Los enlaces de información, proyecto y fuentes están al pie, junto al estado y al botón de aplicación. En ventanas pequeñas puedes desplazarte por el contenido central.

La retirada valida el registro y los hashes de todos los componentes de pantalla antes de aplicar el parche base o eliminar archivos. Conserva archivos ajenos o modificados y permite repetir una retirada interrumpida. Si ocurre un error, una opción puede haber terminado y la otra quedar pendiente. Lee el mensaje y vuelve a aplicar con el juego cerrado. Las órdenes CLI mantienen su alcance explícito: `--apply` y `--apply-with-voices` conservan pantalla; `--remove-screen` la retira. Una compilación de desarrollo sin componentes de pantalla mantiene esa opción deshabilitada y no la retira al aplicar voces.

La [ficha de v1.0.6](CANDIDATO_1.0.6.md) identifica su binario y las pruebas de archivos e interfaz. No equivalen a una partida ni a validar todos los DPI o lectores de pantalla.

Disponible desde [v1.0.6](CANDIDATO_1.0.6.md), publicada el 28/09/2026. La ficha histórica de v1.0.5 conserva su anterior botón de restauración. El trabajo online sigue pendiente.
