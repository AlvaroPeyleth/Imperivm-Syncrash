# Colaborar con Syncrash

Gracias por ayudar a investigar y mejorar Imperivm.

## Comunicar una incidencia

Indica versión de Syncrash, Steam vanilla o mod, mapa, anfitrión, duración y últimas acciones antes del fallo. Aclara si todos llevaban el mismo parche y si era una partida nueva o cargada.

Las incidencias de GitHub son públicas. **No adjuntes logs completos, dumps, partidas con datos personales, IP ni rutas de usuario.** Para enviar los registros completos, utiliza el **[formulario de recopilación de logs](https://forms.gle/QAGziVvHk6peHPor9)** o contacta por privado con **xtalvarotx en Discord**. El formulario es una alternativa si no tienes Discord. Puedes publicar una descripción y una captura previamente revisada.

## Proponer cambios

Explica qué falla, cómo lo comprobaste y qué resuelve tu cambio. Si cambias el parche, indica qué archivos admite y prueba la instalación en copias del juego, incluido el rechazo de versiones desconocidas. Añade una prueba que reproduzca el fallo y compruebe la corrección. Una partida sin fallos por sí sola no demuestra una causa resuelta.

Las contribuciones de código propio se proponen bajo MIT. Identifica su procedencia y conserva atribuciones de terceros. No incluyas archivos completos del juego ni datos de jugadores. El archivo local de investigación no forma parte de este repositorio.

Consulta el [plan de ejecución](docs/PLAN_DE_EJECUCION.md) antes de abordar una corrección nueva.

Para trabajar en el aplicador, consulta la [ficha de 1.0.7](docs/CANDIDATO_1.0.7.md) y los [comandos de compilación del README](README.md#compilar-y-conocer-el-proyecto). Usa las versiones de herramientas indicadas, ejecuta las pruebas y compara dos compilaciones. No incluyas archivos del juego en fixtures ni ejecutes la aplicación real como parte de las pruebas predeterminadas.

## Mantener la documentación al día

Documenta qué cambiaste, cómo lo comprobaste y qué queda pendiente. Actualiza las explicaciones anteriores que hayan quedado obsoletas y revisa los documentos relacionados. Conserva las pruebas históricas con fecha y versión, sin presentarlas como resultados actuales.

Las [reglas del repositorio](AGENTS.md) describen la organización y la revisión previa a publicar. La documentación útil para usuarios va a GitHub; las evidencias originales y los trámites con datos privados se conservan en `work/`, fuera de Git.
