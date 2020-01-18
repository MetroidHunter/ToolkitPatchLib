set outputdir="Z:\SteamLibrary\steamapps\common\RimWorld\Mods\ToolkitPatchLib"

rm -r %outputdir%
mkdir %outputdir%

xcopy /E /Y "..\About" %outputdir%\About\
xcopy /E /Y "..\Assemblies" %outputdir%\Assemblies\