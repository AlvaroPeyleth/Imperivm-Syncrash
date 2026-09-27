# Hoja de ruta de Syncrash

## Producto

Syncrash es **un único producto con perfiles para las ediciones compatibles**. Steam vanilla es la primera base, fijada por hashes. Las siguientes correcciones de cierres o desincronizaciones se incorporarán a nuevas versiones de Syncrash Steam, sin repartir parches independientes. Community Mod tendrá después una variante del mismo producto; sus actualizaciones exigirán identificar y probar de nuevo sus recursos. Otros mods podrán añadirse con perfiles propios.

## Entrega y feedback

La [v1.0.6 publicada](ENTREGA_ACTUAL.md) reúne protección de cierres, LAA, pantalla adaptable con suavizado y voces opcionales en tres idiomas. **Syncrash v1 es experimental: esperamos feedback de los usuarios.** Las dos opciones están marcadas por defecto y son desactivables. El juego se abre normalmente, también desde `gbr.exe`.

La [ficha técnica](CANDIDATO_1.0.6.md) identifica archivos, pruebas y límites. Las partidas de 1.0.4 conservan su propio alcance. No se exigen análisis antivirus periódicos.

## Siguientes pasos de Steam

El [ajuste de casillas e interfaz](FUNCIONAMIENTO.md#casillas-unificadas) está publicado en v1.0.6. Pantalla y voces se retiran al desmarcar y aplicar.

Voces utiliza 188 rutas españolas, 188 italianas y 393 inglesas sin modificar PAK; reaplicar adapta el idioma y desmarcar/aplicar retira los WAV registrados. Quedan ampliar la escucha en los tres idiomas y comprobar multijugador con voces. La resolución interna superior a 1080p permanece fuera de la entrega tras un ensayo fallido.

1. Ampliar la prueba a 2–3 parejas con el mismo EXE. No se exige instalar el observador: basta aplicar Syncrash, jugar y conservar los Logs de ambos ante un incidente. Registrar mapa, configuración, anfitrión y si la partida es nueva; incluir guardado y recarga.
2. Si aparece un desync, localizar la primera diferencia observada entre ambos estados y comprobar su causa en los recursos efectivos de Steam. Las herramientas locales de manifiestos sirven para comparar archivos, pero no prueban igualdad de la simulación.
3. Incorporar una corrección de desync a una **nueva versión del mismo Syncrash Steam** cuando exista una ruta causal comprobada y una prueba con ambos clientes idénticos. Verificar su convivencia con la guarda de cierres.
4. Mantener el ejecutable de uso directo y los controles de hash en cada versión. Cada corrección se describirá con el alcance demostrado; una causa resuelta no implica que todas las demás lo estén.

## Conexión online automática · propuesta

Preparar una tercera casilla independiente, desmarcada por defecto durante el piloto, para facilitar la entrada a las salas habituales. Primero se observará el fallo real y se probarán mapeos automáticos de duración limitada; después se evaluará la negociación NAT existente si permite coordinar los sockets sin añadir servidores. El [diseño y plan de validación](CONEXION_ONLINE.md) concentra decisiones, compatibilidad, riesgos y criterios de avance. **Todavía no está implementada ni incluida en v1.0.6.**

## Community y otros mods

Cuando Steam esté estabilizado, ampliar la investigación a Community y otros mods. Las correcciones de cierres y desincronizaciones que identifiquemos y validemos podrán incorporarse a futuras versiones de Syncrash para aumentar su alcance más allá de vanilla. Syncrash se aplicará sobre el mod previamente instalado por el jugador; no lo descargará ni redistribuirá. Una variante de Syncrash Community necesitará sus propios hashes y ensayos iguales en ambos PC. El mismo criterio servirá para otros mods.

La propuesta de conectividad es una línea independiente de las correcciones de estabilidad: entrar en una partida no demuestra que ambas simulaciones sigan sincronizadas. Los relays y nuevos servicios externos quedan fuera del alcance del piloto.
