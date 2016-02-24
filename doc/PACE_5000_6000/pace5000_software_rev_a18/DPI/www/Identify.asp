<%@ Language="VBscript" %>


<%
Dim bEnable
Dim bIdentify
' GET bEnable = Request.QueryString("enable")	' GET 
	bEnable = Request.Form("enable")	' POST 
	if bEnable = "1" then
		bIdentify = true
	else
		bIdentify = false
	end if

Dim myObject
'Set myObject = CreateObject("AtlAspDevice.SimpleAtl")
Set myObject = CreateObject("WebComCtrl.PaceWebCtrl")


CALL myObject.IdentifyPace(bIdentify)
'You must Set your objects to "nothing" to free up the
'the computer memory that was allocated to it
Set myObject = nothing
%>


