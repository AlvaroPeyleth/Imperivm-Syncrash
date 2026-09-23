# Hoja de ruta de Syncrash

## Producto

Syncrash tendrá **una entrega por edición compatible**. Steam vanilla es la primera base, fijada por hashes. Las siguientes correcciones de cierres o desincronizaciones se incorporarán a nuevas versiones de Syncrash Steam, sin repartir parches independientes. Community Mod tendrá después una variante del mismo producto; sus actualizaciones exigirán identificar y probar de nuevo sus recursos. Otros mods podrán añadirse con perfiles propios.

## Steam v1: candidato privado

La entrega actual es un único ejecutable con bienvenida, enlace al repositorio privado, Steam vanilla seleccionado y Community desactivado. Busca Imperivm Steam y espera al botón **Aplicar parche** para aplicar la guarda V2 para tres rutas de cierre, tras comprobar los hashes admitidos. Si no encuentra el juego, permite seleccionar `gbr.exe`. Reaplica la misma V2 si ya estaba instalada. No crea respaldo ni tiene botón de restauración: para recuperar vanilla se descarga el juego de nuevo o se verifican sus archivos desde Steam.

La reconstrucción y aplicación pasaron pruebas en copias. El observador registró una sesión real de unos 79 minutos con V2, 2.328 muestras sin errores y salida con código 0. Se recibieron Logs de ambos jugadores, sin desync explícito y con 329 registros de generación del mapa idénticos. Falta verificar el hash del segundo PC y ampliar las pruebas a más parejas. **Esta v1 no corrige desyncs Steam vanilla.** Una partida sin desync solo acredita compatibilidad observada en ese escenario.

## Siguientes versiones de Steam

1. Ampliar la prueba a 2–3 parejas con el mismo EXE. No se exige instalar el observador: basta aplicar Syncrash, jugar y conservar los Logs de ambos ante un incidente. Registrar mapa, configuración, anfitrión y si la partida es nueva; incluir guardado y recarga.
2. Si aparece un desync, localizar la primera diferencia observada entre ambos estados y comprobar su causa en los recursos efectivos de Steam. Las herramientas locales de manifiestos sirven para comparar archivos, pero no prueban igualdad de la simulación.
3. Incorporar una corrección de desync a una **nueva versión del mismo Syncrash Steam** cuando exista una ruta causal comprobada y una prueba con ambos clientes idénticos. Verificar su convivencia con la guarda de cierres.
4. Mantener el ejecutable de uso directo y los controles de hash en cada versión. Cada corrección se describirá con el alcance demostrado; una causa resuelta no implica que todas las demás lo estén.

## Community y otros mods

Cuando Steam esté estabilizado, identificar el paquete efectivo de Community Mod y probar dentro del motor el candidato `tgtbool = false`. Esta hipótesis es exclusiva de Community v11.3 y no se traslada a Steam. Syncrash se aplicará sobre el mod previamente instalado por el jugador; no lo descargará ni redistribuirá. Una variante de Syncrash Community necesitará sus propios hashes y ensayos iguales en ambos PC. El mismo criterio servirá para otros mods.

Conectividad, puertos y relay quedan fuera de esta línea de estabilidad: poder entrar en una partida no demuestra que ambas simulaciones sigan sincronizadas.
