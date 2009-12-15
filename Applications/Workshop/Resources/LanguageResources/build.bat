del *.res
Assemble /F:StringResources.res
rem Assemble /T:lang-ar /F:StringResources.ar.res
rem Assemble /T:lang-bg /F:StringResources.bg.res
rem Assemble /T:lang-cn-big /F:StringResources.cn-big.res
Assemble /T:lang-cn-gb /F:StringResources.cn-gb.res
Assemble /T:lang-cz /F:StringResources.cz.res
Assemble /T:lang-de /F:StringResources.de.res
rem Assemble /T:lang-dk /F:StringResources.dk.res
Assemble /T:lang-es /F:StringResources.es.res
Assemble /T:lang-es-mx /F:StringResources.es-mx.res
Assemble /T:lang-fr /F:StringResources.fr.res
Assemble /T:lang-hu /F:StringResources.hu.res
Assemble /T:lang-it /F:StringResources.it.res
rem Assemble /T:lang-jp /F:StringResources.jp.res
Assemble /T:lang-kr /F:StringResources.kr.res
rem Assemble /T:lang-lt /F:StringResources.lt.res
Assemble /T:lang-nl /F:StringResources.nl.res
Assemble /T:lang-no /F:StringResources.no.res
Assemble /T:lang-pl /F:StringResources.pl.res
Assemble /T:lang-pt /F:StringResources.pt.res
Assemble /T:lang-br /F:StringResources.pt-br.res
Assemble /T:lang-ro /F:StringResources.ro.res
rem Assemble /T:lang-ru /F:StringResources.ru.res
Assemble /T:lang-se /F:StringResources.se.res
rem Assemble /T:lang-sl /F:StringResources.sl.res
rem Assemble /T:lang-sr /F:StringResources.sr.res
Assemble /T:lang-tr /F:StringResources.tr.res
rem Assemble /T:lang-fi /F:StringResources.fi.res
ResAsm *.res
copy /Y StringResources.resources ..\..\Source\Main\ICSharpCode.StartUp\Resources
copy /Y StringResources.*.resources ..\..\Output\Data\Resources
del *.resources
del *.res
pause