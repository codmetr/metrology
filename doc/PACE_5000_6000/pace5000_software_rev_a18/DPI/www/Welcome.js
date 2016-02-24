"use strict";

/*global XMLHttpRequest: true,document: true, parent: true, window: true,ActiveXObject: true doAJAXCall: true
showMessageResponse: true
*/

var old_ip;		// to store ip address when config started.
var new_ip;		// to compare and see if it has changed.

var PageTitle;

var model;
var manuf;
var SerialNumber;
var FirmwareRevision;
var Description;

var HostName;
var IPAddr;
var MacAddr;
var VxiAddrString;
var TcpAddrString;


var LXIDeviceModel;
var LXIClass;
var LXIVersion;


// handler to display or disable the dynamic ip configuration section
// associated with the onClick function of the radio buttons.  
function changeDynIp(dyn)
{
    var f = document.theform;
    var dis = !dyn;

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

    // titles
    document.getElementById("TableTitle").innerHTML = manuf + "-" + model;      //++ add LXI
    parent.document.title = manuf + "-" + model;       //++ add LXI
	
    // table data
    document.getElementById("TblTitle1").innerHTML = "Model";    
    document.getElementById("TblTitle2").innerHTML = "Manufacturer";    
    document.getElementById("TblTitle3").innerHTML = "SerialNumber";    
    document.getElementById("TblTitle4").innerHTML = "Software Revision";    
    document.getElementById("TblTitle5").innerHTML = "Description";    
    document.getElementById("TblTitle6").innerHTML = "HostName";    
    document.getElementById("TblTitle7").innerHTML = "IPAddr";    
    document.getElementById("TblTitle8").innerHTML = "MacAddr";    
    document.getElementById("TblTitle9").innerHTML = "TCP Address String";    
    document.getElementById("TblTitle10").innerHTML = "VXI-11 Address String";    

    document.getElementById("TblData1").innerHTML = model;
    document.getElementById("TblData2").innerHTML = manuf;
    document.getElementById("TblData3").innerHTML = SerialNumber;
    document.getElementById("TblData4").innerHTML = FirmwareRevision;
    document.getElementById("TblData5").innerHTML = Description;
    document.getElementById("TblData6").innerHTML = HostName;
    document.getElementById("TblData7").innerHTML = IPAddr;
    document.getElementById("TblData8").innerHTML = MacAddr;
    document.getElementById("TblData9").innerHTML = TcpAddrString;
    document.getElementById("TblData10").innerHTML = VxiAddrString;    
 
	top.frames["topbar"].document.getElementById("pace_image").src = model + ".png"
 
}

// called when web page is loaded in the browser
function initPage()
{
    var xmlhttp;  
    if (window.XMLHttpRequest)
    {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else
    {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
	  
//	xmlhttp.setRequestHeader("Pragma", "Cache-Control: no-cache");

    xmlhttp.open("GET","InstInfo.xml",false);
	xmlhttp.setRequestHeader("If-Modified-Since", "Thu, 1 Jan 1970 00:00:00 GMT");
    xmlhttp.send();
	
    var xmlInfoDoc = xmlhttp.responseXML; 
//    var xmlDocEl=xmlhttp.responseXML.documentElement;

    var items = xmlInfoDoc.getElementsByTagName('WelcomePage');
    for (var i = 0 ; i < items.length ; i++) {
    // get one item after another
        var item = items[i];
        try     {
            model = (item.getElementsByTagName("Model")[0]).firstChild.data;   
            manuf = (item.getElementsByTagName("Manufacturer")[0]).firstChild.data;   
            SerialNumber = (item.getElementsByTagName("SerialNumber")[0]).firstChild.data;   
            FirmwareRevision = (item.getElementsByTagName("FirmwareRevision")[0]).firstChild.data;   
            
            Description = (item.getElementsByTagName("Description")[0]).firstChild.nodeValue;   

            HostName = (item.getElementsByTagName("HostName")[0]).firstChild.nodeValue;    
            IPAddr = (item.getElementsByTagName("IPAddr")[0]).firstChild.nodeValue;              
            MacAddr = (item.getElementsByTagName("MacAddr")[0]).firstChild.nodeValue;   
            VxiAddrString = (item.getElementsByTagName("VxiAddrString")[0]).firstChild.nodeValue;   
            TcpAddrString = (item.getElementsByTagName("TcpAddrString")[0]).firstChild.nodeValue;   
                        
//            LXIDeviceModel= (item.getElementsByTagName("")[0]).firstChild.nodeValue;   
//            LXIClass = (item.getElementsByTagName("")[0]).firstChild.nodeValue;   
//            LXIVersion = (item.getElementsByTagName("")[0]).firstChild.nodeValue;        
        }
        catch (err)
        {
        //Handle errors here
        }
    }
    // populate the fields from the setup records array 
    populateFields(); 
}   // initPage

var Identify = function (chk) {
	// build up the post string when passing variables to the server side page
	var PostStr = "";
	if (chk)	{
		PostStr = "enable=1";
	}
	else    {
		PostStr = "enable=0";
	}
	// use the generic function to make the request
	doAJAXCall('identify.asp', 'POST', PostStr, showIdentifyResponse);
}

// No response from identify, just for errors
var showIdentifyResponse = function (oXML) { 
    var resp;
    resp = oXML.responseText;
    if(resp != "")  {
        alert(oXML.responseText);
    }
};
