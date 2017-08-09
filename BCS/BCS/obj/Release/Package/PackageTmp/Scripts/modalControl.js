$(window).load(function () {

    //var companyCnt = $('#companyCnt').attr('companyCnt');
    var isvalidated = $('#companyCnt').attr('isValidPostback');
    var CompanySelected = $('#companyCnt').attr('CompanySelected');
    var OPCompanySelected = $('#companyCnt').attr('OPCompanySelected');
    var SelectOPAccount = $('#companyCnt').attr('SelectOPAccount');
    var TransactionSuccess = $('#TransactionSuccess').attr('TransactionSuccess');
    var BillingStatus = $('#BillingStatus').attr('BillingStatus');
    var ToggleBillingStatus = $('#BillingStatus').attr('ToggleBillingStatus');
    var SelectedCompany = $('#companyCnt').attr('SelectedCompany');
    var Accountable = $('#Accountable').attr('Accountable');
    var JVAccountTag = $('#JVAccountTag').attr('JVAccountTag');
    var ALERT_TITLE = "BCS Notification";
    var ALERT_BUTTON_TEXT = "Ok";
    document.body.style.cursor = 'default';
    $('input[type=text]').each(function () {
        $(this).attr("autocomplete", "off");
        
    })
    //alert(companyCnt);
    //alert(isvalidated);
    //alert(OPCompanySelected);
    if (JVAccountTag == 'AF' || JVAccountTag == 'BN') {
        var element = document.getElementById("hideVisa");
        element.classList.add("hidden");
        var element1 = document.getElementById("Serial");
        element1.classList.remove("in");
        var element1 = document.getElementById("Serial");
        element1.classList.remove("active");

        document.getElementById("hideNationality").hidden = true;

        document.getElementById("hideGivenName").hidden = true;
        document.getElementById("hideMiddleName").hidden = true;
        document.getElementById("hideSurName").hidden = true;
        document.getElementById("hideVisaName").hidden = true;

        document.getElementById("companyCnt").hidden = true;
        document.getElementById("hideEditGivenName").hidden = true;
        document.getElementById("hideEditMiddleName").hidden = true;
        document.getElementById("hideEditSurName").hidden = true;
        document.getElementById("hideEditVisaName").hidden = true;
        document.getElementById("hideEditNationality").hidden = true;
    }
    else if (JVAccountTag == 'VISA') {
        var element = document.getElementById("hideForm");
        element.classList.add("hidden");
        var element = document.getElementById("hideSerial");
        element.classList.add("hidden");
        document.getElementById("companyCnt2").hidden = true;
        document.getElementById("companyCnt3").hidden = true;
        document.getElementById("hideStart").hidden = true;
        document.getElementById("hideEnd").hidden = true;
        document.getElementById("hideSN1").hidden = true;
        document.getElementById("hideSN2").hidden = true;
        document.getElementById("hideSN3").hidden = true;
        document.getElementById("hideSN4").hidden = true;
        document.getElementById("hideSN5").hidden = true;
        document.getElementById("hideSN6").hidden = true;
        document.getElementById("hideSN7").hidden = true;
        document.getElementById("hideSN8").hidden = true;
        document.getElementById("hideSN9").hidden = true;
        document.getElementById("hideSN10").hidden = true;
        document.getElementById("hideSN11").hidden = true;
        document.getElementById("hideSN12").hidden = true;
        document.getElementById("hideSN13").hidden = true;
        document.getElementById("hideSN14").hidden = true;
        document.getElementById("hideSN15").hidden = true;
        document.getElementById("hideEditStart").hidden = true;
        document.getElementById("hideEditEnd").hidden = true;
        document.getElementById("hideEditSN1").hidden = true;
        document.getElementById("hideEditSN2").hidden = true;
        document.getElementById("hideEditSN3").hidden = true;
        document.getElementById("hideEditSN4").hidden = true;
        document.getElementById("hideEditSN5").hidden = true;
        document.getElementById("hideEditSN6").hidden = true;
        document.getElementById("hideEditSN7").hidden = true;
        document.getElementById("hideEditSN8").hidden = true;
        document.getElementById("hideEditSN9").hidden = true;
        document.getElementById("hideEditSN10").hidden = true;
        document.getElementById("hideEditSN11").hidden = true;
        document.getElementById("hideEditSN12").hidden = true;
        document.getElementById("hideEditSN13").hidden = true;
        document.getElementById("hideEditSN14").hidden = true;
        document.getElementById("hideEditSN15").hidden = true;
    }
    else if (JVAccountTag == 'PF') {
        var element = document.getElementById("hideVisa");
        element.classList.add("hidden");
        var element = document.getElementById("hideForm");
        element.classList.add("hidden");
        document.getElementById("hideGivenName").hidden = true;
        document.getElementById("hideMiddleName").hidden = true;
        document.getElementById("hideSurName").hidden = true;
        document.getElementById("hideVisaName").hidden = true;
        document.getElementById("hideNationality").hidden = true;
        document.getElementById("companyCnt").hidden = true;
        document.getElementById("companyCnt2").hidden = true;
        document.getElementById("hideEditGivenName").hidden = true;
        document.getElementById("hideEditMiddleName").hidden = true;
        document.getElementById("hideEditSurName").hidden = true;
        document.getElementById("hideEditVisaName").hidden = true;
        document.getElementById("hideEditNationality").hidden = true;
    }



    if (OPCompanySelected == "OPSHOW") {
        $('#modalAddOP').modal('show');
        $('#modalSearchCompany').modal('show');
        $('.isDisabled').attr('disabled', true);
    }
    else if (OPCompanySelected == "EDITOPSHOW") {
        $('#modalAddOP').modal('show');
        $('#modalEditSearchCompany').modal('show');
        $('.isDisabled').attr('disabled', true);
    }
    else if (OPCompanySelected == "OPHIDE") {
        $('#modalAddOP').modal('show');
    }
    else if (OPCompanySelected == "EDITOPHIDE") {
        $('#modalEditOP').modal('show');
    }
    else if (OPCompanySelected == "OK") {
        $('#modalSearch').modal('show');
        $('.isDisabled').attr('disabled', true);
    }

    //alert(SelectOPAccount);
    if (SelectOPAccount == "ADDSHOW") {
        $('#modalAddOPDetail').modal('show');
        $('#modalSearchOPAccounts').modal('show');
        $('.isDisabled').attr('disabled', true);
    } else if (SelectOPAccount == "ADDHIDE") {
        $('#modalAddOPDetail').modal('show');
    } else if (SelectOPAccount == "EDITSHOW") {
        $('#modalEditOPDetail').modal('show');
        $('#modalSearchEditOPAccounts').modal('show');
        $('.isDisabled').attr('disabled', true);
    } else if (SelectOPAccount == "EDITHIDE") {
        $('#modalEditOPDetail').modal('show');
    }

    // Update for OP Module 10/10/16 //
    if (Accountable == "AF") {
        document.getElementById("hideGivenName").hidden = true;
        document.getElementById("hideMiddleName").hidden = true;
        document.getElementById("hideSurName").hidden = true;
        document.getElementById("hideVisaName").hidden = true;
        document.getElementById("hideNationality").hidden = true;
    } else if (Accountable == "VISA") {
        document.getElementById("hideStart").hidden = true;
        document.getElementById("hideEnd").hidden = true;
        document.getElementById("hideEditStart").hidden = true;
        document.getElementById("hideEditEnd").hidden = true;

    } else {
        $('#modalAddDiplomat').modal('hide');
    }
    // ----------------------------- //

    // Update for Company Module 09/21/16 //
    //alert(SelectedCompany);
    if (SelectedCompany == "COMPSHOW") {
        $('#modalSearchEnterprise1').modal('show');
        $('.isDisabled').attr('disabled', true);
    } else if (SelectedCompany == "COMPHIDE") {
        $('#modalSearchEnterprise1').modal('hide');
    } 
    // --------------------------------- //
    
    if (TransactionSuccess == "Add") {
        alert('Successfully added');
    }
    else if (TransactionSuccess == "Edit") {
        alert('Successfully updated');
    }
    else if (TransactionSuccess == "delete") {
        alert('Successfully deleted');
    }
    else if (TransactionSuccess == "Complete") {
        alert('Transaction complete');
    }
    else if (TransactionSuccess == "Failed") {
        alert('Transaction Failed');
    }
    else if (TransactionSuccess == "Duplicate") {
        alert('Transaction Failed. Duplicate Entry.');
    }
    else if (TransactionSuccess == "PeriodFailed") {
        alert('Unable to save billing period. Please finalize previous period first.');
    }
    else if (TransactionSuccess == "DatePeriodFailed") {
        alert('Unable to save billing period. Date from must be greater than previous billing period.');
    }
    else if (TransactionSuccess == "ErrorSewerage") {
        alert('Error! A group cannot obtain multiple Sewerage rate.');
    }
    else if (TransactionSuccess == "NoBillMonths") {
        alert('Error! No billing period detected.'); 
    }
    else if (TransactionSuccess == "WaterReadingFailed") {
        alert('Error! Billing period is already used in this Meter number.');
    }
    else if (TransactionSuccess == "DuplicateBillingPeriod") {
        alert('Transaction Failed. Selected billing period is already used by this company.');
    }
    else if (TransactionSuccess == "GenerateEOMFirst") {
        alert('Unable to proceed. EOM Processing for current billing period is not yet generated.');
    }
    else if (TransactionSuccess == "CompanyNotExist") {
        alert('Transaction Failed. Company Doesnt Exist!');
    }
    else if (TransactionSuccess == "NoDesc") {
        alert('Error! Description Doesnt Exist!');
    }

    if (TransactionSuccess == "MeterNumberExist") {
        alert('Meter Number is not yet available.');
    }
   
    if (TransactionSuccess == "PasswordNotMatch") {
        alert('Password do not match.');
    }

    if (TransactionSuccess == "DeleteRate") {
        alert('Rate has been deleted.');
    }

    if (TransactionSuccess == "LessThanoneRate") {
        alert('Unable to delete this record. Rate Category must have at least 1 rate.');
    }

    if (ToggleBillingStatus == "Toggled") {
        alert(BillingStatus);
    }

    $('#modalSearchEnterprise').attr('data-backdrop', 'static'); //modal will only close when DATA-DISMISS attribute is invoked
    $('#modalSearchEnterprise').attr('data-keyboard', false); //modal will not close even ESC button
    $('#modalAdd').attr('data-backdrop', 'static'); //modal will only close when DATA-DISMISS attribute is invoked
    $('#modalAdd').attr('data-keyboard', false); //modal will not close even ESC button

    if (CompanySelected == "OK") {
        $('#modalSearchEnterprise').modal('hide');
        $('.isDisabled').attr('disabled', false);
    } else {
        $('#modalSearchEnterprise').modal('show');
        $('.isDisabled').attr('disabled', true);
        document.getElementById("searchCompanyFld").hidden = false;
        document.getElementById("searchOPAccountFld").hidden = false;
    }

    //if (isvalidated == 'True') {
    //    $('#modalAddRental').modal('hide');
    //} else if (isvalidated == 'False') {
    //    $('#modalAddRental').modal('show');
    //}

    if (isvalidated == 'True') {
        $('#modalAdd').modal('hide');
    } else if (isvalidated == 'False') {
        $('#modalAdd').modal('show');
    }

    $('.checkItem').change(function () {
        var itemClass = $(this).attr("selectItem");
        //alert("itemClass = "+itemClass);
        var BillMode = $(this).val();
        //alert("BillMode 1 = " + BillMode);
        $('.' + itemClass).each(function () {
            if ($(this).hasClass(BillMode) == true) {
                $(this).attr('checked', true);
                //var toStr = this.value.toString();
                //alert("item " + this.value + " = " + document.getElementsById("billingMonth" + this.value).attr("name"));
                //document.getElementsById("billingMonth" + this.value).setAttribute("type", "button");
                //$('#billingMonth' + toStr).attr('name', 'billingMonthsHidden');
                //alert("item " + this.value + " = "+ $('#billingMonth' + this.value).attr('name'));
            } else { "item " + this.value + " = " + $(this).attr('checked', false); }
            //$('#billingMonth' + toStr).attr('name', '');
        });
    });

    $('.adminChkItm').click(function () {
        if ($(this).is(':not(:checked)')) {
            $("label[for='" + this.id + "']").html("View Only");
        } else {
            $("label[for='" + this.id + "']").html("Add|Edit|Delete|View");
        }
    });
});

