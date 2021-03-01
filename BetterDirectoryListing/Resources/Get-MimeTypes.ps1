Clear-Host
Add-Type -AssemblyName System.Net.Http
$exts = Get-ChildItem -File -Recurse | Group-Object -Property Extension | Select-Object Name, @{Name = "Sample"; Expression = { $_.Group[0] } } -First 1
foreach ($item in $exts) {
	$sfi = [System.IO.FileInfo]::new($item.Sample)
	$wc=[System.Net.WebClient]::new();
	$wc.OpenRead($sfi).Read(10)
}