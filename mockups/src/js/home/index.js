import React from "react";
import ReactDOM from "react-dom";
import ExpressBooking from "./ExpressBooking";

/** @type {HTMLDivElement} */
var expressCheckoutParent = document.getElementById("ExpressCheckoutParent");

$(document).ready( function() {
    $('[data-toggle="tooltip"]').tooltip();
});

ReactDOM.render(<ExpressBooking/>, expressCheckoutParent)