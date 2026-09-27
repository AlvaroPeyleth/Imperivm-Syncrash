# Hoja de ruta de Syncrash

## Producto

Syncrash es **un único producto con perfiles para las ediciones compatibles**. Steam vanilla es la primera base, fijada por hashes. Las siguientes correcciones de cierres o desincronizaciones se incorporarán a nuevas versiones de Syncrash Steam, sin repartir parches independientes. Community Mod tendrá después una variante del mismo producto; sus actualizaciones exigirán identificar y probar de nuevo sus recursos. Otros mods podrán añadirse con perfiles propios.

## Entrega pública

La [versión v1.0.4](ENTREGA_ACTUAL.md) reúne protección V2 de cierres, LAA y pantalla adaptable opcional con suavizado GPU, marcada por defecto. El juego se abre desde Steam y el escritorio conserva su resolución. El EXE incluye componentes y fuentes; la pantalla puede retirarse desde el aplicador.

Hay pruebas automáticas, preparación desde commit limpio, CI correcto y una sesión de más de una hora sin incidencias comunicada por dos jugadores con Steam vanilla y la misma versión. La [ficha técnica](CANDIDATO_1.0.4.md) recoge hashes, resultados y límites. La prioridad es recopilar feedback; no se exigen análisis antivirus periódicos.
## Siguientes versiones de Steam

Con 1.0.4 publicada, se ampliará la compatibilidad a partir de incidencias concretas. La resolución interna superior a 1080p sigue fuera de la entrega tras un ensayo fallido. Audio y Community Mod se estudiarán después; la base actual es Steam vanilla.

1. Ampliar la prueba a 2–3 parejas con el mismo EXE. No se exige instalar el observador: basta aplicar Syncrash, jugar y conservar los Logs de ambos ante un incidente. Registrar mapa, configuración, anfitrión y si la partida es nueva; incluir guardado y recarga.
2. Si aparece un desync, localizar la primera diferencia observada entre ambos estados y comprobar su causa en los recursos efectivos de Steam. Las herramientas locales de manifiestos sirven para comparar archivos, pero no prueban igualdad de la simulación.
3. Incorporar una corrección de desync a una **nueva versión del mismo Syncrash Steam** cuando exista una ruta causal comprobada y una prueba con ambos clientes idénticos. Verificar su convivencia con la guarda de cierres.
4. Mantener el ejecutable de uso directo y los controles de hash en cada versión. Cada corrección se describirá con el alcance demostrado; una causa resuelta no implica que todas las demás lo estén.

## Community y otros mods

Cuando Steam esté estabilizado, ampliar la investigación a Community y otros mods. Las correcciones de cierres y desincronizaciones que identifiquemos y validemos podrán incorporarse a futuras versiones de Syncrash para aumentar su alcance más allá de vanilla. Syncrash se aplicará sobre el mod previamente instalado por el jugador; no lo descargará ni redistribuirá. Una variante de Syncrash Community necesitará sus propios hashes y ensayos iguales en ambos PC. El mismo criterio servirá para otros mods.

Conectividad, puertos y relay quedan fuera de esta línea de estabilidad: poder entrar en una partida no demuestra que ambas simulaciones sigan sincronizadas.
