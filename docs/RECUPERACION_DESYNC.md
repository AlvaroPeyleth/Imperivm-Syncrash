# Viabilidad de recuperar una desincronización

**05/10/2026 · Evaluación técnica inicial. Sin prototipo ni prueba de recuperación.** La evaluación corresponde a [v1.0.7](CANDIDATO_1.0.7.md); esta capacidad tampoco se incluye en 1.0.8. Es una línea distinta de la [conexión online](CONEXION_ONLINE.md).

## Dictamen

Adoptar el estado del anfitrión es una técnica válida, pero **su viabilidad en Imperivm permanece condicionada a demostrar guardado y restauración multijugador fiables**. Hay evidencia local suficiente para investigar un prototipo acotado. No hay evidencia suficiente para ofrecer recuperación automática, invisible o con solo el anfitrión modificado.

| Variante | Evaluación actual |
| --- | --- |
| Diagnosticar y corregir una causa concreta | Es la vía con más trabajo previo aprovechable; cada corrección necesita prueba causal y multijugador. |
| Reanudar desde un guardado común mediante recarga coordinada | Primer objetivo de recuperación que probar. Depende de que el motor restaure estado, jugadores y evolución de forma compatible. |
| Pausar el desync, adoptar el estado actual del anfitrión y continuar en la misma sesión | Posible diseño, todavía sin demostrar; exige además interceptar el fallo antes de destruir la sesión y coordinar red y órdenes. |
| Recuperar con el parche solo en el anfitrión | Sin evidencia de un protocolo nativo que los clientes sin modificar acepten. Para el piloto se plantea modificar a todos. |
| Omitir el aviso o igualar artificialmente los hashes | No restaura el estado de los jugadores y no sirve como recuperación. |

## Qué respalda la investigación

- Los informes locales del 08/09/2026 contienen dos parejas de estados con ticks y hashes recíprocos. En ellas aparecen maldición frente a avance y diferencias jugables, incluidas salud, posiciones, órdenes e identificadores. Esto localiza una divergencia real; no determina por sí solo su primera causa ni qué jugador tenía el estado correcto.
- Community Mod v11.3 tiene una ruta de `SHAMAN_ADVANCE.VS` que lee `tgtbool` antes de asignarlo. La investigación histórica del constructor nativo y ocho ensayos emulados respaldan que el byte puede conservar su valor anterior. El candidato de inicialización no se ha validado en partida ni se ha demostrado que causara aquellos incidentes. El script Steam contrastado el 23/09 usa otra selección de objetivo y no contiene ese booleano.
- Ya se investigó el contenedor de guardados LZIS y se reparó una copia concreta que el usuario comunicó haber cargado dos veces. Eso no demuestra que pueda restaurarse una sesión multijugador, ni que el guardado cubra todo el estado necesario.
- El 05/10 se inventariaron cadenas del original Steam, SHA256 `72b09d1abd4f311efe4213a9a1110185519bde4db4ee57769d346b475c748473`, 4.456.448 bytes. Aparecen referencias a detección de desync, carga, multijugador y semilla de sincronización. Son pistas estáticas; no identifican por sí solas funciones invocables ni prueban un protocolo de recuperación.
- El código actual de Syncrash aplica una receta de modificaciones exactas y componentes de presentación/voces. No incorpora el coordinador ni el serializador de recuperación propuestos.

Los originales, manifiestos, direcciones y limitaciones de la revisión se conservan en `work/resync-feasibility/`, fuera de Git. Los ensayos históricos mantienen su fecha y sus hashes: no se repitieron en esta evaluación. La confirmación del druida por su autor y la corrección prevista para Community V12 proceden del relato del usuario; aquí no se inspeccionó V12 ni se reprodujo ese caso.

## Qué falta demostrar

