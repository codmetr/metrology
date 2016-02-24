<!--
"use strict";

/*global XMLHttpRequest: true,document: true, parent: true, window: true,ActiveXObject: true doAJAXCall: true
showMessageResponse: true,onerror: true,alert: true
*/

var txt = "";
function handleErr(msg, url, l)
{
    txt = "There was an error on this page.\n\n";
    txt += "Error: " + msg + "\n";
    txt += "URL: " + url + "\n";
    txt += "Line: " + l + "\n\n";
    alert(txt);
    return true;
}
onerror = handleErr;

function LoadXml(url)	
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
	  
    xmlhttp.open("GET", url, false);
    xmlhttp.send();
	if(!xmlhttp.getResponseHeader("Date")) {
	  var cached = xmlhttp;
		if (window.XMLHttpRequest)
		{// code for IE7+, Firefox, Chrome, Opera, Safari
			xmlhttp = new XMLHttpRequest();
		}
		else
		{// code for IE6, IE5
			xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
		}
	  var ifModifiedSince =
		cached.getResponseHeader("Last-Modified") ||
		new Date(0); // January 1, 1970
	  xmlhttp.open("GET", url, false);
	  xmlhttp.setRequestHeader("If-Modified-Since", ifModifiedSince);
	  xmlhttp.send("");
	  if(xmlhttp.status == 304) {
		xmlhttp = cached;
	  }
	}	
	return xmlhttp.responseXML;
}


/** Start Creative commons code **/


/** XHConn - Simple XMLHTTP Interface - bfults@gmail.com - 2005-04-08        **
 ** Code licensed under Creative Commons Attribution-ShareAlike License      **
 ** http://creativecommons.org/licenses/by-sa/2.0/                           **/

function XHConn()
{
    var xmlhttp, bComplete = false;
    try {   xmlhttp = new ActiveXObject("Msxml2.XMLHTTP"); }
    catch (e) { try { xmlhttp = new ActiveXObject("Microsoft.XMLHTTP"); }
    catch (e) { try { xmlhttp = new XMLHttpRequest(); }
    catch (e) { xmlhttp = false; }}}
    if (!xmlhttp) return null;
    this.connect = function(sURL, sMethod, sVars, fnDone)
    {
        if (!xmlhttp) return false;
        bComplete = false;
        sMethod = sMethod.toUpperCase();
        try {
              if (sMethod == "GET")
              {
                xmlhttp.open(sMethod, sURL+"?"+sVars, true);
                sVars = "";
              }
              else
              {
                xmlhttp.open(sMethod, sURL, true);
                xmlhttp.setRequestHeader("Method", "POST "+sURL+" HTTP/1.1");
                xmlhttp.setRequestHeader("Content-Type","application/x-www-form-urlencoded");
               	xmlhttp.setRequestHeader("If-Modified-Since", "Thu, 1 Jan 1970 00:00:00 GMT");
              }
              xmlhttp.onreadystatechange = function(){
              if (xmlhttp.readyState == 4 && !bComplete)
              {
                  bComplete = true;
                  fnDone(xmlhttp);
              }};
        xmlhttp.send(sVars);
    }
    catch(z) { return false; }
    return true;
    };
    return this;
}
// doAJAXCall : Generic AJAX Handler, used with XHConn
// Author : Bryce Christensen (www.esonica.com)
// PageURL : the server side page we are calling
// ReqType : either POST or GET, typically POST
// PostStr : parameter passed in a query string format 'param1=foo&param2=bar'
// FunctionName : the JS function that will handle the response

var doAJAXCall = function (PageURL, ReqType, PostStr, FunctionName) {

	// create the new object for doing the XMLHTTP Request
	var myConn = new XHConn();

	// check if the browser supports it
	if (myConn)	{
	    
	    // XMLHTTPRequest is supported by the browser, continue with the request
	    myConn.connect('' + PageURL + '', '' + ReqType + '', '' + PostStr + '', FunctionName);    
	} 
	else {
	    // Not support by this browser, alert the user
	    alert("XMLHTTP not available. Try a newer/better browser, this application will not work!");   
	}
}

// launched from button click 
var getMessage = function () {
	// build up the post string when passing variables to the server side page
	var PostStr = "";
	// use the generic function to make the request
	doAJAXCall('serversidetest.asp', 'POST', PostStr, showMessageResponse);
}

// The function for handling the response from the server
var showMessageResponse = function (oXML) { 
    
    // get the response text, into a variable
    var response = oXML.responseText;
    alert(response);
    
    // update the Div to show the result from the server
//	document.getElementById("responseDiv").innerHTML = response;
};
/** End Creative commons code **/


-->