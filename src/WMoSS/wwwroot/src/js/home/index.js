import React from "react";
import ReactDOM from "react-dom";
import ExpressBooking from "./ExpressBooking";

/** @type {HTMLDivElement} */
var expressBookingParent = document.getElementById("ExpressBookingParent");

$(document).ready( function() {
    $('[data-toggle="tooltip"]').tooltip();
});

// prevents dropdown menu from closing when a seat is clicked
$(document).on('click', '#ExpressBookingParent .dropdown-menu', function (e) {
    e.stopPropagation();
});

ReactDOM.render(<ExpressBooking/>, expressBookingParent)