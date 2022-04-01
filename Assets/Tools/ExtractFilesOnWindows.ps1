$a = New-Object -ComObject Scripting.FileSystemObject;
$file = $a.GetFile('{DATA_WAR_PATH}')
$folder = $a.GetParentFolderName($file.ShortPath)
cd {TOOL_PATH}
./war1tool.exe -v $folder ../Warcra~1/