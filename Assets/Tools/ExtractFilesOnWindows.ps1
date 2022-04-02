Function DecompressGzip{
    Param($infile, $outfile = ($infile -replace '\.gz$',''))

    $input = New-Object System.IO.FileStream $inFile, ([IO.FileMode]::Open), ([IO.FileAccess]::Read), ([IO.FileShare]::Read)
    $output = New-Object System.IO.FileStream $outFile, ([IO.FileMode]::Create), ([IO.FileAccess]::Write), ([IO.FileShare]::None)
    $gzipStream = New-Object System.IO.Compression.GzipStream $input, ([IO.Compression.CompressionMode]::Decompress)

    $buffer = New-Object byte[](1024)

    while($true){
        $read = $gzipStream.Read($buffer, 0, 1024)
        if ($read -le 0){
			break
		}
        $output.Write($buffer, 0, $read)
    }

    $gzipStream.Close()
    $output.Close()
    $input.Close()
}

$a = New-Object -ComObject Scripting.FileSystemObject;
$file = $a.GetFile('{DATA_WAR_PATH}')
$folder = $a.GetParentFolderName($file.ShortPath)
cd {TOOL_PATH}
./war1tool.exe -v -m $folder

Get-ChildItem -Path .\ -Filter *.gz -Recurse -File -Name| ForEach-Object {
    $location = Get-Location
	$infile = $location.Path + '\' + $_
	$outFile = $infile.replace('.gz', '')

	DecompressGzip $infile $outfile
	Remove-Item $infile
}

Get-ChildItem *.smp -Recurse | Rename-Item -NewName { $_.name -Replace '\.smp$','.smp.txt' }
Get-ChildItem *.sms -Recurse | Rename-Item -NewName { $_.name -Replace '\.sms$','.sms.txt' }

$path = '..\Warcraft Resources'

if (Test-Path $path) {
	Remove-Item $path -Recurse
}

Move-Item -Path data.wc1 -Destination $path
