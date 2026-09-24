# Syncrash: estado y siguiente ensayo

**Actualizado: 24/09/2026.** Hay un solo producto Syncrash. Se empieza por Steam vanilla y se estudiará Community Mod después.

## Evidencia disponible

- Base Steam: `gbr.exe` original SHA256 `72b09d1abd4f311efe4213a9a1110185519bde4db4ee57769d346b475c748473`; con guarda V2 SHA256 `752c95a475e62b0d61d88ec9fb7fabc07758cb217ab152d78651385a5de3d2cc`; `Packs/data.pak` vanilla SHA256 `6926c286b8e44dba9244723fbcd153a3ee8fc28633e3c22cc49c27d206c96d50`.
- La interfaz 1.0.2 espera al botón Aplicar parche. Se probó instalación y reinstalación real en una copia, ambas con hash V2 exacto; también el rechazo de ejecutable desconocido y PAK distinto sin cambios. No dejó copia de seguridad ni temporal. La búsqueda encontró la instalación Steam local. La nueva interfaz no se ha probado todavía en otro PC.
- El [aplicador 1.0.3.0 publicado como v1.0.3](CANDIDATO_1.0.3.md) añade `--check`, idempotencia sin escritura y 14 pruebas sintéticas y de línea de comandos Windows. Tras la auditoría del 24/09 se repitió el ensayo del nuevo EXE exacto en una copia aislada de archivos reales admitidos: produjo el hash V2 y dejó `data.pak` intacto. Sigue pendiente ejecutar el juego y una partida con ese candidato; estos resultados no sustituyen las pruebas históricas de otros hashes.
- El 23/09 una sesión real de unos 79 minutos con V2 confirmó ProcDump, recogió 2.328 muestras sin errores y terminó con código 0. Los Logs recibidos de ambos jugadores no muestran un desync explícito y coinciden en 329 registros de generación del mapa. Queda sin explicar una observación de tropas visibles en niebla y sin verificar el hash remoto. Este resultado no demuestra que se evitase un crash.
- No hay una corrección causal validada para desync Steam. Las correcciones de cierres y desincronizaciones identificadas y validadas en otros mods podrán ampliar el alcance de futuras versiones de Syncrash.

## Distribución y confianza

- Microsoft: reporte antivirus enviado; resolución final pendiente en la última consulta.
- Para el siguiente candidato: build y hash contrastados, prueba del EXE con protecciones activas, comprobación del juego reconstruido y análisis antivirus del mismo hash; estas últimas verificaciones siguen pendientes en la entrega experimental v1.0.3. Ver el [estado principal](SEGURIDAD.md).
- Mantener el [estado público en Seguridad](SEGURIDAD.md) y la evidencia interna en el archivo local. Seguir las [reglas de documentación](../AGENTS.md).

## Próxima prueba con dos equipos

1. **Instalar el mismo Syncrash.** Con Imperivm cerrado, cada jugador abre `Syncrash.exe` y pulsa **Aplicar parche**. Si no localiza Steam, selecciona su `gbr.exe`. Registrar versión y hash del aplicador. La entrega 1.0.2 reescribe al repetir; el aplicador 1.0.3.0 dice «ya instalado» sin escribir. Si se rechaza un archivo, conservar el mensaje y no forzar el parche.
2. **Comprobar recursos.** Verificar que ambos usan Steam vanilla, el mismo `Packs/data.pak`, mapa y configuración. Si hace falta comparar archivos en detalle, usar las herramientas locales de investigación; no vienen en el ejecutable compartido.
3. **Jugar V2/V2.** Iniciar partida nueva, guardar y recargar en una prueba identificada. El observador es opcional; para las nuevas parejas basta el EXE y los Logs normales. Recoger los Logs completos de ambos inmediatamente ante un fallo y antes de volver a abrir el juego. Registrar duración, anfitrión y acciones relevantes. Ver la [guía sin observador](PRUEBAS_SIN_OBSERVADOR.md).
4. **Interpretar.** Si no hay fallo, anotar compatibilidad observada. Si hay desync, preservar los dos estados del mismo incidente y buscar la primera diferencia observada antes de cambiar código. Una partida V2/original se etiqueta como tal y no sustituye V2/V2.

## Trabajo técnico posterior

| Orden | Trabajo | Criterio de cierre |
| --- | --- | --- |
| 1 | Auditar la guarda V2 y su interacción con el estado multijugador. | Contratos de las tres llamadas documentados; ausencia de regresión reproducible en el perfil ensayado. |
| 2 | Localizar una causa de desync Steam vanilla con registros pareados. | Cadena observada desde decisión o comando hasta la primera mutación diferente. |
| 3 | Integrar la corrección en una nueva versión del **mismo Syncrash Steam**. | Ambos clientes idénticos, prueba causal y prueba de convivencia con la guarda V2. |
| 4 | Abrir la variante Community. | Base y paquetes identificados, carga real del script comprobada y ensayos iguales en ambos PC. |

El archivo local `work/` conserva investigación, herramientas y datos de jugadores fuera de Git. El ejecutable distribuido no incluye `gbr.exe`, PAK completos, dumps ni Logs. Syncrash no conserva una copia de los archivos que sustituye; para recuperar vanilla hay que descargar o verificar los archivos del juego desde Steam.

## Publicación de v1

- Revisión de archivos, historial, datos sensibles y procedencia antes de abrir el repositorio.
- Licencia MIT para el trabajo propio, agradecimientos y guía para colaborar.
- README público, ayuda dentro del aplicador y enlaces a código y descargas.
- Compilación final, pruebas en copias y hash del EXE distribuido.
- Publicación en GitHub y análisis de VirusTotal identificado por el mismo hash; el estado exacto consta en la [ficha de entrega](ENTREGA_ACTUAL.md).
- Siguiente prioridad: ampliar pruebas Steam vanilla con parejas que usen la misma versión; Community permanece desactivado.
