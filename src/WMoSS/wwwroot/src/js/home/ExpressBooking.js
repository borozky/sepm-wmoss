/// <reference path="../index.d.ts" />
import React, { Component } from 'react';
import moment from "moment";
import $ from "jquery";

import SeatMap from "./SeatMap";

class ExpressBooking extends Component {

    constructor(props) {
        super(props);

        this.handleMovieSelected = this.handleMovieSelected.bind(this);
        this.handleTheaterSelected = this.handleTheaterSelected.bind(this);
        this.handleSessionSelected = this.handleSessionSelected.bind(this);
        this.handleSeatSelected = this.handleSeatSelected.bind(this);
        this.getData = this.getData.bind(this);

        let seats = [];
        let rows = ['A', 'B', 'C', 'D', 'E'];
        rows.forEach(r => {
            for (let i = 1; i <= 10; i++) {
                seats.push({
                    row: r,
                    col: i,
                    selected: false,
                    unavailable: props.unavailableSeats ? props.unavailableSeats.filter(us => `${r}${i}` == us).length > 0 : false
                });
            }
        });

        /** @type {ExpressBookingState} */
        this.state = {
            theaters: [],
            movies: [],
            sessions: [],
            selectedMovieId: null,
            selectedTheaterId: null,
            selectedSessionId: null,
            unavailableSeats: null,
            ticketQuantity: 1,
            selected: [],
            seats: seats
        }
     
    }

    componentDidMount() {
        this.getData(this.state.selectedMovieId, this.state.selectedTheaterId);
    }

    refreshSeats(unavailableSeats) {
        let seats = [];
        let rows = ['A', 'B', 'C', 'D', 'E'];
        rows.forEach(r => {
            for (let i = 1; i <= 10; i++) {
                seats.push({
                    row: r,
                    col: i,
                    selected: false,
                    unavailable: unavailableSeats ? unavailableSeats.filter(us => `${r}${i}` == us).length > 0 : false
                });
            }
        });

        let selected = seats.filter(s => s.selected);

        this.setState({
            selected: selected,
            seats: seats,
            unavailableSeats: unavailableSeats
        })
    }

    /**
     * 
     * @param {Event} event 
     * @param {number} row 
     * @param {number} col 
     */
    handleSeatSelected(event, row, col) {
        console.log("TICKET QUIANTITY", this.state.ticketQuantity);

        let seats = this.state.seats.map((seat, index) => {
            if (seat.row == row && seat.col == col) {
                return {
                    ...seat,
                    selected: !seat.selected
                };
            }
            return seat;
        });

        let selectedSeats = seats.filter(s => s.selected).map(s => `${s.row}${s.col}`);

        if (selectedSeats.length <= this.state.ticketQuantity) {
            this.setState({
                seats: seats,
                selected: selectedSeats
            });
        }
    }

    /**
     * @param {number} movieSessionId
     */
    getOccupiedSeats(movieSessionId) {
        let self = this;
        $.getJSON("/Api/ExpressBooking/Seats/Unavailable", { movieSessionId : movieSessionId}, function (data) {
            self.refreshSeats(data);

            console.log(data);
        });
    }


    /**
     * 
     * @param {number?} movieId
     * @param {number?} theaterId
     */
    getData(movieId, theaterId) {
        let self = this;
        $.ajax({
            method: "GET",
            url: "/Api/ExpressBooking",
            data: {
                movieId: movieId,
                theaterId: theaterId
            },
            /**
             * @type {JQuery.Ajax.SuccessCallback}
             * @param {{ movies: Movie[], movieSessions?: MovieSession[], theaters?: Theater[] }} data
             */
            success: function(data) {
                console.log(data);

                self.setState({ 
                    movies: data.movies,
                    sessions: data.movieSessions,
                    theaters: data.theaters,
                    unavailableSeats: null
                });
            },
            /** @type {JQuery.Ajax.ErrorCallback} */
            error: function(error) {
                console.log(error);
            }
        })
    }


    /** @param {Event} event */
    handleMovieSelected(event) {
        /** @type {HTMLSelectElement} */
        let target = event.target;
        let movieId = parseInt(target.value, 10);
        this.setState({
            selectedMovieId: movieId,
            selectedTheaterId: null,
            selectedSessionId: null,
            unavailableSeats: null
        });
        this.getData(movieId, this.state.selectedTheaterId);
    }

