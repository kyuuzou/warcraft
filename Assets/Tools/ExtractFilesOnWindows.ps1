$a = New-Object -ComObject Scripting.FileSystemObject;
$file = $a.GetFile('{DATA_WAR_PATH}')
$folder = $a.GetParentFolderName($file.ShortPath)
cd {TOOL_PATH}
./war1tool.exe -v -m $folder
$path = '..\Warcraft Resources'
if (Test-Path $path) { Remove-Item $path -Recurse }
Move-Item -Path data.wc1 -Destination $path