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

**Syncrash v1 es experimental y esperamos tu feedback.** Queremos saber cómo te funciona: voces, cierres, desincronizaciones y cualquier comportamiento extraño. No es una solución definitiva a todos los fallos. Incorpora protección para tres rutas de cierre identificadas; las correcciones de desincronización siguen en investigación.

La [entrega actual v1.0.5](docs/ENTREGA_ACTUAL.md) reúne memoria ampliada, protección de cierres, pantalla adaptable con suavizado y reparación de voces en español, italiano e inglés. Pantalla y voces son opciones independientes, **marcadas por defecto y desactivables**. Todo va en un único **Syncrash.exe**; el juego sigue abriéndose desde Steam. [Estado, pruebas y límites](docs/ENTREGA_ACTUAL.md).

**El código, las recetas del parche y las instrucciones de compilación son públicos.** Puedes revisarlos y generar tu propia versión: [código y transparencia](docs/SEGURIDAD.md).

## Descargar y jugar

Descarga **[Syncrash.exe desde la última versión](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/releases/latest)**. No necesitas PowerShell ni instalar una herramienta de seguimiento.

1. Cierra Imperivm y abre **Syncrash.exe**.
2. Mantén **Steam vanilla**. Si no encuentra el juego, pulsa **Elegir…** y selecciona `gbr.exe`.
3. Elige las opciones: **Añadir pantalla adaptable** y **Reparar voces de unidades** están marcadas por defecto. Puedes desmarcarlas; memoria y protección de cierres se incluyen siempre.
4. Pulsa **Aplicar parche**. Al terminar, abre Imperivm desde Steam y juega.

El aplicador admite Steam original y actualiza el parche V2 anterior. Si el resultado exacto ya está instalado, no lo reescribe. Para multijugador, utiliza la misma versión del parche y los mismos recursos en ambos equipos.

**Si cambias de idioma:** cierra el juego y vuelve a aplicar Syncrash antes de jugar para actualizar las voces.

**Para quitarlo:** desmarca **Reparar voces de unidades** y aplica; pulsa **Restaurar pantalla original** y después verifica archivos en Steam para recuperar también `gbr.exe`. Desmarcar pantalla conserva la instalada; su botón de restauración la retira. Steam no elimina por sí solo los WAV añadidos. Syncrash no crea copia de `gbr.exe`.

## ¿Puedo jugar con alguien que no tiene Syncrash?

Se han realizado pruebas entre jugadores con Syncrash y sin él, sin problemas comunicados, aunque la compatibilidad sigue en evaluación. La protección se aplica únicamente al equipo donde está instalado: no evita los cierres de otros jugadores ni garantiza que la partida continúe si alguien se desconecta. Que otro jugador no lo tenga no desactiva tu protección.

Recomendamos que todos utilicen la misma versión de Syncrash. Las pruebas mixtas son históricas de V2; la sesión comunicada con pantalla adaptable corresponde a 1.0.4. La comprobación multijugador con las nuevas voces sigue pendiente.

## Compatibilidad y alcance

