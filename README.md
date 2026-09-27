<p align="center">
  <img src="src/Syncrash/assets/mark.png" width="64" alt="Emblema de Syncrash">
</p>
<h1 align="center">Syncrash</h1>
<p align="center"><strong>Estabilidad para Imperivm. Una instalación sencilla.</strong></p>
<p align="center">
  <a href="https://github.com/AlvaroPeyleth/Imperivm-Syncrash/releases/latest">Descargar Syncrash</a> ·
  <a href="docs/TRANSPARENCIA.md">Qué modifica</a> ·
  <a href="docs/SEGURIDAD.md">Código y transparencia</a> ·
  <a href="docs/ENTREGA_ACTUAL.md">Verificar la descarga</a> ·
  <a href="docs/CREDITOS.md">Créditos</a>
</p>

![Ilustración original de Syncrash: un casco romano ante un campamento](src/Syncrash/assets/banner.png)

Syncrash es un proyecto independiente para investigar y reducir cierres y desincronizaciones en **Imperivm RTC: HD Edition — Great Battles of Rome**. Todas las mejoras se reúnen en un mismo producto, empezando por **Steam vanilla**.

**Syncrash v1 es una versión experimental para ampliar las pruebas con jugadores.** Incorpora protección para tres rutas de cierre identificadas. Las correcciones de desincronización siguen en investigación y **no están incluidas en esta versión**.

La [entrega actual v1.0.4](docs/ENTREGA_ACTUAL.md) añade LAA y pantalla adaptable opcional con suavizado GPU, activada por defecto. Mantiene la resolución del escritorio y permite seguir abriendo el juego desde Steam. Todo va incluido en un único **Syncrash.exe**.

**El código, las recetas del parche y las instrucciones de compilación son públicos.** Puedes revisarlos y generar tu propia versión: [código y transparencia](docs/SEGURIDAD.md).

## Descargar y jugar

Descarga **[Syncrash.exe desde la última versión](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/releases/latest)**. No necesitas PowerShell ni instalar una herramienta de seguimiento.

1. Cierra Imperivm y abre **Syncrash.exe**.
2. Mantén **Steam vanilla**. Si no encuentra el juego, pulsa **Elegir…** y selecciona `gbr.exe`.
3. Deja marcada **Añadir pantalla adaptable** para incluir adaptación y suavizado, o desmárcala para aplicar solo memoria y cierres.
4. Pulsa **Aplicar parche**. Al terminar, abre Imperivm desde Steam y juega.

El aplicador admite Steam original y actualiza el parche V2 anterior. Si el resultado exacto ya está instalado, no lo reescribe. Para multijugador, utiliza la misma versión del parche y los mismos recursos en ambos equipos.

**Para quitarlo:** pulsa **Restaurar pantalla original** y después verifica los archivos del juego desde Steam para recuperar también `gbr.exe`. Desmarcar la casilla conserva una pantalla ya instalada; para cambiar de variante, restáurala antes de volver a aplicar. Syncrash no crea copia de `gbr.exe`.

## ¿Puedo jugar con alguien que no tiene Syncrash?

Se han realizado pruebas entre jugadores con Syncrash y sin él, sin problemas comunicados, aunque la compatibilidad sigue en evaluación. La protección se aplica únicamente al equipo donde está instalado: no evita los cierres de otros jugadores ni garantiza que la partida continúe si alguien se desconecta. Que otro jugador no lo tenga no desactiva tu protección.

Recomendamos que todos utilicen la misma versión de Syncrash. Las pruebas mixtas son históricas de V2; la sesión comunicada con pantalla adaptable usó el parche en ambos equipos.

## Compatibilidad y alcance

