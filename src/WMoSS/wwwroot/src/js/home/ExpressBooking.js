/// <reference path="../index.d.ts" />
import React, { Component } from 'react';
import moment from "moment";
import $ from "jquery";

class ExpressBooking extends Component {

    constructor(props) {
        super(props);

        this.handleMovieSelected = this.handleMovieSelected.bind(this);
        this.handleTheaterSelected = this.handleTheaterSelected.bind(this);
        this.handleSessionSelected = this.handleSessionSelected.bind(this);
        this.getData = this.getData.bind(this);

        /** @type {ExpressBookingState} */
        this.state = {
            theaters: [],
            movies: [],
            sessions: [],
            selectedMovieId: null,
            selectedTheaterId: null,
            selectedSessionId: null
        }
     
    }

    componentDidMount() {
        this.getData(this.state.selectedMovieId, this.state.selectedTheaterId);
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
                    theaters: data.theaters
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
            selectedSessionId: null
        });
        this.getData(movieId, this.state.selectedTheaterId);
    }

    /** @param {Event} event */
    handleSessionSelected(event) {
        /** @type {HTMLSelectElement} */
        let target = event.target;
        let sessionId = parseInt(target.value, 10);
        this.setState({
            selectedSessionId: sessionId
        });
        this.getData(this.state.selectedMovieId, this.state.selectedTheaterId);
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
                    <input type="number" name="ticket" className="form-control" min="1" placeholder="Qty" />
                </div>
                <br />
                <div>
                    <button id="selectSeats" className="form-control btn btn-default" data-toggle="modal" data-target="#SeatAllocationModal">
                        Select Seats
                    </button>
                </div>
                <br />
                <div>
                    <input type="submit" name="submit" value="Add To Cart" className="form-control btn btn-primary" />
                </div>
            </div>

        );
    }


}

export default ExpressBooking;