$(document).on('submit', 'form', function () {
    var buttons = $(this).find('[type="submit"]');
    if ($(this).valid()) {
        buttons.each(function (btn) {
            $(buttons[btn]).prop('disabled', true);
        });
    } else {
        buttons.each(function (btn) {
            $(buttons[btn]).prop('disabled', false);
        });
    }
});

function resetmodal() {
    document.getElementById("resetModal").click();
}

function toggleActiveDiffer(id) {
    if (document.getElementById(id).value == "ACTIVE") {
        document.getElementById(id).value = "DIFFER";
        document.getElementById(id + "ChkBx").checked = false;
    } else {
        document.getElementById(id).value = "ACTIVE";
        document.getElementById(id + "ChkBx").checked = true;
    }
}
function billingMonthsList(item) {
    //alert("check box " + item.value + "is = " + item.attr(checked));
    //if (item.Attr('checked') == true) {
    //    $("'#billingMonth" + item.value + "'").attr('disabled', false);
    //} else {
    //    $("'#billingMonth" + item.value + "'").attr('disabled', true);
    //}
}
function selectItem(item) {
    document.getElementById("companyName").value = item;
}
function selectEnterprise(item) {
    document.getElementById("companyName").innerHTML = document.getElementById(item + "EntName").innerHTML;
    document.getElementById("companyAddress").innerHTML = document.getElementById(item + "EntAdd").innerHTML;
}

