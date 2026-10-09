# Hoja de ruta de Syncrash

## Ediciones compatibles

Empezamos por **Steam sin mods**, con los archivos identificados por sus hashes. Las próximas correcciones se añadirán a Syncrash. Después queremos dar soporte a Community Mod y, más adelante, a otros mods. Cada edición necesitará sus propios hashes y pruebas, también cuando se actualice.

## Versión publicada

La [v1.0.8 publicada](ENTREGA_ACTUAL.md) reúne protección de cierres, LAA, pantalla adaptable con suavizado y voces opcionales en tres idiomas. **Sigue siendo experimental.** Las dos opciones están marcadas por defecto y son desactivables. El juego se abre normalmente, también desde `gbr.exe`.

La [ficha técnica](MALDICION_EXPERIMENTAL.md) identifica archivos, pruebas y límites. Las partidas de 1.0.4 conservan su propio alcance. No se exigen análisis antivirus periódicos.

## Siguientes pasos de Steam

La [protección de maldición del 09/10](MALDICION_EXPERIMENTAL.md) está integrada por defecto en el parche publicado `1.0.8`, por decisión del usuario. Se conserva su retirada individual. Queda comprobar continuidad en partida y multijugador. Community v12 exacto solo admite las órdenes individuales de esta protección, sin habilitar las opciones normales del mod.

El [ajuste de casillas e interfaz](FUNCIONAMIENTO.md#casillas-unificadas) está disponible desde v1.0.6. Pantalla y voces se retiran al desmarcar y aplicar.

Voces utiliza 188 rutas españolas, 188 italianas y 393 inglesas sin modificar PAK; reaplicar adapta el idioma y desmarcar/aplicar retira los WAV registrados. Falta ampliar las pruebas de escucha en los tres idiomas y comprobar el multijugador con voces. La resolución interna superior a 1080p permanece fuera de la entrega tras un ensayo fallido.

1. Ampliar la prueba a 2–3 parejas con el mismo EXE. No se exige instalar el observador: basta aplicar Syncrash, jugar y conservar los Logs de ambos ante un incidente. Registrar mapa, configuración, anfitrión y si la partida es nueva; incluir guardado y recarga.
2. Si aparece un desync, localizar la primera diferencia observada entre ambos estados y comprobar su causa en los recursos efectivos de Steam. Las herramientas locales de manifiestos sirven para comparar archivos, pero no prueban igualdad de la simulación.
3. Incorporar una corrección de desync a una **nueva versión del mismo Syncrash Steam** cuando exista una ruta causal comprobada y una prueba con ambos clientes idénticos. Comprobar que funciona junto a la protección de cierres.
4. Mantener el ejecutable de uso directo y los controles de hash en cada versión. Cada corrección se describirá con el alcance demostrado; una causa resuelta no implica que todas las demás lo estén.

## Conexión online automática · propuesta

Preparar una tercera casilla independiente, desmarcada por defecto durante el piloto, para facilitar la entrada a las salas habituales. Primero se observará el fallo real y se probarán mapeos automáticos de duración limitada; después se evaluará la negociación NAT existente si permite coordinar los sockets sin añadir servidores. La [propuesta online](CONEXION_ONLINE.md) explica el diseño y las pruebas necesarias. **Todavía no está implementada ni incluida en v1.0.8.**

## Recuperación de desincronizaciones · evaluación inicial

La [evaluación del 05/10/2026](RECUPERACION_DESYNC.md) encuentra base para investigar una recarga común desde el anfitrión, pero no demuestra recuperación multijugador. Primero hay que comprobar que dos equipos restauran una partida sana y continúan sincronizados; la recuperación dentro de la misma sesión queda condicionada a ese resultado y a conservar la sesión de red. No está implementada ni incluida en v1.0.8.

## Community y otros mods

Cuando Steam esté estabilizado, ampliar la investigación a Community y otros mods. Las correcciones de cierres y desincronizaciones que identifiquemos y validemos podrán incorporarse a futuras versiones de Syncrash para aumentar su alcance más allá de vanilla. Syncrash se aplicará sobre el mod previamente instalado por el jugador; no lo descargará ni redistribuirá. Para admitir Community habrá que identificar sus archivos y probar la misma configuración en ambos PC. El mismo criterio servirá para otros mods.

La propuesta de conectividad es una línea independiente de las correcciones de estabilidad: entrar en una partida no demuestra que ambas simulaciones sigan sincronizadas. Los relays y nuevos servicios externos quedan fuera del alcance del piloto.
