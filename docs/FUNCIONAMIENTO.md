# Cómo funciona el parche

## La protección de cierres

La guarda actual procede de la investigación local de un fallo de tipo de objeto: una ruta obtenía un puntero a partir de un identificador y lo utilizaba como objeto de mapa sin comprobar que lo fuera. En los casos reconstruidos, esto podía acabar en una llamada incompatible y un desequilibrio de pila.

Syncrash redirige tres puntos de llamada a un helper de **42 bytes** que utiliza la comprobación nativa del juego para `CVXMapObj`. Para un objeto compatible, conserva la llamada al predicado con el puntero ajustado. Para un tipo incompatible, devuelve el resultado de rechazo para que continúe la rama existente del llamador.

| Referencia en la imagen admitida | Función |
| --- | --- |
| `0x57177C` y `0x5717F6` | Dos puntos protegidos en la primera guarda de investigación. |
| `0x55F500` | Punto añadido en `ObjList::ClearDead`. |
| `0xA95000` | Helper compartido por las tres llamadas. |

Son direcciones de la imagen admitida, no una búsqueda universal para cualquier edición. Las diferencias exactas están en [`recipe.json`](../src/Syncrash/recipe.json). Los cambios incluyen cabeceras PE, las tres llamadas y una sección añadida; el aplicador verifica todos los bytes de entrada y el hash del resultado.

## Límites conocidos

El cast no detecta todos los identificadores obsoletos reutilizados para otro objeto de mapa válido, ni repara cualquier puntero corrupto, guardado antiguo o ruta de cierre ajena a estas llamadas. No demuestra por sí solo que dos simulaciones multijugador permanezcan sincronizadas.

Las reconstrucciones y ensayos locales sirven de evidencia para este mecanismo. Los registros originales y volcados se mantienen privados; no están incluidos en el repositorio. La validación con más jugadores continúa.

## Aplicación al archivo

El aplicador verifica la base Steam y los recursos, reconstruye el resultado en memoria y prepara un temporal junto a `gbr.exe`. Comprueba de nuevo los archivos y que el juego esté cerrado antes de sustituirlo. No crea copia de seguridad. En el [aplicador 1.0.3.0](CANDIDATO_1.0.3.md), si el resultado exacto ya está instalado, lo comunica sin crear temporal ni reescribir. La entrega pública v1.0.0 sí reescribía el resultado al repetir la aplicación.

Se distribuyen cambios parciales, no una copia completa de Imperivm. Los bytes de referencia del juego se incluyen para identificar y aplicar el cambio; la licencia MIT del trabajo propio no altera los derechos de sus titulares.

## Desincronizaciones y otros mods

Esta v1 no incorpora una corrección causal validada de desync Steam. Las correcciones de cierres y desincronizaciones que identifiquemos y validemos en otros mods podrán incorporarse a futuras versiones de Syncrash para ampliar su alcance más allá de Steam vanilla, con perfiles de compatibilidad comprobados.
