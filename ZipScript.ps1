# --------------------------------------------------------------------------------------
# Script: ZipScript.ps1
# Author: Christiaan Saaiman
# Date: 23/02/2017
# Requires: ExecutionPolicy be set to RemoteSigned [Set-ExecutionPolicy -ExecutionPolicy RemoteSigned]
# NB!!! Change user section of source path to match your current computer user
# --------------------------------------------------------------------------------------

$source = "C:\Users\schut\OneDrive\Hacking-Hares\HackingHares\*.cs"
$destination = "HackingHares.zip"

Compress-Archive -Path $source -DestinationPath $destination