function searchCompany(val) {
    if (val.value == "Search for Company") {
        document.getElementById("searchCompanyFld").hidden = false;
        document.getElementById("searchCompanyBtn").hidden = true;
    } else {
        document.getElementById("searchCompanyFld").hidden = true;
        document.getElementById("searchCompanyBtn").hidden = false;
    }
}

function sendEmail(val) {
    if (val.value == "YES") {
        document.getElementById("AddEmailAddress").readOnly = false;
        document.getElementById("AddSecondaryEmailAddress").readOnly = false;
        document.getElementById("AddEmailAddress").style.backgroundColor = "white";
        document.getElementById("AddSecondaryEmailAddress").style.backgroundColor = "white";
    }
    else {
        document.getElementById("AddEmailAddress").readOnly = true;
        document.getElementById("AddSecondaryEmailAddress").readOnly = true;
        document.getElementById("AddEmailAddress").style.backgroundColor = "lightgrey";
        document.getElementById("AddSecondaryEmailAddress").style.backgroundColor = "lightgrey";
        document.getElementById("AddEmailAddress").value = "";
        document.getElementById("AddSecondaryEmailAddress").value = "";
    }

    if (val.value == "YES") {
        document.getElementById("EmailAddress").readOnly = false;
        document.getElementById("SecondaryEmailAddress").readOnly = false;
        document.getElementById("EmailAddress").style.backgroundColor = "white";
        document.getElementById("SecondaryEmailAddress").style.backgroundColor = "white";
    }
    else {
        document.getElementById("EmailAddress").readOnly = true;
        document.getElementById("SecondaryEmailAddress").readOnly = true;
        document.getElementById("EmailAddress").style.backgroundColor = "lightgrey";
        document.getElementById("SecondaryEmailAddress").style.backgroundColor = "lightgrey";
        document.getElementById("EmailAddress").value = "";
        document.getElementById("SecondaryEmailAddress").value = "";
    }
}

