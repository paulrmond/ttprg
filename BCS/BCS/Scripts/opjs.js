
// --- 10/21/2016 --- //
//function AutoCompute(val) {
//    if (val.value != null) {
//        var QuantiT;
//        var FeeT;
//        var VaT
//        var AmtToT;
//        var WTax;
//        var Disp;
//        QuantiT = document.getElementById("AddQuantity").value;
//        FeeT = document.getElementById("AddAmount").value;
//        VaT = document.getElementById("Withholding").value;
//        if (VaT == "NO") {
//            AmtToT = QuantiT * FeeT;
//            Disp = AmtToT
//        }
//        else
//        {
//            alert(VaT);
//            AmtToT = QuantiT * FeeT;
//            WTax = AmtToT * 0.01;
//            Disp = AmtToT - WTax;
//        }
//        document.getElementById("AddTotAmt").value = Disp;
//    }
//}
// ----------------- //

function Clear(val) {
    document.getElementById("AddType").value = "";
    document.getElementById("AddQuantity").value = "";
    document.getElementById("AddAmount").value = "";
}

function EditOPD(item) {
    var p = document.getElementById(item).getElementsByClassName('cell');
    for (var i = 0; i < p.length; i++) {
        var childId = p[i].title;
        var childValue = p[i].innerHTML;
        //alert("Child " + childId + "=" + childValue);
        var elementToPopulate = document.getElementById(childId);
        //alert(elementToPopulate.value);
        if (childId == "Withholding" && childValue == "YES") {
            document.getElementById("WithholdingYes").checked = true;
            document.getElementById("WithholdingNo").checked = false;
        }
        else {
            document.getElementById("WithholdingYes").checked = false;
            document.getElementById("WithholdingNo").checked = true;
        }
        elementToPopulate.value = childValue;
    }
}
