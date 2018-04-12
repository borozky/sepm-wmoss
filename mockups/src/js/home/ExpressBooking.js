/// <reference path="../index.d.ts" />
import React, { Component } from 'react';

class ExpressBooking extends Component {

    constructor(props) {
        super(props);

        /** @type {ExpressBookingState} */
        this.state = {
            theaters: [
                {id: 1, name: "Theater #1", address: "123 Lygon St., Melbourne", capacity: 50},
                {id: 2, name: "Theater #2", address: "123 Lygon St., Melbourne", capacity: 50},
                {id: 3, name: "Theater #3", address: "123 Lygon St., Melbourne", capacity: 50},
                {id: 4, name: "Theater #4", address: "123 Lygon St., Melbourne", capacity: 50},
                {id: 5, name: "Theater #5", address: "123 Lygon St., Melbourne", capacity: 50}
            ],
            movies: [],
            sessions: []
        }
    }

    render() {
        return (
            <form action="" method="POST">
                <h4>Express Checkout</h4>
                <label htmlFor="ExpressCheckout-theater">Select Theater</label><br/>
                <select name="theater" id="ExpressCheckout-theater">
                {
                    this.state.theaters.map((theater, index) => (
                        <option key={index} value={theater.id}>{theater.name}</option>
                    ))
                }
                </select>
                { /* the rest goes here */ }
            </form>
        )
    }


}

export default ExpressBooking;