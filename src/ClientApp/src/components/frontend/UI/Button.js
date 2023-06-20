import React from 'react';
import './button.css';

export const Navbutton = (props) => {
  return (
    <button className='nav-button'>{props.value}</button>
  )
}

export const Styledbutton = (props) => {
    return (
      <button className='nav-styled-button' id='cont-btn'>{props.value}</button>
    )
}

export const Landingbutton = (props) => {
  return (
    <button className='styled-button'>{ props.value }</button>
  )
}


export const Button = (props) => {
  return (
    <button className='nav-button'>{props.value}</button>
  )
}