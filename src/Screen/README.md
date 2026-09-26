# Fuentes de pantalla adaptable

Instantánea del código nativo probado el 26/09/2026, conservada con el aplicador
del 27/09. Los ZIP contienen únicamente fuentes de texto y comandos de
compilación; no incluyen DLL, ejecutables del juego, partidas ni registros.
Se conservan como archivos fuente para mantener los mismos bytes y hashes que
las fuentes incrustadas y verificadas en el EXE.

- `Sources/proxy-source.zip`: fuentes completas del proxy, geometría de pantalla,
  márgenes negros, pruebas y `build-proxy.cmd`. Crear una carpeta `build` junto
  a `source` antes de ejecutar el comando.
- `Sources/syncrash-dxwnd-changes.zip`: cambios de DxWnd, superficie persistente,
  presentación GPU, pruebas y receta de compilación. Seguir su
  `BUILD-AND-CHANGES.txt` sobre las fuentes upstream indicadas abajo.
- `profiles/dxwnd-smooth.dxw`: perfil GPU usado por la casilla de pantalla.
  Se instala como `dxwnd.dxw`. El otro perfil conserva el modo sin filtro para
  las órdenes avanzadas de prueba; no se muestran dos opciones en la interfaz.
- `LICENSE` y `PROVENANCE.txt`: GPL y procedencia del componente nativo,
  independientes de la licencia MIT del aplicador C#.

## Base upstream

Se necesita el [archivo oficial de fuentes de DxWnd 2.06.15](https://sourceforge.net/projects/dxwnd/files/Sources/v2_06_15_src.rar/),
SHA256 `c0f7632332c5389a1876b0c561286b82594729c192d72e5b9d815f1828f70d38`.
El EXE completo también contiene ese archivo, exportable desde «Licencias y
fuentes de pantalla». No se añade el archivo upstream ni sus bibliotecas
precompiladas a Git. Las herramientas Microsoft son requisitos externos.

| Archivo fuente | SHA256 |
| --- | --- |
| proxy-source.zip | `9ff1a14853c56f5d97102486e844e5e7184b9077b857c10f0e715a990d60f2d8` |
| syncrash-dxwnd-changes.zip | `a513116c3d82dc6a4d857b097f8ee23b10df3b341674c8cdd815c1e9620b6a26` |

La reconstrucción desde upstream más estos cambios se comprobó localmente.
Las compilaciones nativas no se declaran idénticas entre fechas o toolchains:
el build del aplicador exige las DLL exactas fijadas por hash. La [ficha del
candidato](../../docs/CANDIDATO_1.0.4.md) distingue esa limitación de la
reproducibilidad del EXE C# con sus entradas nativas verificadas.
