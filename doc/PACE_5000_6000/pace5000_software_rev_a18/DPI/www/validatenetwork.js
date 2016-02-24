<!--

// Verifies an IP address
function verifyIP (ipValue, theName, required, zeroOk) 
{
    var errorString = "";

    var ipPattern = /^(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})$/;
    var ipArray = ipValue.match(ipPattern); 

    // Some fields do not require that an IP address be entered
    if (ipValue == "")
    {
        if (required)
        {

          alert(theName + ': required field.');

          return false;
        }
        else
          return true;
    }
    
    // Check for special cases
    if ((!zeroOk) && (ipValue == "0.0.0.0"))

        errorString = theName + ': '+ipValue+' is a special IP address and cannot be used.';

    else if (ipValue == "255.255.255.255")

        errorString = theName + ': '+ipValue+' is a special IP address and cannot be used.';

    else if (ipArray == null)
    {

        errorString = theName + ': '+ipValue+' is not a valid IP address.';

    }
    else 
    {
        for (i = 1; i <= 4; i++) 
        {
            thisSegment = ipArray[i];
            if (thisSegment > 255) 
            {

                errorString = theName + ': '+ipValue+' is not a valid IP address.';

                i = 4;
            }
        }
    }
    if (errorString == "")
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
    errorString = "";

    errString = theName + ': ' + ipValue + ' is not a valid network mask.';

    var maskEnd = 0;
    var val, calc;
    var ipPattern = /^(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})$/;
    var ipArray = ipValue.match(ipPattern); 

    if (ipArray == null) {
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
            if (calc) 
              val += calc;
            else
              break;
          }
          if (thisSegment != val) {	// error not contiguous bits.
            errorString = errString;
            break;
          }
          else
            maskEnd = i;            
        }
      }
    }
    if (errorString == "") {
      return true;
    }
    else {
      alert (errorString);
      return false;
    }
}  // verifyMask

-->