| Edición o función | Estado |
| --- | --- |
| Steam vanilla | Disponible para los archivos exactos [identificados por SHA256](docs/TRANSPARENCIA.md#archivos-admitidos). |
| Protección de cierres | Comprueba el tipo de objeto antes de tres llamadas concretas. No cubre todos los posibles cierres. |
| Memoria | LAA amplía el espacio de direcciones disponible para el juego x86 en Windows de 64 bits. |
| Pantalla adaptable | Opcional, con suavizado GPU y márgenes negros; conserva el escritorio. |
| Voces de unidades | Opcional, en español, italiano e inglés, desde los PAK del jugador sin modificarlos. Cobertura auditiva todavía en ampliación. |
| Desincronizaciones | En investigación; v1 no incorpora una corrección de desync. |
| Community Mod | Próximamente. La opción está desactivada. |
| Otros mods | Sin compatibilidad validada. |

Cuando llegue Community, Syncrash se aplicará **después de instalar el mod por sus canales habituales**. No lo incluiremos ni cambiaremos su distribución. [Hoja de ruta](docs/ROADMAP.md).

## Qué cambia en tu equipo

- Comprueba `gbr.exe` y `Packs/data.pak` antes de actuar; rechaza versiones desconocidas.
- Sustituye `gbr.exe`. Si eliges pantalla adaptable, instala también sus componentes y registro junto al juego. Con voces marcadas, extrae WAV a rutas sueltas y registra su propiedad. No modifica mapas, guardados ni PAK.
- No instala servicios, observadores ni actualizadores. No envía datos ni registros.
- Abrir la interfaz no modifica el juego. La aplicación, retirada de voces/pantalla y exportación se realizan al pulsar sus acciones. Las órdenes CLI están descritas en la [ficha técnica](docs/CANDIDATO_1.0.5.md).

El aplicador contiene las diferencias necesarias, **no el ejecutable completo del juego**. Necesitas tu propia instalación de Steam.

## Pruebas de estabilidad

v1.0.5 se comprueba con pruebas automáticas, copias reales del juego y comparación de compilaciones. El prototipo español recibió feedback favorable; **la escucha completa en italiano/inglés y el multijugador con voces siguen pendientes**. Las partidas históricas de 1.0.4 no se presentan como nuevas pruebas de esta entrega. [Evidencia y alcance](docs/CANDIDATO_1.0.5.md).

## Ayuda a mejorar Syncrash

**Tu feedback es parte de esta v1.** Cuéntanos la versión, idioma, opciones activadas, duración aproximada, mapa y si las voces sonaron correctamente o hubo cierres, desyncs o anomalías. **Si todo fue bien, también queremos saberlo.** No necesitas instalar un observador para participar.

Ante un fallo, conserva los **Logs de ambos jugadores antes de volver a abrir el juego**. Puedes enviarlos mediante el **[formulario de recopilación de logs](https://forms.gle/QAGziVvHk6peHPor9)** o por mensaje privado a **`xtalvarotx` en Discord**. Si no tienes Discord, utiliza el formulario. Los registros pueden contener IP y datos personales: no los publiques en incidencias de GitHub ni en canales públicos. [Guía de pruebas sencilla](docs/PRUEBAS_SIN_OBSERVADOR.md).

¿Buscas comunidad? Únete a **[Imperivm III Editores en Discord](https://discord.gg/YmWHktysx)**, un punto de encuentro para jugadores y modders, donde encontrarás Community Mod y otros proyectos de Imperivm.

Puedes abrir una [incidencia](https://github.com/AlvaroPeyleth/Imperivm-Syncrash/issues) con una descripción sin datos privados. Consulta [cómo colaborar](CONTRIBUTING.md).

## Otros proyectos de la comunidad

Descubre otros proyectos para ampliar o mejorar Imperivm en el [directorio de mods y herramientas de la comunidad](docs/PROYECTOS_COMUNIDAD.md).

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

El código propio conserva MIT; los componentes de pantalla mantienen sus licencias y fuentes exportables. La [ficha de 1.0.5](docs/CANDIDATO_1.0.5.md) identifica la compilación, sus pruebas y límites.

En Windows, con **PowerShell 7.6.5 (`pwsh`)**, .NET SDK `8.0.400` y referencias de .NET Framework 4.8:

```powershell
& ./src/Syncrash/build.ps1 -OutputPath ./work/mi-candidato/Syncrash.exe
& ./tests/run.ps1 -OutputDirectory ./work/mi-prueba
& ./scripts/compare-builds.ps1 -OutputDirectory ./work/mi-comparacion
```

Los comandos anteriores generan la base de desarrollo con voces y sin pantalla. Para el EXE completo, añade `-ScreenBundleDirectory <bundle revisado>` a cada comando: el build valida por hash componentes, perfiles y fuentes antes de incrustarlos. No descarga dependencias ni sobrescribe un EXE existente. [Compilación, hashes y recuperación](docs/CANDIDATO_1.0.5.md).

[Funcionamiento](docs/FUNCIONAMIENTO.md) · [Transparencia](docs/TRANSPARENCIA.md) · [Cambios](CHANGELOG.md) · [Hoja de ruta](docs/ROADMAP.md) · [Plan de pruebas](docs/PLAN_DE_EJECUCION.md)
