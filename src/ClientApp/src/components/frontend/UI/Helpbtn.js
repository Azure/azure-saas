import React from 'react'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faPhoneFlip } from '@fortawesome/free-solid-svg-icons';
import './helpBtnStyle.css';

export const Helpbtn = () => {
  return (
    <div className='help-btn'>
        <FontAwesomeIcon icon={faPhoneFlip} className='help-icon'/>
    </div>
  )
}
