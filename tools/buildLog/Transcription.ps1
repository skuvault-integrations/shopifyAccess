function Enter-BuildScript {
	if($logfile) {
		if( $Host -and $Host.UI -and $Host.UI.RawUI ) {
			$rawUI = $Host.UI.RawUI
			$oldBufferSize = $rawUI.BufferSize
			$typeName = $oldBufferSize.GetType().FullName
			$newSize = New-Object $typeName (128, $oldBufferSize.Height)
			$rawUI.BufferSize = $newSize
		}
		
		$logfolder = Split-Path $logfile -Parent
		New-Item $logfolder -Type directory -Force  | Out-Null
		
		$transcribing = $true
		Start-Transcript $logfile
	}
}

function Exit-BuildScript {
	if( $transcribing ) {
		Write-Host @'

---------- Transcript Build Summary ----------

'@
		
		foreach($_ in $Result.AllTasks) {
			Write-Host ('{0,-16} {1} {2}:{3}' -f $_.Elapsed, $_.Name, $_.InvocationInfo.ScriptName, $_.InvocationInfo.ScriptLineNumber)
			if ($_.Error) {
				Write-Host -ForegroundColor Red (*Err* $_.Error $_.Error.InvocationInfo)
			}
		}
	
		if( $oldBufferSize -ne $null ) {
			#$host.UI.RawUI.BufferSize = $oldBufferSize
		}

		Stop-Transcript
		
		Write-Host @'
		
***********************************************************
	
'@
	}
}