function searchOPAccount(val) {
    if (val.value == "Search for OPAccount") {
        document.getElementById("searchOPAccountFld").hidden = false;
        document.getElementById("searchOPAccountBtn").hidden = true;
    } else {
        document.getElementById("searchOPAccountFld").hidden = true;
        document.getElementById("searchOPAccountBtn").hidden = false;
    }
}

$('.toggleDisable').click(function () {
    var itemClass = $(this).attr("selectItem");
    if ($('.toggleDisable').attr('value') == 'yes') {
        $('.toggleDisable').attr('value', 'no');
        $('.' + itemClass).attr('disabled', true);
    } else {
        $('.toggleDisable').attr('value', 'yes');
        $('.' + itemClass).attr('disabled', false);
    }
});

$('.chkbxfilter').click(function () {
    if (this.id == 'filterbYCompany') {
        $('.companySearch').attr('hidden', false);
    } else {
        $('.companySearch').attr('hidden', true);
    }
});

function fourCharVal() {
    var str = $('#searchCompany').val();
    if (/^[a-zA-Z0-9-.& ]*$/.test(str) == false) {
        alert('Your search string contains invalid character.');
    } if (str.length > 2) {
        $('#searchBtn').attr("disabled", false);
    } else if (str.length < 3 && str.length > 0) {
        $('#searchBtn').attr("disabled", true);
    } else {
        $('#searchBtn').attr("disabled", true);
    }
}

