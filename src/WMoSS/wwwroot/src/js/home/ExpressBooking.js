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
            movies: [
                {id: 1, name: "Movie #1", rating: "R", price: 20},
                {id: 2, name: "Movie #2", rating: "R", price: 20},
                {id: 3, name: "Movie #3", rating: "R", price: 20},
                {id: 4, name: "Movie #4", rating: "R", price: 20},
                {id: 5, name: "Movie #5", rating: "R", price: 20}
            ],
            sessions: [
                {id: 1, day: "Monday ", time: "4:30", date: 30/1},
                {id: 2, day: "Monday ", time: "4:30", date: 30/1},
                {id: 3, day: "Monday ", time: "4:30", date: 30/1},
                {id: 4, day: "Monday ", time: "4:30", date: 30/1},
                {id: 5, day: "Monday ", time: "4:30", date: 30/1}
            ]
        }
     
    }
    
 

    render() {
        return (

            <form action="" method="POST">
                <div class="row">
                    <div class="col-sm-2 col-md-2">
                        <label htmlFor="ExpressCheckout-theater">Select Theater</label><br/>
                        <select name="theater" id="ExpressCheckout-theater" className="form-control">
                        <option selected="selected">select a theater</option>
                        {
                            this.state.theaters.map((theater, index) => (
                                <option key={index} value={theater.id}>{theater.name}</option>
                            ))
                        }
                        </select>
                    </div>

                    <div className="col-sm-2 col-md-2">
                        <label htmlFor="ExpressCheckout-movie">Select Movie</label><br/>
                        <select name="movie" id="ExpressCheckout-movie" className="form-control col-sm-3 col-md-3">
                        <option selected="selected">select a movie</option>
                        {
                            this.state.movies.map((movie, index) => (
                                <option key={index} value={movie.id}>{movie.name}</option>
                            ))
                        }
                        </select>
                    </div>

                    <div className="col-sm-2 col-md-2">
                        <label htmlFor="ExpressCheckout-session">Select Session</label><br/>
                        <select name="movie" id="ExpressCheckout-session" className="form-control col-sm-3 col-md-3">
                        <option selected="selected">select a session</option>
                        {
                            this.state.sessions.map((session, index) => (
                                <option key={index} value={session.id}>{session.day}{session.time}</option>
                            ))
                        }
                        </select>
                    </div>

                    <div class="col-sm-1 col-md-1">
                        <label htmlFor="ExpressCheckout-ticket">Tickets</label><br/>
                            <input type="number" name="ticket" className="form-control" min="1" placeholder="Select"/>
                    </div>

                    <div className="col-sm-2 col-md-2">
                        <label htmlFor="ExpressCheckout-seat">Select Seats</label><br/>
                            <button className="form-control btn btn-default">Select from the map</button>
                    </div>

                </div>

                <br />

                <div className="row">
                    <div className="col-sm-2 col-md-2 pull-right">
                        <input type="submit" name="submit" value="Add To Cart" className="form-control btn btn-primary"/>
                    </div>
                </div>
            </form>
           
        )
    }


}

export default ExpressBooking;