    /** @param {Event} event */
    handleSessionSelected(event) {
        /** @type {HTMLSelectElement} */
        let target = event.target;
        let sessionId = parseInt(target.value, 10);
        this.setState({
            selectedSessionId: sessionId,
        });
        this.getOccupiedSeats(sessionId);
    }

    /** @param {Event} event */
    handleTheaterSelected(event) {
        /** @type {HTMLSelectElement} */
        let target = event.target;
        let theaterId = parseInt(target.value, 10);
        this.setState({
            selectedTheaterId: theaterId,
            selectedSessionId: null
        });
        this.getData(this.state.selectedMovieId, theaterId);
    }

    
    
 

    render() {
        return (
            <div>
                {this.state.selectedSessionId ? <input type="hidden" name="MovieSessionId" value={this.state.selectedSessionId}/> : ""}
                {this.state.ticketQuantity ? <input type="hidden" name="TicketQuantity" value={parseInt(this.state.ticketQuantity, 10)}/> : ""}
                <div>
                    <label htmlFor="ExpressCheckout-movie">Select Movie</label><br />
                    <select name="movie" id="ExpressCheckout-movie" className="form-control" value={this.state.selectedMovieId || ""} onChange={this.handleMovieSelected}>
                        <option value="">Select Movie</option>
                        {
                            this.state.movies &&
                                this.state.movies.map((movie, index) => (
                                    <option key={index} value={movie.id}>{movie.title}</option>
                                ))
                        }
                    </select>
                </div>
                <br />

                <div>
                    <label htmlFor="ExpressCheckout-theater">Select Theater</label><br />
                    <select name="theater" id="ExpressCheckout-theater" className="form-control" value={this.state.selectedTheaterId || ""} onChange={this.handleTheaterSelected}>
                        <option value="">Select Theater</option>
                        {
                                this.state.theaters && this.state.theaters.map((theater, index) => (
                                    <option key={index} value={theater.id}>{theater.name}</option>
                                ))
                        }
                    </select>
                </div>
                <br />

                <div>
                    <label htmlFor="ExpressCheckout-session">Select Session</label><br />
                    <select name="movie" id="ExpressCheckout-session" className="form-control" value={this.state.selectedSessionId || ""} onChange={this.handleSessionSelected}>
                        <option value="">select Session</option>
                        {
                            this.state.sessions && this.state.sessions.map((session, index) => (
                                <option key={index} value={session.id}>{moment(session.scheduledAt, "YYYY-MM-DDThh:mm:ss").format("hh:mm a dddd, MMM Do")}</option>
                            ))
                        }
                    </select>
                </div>
                <br />
                
                <div>
                    <label htmlFor="ExpressCheckout-ticket">Tickets</label><br />
                    <input type="number" name="ticket" className="form-control" min="1" placeholder="Qty" 
                        value={this.state.ticketQuantity} onChange={e => {this.setState({ ticketQuantity: parseInt(e.target.value, 10) })}} />
                </div>
                <br />
                <div>
                    <div className="dropdown" id="SelectSeatsDropdown">
                        <button id="SelectSeatsDropdownButton" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                            Select Seats <span className="caret"></span>
                        </button>
                        <ul className="dropdown-menu" aria-labelledby="SelectSeatsDropdownButton">
                            <li id="SeatMapParent">
                                <p id="SeatsRemaining">Selected: {this.state.selected.length} of {this.state.ticketQuantity} seats</p>
                                <SeatMap 
                                    unavailableSeats={this.state.unavailableSeats} 
                                    ticketQuantity={this.state.ticketQuantity} 
                                    movieSessionId={this.state.selectedMovieId}
                                    seats={this.state.seats}
                                    handleSeatSelected={this.handleSeatSelected}
                                     />
                            </li>
                        </ul>
                    </div>
                </div>
                <br />
                <div>
                    <input type="submit" name="submit" value="Add To Cart" 
                        className="form-control btn btn-primary" disabled={this.state.selectedSessionId == null || this.state.ticketQuantity < 1}/>
                </div>
            </div>

        );
    }


}

export default ExpressBooking;