//-------------CONVERT TO MONEY FORMAT
function formatNumber(event, id, assignValue) {
    
    var a = document.getElementById(id).value; //fake amount element
    var assignval = document.getElementById(assignValue); //orig amount element
    var isValidDecimal = a.split('.').length > 2 ? false : true; //check two decimal point in the sequence

    if (a.length == 0) { //this will resolve long backspace press...
        assignval.value = "";
    }

    if ((event >= 48 && event <= 57) || event == 46 || event == 8 || (event >= 96 && event <= 105) || (event == 190 && isValidDecimal) || (event == 110 && isValidDecimal)) { //only valid keychar is allowed        
        for (indx in a) {
            if (a[indx] == '.') {
                document.getElementById(id).value = a;
                break;
            }
            else {
                formatAmount(id);
                setElementValue(id, assignValue);
            }
        }
    }
    else {
        if (event !== 37 && event !== 38 && event !== 39 && event !== 40) { //do nothing if arrow keys is pressed.
            var b = a.substring(0, a.length - 1); //remove last character if not valid keycode.
            document.getElementById(id).value = b;
            setElementValue(id, assignValue); //remove comma/s and assign value in orig Amount element.
        }
    }
}

function formatAmount(id) {
    var a = document.getElementById(id).value;

    var num = a.replace(/,/gi, ""); //remove commas
    var num2 = num.split(/(?=(?:\d{3})+$)/).join(","); //split and insert comma
    document.getElementById(id).value = num2;
}

function formatAmountForEdit(value) {
    var newValue = parseFloat(value);
    newValue = newValue.toFixed(4);
    return (newValue).toLocaleString("en-US");
}

function setElementValue(id, assignValue) {
    var origvalue = document.getElementById(id).value;
    var newvalue = "";
    var splitvalue = origvalue.split(',');

    for (indx in splitvalue) {
        newvalue += splitvalue[indx];
    }
    document.getElementById(assignValue).value = newvalue;
}
//-------------CONVERT TO MONEY FORMAT

//-------------FUNCTIONS FOR BILLING MONTHS
function setbillmonths(billmonths) {
    clearchk();
    var billmonths = billmonths;
    var list = billmonths.split(",");
    $(list).each(function () {
        $('input[type=checkbox][name=billingMonths][bm=' + this + ']').prop('checked', true);
    })
}

function clearchk() {
    $('input[type=checkbox]').each(function () {
        $(this).prop('checked', false);
    })
}

function setBillMode(billmode) {
    var billmonths = "";
    if (billmode.value.toUpperCase() == "ANNUAL")
        billmonths = "1";
    else if (billmode.value.toUpperCase() == "BI-ANNUAL")
        billmonths = "1,6";
    else if (billmode.value.toUpperCase() == "QUARTERLY")
        billmonths = "3,6,9,12";
    else if (billmode.value.toUpperCase() == "MONTHLY")
        billmonths = "1,2,3,4,5,6,7,8,9,10,11,12";
    else if (billmode.value.toUpperCase() == "IRREGULAR")
        billmonths = "";

    setbillmonths(billmonths);
}
//-------------END FUNCTIONS FOR BILLING MONTHS

//------------START OF HASHING
function hashPassword(data) {
    var userValue = $('#userValue').val();
    var key = $('#Ekey').val();
    var newUrl = '';
    //alert(userValue + " " + key)
    if (userValue != "" || key != "") {
        newUrl = data == 'Encrypt' ? '/Crypto/Encrypt' : '/Crypto/Decrypt';

        $.ajax({
            type: 'POST',
            url: newUrl,
            data: { val: userValue, ekey: key },
            dataType: 'json',
            success: function (data) {
                alert('Hashin complete..');
                $('#HashedValue').val(data.encrypted);
            },
            error: function (err) {
                alert("Error: " + err.statusText);
            }
        })
    }
}
//------------END OF HASHING

function isChildExist(elementToPopulate, childValue) {
    //alert('a');
    var isExist = false;

    for (var i = 0; i < elementToPopulate.length; i++) {
        var x = elementToPopulate.options[i].text;
        //alert(":"+x.trim() + ":" + childValue.trim()+":");
        if (x.trim() == childValue.trim()) {
            isExist = true;
            //alert(isExist);
            break;
        }
    }
    return isExist;
}



jq111(function () {
    jq111(".date-picker").datepicker();
})

function selectBillingPayment() {
   
  // alert($('input:radio.sort:checked').val());
    var type = $('input:radio.sort:checked').val();


    $.ajax({
        type: 'POST',
        url: '/BillingPayments/ViewPaymentsType',
        data: { type: type },
        success: function (data) {
         
           // alert(data);
                $("#companyCnt").empty();
                $("#companyCnt").append(data);

        },
        error: function(jqxHTR){
            alert(jqxHTR);
    }
    })
}