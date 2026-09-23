# Entrega privada de prueba

Fecha: 23/09/2026. Producto: Syncrash Steam v1. Interfaz: 1.0.1.0. Protección aplicada al juego: guarda V2.

| Dato | Valor |
| --- | --- |
| Archivo para compartir | `Syncrash.exe` |
| Tamaño | 2.397.184 bytes |
| SHA256 del aplicador | `9b782d8c1ed88d58232778b62f9129219ffaf26ed3c30314a69aed4549111533` |
| SHA256 de `gbr.exe` tras aplicar | `752c95a475e62b0d61d88ec9fb7fabc07758cb217ab152d78651385a5de3d2cc` |

Esta ficha identifica el binario entregado para pruebas; no es una publicación pública ni una garantía de ausencia de fallos. El repositorio sigue privado y el candidato no está firmado digitalmente.

## Cambios de la interfaz

- Pantalla de bienvenida compacta con ilustración e icono originales, enlace al repositorio y texto breve.
- Steam vanilla disponible; Community desactivado. Su futura variante se aplicará sobre el mod instalado por separado.
- El juego se modifica solo al pulsar Aplicar parche. Si ya hay V2 exacta, se vuelve a escribir y se muestra el mismo mensaje de éxito.
- Sin respaldo, descarga de mods, observador obligatorio ni envío de datos. La recuperación se hace desde Steam.

## Verificación

El binario final pasó instalación y reinstalación en una copia aislada con ruta que contiene espacios y `ñ`. Ambas operaciones terminaron con código 0 y el mismo hash V2. La reinstalación volvió a escribir el archivo. En esa instalación solo quedaron `gbr.exe` y `Packs/data.pak`, sin respaldos ni temporales. La lógica de rechazo de ejecutable desconocido y PAK distinto se probó durante esta revisión y dejó los archivos intactos.

La primera bienvenida se revisó en el escritorio. La versión visual final, tras la petición de simplificarla, se revisó mediante un render nativo fuera de pantalla, sin retomar la automatización del escritorio detenida por el usuario. El icono, la distribución y el texto son legibles en ese render. La revisión independiente no encontró defectos materiales en el render ni en el código; no equivale a una prueba completa de interacción, teclado o escalado DPI en otros equipos.

Los registros detallados de pruebas permanecen en `work/`, fuera del repositorio, porque incluyen copias de archivos del juego.
