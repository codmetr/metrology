<%@ Language="VBscript" %>


<%
' GET bEnable = Request.QueryString("enable")	' GET 
Dim sClientIP
Dim sPassword
Dim sSettings
Dim sAutoip
Dim sHostname
Dim sIpAddr	
Dim sSubnet
Dim sDefGw
Dim sDns	

'   "password=undefined&AutoIP=true&Hostname=&IpAddr=0.0.0.0&Subnet=0.0.0.0&DefGw=0.0.0.0&Dns=0.0.0.0"

    sClientIP = Request.ServerVariables("REMOTE_ADDR")
    sPassword= Request.Form("password")	' POST 
    sAutoip = Request.Form("Autoip")	
    sHostname = Request.Form("Hostname")	
    sIpAddr	= Request.Form("IpAddr")	
    sSubnet = Request.Form("Subnet")
    sDefGw = Request.Form("DefGw")
    sDns = Request.Form("Dns")
	
Dim myObject
'Set myObject = CreateObject("AtlAspDevice.SimpleAtl")
Set myObject = CreateObject("WebComCtrl.PaceWebCtrl")

Dim retVal
    retVal = myObject.ConfigNetwork(sClientIP,sPassword,sAutoip,sHostname,sIpAddr,sSubnet,sDefGw,sDns)
    response.Write(retVal)
    'You must Set your objects to "nothing" to free up the
    'the computer memory that was allocated to it
Set myObject = nothing
' redirect to new ip?, Can't as we don't know the dhcp address
%>
