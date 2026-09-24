# Reglas de trabajo de Syncrash

## Documentación obligatoria

Todo trabajo debe quedar documentado antes de darlo por terminado: cambios de código o interfaz, pruebas, investigación, decisiones, publicaciones y trámites externos. Registrar qué se hizo, fecha, evidencia, resultado, límites y siguiente paso. Si algo no se pudo comprobar, indicarlo.

- Actualizar o sustituir la explicación anterior cuando deje de describir el estado actual. Eliminar instrucciones obsoletas y evitar estados contradictorios entre documentos.
- Conservar los resultados históricos útiles con su fecha y versión en el historial o en el archivo de investigación. No reescribir una prueba antigua como si se hubiera repetido ni borrar evidencia para mejorar la presentación.
- Mantener una fuente principal por tema y enlazarla desde los resúmenes. No crear documentos duplicados para cada sesión.
- Revisar los documentos relacionados y sus enlaces antes de cerrar un cambio. Actualizar el historial sin inventar una nueva versión del ejecutable por un cambio de documentación.

## Organización y publicación

- `README.md`: presentación, uso y resumen del estado público.
- `docs/ENTREGA_ACTUAL.md`: versión distribuida, hashes y comprobaciones de esa entrega.
- `docs/SEGURIDAD.md`: estado actual de firma digital y gestiones antivirus; `docs/REVISION_ANTIVIRUS.md`: evidencia técnica fechada.
- `docs/PLAN_DE_EJECUCION.md` y `docs/ROADMAP.md`: próximos pasos y alcance.
- `CHANGELOG.md`: cambios relevantes para usuarios y colaboradores.
- `work/`: registro interno, trámites, evidencias originales, ensayos y datos de jugadores. Permanece excluido de Git.

Documentar no implica publicar. Antes de subir archivos, revisar el contenido y el diff preparado. Publicar solo información útil, comprobada y adecuada para el repositorio público. No subir credenciales, facturación, documentos de identidad, direcciones privadas, identificadores internos de Azure, expedientes privados, logs o dumps de jugadores ni archivos completos del juego. Publicar resúmenes anonimizados cuando aporten valor.

Mantener la lista de archivos permitidos de `.gitignore`; no usar `git add -f` para sortearla. Añadir rutas públicas solo después de revisarlas. No cambiar visibilidad, publicar comunicaciones ni conceder permisos sin la autorización correspondiente.

## Exactitud del estado

Distinguir entre planificado, enviado, en revisión y completado. Vincular cada análisis a su hash y cada conclusión a su evidencia. Una solicitud de revisión no es una aprobación del antivirus; una firma identifica al editor y protege la integridad, pero no garantiza que desaparezca SmartScreen. Al firmar una entrega, actualizar hash, ficha, informes y archivos de la release de forma coherente.