1. **Captura y restauración:** localizar las funciones nativas, sus condiciones de llamada y qué conservan: RNG, VM y scripts suspendidos, objetos e identidades, órdenes pendientes, temporizadores y asignación de jugadores. El texto `desync.txt` es diagnóstico; no se ha demostrado que sea un guardado cargable.
2. **Punto de intervención:** seguir la ruta desde la discrepancia hasta la salida, desconexión o división de red. Un proceso que sigue abierto no implica que la sesión permanezca recuperable. Pausar la simulación debe permitir que el canal de recuperación siga funcionando.
3. **Coordinación:** todos deben aceptar una referencia, restaurarla y separar las órdenes antiguas de las nuevas. Una transferencia incompleta no puede reemplazar el último punto utilizable. La coincidencia del hash del archivo recibido solo prueba integridad de transporte.
4. **Verificación:** comparar estado lógico en el mismo tick y fase, comprender la cobertura del hash nativo y comprobar la evolución posterior. Pantallas iguales o un aviso que desaparece no prueban sincronización. Copiar memoria bruta entre procesos no sustituye la serialización.
5. **Integración:** componente x86 que se cargue con el juego tras aplicar una vez; convivencia con el `winmm.dll` de pantalla, instalación y retirada verificables. Se conserva como requisito abrir directamente `gbr.exe` con el aplicador cerrado, todavía sin validar para esta función.

Adoptar al anfitrión establece una referencia acordada, no garantiza que su estado sea correcto o esté libre de corrupción. Restaurar tampoco elimina una causa persistente: un script defectuoso puede volver a separar a los jugadores.

## Experimento que decide si avanzar

| Paso | Prueba | Criterio |
| --- | --- | --- |
| 1 | Dos instalaciones de laboratorio con ejecutable y recursos efectivos iguales; partida sana y guardado común del anfitrión. | Demostrar carga multijugador, mismos puestos y coincidencia del estado cubierto tras cargar. Primero sin provocar fallos. |
| 2 | Repetir secuencias controladas de órdenes, con movimiento, combate, habilidades y scripts activos; conservar referencia sin recarga. | Ambos equipos evolucionan igual tras cargar. Comparar también con la referencia para medir lo perdido o alterado por la recarga. |
| 3 | Introducir una divergencia acotada en laboratorio; conservar diagnóstico, restaurar y repetir las órdenes de prueba. | Recuperación real, sin recaída inmediata ni comandos duplicados. Alternar anfitrión y equipo afectado. |
| 4 | Interceptar el desync dentro de la sesión y ensayar cortes/transferencias fallidas. | Barrera coordinada, tiempos acotados, último guardado preservado y salida controlada si no se puede recuperar. Solo después ampliar a más jugadores. |

Si la recarga común funciona pero la sesión no admite restauración en caliente, la primera función podría ser **reanudar desde un punto de control con recarga visible**. Si ni siquiera es posible restaurar una partida sana de forma consistente, se debe replantear la recuperación general: reconstruir un serializador completo por ingeniería inversa sería un proyecto mucho mayor. El diagnóstico y las correcciones concretas siguen teniendo valor en ambos casos.

## Fuentes y comprobaciones de esta revisión

- Lectura de la propuesta completa, código y archivo de investigación local; inventario estático reproducible sobre el ejecutable identificado. Sin ejecutar el juego, instalar parches, capturar tráfico ni probar dos PC. No se ofrece una estimación fiable de plazo antes del experimento de restauración.
- [Factorio, recuperación de desync](https://factorio.com/blog/post/fff-51): precedente de pausa, reconexión y descarga del estado. Es evidencia de la técnica, no de soporte en Imperivm.
- [Factorio, guardado/carga determinista](https://www.factorio.com/blog/post/fff-270): explica por qué restaurar sin cambiar la evolución requiere serialización cuidadosa.
- [Guía de GGPO](https://github.com/pond3r/ggpo/blob/master/doc/DeveloperGuide.md): el juego debe proporcionar guardado y restauración de su estado. Incorporar una biblioteca no resuelve por sí solo esas funciones.

**Siguiente paso:** seguir las rutas nativas de carga y detección sobre la imagen verificada y preparar el ensayo sano de dos equipos. La implementación de recuperación permanece pendiente.
