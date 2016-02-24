"use strict";
/*global
 XMLHttpRequest: true,document: true, parent: true, window: true,ActiveXObject: true doAJAXCall: true
 showMessageResponse: true, alert: true
*/

// Setup Records Arrays

var old_ip;		// to store ip address when config started.
var new_ip;		// to compare and see if it has changed.

var FirmwareRevision;
var Description;
var AutoIP;
var HostName;
var NewHostName;
var IPAddrAuto;
var IPAddrStatic;
var Subnet;
var Gateway;
var Dns;
var Description;

var networkConfig = function (sPassword,bAutoip,sHostname,sIpAddr,sSubnet,sDefGw,sDns) {
	var PostStr; 
	
	response = "";
	// build up the post string when passing variables to the server side page
    // post format param1=foo&param2=bar
	PostStr = "";
	PostStr += "password=" + sPassword;
	PostStr += "&AutoIP=" + bAutoip;
	PostStr += "&Hostname=" + sHostname;
	PostStr += "&IpAddr=" + sIpAddr;
	PostStr += "&Subnet=" + sSubnet;
	PostStr += "&DefGw=" + sDefGw;
	PostStr += "&Dns=" + sDns;
	// use the generic function to make the request
	doAJAXCall('NetConf.asp', 'POST', PostStr, showNetConfResponse);
};

// No response from identify
var showNetConfResponse = function (oXML) { 
    var errVal = parseInt(oXML.responseText);
    var f = document.theform;
    var response;

    switch (errVal)
    {
    case 0:
        if(NewHostName === "") {
            // an empty hostname sets the device default, so allow it to be set
            response = "Done, Refreshing Host Name...";
            window.setTimeout("initPage()", 2000);       
        }
        else    {
            response = "Done";
        }
        break;
    case -1:
        response = "Password Error";
        break;
    case -2:
        response = "Access restricted";
        break;
    default:
        alert(oXML.responseText);
    }
    
    document.getElementById("statmesg").innerHTML = response;     
};

function verifyHostname (hname) 
{
    var errorString,ValidHostnameRegex,hostArray;
    
    errorString = "";
    ValidHostnameRegex = "^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z]|[A-Za-z][A-Za-z0-9\-]*[A-Za-z0-9])$";
    hostArray = hname.match(ValidHostnameRegex); 

    if(hname === "")    {
        return true;
    }
    else if (hostArray === null)
    {
        errorString = hname+' is not a valid HostName.';
    }
    if (errorString === "")
    {
        return true;
    }
    else
    {
        alert (errorString);
        return false;
    }
}
// Verifies an IP address
function verifyIP (ipValue, theName, required, zeroOk) 
{
    var errorString, ipPattern, ipArray, thisSegment,i; 
    errorString = "";
    ipPattern = /^(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})$/;
    ipArray = ipValue.match(ipPattern); 

    // Some fields do not require that an IP address be entered
    if (ipValue === "")
    {
        if (required)
        {
          alert(theName + ': required field.');
            return false;
        }
        else    {
            return true;
        }
    }
    // Check for special cases
    if ((!zeroOk) && (ipValue === "0.0.0.0"))    {
        errorString = theName + ': '+ipValue+' is a special IP address and cannot be used.';
    }
    else if (ipValue === "255.255.255.255")  {
        errorString = theName + ': '+ipValue+' is a special IP address and cannot be used.';
    }
    else if (ipArray === null)
    {
        errorString = theName + ': '+ipValue+' is not a valid IP address.';
    }
    else 
    {
        for (i = 1; i <= 4; i++) 
        {
            thisSegment = ipArray[i];
            if (thisSegment > 255)  {
                errorString = theName + ': '+ipValue+' is not a valid IP address.';
                i = 4;
            }
        }
    }
    if (errorString === "")
    {
        return true;
    }
    else
    {
        alert (errorString);
        return false;
    }
}  // verifyIP

// Verifies a subnet mask
function verifyMask (ipValue, theName) {

    var errorString, errString, maskEnd, ipPattern ,ipArray, thisSegment, i, j, val, calc; 

    errorString = "";
    errString = theName + ': ' + ipValue + ' is not a valid network mask.';
    maskEnd = 0;
    ipPattern = /^(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})$/;
    ipArray = ipValue.match(ipPattern); 

    if (ipArray === null) {
      errorString = errString;
    }
    else 
    {
      for (i = 1; i <= 4; i++) {
        thisSegment = ipArray[i];
        if (thisSegment > 255) {
          errorString = errString;
          break;
        }
        if (maskEnd > 0) {
          if (thisSegment != 0) {
            errorString = errString;
            break;
          }
        } 
        if (thisSegment < 255) {	// mask ending in this byte
          val = 0;
          for (j = 0; j < 8; j++) {
            calc = (thisSegment & (0x80 >> j));
            if (calc)   {
              val += calc;
            }
            else    {
              break;
            }
          }
          if (thisSegment != val) {	// error not contiguous bits.
            errorString = errString;
            break;
          }
          else  {
            maskEnd = i;            
          }
        }
      }
    }
    if (errorString === "") {
      return true;
    }
    else {
      alert (errorString);
      return false;
    }
}   // verifyMask


