import React from "react";
import ReactDOM from "react-dom";
import ExpressBooking from "./ExpressBooking";
import $ from "jquery";


// $(document).ready( function() {
//     $('[data-toggle="tooltip"]').tooltip();
// });

// prevents dropdown menu from closing when a seat is clicked
$(document).on('click', '#ExpressBookingParent .dropdown-menu', function (e) {
    e.stopPropagation();
});



/** @type {HTMLDivElement} */
var expressBookingParent = document.getElementById("ExpressBookingParent");
if (expressBookingParent) {
    ReactDOM.render(<ExpressBooking/>, expressBookingParent);
}
