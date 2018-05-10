import React, { Component } from 'react';

const Seat = ({ row, col, handleSeatSelected, selected = false, unavailable = false }) => 
    <span className={`seatitem${selected ? " selected" : ""}${unavailable ? " unavailable" : ""}`}>
        <input type="checkbox" id={`Seat-${row}${col}`} value={unavailable ? false : selected } disabled={unavailable} name="Seats[]" value={`${row}${col}`} onChange={e => handleSeatSelected(e, row, col)}/>
        <label htmlFor={`Seat-${row}${col}`} className="seatlabel">{`${row}${col}`}</label>
    </span>


class SeatMap extends Component {

    render() {
        return (
            (this.props.movieSessionId != null && this.props.unavailableSeats != null) ? 
                <div id="SeatMap">
                    {
                        this.props.seats.map((s, index) => 
                        <Seat key={index} row={s.row} col={s.col} selected={s.selected} unavailable={s.unavailable} handleSeatSelected={this.props.handleSeatSelected}/>)
                    }
                </div>
                :
                <small id="SeatMapError">Please select a movie session before you select your seats</small>
        );
    }
}

export default SeatMap;