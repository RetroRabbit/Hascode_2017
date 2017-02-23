# --------------------------------------------------------------------------------------
# Script: ZipScript.ps1
# Author: Christiaan Saaiman
# Date: 23/02/2017
# Requires: ExecutionPolicy be set to RemoteSigned [Set-ExecutionPolicy -ExecutionPolicy RemoteSigned]
# --------------------------------------------------------------------------------------

$source = "HackingHares\*.cs"
$destination = "HackingHares.zip"

Compress-Archive -Path $source -DestinationPath $destination