// handler to display or disable the dynamic ip configuration section
// associated with the onClick function of the radio buttons.  
function changeDynIp(dyn)
{
    var f, dis;

    f = document.theform;
    dis = !dyn;
    
    f.dhcphname.disabled = dis;
    
    f.ipaddrstatic.disabled = dyn;
    f.ipmask.disabled = dyn;
    f.ipgw.disabled = dyn;
    f.dnsip.disabled = dyn;
}


// fills in the fields with the values from setup records array
// once the fields are filled in, enables/disabled fields as appropriate
function populateFields()
{
    var f, i, dyn, ipPattern;

    document.getElementById("statmesg").innerHTML = "";     

    f = document.theform;
    i = 0;
    dyn = false;
    ipPattern = /^(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})$/;

    old_ip = IPAddrStatic;  // store for checking
    
    if (AutoIP === "1")     { //++ exact match so check if num or string
        dyn = true;
    }
    else    {
        dyn = false;
    }
    f.dhcphname.value = HostName;   

    // setting all the properties from the obtained values
    if (dyn) {
        f.dynip[0].checked = true;
        f.dynip[1].checked = false;
    }
    else {
        f.dynip[0].checked = false;   
        f.dynip[1].checked = true;
    }  
    f.ipaddrstatic.value = IPAddrStatic;
    f.ipmask.value = Subnet;
    f.ipgw.value = Gateway;
    f.dnsip.value = Dns;
	document.getElementById("Description").innerHTML=Description;
    changeDynIp(dyn);	  // select the correct set to disable
}

// called when web page is loaded in the browser
function initPage()
{
    var xmlhttp, xmlInfoDoc, items, i, item; 
    
    if (window.XMLHttpRequest)      {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else    {// code for IE6, IE5
      xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.open("GET","InstInfo.xml",false);
	xmlhttp.setRequestHeader("If-Modified-Since", "Thu, 1 Jan 1970 00:00:00 GMT");
    xmlhttp.send();
	
    xmlInfoDoc = xmlhttp.responseXML; 

    items = xmlInfoDoc.getElementsByTagName('LANConfiguration');
    for ( i = 0 ; i < items.length ; i++) {
    // get one item after another
    item = items[i];
        AutoIP = (item.getElementsByTagName("AutoIP")[0]).firstChild.nodeValue;              
        IPAddrAuto = (item.getElementsByTagName("IPAddrAuto")[0]).firstChild.nodeValue;              
        IPAddrStatic = (item.getElementsByTagName("IPAddrStatic")[0]).firstChild.nodeValue;              
        Subnet = (item.getElementsByTagName("IPSubnet")[0]).firstChild.nodeValue;              
        Gateway = (item.getElementsByTagName("IPGateway")[0]).firstChild.nodeValue;              
        Dns = (item.getElementsByTagName("IPDns")[0]).firstChild.nodeValue;              
        HostName = (item.getElementsByTagName("HostName")[0]).firstChild.nodeValue;    
    }
	
    items = xmlInfoDoc.getElementsByTagName('WelcomePage');
    for ( i = 0 ; i < items.length ; i++) {
    // get one item after another
		item = items[i];
        Description = (item.getElementsByTagName("Description")[0]).firstChild.nodeValue;   
    }
	
    // populate the fields from the setup records array
    populateFields(); 
}   // initPage


function cancelForm()
{
    // re-init page
    populateFields();
}

// Called when Apply is selected; validates the fields, and if all is well,
// opens the status window, submits the form, and changes the frame contents.
function applyForm()
{

    var ok, f, RecDoc, i, ipaddr, nmask, gwaddr, dnss, hname1, hname2, tempArr, recstr, dyn;

    ok = true;
    f = document.theform;
    RecDoc = parent.frames.leftmenu;
    i = 0;
    ipaddr = "";
    nmask = "";
    gwaddr = "";
    dnss = "";
    hname1 = ""; 
    hname2 = "";
    tempArr = [];   // new way of declaring arrays
    recstr = "";
    dyn = false;
  
    if (f.dynip[0].checked) {
        dyn = true;
    }  
    else    {
        dyn = false;
    } 

    if (dyn) {
        ipaddr = "0.0.0.0";
        nmask = "0.0.0.0";
        gwaddr = "0.0.0.0";  
        dnss = "0.0.0.0";
    } 
    else {
        ipaddr =  f.ipaddrstatic.value;
        nmask = f.ipmask.value;
        gwaddr = f.ipgw.value;
        dnss = f.dnsip.value;
        ok = (verifyIP(ipaddr, "IP Address", true, false) &&
              verifyMask(nmask, "Subnet Mask") &&    
              verifyIP(gwaddr, "Default Gateway", false, true) &&
	          verifyIP(dnss, "DNS Server", false, true));
    }

    hname1 = f.dhcphname.value;

    ok = verifyHostname(hname1);
    
    if (ok) {
        // verify hostname to contain ... whatever restrictions we have to apply

        if (hname1.length > 15)  {
            hname2 = hname1.substr(15);
        }
        else    {
            hname2 = hname1;
        }
    }      

    if (ok) {         
        NewHostName = hname2;
        // update the setup records global array..
        new_ip = ipaddr;
        networkConfig(f.password.value,dyn,hname2,ipaddr,nmask,gwaddr,dnss);
    }

} // applyForm


