import React from "react";
import ReactDOM from "react-dom";
import ExpressBooking from "./ExpressBooking";

/** @type {HTMLDivElement} */
var expressCheckoutParent = document.getElementById("ExpressCheckoutParent");

ReactDOM.render(<ExpressBooking/>, expressCheckoutParent)