| Edición o función | Estado |
| --- | --- |
| Steam vanilla | Disponible para los archivos exactos [identificados por SHA256](docs/TRANSPARENCIA.md#archivos-admitidos). |
| Protección de cierres | Comprueba el tipo de objeto antes de tres llamadas concretas. No cubre todos los posibles cierres. |
| Memoria | LAA amplía el espacio de direcciones disponible para el juego x86 en Windows de 64 bits. |
| Pantalla adaptable | Opcional, con suavizado GPU y márgenes negros; conserva el escritorio. |
| Desincronizaciones | En investigación; v1 no incorpora una corrección de desync. |
| Community Mod | Próximamente. La opción está desactivada. |
| Otros mods | Sin compatibilidad validada. |

Cuando llegue Community, Syncrash se aplicará **después de instalar el mod por sus canales habituales**. No lo incluiremos ni cambiaremos su distribución. [Hoja de ruta](docs/ROADMAP.md).

## Qué cambia en tu equipo

- Comprueba `gbr.exe` y `Packs/data.pak` antes de actuar; rechaza versiones desconocidas.
- Sustituye `gbr.exe`. Si eliges pantalla adaptable, instala también sus componentes y registro junto al juego. No modifica mapas, guardados, PAK ni audio.
- No instala servicios, observadores ni actualizadores. No envía datos ni registros.
- Abrir la interfaz no modifica el juego. La aplicación, retirada de pantalla y exportación se realizan al pulsar sus acciones. Las órdenes CLI están descritas en la [ficha técnica](docs/CANDIDATO_1.0.4.md).

El aplicador contiene las diferencias necesarias, **no el ejecutable completo del juego**. Necesitas tu propia instalación de Steam.

## Pruebas de estabilidad

La versión 1.0.4 supera 24 pruebas locales, dos builds completos idénticos y 22 operaciones sobre copias reales. El usuario comunica más de una hora de multijugador sin incidencias en dos equipos Steam vanilla con la misma versión y los mismos componentes gráficos. Es compatibilidad observada; no una corrección causal de todos los cierres o desyncs. [Evidencia y alcance](docs/CANDIDATO_1.0.4.md).

## Ayuda a mejorar Syncrash

Las partidas normales también ayudan. Cuéntanos la versión, duración aproximada, mapa y si observaste un cierre, desync o anomalía. Si todo fue bien, también interesa saberlo.

Ante un fallo, conserva los **Logs de ambos jugadores antes de volver a abrir el juego**. Puedes enviarlos mediante el **[formulario de recopilación de logs](https://forms.gle/QAGziVvHk6peHPor9)** o por mensaje privado a **`xtalvarotx` en Discord**. Si no tienes Discord, utiliza el formulario. Los registros pueden contener IP y datos personales: no los publiques en incidencias de GitHub ni en canales públicos. [Guía de pruebas sencilla](docs/PRUEBAS_SIN_OBSERVADOR.md).

¿Buscas comunidad? Únete a **[Imperivm III Editores en Discord](https://discord.gg/YmWHktysx)**, un punto de encuentro para jugadores y modders, donde encontrarás Community Mod y otros proyectos de Imperivm.

Puedes abrir una [incidencia](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/issues) con una descripción sin datos privados. Consulta [cómo colaborar](CONTRIBUTING.md).

## Autoría y reutilización

**Creado por [AlvaroPeyleth](https://github.com/AlvaroPeyleth) · Discord: `xtalvarotx`.**

El código propio se publica bajo [licencia MIT](LICENSE). Puedes usarlo, modificarlo e integrarlo en tu mod, conservando el aviso de copyright y la licencia. Si lo incorporas a tu proyecto, agradecemos esta mención visible:

> Incluye trabajo de Syncrash, de AlvaroPeyleth (Discord: xtalvarotx).
> https://github.com/AlvaroPeyleth/Imperivm-Syncrash

La mención visible es una petición de reconocimiento; las obligaciones legales son las de MIT. Los derechos sobre Imperivm y los proyectos de terceros pertenecen a sus titulares. Syncrash no está afiliado al desarrollador ni al editor del juego.

## Gracias por hacerlo posible

Gracias a **Feronidas, Rubeneitor, Wini, Upercat y Osuka9 / Osquita** por compartir registros que ayudan a investigar los fallos. Gracias también a mi hermano y a quienes dedican tiempo a probar y comunicar lo que ocurre.

Cada aportación ayuda a avanzar. Los [créditos](docs/CREDITOS.md) distinguen las pruebas, los registros y las referencias técnicas, sin publicar datos de las partidas.

## Compilar y conocer el proyecto

La [versión 1.0.4.0](docs/CANDIDATO_1.0.4.md) y sus fuentes están publicadas. Las fuentes y licencias de pantalla también pueden exportarse desde el EXE; el código propio conserva MIT y los componentes de terceros sus licencias respectivas.

En Windows PowerShell, con .NET SDK `8.0.400` y referencias de .NET Framework 4.8:

```powershell
& ./src/Syncrash/build.ps1 -OutputPath ./work/mi-candidato/Syncrash.exe
& ./tests/run.ps1 -OutputDirectory ./work/mi-prueba
& ./scripts/compare-builds.ps1 -OutputDirectory ./work/mi-comparacion
```

El EXE completo 1.0.4 incorpora pantalla adaptable con suavizado GPU incluido como una sola opción, marcada por defecto y desactivable. Basta descargar `Syncrash.exe`; las DLL se instalan en el juego automáticamente y este se sigue abriendo desde Steam. Las fuentes/licencias se exportan desde la interfaz. Desmarcar conserva una pantalla instalada: usa Restaurar pantalla original para retirarla o cambiar de configuración. El usuario comunica una sesión de más de una hora con dos equipos Steam vanilla y la misma versión, sin incidencias. No añade texturas ni modos internos superiores a 1080p.

Los comandos anteriores generan la base de desarrollo sin pantalla. Para el EXE completo, añade `-ScreenBundleDirectory <bundle revisado>` a cada comando: el build valida por hash componentes, perfiles y fuentes antes de incrustarlos. No descarga dependencias. Consulta la [ficha actual](docs/CANDIDATO_1.0.4.md) para hashes, pruebas y recuperación.

La compilación no sobrescribe un EXE existente e incrusta la licencia. En nuestra máquina, dos compilaciones del mismo código produjeron archivos idénticos. La [ficha técnica](docs/CANDIDATO_1.0.4.md) detalla los comandos, las pruebas y sus límites. Con cualquier compilación, el `gbr.exe` parcheado debe coincidir con el hash documentado.

[Funcionamiento](docs/FUNCIONAMIENTO.md) · [Transparencia](docs/TRANSPARENCIA.md) · [Cambios](CHANGELOG.md) · [Hoja de ruta](docs/ROADMAP.md) · [Plan de pruebas](docs/PLAN_DE_EJECUCION.md)
