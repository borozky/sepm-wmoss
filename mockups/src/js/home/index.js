import React from "react";
import ReactDOM from "react-dom";
import ExpressBooking from "./ExpressBooking";

/** @type {HTMLDivElement} */
var expressCheckoutParent = document.getElementById("ExpressCheckoutParent");

$(document).ready( function() {
    $('[data-toggle="tooltip"]').tooltip();
});

$("#selectSeats").on("click", function(e) {
    e.preventDefault();
});

$('#SeatAllocationModal').on('shown.bs.modal', function () {
    $('#selectSeats').focus()
  })

ReactDOM.render(<ExpressBooking/>, expressCheckoutParent)