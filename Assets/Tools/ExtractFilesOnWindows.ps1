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

Function CopyFolder{
    Param(  
        [Parameter(Mandatory=$true)]  
        [string]$sourcePath,  
        [Parameter(Mandatory=$true)]  
        [string]$destinationPath  
    )  

    $files = Get-ChildItem -Path $sourcePath -Recurse -Filter "*.*"  
  
    foreach($file in $files){  
        $sourcePathFile = $file.FullName  
        $destinationPathFile = $file.FullName.Replace($sourcePath,  $destinationPath)  
  
        $exists = Test-Path $destinationPathFile  
  
        if(!$exists) {  
            $dir = Split-Path -parent $destinationPathFile  
            if (!(Test-Path($dir))) { New-Item -ItemType directory -Path $dir }  
            Copy-Item -Path $sourcePathFile -Destination $destinationPathFile -Recurse -Force  
        } else {  
            $isFile = Test-Path -Path $destinationPathFile -PathType Leaf  
         
            if($isFile) {  
                $different = Compare-Object -ReferenceObject $(Get-Content $sourcePathFile) -DifferenceObject $(Get-Content $destinationPathFile)  
                if(Compare-Object -ReferenceObject $(Get-Content $sourcePathFile) -DifferenceObject $(Get-Content $destinationPathFile)){  
                $dir = Split-Path -parent $destinationPathFile  
                if (!(Test-Path($dir))) { New-Item -ItemType directory -Path $dir }  
  
                    Copy-Item -Path $sourcePathFile -Destination $destinationPathFile -Recurse -Force  
                }  
            }  
        }  
    }  
}

$a = New-Object -ComObject Scripting.FileSystemObject;
$file = $a.GetFile('{DATA_WAR_PATH}')
$folder = $a.GetParentFolderName($file.ShortPath)
cd {TOOL_PATH}

if (Test-Path data.wc1) {
    Remove-Item data.wc1 -Recurse
}

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
Get-ChildItem -Path $path -Recurse -File | Where {($_.Extension -ne '.meta')} | Remove-Item

CopyFolder data.wc1 $path
Remove-Item data.wc1 -Recurse
